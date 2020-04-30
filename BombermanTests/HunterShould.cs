using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CB_Bomberman;
using CB_Bomberman.Game;
using CB_Bomberman.Pathfinding;
using CB_Bomberman.Pathfinding.Graph;
using CB_Bomberman.State;
using static MoreLinq.Extensions.ExceptByExtension;
using NUnit.Framework;
using VectorInt;

namespace BombermanTests
{
    class HunterShould
    {
        private Mastermind mind;
        private MapParser mapParser;
        private Graph<Tile> graph;
        private Map map;
        private Tile playerTile;
        private Hunter hunter;

        public static string Empty =
            "0 0 0 0 0 0 0" +
            "0 - - - - - 0" +
            "0 - - - - - 0" +
            "0 - - - - - 0" +
            "0 - - - - - 0" +
            "0 0 0 0 0 0 0";

        public static string Trap =
            "0 0 0 0 0 0 0" +
            "0 - - - - - 0" +
            "0 - 0 0 0 - 0" +
            "0 0 - P - 2 0" +
            "0 - - 0 E E 0" +
            "0 0 0 0 0 0 0";

        public static string Trap2 =
            "0 0 0 0 0 0 0" +
            "0 - - - - - 0" +
            "0 - 0 0 0 - 0" +
            "0 0 P - - 1 0" +
            "0 - - 0 E E 0" +
            "0 0 0 0 0 0 0";

        public static string Trap3 =
            "0 0 0 0 0 0 0" +
            "0 - 0 1 - E 0" +
            "0 - 0 - - O 0" +
            "0 - P - - - 0" +
            "0 - 0 - - E 0" +
            "0 0 0 0 0 0 0";

        public static string Trap4 =
            "- - - - - 0 0" +
            "- O - - - - 0" +
            "O - - 1 - - 0" +
            "- 0 P - - - 0" +
            "- - - - - - 0" +
            "0 0 E 0 0 0 0";

        public static string Trap5 =
            "0 0 0 M 0 0 0" +
            "0 - - - - - 0" +
            "0 1 - P 1 - 0" +
            "0 0 3 - - - 0" +
            "0 - - 4 E - 0" +
            "0 0 0 0 0 0 0";

        public static string Trap6 =
            "0 - - - - - 0" +
            "0 - 0 1 - - 0" +
            "0 - - P M - 0" +
            "0 - - E 0 - 0" +
            "0 - - - - - 0" +
            "0 0 0 0 0 0 0";

        public static string Trap7 =
            "0 - 0 0 - M 0" +
            "0 0 - - 0 - 0" +
            "0 - 2 P - - 0" +
            "0 0 E 4 0 - 0" +
            "0 - 0 0 - - 0" +
            "0 0 0 0 0 0 0";

        public static string Trap8 =
            "0 - - - - - 0" +
            "0 - 3 - - - 0" +
            "0 P - 0 - - 0" +
            "0 - - 0 - - 0" +
            "0 M - 0 - - 0" +
            "0 - - 0 0 0 0";

        public static string Trap9 =
            "0 - - - - - 0" +
            "0 - 1 - - - 0" +
            "0 3 - 0 - - 0" +
            "0 - P 0 - - 0" +
            "0 M - 0 - - 0" +
            "0 - - 0 0 0 0";

        public static string Trap10 =
            "0 0 0 0 0 0 0" +
            "- - - P - - 0" +
            "- E E - - 2 0" +
            "0 0 - E - 0 0" +
            "0 - - - - - 0" +
            "0 - - 0 0 0 0";

        public static string Trap11 =
            "0 0 0 0 0 0 0" +
            "E - - P - - 0" +
            "- - O - 3 - 0" +
            "0 0 0 0 - 0 0" +
            "0 - - - - - 0" +
            "0 - - 0 0 0 0";

        public static string Trap12 =
            "0 0 0 0 0 0 0" +
            "- - - P - - 0" +
            "- - 0 0 - - 0" +
            "0 - - E - 4 0" +
            "0 - - 0 - - 0" +
            "0 - - 0 0 0 0";

