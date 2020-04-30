using System;
using System.Collections.Generic;
using System.Text;
using CB_Bomberman.State;

namespace CB_Bomberman.Pathfinding.Graph
{
    public class Graph<TVal>
    {
        public readonly Dictionary<TVal, GraphNode<TVal>> Nodes = new Dictionary<TVal, GraphNode<TVal>>();
        public readonly HashSet<Edge<TVal>> Edges = new HashSet<Edge<TVal>>();

        public void AddEdge(TVal first, TVal second, int weight = 1)
        {
            if(!Nodes.ContainsKey(first) || !Nodes.ContainsKey(second))
                throw new ArgumentException(
                    $"there are no such nodes to connect them ({first} and {second})");
            var firstNode = Nodes[first];
            var secondNode = Nodes[second];
            var newEdge = new Edge<TVal>(firstNode, secondNode, weight);

            firstNode.Edges.Add(newEdge);
            secondNode.Edges.Add(newEdge);
            Edges.Add(newEdge);
        }

        public void AddNode(TVal value)
        {
            if(Nodes.ContainsKey(value)) 
                throw new ArgumentException("value already exists");
            
            Nodes.Add(value, new GraphNode<TVal>(value));
        }

        public void DeleteEdge(TVal first, TVal second)
        {
            var firstNode = Nodes[first];
            var secondNode = Nodes[second];

            if(!Nodes.ContainsKey(first) || !Nodes.ContainsKey(second))
                throw new ArgumentException("there are no such nodes");
            if(!firstNode.IsAdjacent(secondNode))
                throw new ArgumentException("these nodes are not connected already");

            for (var i = 0; i < firstNode.Edges.Count; i++)
            {
                var edge = firstNode.Edges[i];
                if (!edge.Contains(secondNode)) continue;
                DeleteEdge(edge);
            }
        }

        public void DeleteEdge(Edge<TVal> edge)
        {
            if(!Edges.Contains(edge)) 
                throw new ArgumentException("no such edge");
            edge.FirstNode.Edges.Remove(edge);
            edge.SecondNode.Edges.Remove(edge);
            Edges.Remove(edge);
        }

        public void DeleteNode(TVal val)
        {
            if(!Nodes.ContainsKey(val))
                throw new ArgumentException("no such node");
            HashSet<Edge<TVal>> edgesDorDelete = new HashSet<Edge<TVal>>();
            for (var i = 0; i < Nodes[val].Edges.Count; i++)
            {
                edgesDorDelete.Add(Nodes[val].Edges[i]);
            }

            foreach (var edge in edgesDorDelete)
            {
                DeleteEdge(edge);
            }

            Nodes.Remove(val);
        }
    }
}
