using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CB_Bomberman;
using CB_Bomberman.Game;
using CB_Bomberman.Pathfinding.Graph;
using CB_Bomberman.Pathfinding;
using CB_Bomberman.State;
using NUnit.Framework;
using VectorInt;

namespace BombermanTests
{
    class BroadTests
    {
        public static string GameField1 =
            "0 0 0 0 0 0 0 0" +
            "0 P O O - - - 0" +
            "0 - - - - - O 0" +
            "0 - 0 O O - - 0" +
            "0 - - 0 - - - 0" +
            "0 - - - - - - 0" +
            "0 0 0 0 0 0 0 0 ";
        public static string GameField2 =
            "0 0 0 0 0 0 0 0" +
            "- - 0 M - - 0 0" +
            "- - E 1 - - 0 0" +
            "0 - - P - - 0 0" +
            "0 - - - - - 0 0" +
            "0 - - - - - 0 0" +
            "0 - - 0 0 0 0 0";

        private readonly Dictionary<char, TileTypes> codes = GameLib.TestCodes;

        private Mastermind mind;
        private MapParser mapParser;
        private Graph<Tile> graph;
        private Map map;
        private DijkstraPathfinder pathfinder = new DijkstraPathfinder();
        private Tile playerTile;

        [SetUp]
        public void Setup()
        {
            GameField1 = string.Join("", GameField1.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            mapParser = new MapParser(codes);
            map = mapParser.ParseMapFromString(GameField1, new VectorInt2(8, 7));
            graph = map.ToFullGraph();
            mind = new Mastermind(""); 
            pathfinder = new DijkstraPathfinder();
            playerTile = map.Values.First(x => x.IsPlayer);
            pathfinder.FindAllPaths(graph, playerTile);
        }

        [Test]
        public void RightTilesInMap()
        {
            Assert.AreEqual(GameField1.Length, 56);
            foreach (var tileTypes in codes)
            {
                var symbolCount = GameField1.Count(x => x == tileTypes.Key);
                var tilesCount = map.Values.Count(x => x.TileType == tileTypes.Value);
                Assert.AreEqual(symbolCount, tilesCount);
            }
        }

        [Test]
        public void RightNodesInGraph()
        {
            foreach (var tileTypes in codes)
            {
                if(tileTypes.Value == TileTypes.StrongWall) continue;
                var symbolCount = GameField1.Count(x => x == tileTypes.Key);
                var nodesCount = graph.Nodes.Keys.Count(x => x.TileType == tileTypes.Value);
                Assert.AreEqual(symbolCount, nodesCount);
            }
        }

        [Test]
        public void SimplePath()
        {
            var path = pathfinder.GetPath(map[1, 4]);
            var allPaths = Dijkstra<Tile>.FindDistancesFrom(graph, graph.Nodes[playerTile]);
            Assert.AreNotEqual(0, allPaths.Count);

            Assert.AreEqual(allPaths[graph.Nodes[map[1,4]]], graph.Nodes[playerTile]);

            Assert.AreEqual(2, path.Count);
        }

        [Test]
        public void RightTilesInBlastZones()
        {
            var field = string.Join("", GameField2.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            map = mapParser.ParseMapFromString(field, new VectorInt2(8, 7));
            playerTile = map.Values.First(x => x.IsPlayer);
            Assert.AreEqual(1, map.AllBombs.Count);
            Assert.AreEqual(10, map.GetBombBlastZone(new Tile(new VectorInt2(3,4), TileTypes.Bomb1)).Count);
            Assert.AreEqual(10, map.GetChainBombBlastZone(new Tile(new VectorInt2(3,4), TileTypes.Bomb1)).Count);
            Assert.AreEqual(10, map.GetAllBlastZones().Count);
        }


        



    }
}