        public static string Trap13 =
            "0 0 0 0 1 0 0" +
            "- - 2 - - - 0" +
            "- - - P - E 0" +
            "0 0 0 - - - 0" +
            "0 - M - - - 0" +
            "0 - - 0 0 0 0";

        public static string Trap14 =
            "0 0 0 0 0 0 0" +
            "- - 0 M - - 0" +
            "- - E 1 - - 0" +
            "0 - - P - - 0" +
            "0 - - - - - 0" +
            "0 - - 0 0 0 0";

        public static string Trap15 =
            "0 E 0 0 0 0 0" +
            "- - - - - - 0" +
            "0 0 M - - - 0" +
            "0 - - P - - 0" +
            "0 - - - - - 0" +
            "0 - - 0 0 0 0";

        public static string Trap16 =
            "0 0 0 0 0 0 0" +
            "- - - - - - 0" +
            "0 0 - - - - 0" +
            "0 - E P 0 - 0" +
            "0 - - 0 - - 0" +
            "0 - 1 0 0 0 0";

        
        public static string Trap17 =
            "- 0 0 0 0 - 0" +
            "- - - - - - -" +
            "- - - - P - -" +
            "- E - - - - -" +
            "- 0 - - - - -" +
            "- 0 - E 0 E 0";

        public static string Trap18 =
            "0 0 0 0 0 0 0" +
            "- - - - 0 - 0" +
            "- 0 0 P O - 0" +
            "- - 0 - 0 - 0" +
            "0 - E - - - 0" +
            "0 - - 0 0 0 0";

        public static string Trap19 =
            "- - E - - O -" +
            "0 - 4 3 0 - 0" +
            "- 0 P - - - -" +
            "- 0 - - 0 - -" +
            "- - O - 0 - 0" +
            "- - - - 0 - -";

        public static string Trap20 =
            "- - - - - 0 -" +
            "- - O - - 0 -" +
            "O 0 - O - 0 -" +
            "- 0 O - - 0 -" +
            "- 0 E P 4 - -" +
            "- 0 - - - - -";

        public static string Trap21 =
            "- - - - - - -" +
            "O - - E - - -" +
            "0 0 M P - - 0" +
            "- - - 4 - 0 -" +
            "- - - - - - -" +
            "0 0 0 0 1 - -";

        public static string Trap22 =
            "- - - 0 - - 0" +
            "- - - 0 - 1 0" +
            "- - - 0 P 2 0" +
            "- - - 0 - - 0" +
            "- - - 0 M E 0" +
            "- - - 0 - - 0";

        public static string Trap23 =
            "- - - - - E 0" +
            "- - - - - - 0" +
            "- - - # # # 0" +
            "- - 1 - - P 0" +
            "- - - - - - 0" +
            "- - - - - - 0";

        public static string Trap24 =
            "- - - - - - 0" +
            "- - - - - 1 0" +
            "- - - - 1 - 0" +
            "- - - 1 # - 0" +
            "E - - - - P 0" +
            "- - - - - - 0";

        public static string Trap25 =
            "- 0 - - - - 0" +
            "- 0 - - - - 0" +
            "- 0 - 3 - - 1" +
            "- 0 P - - 0 M" +
            "- - O - - 0 M" +
            "- E - - - 0 -";

        public static string Trap26 =
            "- - - - - - 0" +
            "- - - - - - 0" +
            "- - - E 1 - -" +
            "- - P - 1 - -" +
            "- - - - 1 - M" +
            "- - - - - - -";

        public static string Trap27 =
            "0 0 0 0 0 0 0" +
            "0 - - - - 1 0" +
            "0 - - - - E 0" +
            "0 - 1 - - - 0" +
            "0 - - - P - -" +
            "0 0 - - 0 - 0";

        public static string Trap28 =
            "0 0 0 0 0 0 0" +
            "0 - 3 - - E 0" +
            "0 M - - 0 0 0" +
            "0 2 P - - - -" +
            "0 - 0 - - - -" +
            "0 - - - - - -";

