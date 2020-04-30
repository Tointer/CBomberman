using System;
using System.Collections.Generic;
using System.Text;
using CB_Bomberman.Game;
using CB_Bomberman.State;
using VectorInt;

namespace CB_Bomberman
{
    public abstract class Agent
    {
        public abstract AgentAction MakeMove();

        protected Agent(Map map, GameInfo info)
        {
        }
    }

    public enum AgentAction
    {
        DoNothing, GoRight, GoLeft, GoUp, GoDown,
        BombLeft, BombRight, BombUp, BombDown, Debug
    }
}
