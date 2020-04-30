using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CB_Bomberman.State;

namespace CB_Bomberman.Game
{
    public static class AutoTestCreator
    {
        public static void CheckForResolve(string deathLogPath, LinkedList<Map> maps)
        {
            //maps.RemoveLast(); //already dead map
            if (maps.Last.Previous == null) return;
            var actions = GetActionsToSurvive(maps.Last.Previous.Value);
            if(actions.Count == 0) return;

            var myPath = Environment.CurrentDirectory + "\\Autotests";
            Directory.CreateDirectory(myPath);
            myPath = myPath + "\\ " + $@"{Guid.NewGuid()}.txt";

            string deathLog = File.ReadAllText(deathLogPath);
            var testMap = deathLog.Split('\n').Skip(9).Take(7);
            File.WriteAllLines(myPath, testMap);
            File.AppendAllLines(myPath, actions.Select(x=> x.ToString()));

        }

        public static List<AgentAction> GetActionsToSurvive(Map map)
        {
            List<AgentAction> possibleActions = new List<AgentAction>();
            foreach (var action in (AgentAction[]) Enum.GetValues(typeof(AgentAction)))
            {
                var newMap = PocketServer.GetNewState(map, action);
                if (newMap.PlayerTile != default) possibleActions.Add(action);
            }

            return possibleActions;
        }

    }
}