        public static string Trap29 =
            "0 0 0 0 O 0 0" +
            "0 - - - - - -" +
            "0 0 0 0 2 - -" +
            "0 - - 3 - - -" +
            "0 0 - - P E -" +
            "0 - - - - - 0";
        
        public static string Trap30 =
            "0 0 - - 0 0 0" +
            "0 0 - - 1 - 0" +
            "0 0 - 3 O - 0" +
            "0 0 P 4 - - 0" +
            "0 0 O - 0 - 0" +
            "0 0 - E 0 - 0";

        public static string Trap31 =
            "- 0 - - - - 0" +
            "M - # 0 4 - 0" +
            "0 0 # P 3 1 0" +
            "- 0 # # # 0 -" +
            "# # # # # # #" +
            "- - - # 0 - -";

        public static string Trap32 =
            "0 0 0 0 0 0 -" +
            "0 - - - - - -" +
            "0 O - 0 - - -" +
            "1 3 4 E P 4 -" +
            "0 0 - - - 0 -" +
            "- - - - - # #";


        [SetUp]
        public void Setup()
        {
            mapParser = new MapParser(GameLib.TestCodes);
            mind = new Mastermind("");
        }

        [Test]
        public void SimplePanic()
        {
            Arrange(Trap2, new GameInfo(true));
            Assert.AreEqual(AgentAction.GoDown, hunter.MakeMove());
        }

        [Test]
        public void ComplexPanic()
        {
            Arrange(Trap, new GameInfo(true));
            Assert.AreEqual(AgentAction.GoLeft, hunter.MakeMove());
        }

        [Test]
        public void NotEnterBlastZone()
        {
            Arrange(Trap3, new GameInfo(true));
            Assert.AreNotEqual(AgentAction.GoRight, hunter.MakeMove());
        }

        [Test]
        public void BombInSafeDirection()
        {
            Arrange(Trap4, new GameInfo(true));
            Assert.AreNotEqual(AgentAction.BombUp, hunter.MakeMove());
        }

        [Test]
        public void AdvancedBlastEvasion()
        {
            Arrange(Trap5, new GameInfo(true));
            Assert.AreEqual(AgentAction.GoDown, hunter.MakeMove());
        }

        [Test]
        public void TightSituation()
        {
            Arrange(Trap6, new GameInfo(true));
            Assert.AreEqual(AgentAction.GoLeft, hunter.MakeMove());
        }

        [Test]
        public void NotGoingInCertainDeath()
        {
            Arrange(Trap7, new GameInfo(true));
            Assert.AreEqual(AgentAction.DoNothing, hunter.MakeMove());
        }

        //disabled for now
        //[Test]
        public void NoKamikazeBombing()
        {
            Arrange(Trap8, new GameInfo(true));
            Assert.AreNotEqual(AgentAction.BombRight, hunter.MakeMove());
        }

        [Test]
        public void GoToMonsterInsteadOfDying()
        {
            Arrange(Trap9, new GameInfo(true));
            Assert.AreEqual(AgentAction.GoLeft, hunter.MakeMove());
        }

        [Test]
        public void NotBombingIntoOtherBombBlastZone()
        {
            Arrange(Trap10, new GameInfo(true));
            Assert.AreNotEqual(AgentAction.BombDown, hunter.MakeMove());
        }

        [Test]
        public void NotBombingIntoDeadZone()
        {
            Arrange(Trap11, new GameInfo(true));
            Assert.AreNotEqual(AgentAction.BombDown, hunter.MakeMove());
        }

        [Test]
        public void NoGoingToEnemyFromBlastSide()
        {
            Arrange(Trap12, new GameInfo(true));
            Assert.AreNotEqual(AgentAction.GoRight, hunter.MakeMove());
        }

        [Test]
        public void NotBombingIntoOtherBombBlastZone2()
        {
            Arrange(Trap13, new GameInfo(true));
            Assert.AreNotEqual(AgentAction.BombUp, hunter.MakeMove());
        }

        [Test]
        public void PrioritizeCellsWithoutEnemies()
        {
            Arrange(Trap14, new GameInfo(true));
            Assert.AreNotEqual(AgentAction.GoLeft, hunter.MakeMove());
        }

