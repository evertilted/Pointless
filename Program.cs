using Pointless.Constants;
using Pointless.Game;

namespace Pointless
{
    public class Program
    { 
        public static void Main()
        {
            using (var game = new GameContext(GameContextConstants.DefaultClientSize.X, GameContextConstants.DefaultClientSize.Y, GameContextConstants.WindowName))
            {
                game.Run();
            }
        }
    }
}