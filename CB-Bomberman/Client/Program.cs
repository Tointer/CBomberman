﻿/*-
 * #%L
 * Codenjoy - it's a dojo-like platform from developers to developers.
 * %%
 * Copyright (C) 2018 Codenjoy
 * %%
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public
 * License along with this program.  If not, see
 * <http://www.gnu.org/licenses/gpl-3.0.html>.
 * #L%
 */
using System;
using System.Threading.Tasks;
using CB_Bomberman;

namespace Demo
{
    class Program
    {

        // you can get this code after registration on the server with your email
        static string ServerUrl = "http://codebattle2020final.westeurope.cloudapp.azure.com/codenjoy-contest/board/player/q1ddqy8p0ttd83ihyh7a?code=1644583062307702826&gameName=bomberman";

        static void Main(string[] args)
        {
            //Console.SetWindowSize(Console.LargestWindowWidth - 3, Console.LargestWindowHeight - 3);

            // creating custom AI client
            var bot = new Mastermind(ServerUrl);

            // starting thread with playing game
            Task.Run(() => bot.Play());

            // waiting for any key
            Console.ReadKey();

            // on any key - asking AI client to stop.
            bot.InitiateExit();
        }
    }
}