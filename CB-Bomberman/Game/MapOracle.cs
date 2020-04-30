using System;
using System.Collections.Generic;
using System.Text;
using CB_Bomberman.State;

namespace CB_Bomberman.Game
{
    class MapOracle
    {
        private Map oldMap;
        private Map currentMap;



        public void LoadNewMap(Map map)
        {
            if (oldMap == null) oldMap = map;
            else if (currentMap == null) currentMap = map;
            else
            {
                oldMap = currentMap;
                currentMap = map;
            }
        }

        //public HashSet<Tile> GivePlayersPrediction()
        //{

        //}

    }
}
