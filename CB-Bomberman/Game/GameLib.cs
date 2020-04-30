using System;
using System.Collections.Generic;
using System.Linq;
using CB_Bomberman.State;
using VectorInt;

namespace CB_Bomberman.Game
{
    public static class GameLib
    {
        public const int BombExplosionPower = 4;
        public const int MaxBombsCount = 4;
        public const int PlayerKillScore = 700;
        public const int MonsterKillScore = 250;
        public const int WallDestroyScore = 30;

        public static HashSet<Tile> GetWalkableTilesAroundBlast(Map map, Tile bomber)
        {
            var imaginaryBombBlastZone = map.GetChainBombBlastZone(bomber);
            var adjacentToBlastZone = new HashSet<Tile>();
            foreach (var tile in imaginaryBombBlastZone)
            {
                adjacentToBlastZone.UnionWith(map.GetAdjacent(tile));
            } 
            adjacentToBlastZone.ExceptWith(imaginaryBombBlastZone);
            return  adjacentToBlastZone.Where(x => x.IsWalkable).ToHashSet();
        }

        public static AgentAction GetRandomSafeDirection(Map map, Tile tile)
        {
            var rand = new Random();
            var newTile = map.GetAdjacent(tile)
                .Where(x => x.IsWalkable)
                .Where(x => !map.IsTileHasAdjacentMonsters(x))
                .Where(x => !map.BlastZones.ContainsKey(x) || map.BlastZones[x] > 2)
                .OrderBy((x) => rand.Next())
                .FirstOrDefault();
            return newTile == default ? GetRandomMoveDirection() : DirectionTo(tile, newTile); //may be problems at (0,0)
        }

        public static AgentAction GetRandomMoveDirection()
        {
            int random = new Random().Next(4);
            switch (random)
            {
                case 0: return AgentAction.GoDown;
                case 1: return AgentAction.GoLeft;
                case 2: return AgentAction.GoRight;
                case 3: return AgentAction.GoUp;
            }
            throw new Exception();
        }

        public static AgentAction DirectionTo(Tile from, Tile to)
        {
            if (from.X - to.X == 1) return AgentAction.GoLeft;
            if (from.X - to.X == -1) return AgentAction.GoRight;
            if (from.Y - to.Y == 1) return AgentAction.GoDown;
            if (from.Y - to.Y == -1) return AgentAction.GoUp;
            throw new ArgumentException();
        }



        public static AgentAction BombDirectionTo(Tile from, Tile to)
        {
            if (from.X - to.X == 1) return AgentAction.BombLeft;
            if (from.X - to.X == -1) return AgentAction.BombRight;
            if (from.Y - to.Y == 1) return AgentAction.BombDown;
            if (from.Y - to.Y == -1) return AgentAction.BombUp;
            throw new ArgumentException();
        }


        public static readonly Dictionary<char, TileTypes> Codes = new Dictionary<char, TileTypes>()
        {
            {'☺', TileTypes.Player },
            {'☻', TileTypes.Player },
            {'Ѡ', TileTypes.Unknown },
            {'♥', TileTypes.EnemyPlayer },
            {'♠', TileTypes.Bomb3 },
            {'♣', TileTypes.Blast },
            {'5', TileTypes.Bomb5 },
            {'4', TileTypes.Bomb4 },
            {'3', TileTypes.Bomb3 },
            {'2', TileTypes.Bomb2 },
            {'1', TileTypes.Bomb1 },
            {'҉', TileTypes.Blast },
            {'☼', TileTypes.StrongWall },
            {'#', TileTypes.Wall },
            {'H', TileTypes.Blast },
            {'&', TileTypes.Ghost },
            {'x', TileTypes.Blast },
            {' ', TileTypes.Empty },
            {'n', TileTypes.Empty },
        };
        public static readonly Dictionary<TileTypes, char> ReverseCodes = Codes.Reverse();

        public static readonly Dictionary<char, TileTypes> TestCodes = new Dictionary<char, TileTypes>
        {
            {'0', TileTypes.StrongWall},
            {'P', TileTypes.Player},
            {'O', TileTypes.Wall},
            {'-', TileTypes.Empty},
            {'1', TileTypes.Bomb1},
            {'2', TileTypes.Bomb2},
            {'3', TileTypes.Bomb3},
            {'4', TileTypes.Bomb4},
            {'5', TileTypes.Bomb5},
            {'E', TileTypes.EnemyPlayer},
            {'M', TileTypes.Ghost},
            {'#', TileTypes.Blast}
        };
        public static readonly Dictionary<TileTypes, char> ReverseTestCodes = TestCodes.Reverse();

        public static Dictionary<TValue, TKey> Reverse<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            var dictionary = new Dictionary<TValue, TKey>();
            foreach (var entry in source)
            {
                if(!dictionary.ContainsKey(entry.Value))
                    dictionary.Add(entry.Value, entry.Key);
            }
            return dictionary;
        } 
    }
}
