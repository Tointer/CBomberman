using System;
using System.Collections.Generic;
using System.Text;

namespace CB_Bomberman.Game
{
    public struct GameInfo
    {
        public readonly bool BombAvailable;

        public GameInfo(bool bombAv)
        {
            BombAvailable = bombAv;
        }
    }
}
