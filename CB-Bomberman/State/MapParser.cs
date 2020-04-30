using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using VectorInt;

namespace CB_Bomberman.State
{
    public class MapParser
    {
        public readonly Dictionary<char, TileTypes> Codes;

        public MapParser(Dictionary<char, TileTypes> codes)
        {
            Codes = codes;
        }

        public Map ParseMapFromString(string str, VectorInt2 size)
        {
            if(str.Length != size.Y*size.X) throw new ArgumentException();
            var result = new Dictionary<VectorInt2, Tile>();
            for (var i = 0; i < size.Y; i++)
            {
                for (var j = 0; j < size.X; j++)
                {
                    var pos = new VectorInt2(j, size.Y - i - 1);
                    var tileType = Codes[str[size.X * i + j]];
                    result.Add(pos, new Tile(pos,tileType));
                }
            }

            return new Map(size, result);
        }
    }
}
