using System.Linq;
using CB_Bomberman.Pathfinding.Graph;
using CB_Bomberman.State;
using NUnit.Framework;

namespace BombermanTests
{
    public class GraphShould
    {
        private Graph<Tile> graph;
        private Tile strongWallTile;
        private Tile emptyTile;
        private Tile playerTile;

        [SetUp]
        public void Setup()
        {
            graph = new Graph<Tile>();
            strongWallTile = new Tile(1, 2, TileTypes.StrongWall);
            emptyTile = new Tile(1, 1, TileTypes.Empty);
            playerTile = new Tile(1,3, TileTypes.Player);

        }

        [Test]
        public void AddOneNode()
        {
            graph.AddNode(strongWallTile);
            Assert.That(graph.Nodes.Count == 1 && graph.Nodes.ContainsKey(strongWallTile));
        }

        [Test]
        public void AddEdge()
        {
            graph.AddNode(strongWallTile);
            graph.AddNode(emptyTile);
            graph.AddEdge(strongWallTile, emptyTile);
            var edge = graph.Edges.First();

           Assert.That(edge.GetOther(graph.Nodes[strongWallTile]) == graph.Nodes[emptyTile]);
           Assert.That(edge.GetOther(graph.Nodes[emptyTile]) == graph.Nodes[strongWallTile]);

           Assert.That(edge.GetOther(strongWallTile) == emptyTile);
           Assert.That(edge.GetOther(emptyTile) == strongWallTile);
        }

        [Test]
        public void DeleteNode()
        {
            graph.AddNode(strongWallTile);
            graph.AddNode(emptyTile);
            graph.AddNode(playerTile);
            graph.DeleteNode(emptyTile);

            Assert.That(graph.Nodes.Count == 2);
            Assert.That(!graph.Nodes.ContainsKey(emptyTile));
            Assert.That(graph.Nodes.ContainsKey(strongWallTile));
            Assert.That(graph.Nodes.ContainsKey(playerTile));
        }

        [Test]
        public void DeleteEdge()
        {
            graph.AddNode(strongWallTile);
            graph.AddNode(emptyTile);
            graph.AddEdge(strongWallTile, emptyTile);
            graph.DeleteEdge(strongWallTile, emptyTile);
            Assert.AreEqual(0, graph.Edges.Count);
        }

        [Test]
        public void DeleteNodeWithEdges()
        {
            graph.AddNode(strongWallTile);
            graph.AddNode(emptyTile);
            graph.AddNode(playerTile);
            graph.AddEdge(strongWallTile, emptyTile);
            graph.AddEdge(playerTile, emptyTile);
            graph.AddEdge(playerTile, strongWallTile);
            Assert.AreEqual(3, graph.Edges.Count);
            Assert.AreEqual(2, graph.Nodes[emptyTile].Edges.Count);
            graph.DeleteNode(emptyTile);
            Assert.AreEqual(1, graph.Edges.Count);
        }

    }
}