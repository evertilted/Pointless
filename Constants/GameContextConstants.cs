using System.Drawing;
using OpenTK.Mathematics;

namespace Pointless.Constants
{
    /// <summary>
    /// constants for creating and managing the game context
    /// </summary>
    internal static class GameContextConstants
    {
        public static Vector2i DefaultClientSize { get { return new Vector2i(800, 600); } }

        public static String WindowName { get { return "the pointless";  } }

        public static Color ClearColor { get { return Color.RebeccaPurple; } }
    }
}
