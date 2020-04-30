using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CB_Bomberman.Pathfinding.Graph
{
    public class GraphNode<TVal>
    {
        public GraphNode(TVal value)
        {
            Edges = new List<Edge<TVal>>();
            Value = value;
        }

        public bool IsAdjacent(GraphNode<TVal> other)
        {
            return Edges.Any(edge => edge.GetOther(this) == other);
        }

        public int DistanceToAdjacent(GraphNode<TVal> other)
        {
            if(!IsAdjacent(other)) throw new ArgumentException("this node is not adjacent");
            return Edges.First(x => x.GetOther(this) == other).Weight;
        }

        public List<GraphNode<TVal>> GetAllAdjacent()
        {
            return Edges.Select(edge => edge.GetOther(this)).ToList();
        }

        public List<Edge<TVal>> Edges { get; set; }
        public TVal Value { get; set; }
    }
}
