﻿using Pointless.Game;
using Pointless.Game.Constants;

namespace Pointless
{
    public class Program
    { 
        public static void Main()
        {
            try
            {
                using (var game = new GameContext(GameContextConstants.DefaultClientSize.X, GameContextConstants.DefaultClientSize.Y, GameContextConstants.WindowName))
                {
                    game.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}