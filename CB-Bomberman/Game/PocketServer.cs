using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CB_Bomberman.State;
using VectorInt;

namespace CB_Bomberman.Game
{
    public static class PocketServer
    {
        public static Map GetNewState(Map map, AgentAction action)
        {
            var result = new MutableMap(map.Size, new Dictionary<VectorInt2, Tile>(map));
            var player = result.PlayerTile;
            if (player != default && action != AgentAction.DoNothing)
            {
                var newTile = PlayerMoveTile(player, action, result, out var bombing);

                if (newTile != null && newTile.Value.IsWalkable)
                {
                    result[newTile.Value.Position] = new Tile(newTile.Value.Position, TileTypes.Player);
                    if (bombing)
                        result[player.Position] = new Tile(player.Position, TileTypes.Bomb4);
                    else
                        result[player.Position] = new Tile(player.Position, TileTypes.Empty);
                }
            }

            var gonnaBlast = result.BlastZones
                .Where(x => x.Value == 1)
                .Select(x => x.Key.Position);

            foreach (var tilePos in gonnaBlast)
            {
                result[tilePos] = new Tile(tilePos, TileTypes.Empty);
            }

            //Console.WriteLine(result.PlayerTile);
            return result.ToImmutableMap();
        }

        private static Tile? PlayerMoveTile(Tile player, AgentAction action, MutableMap map, out bool spawnBomb)
        {

            spawnBomb = true;
            switch (action)
            {
                case AgentAction.BombDown:
                    return map.GetDown(player);
                case AgentAction.BombLeft:
                    return map.GetLeft(player);
                case AgentAction.BombRight:
                    return map.GetRight(player);
                case AgentAction.BombUp:
                    return map.GetUp(player);
            }

            spawnBomb = false;
            return action switch
            {
                AgentAction.GoDown => map.GetDown(player),
                AgentAction.GoLeft => map.GetLeft(player),
                AgentAction.GoRight => map.GetRight(player),
                AgentAction.GoUp => map.GetUp(player),
                _ => player
            };
        }


    }
}
