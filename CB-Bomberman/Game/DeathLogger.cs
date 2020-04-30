using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CB_Bomberman.State;
using CodenameGenerator.Lite;

namespace CB_Bomberman.Game
{
    public static class DeathLogger
    {
        public static void Log(LinkedList<TurnMold> latestMaps, Dictionary<TileTypes, char> reverseCodes, Tile pivot)
        {
            // watch.Start();
            var path = Environment.CurrentDirectory + "\\Deathlogs";
            Directory.CreateDirectory(path);
           // var generator = new Generator {Separator = "", Casing = Casing.PascalCase}; 
            //generator.SetParts(WordBank.Adjectives, WordBank.FirstNames, WordBank.LastNames);
            //var name = generator.GenerateAsync();
            //  watch.Stop();
            //log.NewLogLine("GENERATOR PERFOMANCE: " + watch.ElapsedMilliseconds);
            //watch.Reset();
            path = path + "\\ " + $@"{Guid.NewGuid()}.txt";

            using (StreamWriter file = File.CreateText(path))
            {

                var current = latestMaps.First;
                while (current != null)
                {
                    var area = current.Value.Map.GetTilesInArea(pivot, 3);
                    int side = 7;
                    for (var y = 0; y < side; y++)
                    {
                        for (var x = 0; x < side; x++)
                        {
                            if (reverseCodes.ContainsKey(area[x,y].TileType))
                                file.Write(reverseCodes[area[x,y].TileType] + " ");
                            else file.Write("% ");
                        }
                        file.Write('\n');
                        if(y == side - 1) file.WriteLine(current.Value.Action.ToString());
                    }

                    file.WriteLine('\n');
                    current = current.Next;
                }
            }

            //AutoTestCreator.CheckForResolve(path, latestMaps);
        }
    }
}
