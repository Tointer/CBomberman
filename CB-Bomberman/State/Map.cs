
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using CB_Bomberman.Game;
using VectorInt;

namespace CB_Bomberman.State
{
    public class Map: IReadOnlyDictionary<VectorInt2, Tile>
    {
        private readonly Dictionary<VectorInt2, Tile> Tiles;
        public readonly VectorInt2 Size;
        public readonly Tile PlayerTile;
        

        public HashSet<Tile> AllBombs => allBombs.Value;
        private readonly Lazy<HashSet<Tile>> allBombs;

        public Dictionary<Tile, int> BlastZones => blastZones.Value;
        private readonly Lazy< Dictionary<Tile, int>> blastZones;

        public HashSet<Tile> AllEnemyPlayers => allEnemyPlayers.Value;
        private readonly Lazy<HashSet<Tile>> allEnemyPlayers;

        public HashSet<Tile> AllMonsters => allMonsters.Value;
        private readonly Lazy<HashSet<Tile>> allMonsters;

        public HashSet<Tile> AllWalls => allWalls.Value;
        private readonly Lazy<HashSet<Tile>> allWalls;


        public Map(VectorInt2 size, Dictionary<VectorInt2, Tile> tiles)
        {
            Size = size;
            Tiles = tiles;
            PlayerTile = Tiles.Values.FirstOrDefault(x => x.IsPlayer);

            allBombs = new Lazy<HashSet<Tile>>(Tiles.Values.Where(x => x.IsBomb).ToHashSet());
            allEnemyPlayers = new Lazy<HashSet<Tile>>(Tiles.Values.Where(x => x.IsEnemyPlayer).ToHashSet());
            allMonsters = new Lazy<HashSet<Tile>>(Tiles.Values.Where(x => x.IsMonster).ToHashSet());
            allWalls = new Lazy<HashSet<Tile>>(Tiles.Values.Where(x => x.IsRegularWall).ToHashSet);
            blastZones = new Lazy<Dictionary<Tile, int>>(Tiles.GetAllBlastZones());
        }


        public Tile this[int x, int y] => Tiles[new VectorInt2(x, y)];

        public bool ContainsKey(VectorInt2 key)
        {
            return Tiles.ContainsKey(key);
        }

        public bool TryGetValue(VectorInt2 key, out Tile value)
        {
            return Tiles.TryGetValue(key, out value);
        }

        public Tile this[VectorInt2 pos] => Tiles[pos];
        public IEnumerable<VectorInt2> Keys => Tiles.Keys;
        public IEnumerable<Tile> Values => Tiles.Values;

        public IEnumerator<KeyValuePair<VectorInt2, Tile>> GetEnumerator()
        {
            return Tiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Tiles.GetEnumerator();
        }

        public int Count => Tiles.Count;
    }



}
