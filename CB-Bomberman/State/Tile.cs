using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using VectorInt;

namespace CB_Bomberman.State
{
    public struct Tile
    {
        public int X;
        public int Y;
        public VectorInt2 Position => new VectorInt2(X,Y);
        public TileTypes TileType;

        public Tile(VectorInt2 position, TileTypes tileType)
        {
            X = position.X;
            Y = position.Y;
            TileType = tileType;
        }

        public Tile(int x, int y, TileTypes tileType)
        {
            X = x;
            Y = y;
            TileType = tileType;
        }

        public static bool operator== (Tile obj1, Tile obj2)
        {
            return obj1.Equals(obj2);
        }
        public static bool operator!= (Tile obj1, Tile obj2)
        {
            return !(obj1 == obj2);
        }

        public bool IsWalkable => TileType == TileTypes.Empty || TileType == TileTypes.Player;
        public bool IsMonster => TileType == TileTypes.Ghost;
        public bool IsEnemyPlayer => TileType == TileTypes.EnemyPlayer;
        public bool IsRegularWall => TileType == TileTypes.Wall;
        public bool IsBomb => TileType == TileTypes.Bomb1 || TileType == TileTypes.Bomb2 || TileType == TileTypes.Bomb3 ||
                              TileType == TileTypes.Bomb4 || TileType == TileTypes.Bomb5;
        public bool IsPlayer => TileType == TileTypes.Player;
        public bool IsStrongWall => TileType == TileTypes.StrongWall;
        public bool IsObstacle => IsRegularWall || IsStrongWall;

        public int GetBombTick()
        {
            switch (TileType)
            {
                case TileTypes.Bomb1:
                    return 1;
                case TileTypes.Bomb2:
                    return 2;
                case TileTypes.Bomb3:
                    return 3;
                case TileTypes.Bomb4:
                    return 4;
            }
            throw new InvalidOperationException("This tile is not bomb");
        }

        public Tile newTileOnSamePosition(TileTypes newType)
        {
            return new Tile(Position, newType);
        }

        public override string ToString()
        {
            return $"{TileType}({X}, {Y})";
        }
    }
}
