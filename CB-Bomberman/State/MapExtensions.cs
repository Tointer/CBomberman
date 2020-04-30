using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using CB_Bomberman.Game;
using CB_Bomberman.Pathfinding.Graph;
using VectorInt;

namespace CB_Bomberman.State
{
    public static class MapExtensions
    {
        public static Graph<Tile> ToFullGraph(this Map map)
        {
            HashSet<Tile> visitedTiles = new HashSet<Tile>();
            Graph<Tile> result = new Graph<Tile>();

            foreach (var tile in map.Values)
            {
                visitedTiles.Add(tile);
                if(tile.TileType == TileTypes.StrongWall) continue;
                if (!result.Nodes.ContainsKey(tile))
                    result.AddNode(tile);
                foreach (var adjTile in map.GetAdjacent(tile))
                {
                    if(adjTile.TileType == TileTypes.StrongWall || visitedTiles.Contains(adjTile)) continue;
                    if (!result.Nodes.ContainsKey(adjTile))
                        result.AddNode(adjTile);
                    result.AddEdge(tile, adjTile);
                }
            }

            return result;
        }


        public static Tile TileAfterAction(this Map map, Tile tile, AgentAction action)
        {
            switch (action)
            {
                case AgentAction.DoNothing:
                    return tile;
                case AgentAction.GoRight:
                    return map.GetRight(tile) == null ? tile : map.GetRight(tile).Value;
                case AgentAction.GoLeft:
                    return map.GetLeft(tile) == null ? tile : map.GetLeft(tile).Value;
                case AgentAction.GoUp:
                    return map.GetUp(tile) == null ? tile : map.GetUp(tile).Value;
                case AgentAction.GoDown:
                    return map.GetDown(tile) == null ? tile : map.GetDown(tile).Value;
                case AgentAction.BombLeft:
                    return map.GetLeft(tile) == null ? tile : map.GetLeft(tile).Value;
                case AgentAction.BombRight:
                    return map.GetRight(tile) == null ? tile : map.GetRight(tile).Value;
                case AgentAction.BombUp:
                    return map.GetUp(tile) == null ? tile : map.GetUp(tile).Value;
                case AgentAction.BombDown:
                    return map.GetDown(tile) == null ? tile : map.GetDown(tile).Value;
                case AgentAction.Debug:
                    return tile;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        public static Tile? GetUp(this IReadOnlyDictionary<VectorInt2, Tile> rawMap, Tile tile, int amount = 1)
        {
            var upTilePos = new Vector2(tile.X, tile.Y+amount);
            if (rawMap.ContainsKey(upTilePos)) return rawMap[upTilePos];
            return null;
        }

        public static Tile? GetDown(this IReadOnlyDictionary<VectorInt2, Tile> rawMap,Tile tile, int amount = 1)
        {
            var upTilePos = new Vector2(tile.X, tile.Y-amount);
            if (rawMap.ContainsKey(upTilePos)) return rawMap[upTilePos];
            return null;
        }
        public static Tile? GetRight(this IReadOnlyDictionary<VectorInt2, Tile> rawMap,Tile tile, int amount = 1)
        {
            var upTilePos = new Vector2(tile.X+amount, tile.Y);
            if (rawMap.ContainsKey(upTilePos)) return rawMap[upTilePos];
            return null;
        }
        public static Tile? GetLeft(this IReadOnlyDictionary<VectorInt2, Tile> rawMap,Tile tile, int amount = 1)
        {
            var upTilePos = new Vector2(tile.X-amount, tile.Y);
            if (rawMap.ContainsKey(upTilePos)) return rawMap[upTilePos];
            return null;
        }

        public static HashSet<Tile> GetAdjacent(this IReadOnlyDictionary<VectorInt2, Tile> rawMap, Tile tile)
        {
            var result = new HashSet<Tile>();
            var tempTile = rawMap.GetUp(tile);

            if(tempTile != null) result.Add(tempTile.Value);
            tempTile = rawMap.GetDown(tile);
            if(tempTile != null) result.Add(tempTile.Value);
            tempTile = rawMap.GetRight(tile);
            if(tempTile != null) result.Add(tempTile.Value);
            tempTile = rawMap.GetLeft(tile);
            if(tempTile != null) result.Add(tempTile.Value);

            return result;
        }

        public static HashSet<Tile> GetBombBlastZone(this IReadOnlyDictionary<VectorInt2, Tile> rawMap, Tile bombTile,
            HashSet<TileTypes> additionalTilesToStopBlast = null)
        {
            var result = new HashSet<Tile> {bombTile};
            var tilesToStopBlast = new HashSet<TileTypes> {TileTypes.Wall};
            if(additionalTilesToStopBlast != null) tilesToStopBlast.UnionWith(additionalTilesToStopBlast);

            for (var i = 1; i < GameLib.BombExplosionPower; i++)
            {
                var pos = new VectorInt2(bombTile.X, bombTile.Y + i);
                if(!rawMap.ContainsKey(pos)) break;
                var up = rawMap[pos];
                if(up.IsStrongWall) break;
                result.Add(up);
                if(tilesToStopBlast.Contains(up.TileType)) break;
            }
            for (var i = 1; i < GameLib.BombExplosionPower; i++)
            {
                var pos = new VectorInt2(bombTile.X, bombTile.Y-i);
                if(!rawMap.ContainsKey(pos)) break;
                var down = rawMap[pos];
                if(down.IsStrongWall) break;
                result.Add(down);
                if(tilesToStopBlast.Contains(down.TileType)) break;
            }
            for (var i = 1; i < GameLib.BombExplosionPower; i++)
            {
                var pos = new VectorInt2(bombTile.X + i, bombTile.Y);
                if(!rawMap.ContainsKey(pos)) break;
                var right = rawMap[pos];
                if(right.IsStrongWall) break;
                result.Add(right);
                if(tilesToStopBlast.Contains(right.TileType)) break;
            }
            for (var i = 1; i < GameLib.BombExplosionPower; i++)
            {
                var pos = new VectorInt2(bombTile.X - i, bombTile.Y);
                if(!rawMap.ContainsKey(pos)) break;
                var left = rawMap[pos];
                if(left.IsStrongWall) break;
                result.Add(left);
                if(tilesToStopBlast.Contains(left.TileType)) break;
            }
            return result;
        }

        public static HashSet<Tile> GetChainBombBlastZone(this IReadOnlyDictionary<VectorInt2, Tile> rawMap, Tile bombTile, bool enemiesStandStill = false)
        {
            var a = new HashSet<TileTypes> {TileTypes.EnemyPlayer, TileTypes.Ghost};
            var blastZone = enemiesStandStill ? rawMap.GetBombBlastZone(bombTile, a) : rawMap.GetBombBlastZone(bombTile);
            var visitedBombs = new HashSet<Tile>();

            while (true)
            {
                var viableBombs = blastZone.Where(x => x.IsBomb && !visitedBombs.Contains(x)).ToHashSet();
                if(viableBombs.Count == 0) break;

                var bomb = viableBombs.First();
                blastZone.UnionWith(
                    enemiesStandStill ? rawMap.GetBombBlastZone(bomb, a) : rawMap.GetBombBlastZone(bomb));
                visitedBombs.Add(bomb);

            }

            return blastZone;
        }

        public static Dictionary<Tile, int> GetAllBlastZones(this IReadOnlyDictionary<VectorInt2, Tile> rawMap, IReadOnlyDictionary<VectorInt2, Tile> alternativeMap = null, bool enemiesStandStill = false)
        {
            Dictionary<Tile, int> blastZone = new Dictionary<Tile, int>();
            var map = rawMap;
            if (alternativeMap != null)
            {
                map = alternativeMap;
            }

            var bombs = map.Values.Where(x => x.IsBomb).ToHashSet();

            foreach (var bomb in bombs)
            {
                int estimated = 1;
                if (bomb.TileType == TileTypes.Bomb2) estimated = 2;
                if (bomb.TileType == TileTypes.Bomb3) estimated = 3;
                if (bomb.TileType == TileTypes.Bomb4) estimated = 4;
                if (bomb.TileType == TileTypes.Bomb5) estimated = 5;
                foreach (var tile in map.GetChainBombBlastZone(bomb, enemiesStandStill))
                {
                    if (blastZone.ContainsKey(tile))
                    {
                        if(blastZone[tile] > estimated)
                            blastZone[tile] = estimated;
                    }
                    else blastZone.Add(tile, estimated);
                }
            }
            //Console.WriteLine("BOOOOL " + enemiesStandStill);
            //foreach (var tile1 in blastZone.Keys)
            //{
            //    Console.WriteLine(tile1);
            //}
            return blastZone;

        }

        public static Tile[,] GetTilesInArea(this IReadOnlyDictionary<VectorInt2, Tile> rawMap, Tile tile, int radius)
        {
            var side = radius * 2 + 1;
            var result = new Tile[radius*2+1,radius*2+1];
            var startPosition = new VectorInt2(tile.X - radius, tile.Y + radius);

            for (var y = 0; y < side; y++)
            {
                for (var x = 0; x < side; x++)
                {
                    var newPosition = new VectorInt2(startPosition.X + x, startPosition.Y - y);
                    if (rawMap.ContainsKey(newPosition)) result[x, y] = rawMap[newPosition];
                    else result[x,y] = new Tile(default, TileTypes.StrongWall);
                }
            }

            return result;
        }
        public static HashSet<Tile> GetAdjacentWalkableTiles(this IReadOnlyDictionary<VectorInt2, Tile> rawMap, Tile tile, int steps = 1, bool shouldExcludePlayer = false, bool shouldIncludeBlast = false)
        {
            var visited = new HashSet<Tile> {tile};
            if (steps == 0) return visited;

            for (var i = 0; i < steps; i++)
            {
                var newWave = new HashSet<Tile>();
                foreach (var nTile in visited)
                {
                    var adj = rawMap
                        .GetAdjacent(nTile)
                        .Where(x => x.IsWalkable || (shouldIncludeBlast && x.TileType == TileTypes.Blast))
                        .Where(x => !(shouldExcludePlayer && x.IsPlayer));
                    newWave.UnionWith(adj);
                }
                visited.UnionWith(newWave);

            }
            visited.Remove(tile);
            return visited;
        }

        public static int GetManhattanDistance(this IReadOnlyDictionary<VectorInt2, Tile> rawMap, Tile tile1, Tile tile2)
        {
            return Math.Abs(tile1.X - tile2.X) + Math.Abs(tile1.Y - tile2.Y);
        }

        public static bool IsTileHasAdjacentMonsters(this IReadOnlyDictionary<VectorInt2, Tile> rawMap, Tile tile)
        {
            return rawMap.GetAdjacent(tile).Any(x => x.IsMonster);
        }

    }


}
