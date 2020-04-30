using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CB_Bomberman.Pathfinding;
using CB_Bomberman.State;
using VectorInt;
using static MoreLinq.Extensions.MaxByExtension;
using static MoreLinq.Extensions.MinByExtension;

namespace CB_Bomberman.Game
{
    public class Hunter:Agent
    {
        private readonly DijkstraPathfinder pathfinder = new DijkstraPathfinder();
        private readonly Map map;
        private readonly GameInfo info;
        private readonly Dictionary<Tile, int> blastZone;
        private readonly Tile myTile;
        public Hunter(Map map, GameInfo info) : base(map, info)
        {
            this.map = map;
            blastZone = map.BlastZones;
            this.info = info;
            myTile = map.PlayerTile;

            var dangerousZone = blastZone.Where(x => x.Value == 1)
                .Select(x => x.Key).ToHashSet();
            var graph = map.ToFullGraph()
                .IncreaseWeightAround(x => x.IsMonster, 4)
                .IncreaseWeightAround(x => blastZone.ContainsKey(x), 1, 2)
                .IncreaseWeightAround(x => dangerousZone.Contains(x), 1, 1001)
                .DeleteTiles(x => !x.IsWalkable);
            pathfinder.FindAllPaths(graph, myTile);
        }

        public override AgentAction MakeMove()
        {
            if (ShouldPanic()) return Panic();
            if (TargetInBombRadius(myTile) || MischiefChance())
            {
                var tileToRun = GetTileToRunAfterBomb();
                if (tileToRun != null)
                    return GameLib.BombDirectionTo(myTile, pathfinder.NextTileOnPath(tileToRun.Value));
            }

            var target = ChooseTarget();
            if (target == default)
            {
                //взрываем себя если всё равно на следующий ход СМЕРТ
                return IsTileDeadlyToGo(myTile) ? AgentAction.BombUp : AgentAction.DoNothing;
            }

            return GameLib.DirectionTo(myTile, pathfinder.NextTileOnPath(target));
        }

        private Tile ChooseTarget()
        {

            HashSet<Tile> targets = new HashSet<Tile>();
            targets.UnionWith(map.AllEnemyPlayers);
            targets.UnionWith(map.AllMonsters);
            targets.UnionWith(map.AllWalls);

            float maxPriority = 0;
            float numberOfTargetsOnMaxPriority = 0;
            Tile finalTarget = default;


            foreach (var target in targets)
            {
                var nearestWalkable = GetNearestWalkableTileToPlaceBomb(target, out var numberOfTargets);
                if (nearestWalkable == null) 
                    continue;
                if (IsTileDangerousToGo(pathfinder.NextTileOnPath(nearestWalkable.Value))) 
                    continue;
                if (map.GetManhattanDistance(myTile, target) == 1)
                    continue;

                float score = 0;
                if (target.IsEnemyPlayer) score = GameLib.PlayerKillScore;
                if (target.IsMonster) score = GameLib.MonsterKillScore;
                if (target.IsRegularWall) score = GameLib.WallDestroyScore;

                if (blastZone.ContainsKey(target) && map.GetManhattanDistance(myTile, target) < 5)
                {
                    score *= (blastZone[target] - 1)*0.25f;
                    score = Math.Clamp(score, 0, int.MaxValue);
                    if (target.IsRegularWall) score = 0;
                }

                var distance = Math.Clamp(pathfinder.GetDistance(nearestWalkable.Value) - 3, 1, int.MaxValue);
                if(distance > 1000) continue;

                float priority = score / (distance*distance);
                if (Math.Abs(priority - maxPriority) < 0.01 && numberOfTargets > numberOfTargetsOnMaxPriority)
                {
                    maxPriority = priority;
                    finalTarget = nearestWalkable.Value;
                }
                if (priority > maxPriority)
                {
                    maxPriority = priority;
                    finalTarget = nearestWalkable.Value;
                }
            }
            return finalTarget;
        }

        private Map GetMapMockupAfterBombPlaced(bool enemiesToBombs = false)
        {
            var mapAfterBombing = new Dictionary<VectorInt2, Tile>(map);
            var neededTiles = mapAfterBombing.ToHashSet();
            foreach (var tile in neededTiles)
            {
                switch (tile.Value.TileType)
                {
                    case TileTypes.Bomb1:
                        foreach (var blastTile in mapAfterBombing.GetChainBombBlastZone(tile.Value).ToHashSet())
                        {
                            mapAfterBombing[blastTile.Position] = mapAfterBombing[blastTile.Position].newTileOnSamePosition(TileTypes.Blast);
                        }
                        break;
                    case TileTypes.Bomb2:
                        mapAfterBombing[tile.Key] = mapAfterBombing[tile.Key].newTileOnSamePosition(TileTypes.Bomb1);
                        break;
                    case TileTypes.Bomb3:
                        mapAfterBombing[tile.Key] = mapAfterBombing[tile.Key].newTileOnSamePosition(TileTypes.Bomb2);
                        break;
                    case TileTypes.Bomb4:
                        mapAfterBombing[tile.Key] = mapAfterBombing[tile.Key].newTileOnSamePosition(TileTypes.Bomb3);
                        break;
                    case TileTypes.EnemyPlayer:
                        if(enemiesToBombs)
                            mapAfterBombing[tile.Key] = mapAfterBombing[tile.Key].newTileOnSamePosition(TileTypes.Bomb4);
                        break;
                }
            }
            mapAfterBombing[myTile.Position] = new Tile(myTile.Position, TileTypes.Bomb4);

            return new Map(map.Size, mapAfterBombing);
        }

