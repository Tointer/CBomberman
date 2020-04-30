using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bomberman.Api;
using CB_Bomberman.Client;
using CB_Bomberman.Game;
using CB_Bomberman.State;
using VectorInt;

namespace CB_Bomberman
{
    public class Mastermind: AbstractSolver
    {
        private List<int> bombsCooldown = new List<int>();
        private readonly ConsoleInterface log = new ConsoleInterface();

        private readonly LinkedList<TurnMold> molds = new LinkedList<TurnMold>();
        private const int MapLogSize = 5;
        private Tile playerTile;
        private Tile lastPlayerTile;
        private int idleCount = 0;

        public AgentAction GetDecision(Map map)
        {
            playerTile = map.PlayerTile;

            if (playerTile == default)
            {
                log.NewLogLine("DEATH");
                //Task.Run(() => DeathLogger.Log(new LinkedList<TurnMold>(molds), GameLib.ReverseTestCodes, lastPlayerTile));
                DeathLogger.Log(new LinkedList<TurnMold>(molds), GameLib.ReverseTestCodes, lastPlayerTile);
                return AgentAction.DoNothing;
            }

            if (playerTile == lastPlayerTile)
            {
                idleCount++;
                if (idleCount == 5) return ShakeOut(map);
            }
            else idleCount = 0;

            lastPlayerTile = playerTile;
            

            bombsCooldown = bombsCooldown.Select(x => x - 1)
                .Where(x => x > 0)
                .ToList();
            var info = bombsCooldown.Count < GameLib.MaxBombsCount ? new GameInfo(true) : new GameInfo(false);


            var thisTurnAgent = new Hunter(map, info);
            var move = thisTurnAgent.MakeMove();
            if(move == AgentAction.BombDown || move == AgentAction.BombLeft
            || move == AgentAction.BombRight || move == AgentAction.BombUp) bombsCooldown.Add(5);
            

            return move;
           // return AgentAction.BombDown;
        }


        public Mastermind(string server)
            : base(server)
        {
        }

        protected internal override string Get(Map map)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var action = GetDecision(map);

            molds.AddLast(new TurnMold(action, map));
            if(molds.Count > MapLogSize) molds.RemoveFirst();

            watch.Stop();
            log.NewLogLine("Do " + action + ": " + watch.ElapsedMilliseconds + "ms");
            watch.Reset();

            switch (action)
            {
                case AgentAction.BombDown:
                    return "ActDown";
                case AgentAction.BombLeft:
                    return "ActLeft";
                case AgentAction.BombRight:
                    return "ActRight";
                case AgentAction.BombUp:
                    return "ActUp";
                case AgentAction.GoDown:
                    return "Down";
                case AgentAction.GoLeft:
                    return "Left";
                case AgentAction.GoRight:
                    return "Right";
                case AgentAction.GoUp:
                    return "Up";
            }

            return "";
        }

        private AgentAction ShakeOut(Map map)
        {
            var adjacent = map.GetAdjacentWalkableTiles(playerTile);
            if (adjacent.Count == 0) return AgentAction.BombDown;
            Random random = new Random();
            var randomAdjTile = adjacent.OrderBy(x => random.Next()).First();
            return GameLib.DirectionTo(playerTile, randomAdjTile);
        }

    }
}
