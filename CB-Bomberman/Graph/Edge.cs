using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace CB_Bomberman.Pathfinding.Graph
{
    public sealed class Edge<TVal>
    {
        public int Weight { get; set; }

        public GraphNode<TVal> FirstNode { get; private set; }
        public GraphNode<TVal> SecondNode { get; private set; }

        public GraphNode<TVal> GetOther(GraphNode<TVal> node)
        {
            if (node == FirstNode) return SecondNode;
            if (node == SecondNode) return FirstNode;
            throw new ArgumentException("edge does not contain this node");
        }

        public TVal GetOther(TVal tile)
        {
            if (tile.Equals(FirstNode.Value)) return SecondNode.Value;
            if (tile.Equals(SecondNode.Value)) return FirstNode.Value;
            throw new ArgumentException("edge does not contain this node");
        }

        public bool Contains(GraphNode<TVal> node)
        {
            if (node == FirstNode) return true;
            return node == SecondNode;
        }

        public Edge(GraphNode<TVal> firstNode, GraphNode<TVal> secondNode, int weight)
        {
            Weight = weight;
            FirstNode = firstNode;
            SecondNode = secondNode;
        }



    }
}