        private Tile? GetTileToRunAfterBomb()
        {
            if (!info.BombAvailable) return null;
           // if (blastZone.ContainsKey(myTile) && blastZone[myTile] < 2) return null;
            var mockup = GetMapMockupAfterBombPlaced(true);
            var adj = mockup.GetAdjacentWalkableTiles(myTile);
            if (adj.Count == 0) return null;


            var nextTarget = ChooseTarget();

            //var viableTiles = adj
            //    .Where(x => !IsTileDangerousToGo(x, mockup, true))
            //    .ToHashSet();
            var viableTiles = adj
                .Where(x =>  mockup.GetAdjacentWalkableTiles(x).Any(k => !IsTileDangerousToGo(k, mockup, true)))
                .Where(x => !mockup.GetAdjacentWalkableTiles(x).Any(k=>k.IsMonster))
                .ToHashSet();

            if (viableTiles.Count == 0) return null;

            HashSet<Tile> deadEndTiles = new HashSet<Tile>();
            foreach (var viableTile in viableTiles)
            {
                var walkableTiles = mockup.GetAdjacentWalkableTiles(viableTile, 4, true, true);
                if (walkableTiles.Count < 4) deadEndTiles.Add(viableTile); //even if dead end is not dangerous, it's waste of time
                if (walkableTiles.All(x => mockup.BlastZones.ContainsKey(x))) deadEndTiles.Add(viableTile);
            }
            viableTiles.ExceptWith(deadEndTiles);
            if (viableTiles.Count == 0) return null;
            var toNextTarget = viableTiles.FirstOrDefault(x => pathfinder.IsPathExists(nextTarget) && x == pathfinder.NextTileOnPath(nextTarget));
            if (toNextTarget != default && !nextTarget.IsMonster) return toNextTarget;
            return viableTiles.MaxBy(EvaluateSimpleTileSafety).First();
        }

        private bool TargetInBombRadius(Tile from)
        {
            return GetTargetsInBombRadius(from).Count > 0;
        }

        private HashSet<Tile> GetTargetsInBombRadius(Tile tile)
        {
            return map
                .GetBombBlastZone(tile)
                .Where(x => !(blastZone.ContainsKey(x) && blastZone[x] == 1))
                .Where(x => x.IsMonster
                            || x.IsEnemyPlayer
                            || x.IsRegularWall && !blastZone.ContainsKey(x))
                .ToHashSet();
        }

        private bool ShouldPanic()
        {
            return IsTileDangerousToGo(myTile);
        }

        private bool IsTileDeadlyToGo(Tile tile, Map customMap = null, bool shouldExcludePlayer = false, bool enemiesStandStill = true)
        {
            //if (!tile.IsWalkable) return true;
            if (tile.IsMonster) return true;
            if (customMap == null) customMap = map;

            var blastZones = enemiesStandStill ? customMap.GetAllBlastZones(enemiesStandStill: enemiesStandStill) : customMap.BlastZones;

            //foreach (var tile1 in blastZones.Keys)
            //{
            //    Console.WriteLine(tile1);
            //}

            if (blastZones.ContainsKey(tile))
            {
                for (var i = 0; i < 4; i++)
                {
                    var adj = customMap.GetAdjacentWalkableTiles(tile, i);
                    adj.Add(tile);
                    if (adj
                        .Where(x => !(shouldExcludePlayer && x.IsPlayer))
                        .All(x => blastZones.ContainsKey(x) && blastZones[x] == i+1))
                        return true;
                }

            }

            if (blastZones.ContainsKey(tile))
            {
                var startingBlastDanger = blastZones[tile];
                HashSet<Tile> lastAdj = new HashSet<Tile>();
                for (var i = 0; i < 4; i++)
                {
                    var adj = customMap.GetAdjacentWalkableTiles(tile, i)
                        .Where(x => !(shouldExcludePlayer && x.IsPlayer))
                        .ToHashSet();
                    adj.Add(tile);
                    var perimeter = adj.Except(lastAdj).ToHashSet();
                    if(!perimeter.Any()) break;
                    if(!perimeter.All(blastZones.ContainsKey)) break;

                    var val = blastZones[perimeter.First()];
                    if (perimeter.All(x => blastZones[x] == val) && val == startingBlastDanger - i)
                    {
                        if (val == 1) return true;
                    }
                    else break;
                    

                    lastAdj = adj;
                }

            }

            return false;
        }



        private bool IsTileDangerousToGo(Tile tile, Map customMap = null, bool shouldExcludePlayer = false)
        {
            if (customMap == null) customMap = map;
            if (IsTileDeadlyToGo(tile, customMap, shouldExcludePlayer))
                return true;
            if (IsTileDeadlyToGo(tile, customMap, shouldExcludePlayer, false))
                return true;


            return customMap.GetAdjacent(tile).Any(x => x.IsMonster);
        }


