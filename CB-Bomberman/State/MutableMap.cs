using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VectorInt;

namespace CB_Bomberman.State
{
    class MutableMap: IDictionary<VectorInt2, Tile>, IReadOnlyDictionary<VectorInt2,Tile>
    {
        private readonly Dictionary<VectorInt2, Tile> Tiles;


        public readonly VectorInt2 Size;
        public Tile PlayerTile => Tiles.Values.FirstOrDefault(x => x.IsPlayer);
        public HashSet<Tile> AllBombs => Tiles.Values.Where(x => x.IsBomb).ToHashSet();

        public Dictionary<Tile, int> BlastZones => blastZones.Value;
        private readonly Lazy< Dictionary<Tile, int>> blastZones;

        public HashSet<Tile> AllEnemyPlayers => Tiles.Values.Where(x => x.IsEnemyPlayer).ToHashSet();

        public HashSet<Tile> AllMonsters => Tiles.Values.Where(x => x.IsMonster).ToHashSet();

        public HashSet<Tile> AllWalls => Tiles.Values.Where(x => x.IsRegularWall).ToHashSet();


        public MutableMap(VectorInt2 size, Dictionary<VectorInt2, Tile> tiles)
        {
            Size = size;
            Tiles = tiles;
            blastZones = new Lazy<Dictionary<Tile, int>>(Tiles.GetAllBlastZones());
        }


        public Tile this[int x, int y] => Tiles[new VectorInt2(x, y)];

        public void Add(VectorInt2 key, Tile value)
        {
            Tiles.Add(key, value);
        }

        public bool ContainsKey(VectorInt2 key)
        {
            return Tiles.ContainsKey(key);
        }

        public Map ToImmutableMap()
        {
            return new Map(Size, new Dictionary<VectorInt2, Tile>(Tiles));
        }

        public bool Remove(VectorInt2 key)
        {
            return Tiles.Remove(key);
        }

        public bool TryGetValue(VectorInt2 key, out Tile value)
        {
            return Tiles.TryGetValue(key, out value);
        }

        public Tile this[VectorInt2 pos]
        {
            get => Tiles[pos];
            set => Tiles[pos] = value;
        }

        IEnumerable<VectorInt2> IReadOnlyDictionary<VectorInt2, Tile>.Keys => Keys;

        IEnumerable<Tile> IReadOnlyDictionary<VectorInt2, Tile>.Values => Values;

        public ICollection<VectorInt2> Keys => Tiles.Keys;
        public ICollection<Tile> Values => Tiles.Values;

        public IEnumerator<KeyValuePair<VectorInt2, Tile>> GetEnumerator()
        {
            return Tiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Tiles.GetEnumerator();
        }

        public void Add(KeyValuePair<VectorInt2, Tile> item)
        {
            Tiles.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Tiles.Clear();
        }

        public bool Contains(KeyValuePair<VectorInt2, Tile> item)
        {
            return Tiles.Contains(item);
        }

        public void CopyTo(KeyValuePair<VectorInt2, Tile>[] array, int arrayIndex)
        {
            var i = 0;
            foreach (var tile in Tiles)
            {
                array[i] = tile;
                i++;
            }
        }

        public bool Remove(KeyValuePair<VectorInt2, Tile> item)
        {
            return Tiles.Remove(item.Key);
        }


        public int Count => Tiles.Count;
        public bool IsReadOnly => false;
    }
}
