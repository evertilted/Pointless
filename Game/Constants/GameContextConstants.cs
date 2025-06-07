using System.Drawing;
using OpenTK.Mathematics;

namespace Pointless.Game.Constants
{
    /// <summary>
    /// The constants for <see cref="GameContext"/>
    /// </summary>
    internal static class GameContextConstants
    {
        public static Vector2i DefaultClientSize { get { return new Vector2i(800, 600); } }

        public static string WindowName { get { return "the pointless";  } }

        public static Color ClearColor { get { return Color.RebeccaPurple; } }
    }
}
