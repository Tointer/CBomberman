using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using CB_Bomberman.Pathfinding;
using CB_Bomberman.State;

namespace CB_Bomberman.Game
{
    class ConstantBomber : Agent
    {
        readonly DijkstraPathfinder pathfinder = new DijkstraPathfinder();
        private Tile myTile;
        private Map map;
        private GameInfo info;

        public override AgentAction MakeMove()
        {
            myTile = map.Values.First(x => x.IsPlayer);

            if (map.BlastZones.ContainsKey(myTile))
            {
                if (map.BlastZones[myTile] <= 2) return GameLib.GetRandomSafeDirection(map, myTile);
            }

            if (!info.BombAvailable) return GameLib.GetRandomSafeDirection(map, myTile);

            var graph = map.ToFullGraph().DeleteTiles(x => !x.IsWalkable);
            pathfinder.FindAllPaths(graph, myTile);

            var a = GameLib.GetWalkableTilesAroundBlast(map, myTile);
            if (a.Count != 0) 
                return PlantBomb(a);

            return GameLib.GetRandomSafeDirection(map, myTile);

        }

        private AgentAction PlantBomb(HashSet<Tile> safeTiles)
        {
            var tileTo = pathfinder.NextTileOnPath(safeTiles.OrderBy(x => pathfinder.GetDistance(x)).First());
            return GameLib.BombDirectionTo(myTile, tileTo);
        }

        public ConstantBomber(Map map, GameInfo info) : base(map, info)
        {
            this.map = map;
            this.info = info;
        }
    }
}
