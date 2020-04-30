using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CB_Bomberman.Game;
using CB_Bomberman.Pathfinding.Graph;

namespace CB_Bomberman.State
{
    static class GraphExtensions
    {
        public static Graph<Tile> DeleteTiles(this Graph<Tile> graph, Func<Tile, bool> criteria)
        {
            HashSet<Tile> tilesForDelete = graph.Nodes.Keys.Where(criteria).ToHashSet();
            foreach (var tile in tilesForDelete)
            {
                graph.DeleteNode(tile);
            }

            return graph;
        }

        public static Graph<Tile> IncreaseWeightAround(this Graph<Tile> graph, Func<Tile, bool> criteria, int radius, int power = 4)
        {
            HashSet<Edge<Tile>> edges = new HashSet<Edge<Tile>>();
            HashSet<GraphNode<Tile>> visitedNodes = new HashSet<GraphNode<Tile>>();

            HashSet<GraphNode<Tile>> unvisitedNodes = graph.Nodes.Keys
                .Where(criteria)
                .Select(x => graph.Nodes[x])
                .ToHashSet();

            for (int i = 0; i < radius; i++)
            {
                var newWave = new HashSet<GraphNode<Tile>>();
                foreach (var node in unvisitedNodes)
                {
                    visitedNodes.Add(node);
                    edges.UnionWith(node.Edges);
                    newWave.UnionWith(node.GetAllAdjacent());
                }
                unvisitedNodes.UnionWith(newWave);
                unvisitedNodes.ExceptWith(visitedNodes);

                foreach (var edge in edges)
                {
                    edge.Weight += power;
                }
            }

            return graph;
        }


        public static Graph<Tile> IncreaseWeightInBlastZones(this Graph<Tile> graph)
        {
            HashSet<Edge<Tile>> edges = new HashSet<Edge<Tile>>();
            HashSet<GraphNode<Tile>> visitedNodes = new HashSet<GraphNode<Tile>>();
            HashSet<GraphNode<Tile>> unvisitedNodes = graph.Nodes.Keys
                .Where(x => x.TileType == TileTypes.Bomb2)
                .Select(x => graph.Nodes[x])
                .ToHashSet();
            return graph;
        }

        public static GraphNode<Tile> GetUp(this Graph<Tile> graph, GraphNode<Tile> node)
        {
            return node.GetAllAdjacent().FirstOrDefault(x => x.Value.Y - node.Value.Y == 1);
        }
        public static GraphNode<Tile> GetDown(this Graph<Tile> graph, GraphNode<Tile> node)
        {
            return node.GetAllAdjacent().FirstOrDefault(x => x.Value.Y - node.Value.Y == -1);
        }
        public static GraphNode<Tile> GetRight(this Graph<Tile> graph, GraphNode<Tile> node)
        {
            return node.GetAllAdjacent().FirstOrDefault(x => x.Value.X - node.Value.X == 1);
        }
        public static GraphNode<Tile> GetLeft(this Graph<Tile> graph, GraphNode<Tile> node)
        {
            return node.GetAllAdjacent().FirstOrDefault(x => x.Value.X - node.Value.X == -1);
        }
    }
}