        private AgentAction Panic()
        {
            //Console.WriteLine("PANIC");
            HashSet<Tile> viableTurns = map.GetAdjacentWalkableTiles(myTile)
                .Where(x => !IsTileDeadlyToGo(x))
                .ToHashSet();

            if (viableTurns.Count == 0) //Blown yourself up if you gonna die anyway
            {
                return IsTileDeadlyToGo(myTile) ? AgentAction.BombUp : AgentAction.DoNothing;
            }
            if (viableTurns.Count == 1) return GameLib.DirectionTo(myTile, viableTurns.First());

            var finalTile = NotEnemiesBlastZonesOrInitial(NotDangerousOrInitial(viableTurns))
                .MaxBy(EvaluateSimpleTileSafety)
                .First();
            
            return GameLib.DirectionTo(myTile, finalTile);
        }

        private HashSet<Tile> NotDangerousOrInitial(HashSet<Tile> initial)
        {
            var notDangerous = initial
                .Where(x => !IsTileDangerousToGo(x))
                .ToHashSet();
            if (notDangerous.Count == 0) return initial.ToHashSet();
            return notDangerous;
        }

        private HashSet<Tile> NotEnemiesBlastZonesOrInitial(HashSet<Tile> initial)
        {
            var enemiesBlastZones = map.AllEnemyPlayers
                .Where(x => IsTileDangerousToGo(x))
                .SelectMany(x => map.GetChainBombBlastZone(x))
                .ToHashSet();
            var notInBlastZones = initial
                .Except(enemiesBlastZones)
                .ToHashSet();

            if (notInBlastZones.Count == 0) return initial.ToHashSet();
            return notInBlastZones;
        }


        private Tile? GetNearestWalkableTileToPlaceBomb(Tile tile, out int priority)
        {
            var directions = new HashSet<VectorInt2>
            {
                new VectorInt2(tile.X, tile.Y + 2),
                new VectorInt2(tile.X, tile.Y - 2),
                new VectorInt2(tile.X + 2, tile.Y),
                new VectorInt2(tile.X - 2, tile.Y)
            };


            var notTooCloseTiles = map.GetAdjacentWalkableTiles(tile, 2)
                .Where(x => directions.Contains(x.Position))
                .Where(x => pathfinder.IsPathExists(x))
                .Where(x => !IsTileDangerousToGo(pathfinder.NextTileOnPath(x)))
                .ToHashSet();
            var finalSet = notTooCloseTiles;
            if (notTooCloseTiles.Count == 0)
            {
                finalSet = map.GetAdjacentWalkableTiles(tile)
                    .Where(x => pathfinder.IsPathExists(x))
                    .Where(x => !IsTileDangerousToGo(pathfinder.NextTileOnPath(x)))
                    .ToHashSet();
            }

            priority = 0;
            if (finalSet.Count == 0) return null;

            var final = finalSet.MinBy(x => pathfinder.GetDistance(x))
                .MaxBy(x => GetTargetsInBombRadius(x).Count)
                .First();

            priority = GetTargetsInBombRadius(final).Count;

            return final;
        }

        private int EvaluateSimpleTileSafety(Tile tile)
        {
            var safety = 100;
            if(!tile.IsWalkable) throw new ArgumentException();
            HashSet<Tile> adjacent = map.GetAdjacent(tile);

            int wallMultiplier = 1;
            foreach (var adj in adjacent)
            {
                if (adj.IsObstacle) safety -= 8 * wallMultiplier++;
                if (adj.IsEnemyPlayer) safety -= 80;
                if (map.IsTileHasAdjacentMonsters(adj)) safety -= 15;
                if (adj.IsMonster) safety -= 100;
            }

            return Math.Clamp(safety, 0, 100);
        }

        private bool MischiefChance()
        {
            var enemies = map.AllEnemyPlayers;
            var criticalPoints = new HashSet<Tile>();
            var myBlastZone = map.GetBombBlastZone(myTile);

            if (map.GetAdjacentWalkableTiles(myTile)
                .SelectMany(x => map.GetChainBombBlastZone(x))
                .Any(x => enemies.Contains(x)))
                return true;

            foreach (var en in enemies.Where(x=> IsTileDangerousToGo(x)))
            {
                var viableTurns = map.GetAdjacentWalkableTiles(en)
                    .Where(x => !IsTileDangerousToGo(x))
                    .ToHashSet();
                if (viableTurns.Count == 1 )
                    criticalPoints.Add(viableTurns.First());
            }

            //Console.WriteLine(criticalPoints.Count);
            if (criticalPoints.Count == 0) return false; 
            if (criticalPoints.Any(x=> myBlastZone.Contains(x))) return true;


            var mapAfterBombing = GetMapMockupAfterBombPlaced();

            foreach (var point in criticalPoints)
            {
                if (IsTileDangerousToGo(point, mapAfterBombing)) return true;
            }

            return false;
        }

    }
}
