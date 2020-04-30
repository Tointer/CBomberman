using System;
using System.Collections.Generic;
using System.Text;

namespace CB_Bomberman.State
{
    public class TurnMold
    {
        public readonly Map Map;
        public readonly AgentAction Action;

        public TurnMold(AgentAction action, Map map)
        {
            Action = action;
            Map = map;
        }
    }

}
