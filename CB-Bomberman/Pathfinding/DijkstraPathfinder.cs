using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CB_Bomberman.Game;
using CB_Bomberman.Pathfinding.Graph;
using CB_Bomberman.State;

namespace CB_Bomberman.Pathfinding
{
    public class DijkstraPathfinder
    {
        private IDictionary<GraphNode<Tile>, GraphNode<Tile>> paths;
        private Graph<Tile> currentGraph;
        private Tile startingPos;

        public bool IsPathExists(Tile to)
        {
            return currentGraph.Nodes.ContainsKey(to) && paths.ContainsKey(currentGraph.Nodes[to]);
        }

        public Tile NextTileOnPath(Tile to)
        {
            if(!IsPathExists(to)) throw  new ArgumentException();
            var path = GetPath(to);
            return path[^2].Value;
        }

        public int GetDistance(Tile to)
        {
            if (!IsPathExists(to)) return int.MaxValue;
            var path = GetPath(to);
            var distance = 0;
            for (var i = 1; i < path.Count; i++)
            {
                distance += path[i - 1].DistanceToAdjacent(path[i]);
            }
            return distance;
        }

        public int GetAbsoluteDistance(Tile to)
        {
            return GetPath(to).Count;
        }

        public void FindAllPaths(Graph<Tile> graph, Tile startingPoint)
        {
            currentGraph = graph;
            startingPos = startingPoint;
            paths = Dijkstra<Tile>.FindDistancesFrom(currentGraph, currentGraph.Nodes[startingPoint]);
        }

        public  List<GraphNode<Tile>> GetPath(Tile to)
        {
            if (!currentGraph.Nodes.ContainsKey(startingPos) || !currentGraph.Nodes.ContainsKey(startingPos)
                || !paths.ContainsKey(currentGraph.Nodes[to]))
                throw new ArgumentException();
            if(!IsPathExists(to)) throw new ArgumentException();

            var result = new List<GraphNode<Tile>>();
            var curNode = currentGraph.Nodes[to];
            result.Add(curNode);

            while (paths.ContainsKey(curNode))
            {
                curNode = paths[curNode];
                result.Add(curNode);
            }

            return result;
        }
    }
}