        [Test]
        public void NotGoingToMonsterIfNoDanger()
        {
            Arrange(Trap15, new GameInfo(true));
            Assert.AreNotEqual(AgentAction.GoLeft, hunter.MakeMove());
            Assert.AreNotEqual(AgentAction.GoUp, hunter.MakeMove());
        }

        [Test]
        public void NotBombingEnemyThatAboutToBlownUp()
        {
            Arrange(Trap16, new GameInfo(true));
            Assert.AreNotEqual(AgentAction.BombUp, hunter.MakeMove());
        }

        [Test]
        public void PrioritizeTilesWithMultipleTargets()
        {
            Arrange(Trap17, new GameInfo(true));
            Assert.AreEqual(AgentAction.GoDown, hunter.MakeMove());
        }

        [Test]
        public void BombInNextTargetDirection()
        {
            Arrange(Trap18, new GameInfo(true));
            Assert.AreEqual(AgentAction.BombDown, hunter.MakeMove());
        }

        [Test]
        public void NotBombingIntoOtherBombBlastZone3()
        {
            Arrange(Trap19, new GameInfo(true));
            Assert.AreNotEqual(AgentAction.BombRight, hunter.MakeMove());
        }

        [Test]
        public void SmartBombing()
        {
            Arrange(Trap20, new GameInfo(true));
            Assert.AreNotEqual(AgentAction.BombUp, hunter.MakeMove());
        }

        [Test]
        public void NotBombingIntoOtherBombBlastZone4()
        {
            Arrange(Trap21, new GameInfo(true));
            Assert.AreEqual(AgentAction.DoNothing, hunter.MakeMove());
        }


        [Test]
        public void ThinkThatBlastIsUncrossable()
        {
            Arrange(Trap23, new GameInfo(true));
            Assert.AreEqual(AgentAction.GoDown, hunter.MakeMove());
        }

        [Test]
        public void ThinkThatBlastPassableByAnotherBlast()
        {
            Arrange(Trap24, new GameInfo(true));
            Assert.AreEqual(AgentAction.GoDown, hunter.MakeMove());
        }

        [Test]
        public void NotBombingIntoOtherBombBlastZone5()
        {
            Arrange(Trap25, new GameInfo(true));
            Assert.AreEqual(AgentAction.DoNothing, hunter.MakeMove());
        }

        [Test]
        public void TakeEveryChanceForSurvival()
        {
            Arrange(Trap26, new GameInfo(true));
            Assert.AreEqual(AgentAction.GoUp, hunter.MakeMove());
        }

        [Test]
        public void CommitMischief()
        {
            Arrange(Trap27, new GameInfo(true));
            Assert.AreEqual(AgentAction.BombLeft, hunter.MakeMove());
        }

        [Test]
        public void MegaMindMischief()
        {
            Arrange(Trap28, new GameInfo(true));
            Assert.AreEqual(AgentAction.BombRight, hunter.MakeMove());
        }

        [Test]
        public void NotBombingStupidly()
        {
            Arrange(Trap29, new GameInfo(true));
            Assert.AreNotEqual(AgentAction.BombUp, hunter.MakeMove());
        }

        [Test]
        public void Panic()
        {
            Arrange(Trap30, new GameInfo(true));
            Assert.AreNotEqual(AgentAction.GoUp, hunter.MakeMove());
        }


        
        [Test]
        public void BlownHimselfUpIfNoOtherChoice()
        {
            Arrange(Trap31, new GameInfo(true));
            Assert.AreNotEqual(AgentAction.DoNothing, hunter.MakeMove());
        }

        [Test]
        public void Panic2()
        {
            Arrange(Trap32, new GameInfo(true));
            Assert.AreNotEqual(AgentAction.DoNothing, hunter.MakeMove());
        }

        public void Arrange(string board, GameInfo info)
        {
            var str = string.Join("", board.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            map = mapParser.ParseMapFromString(str, new VectorInt2(7, 6));
            graph = map.ToFullGraph();
            playerTile = map.PlayerTile;
            hunter = new Hunter(map, info);
        }
    }
}
