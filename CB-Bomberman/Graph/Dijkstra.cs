using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using CB_Bomberman.Pathfinding.Graph;
using Priority_Queue;

namespace CB_Bomberman.Pathfinding
{
    public class Dijkstra<T>
    {
        public static IDictionary<GraphNode<T>, GraphNode<T>> FindDistancesFrom(Graph<T> graph, GraphNode<T> start)
        {
            IDictionary<GraphNode<T>, int> distances = new Dictionary<GraphNode<T>, int>();
            IDictionary<GraphNode<T>, GraphNode<T>> nodeParents = new Dictionary<GraphNode<T>, GraphNode<T>>();
            SimplePriorityQueue<GraphNode<T>, int> unexploredNodes = new SimplePriorityQueue<GraphNode<T>, int>();
            HashSet<GraphNode<T>> exploredNodes = new HashSet<GraphNode<T>>();

            unexploredNodes.Enqueue(start, 0);

            foreach (var node in graph.Nodes.Values)
            {
                //nodeParents.Add(new KeyValuePair<GraphNode<T>, GraphNode<T>>(node, null));
                //unexploredNodes.Add(node);
                distances.Add(new KeyValuePair<GraphNode<T>, int>(node, int.MaxValue));
            }

            distances[start] = 0;

            while (unexploredNodes.Count > 0)
            {
                var currentNode = unexploredNodes.Dequeue();
                exploredNodes.Add(currentNode);

                foreach (var edge in currentNode.Edges)
                {
                    var otherNode = edge.GetOther(currentNode);
                    if (exploredNodes.Contains(otherNode)) continue;
                    if(!unexploredNodes.Contains(otherNode)) unexploredNodes.Enqueue(otherNode, int.MaxValue);
                    if(!nodeParents.ContainsKey(otherNode)) nodeParents.Add(otherNode, currentNode);
                    int dist = distances[currentNode] + edge.Weight;
                    if (dist >= distances[otherNode]) continue;
                    distances[otherNode] = dist;
                    nodeParents[otherNode] = currentNode;
                }

                exploredNodes.Add(currentNode);
            }

            return nodeParents;
        }
    }
}
