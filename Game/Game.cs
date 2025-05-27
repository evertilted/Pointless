using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Pointless.Constants;
using Pointless.Game.TextRenderer;

namespace Pointless.Game
{
    /// <summary>
    /// The context - top level of control over game
    /// </summary>
    public class GameContext : GameWindow
    {
        private ScreenText _text;

        public GameContext(int width, int height, string title) :
            base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title })
        { }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(GameContextConstants.ClearColor);

            try
            {
                _text = new ScreenText();
            }
            catch
            {
                throw;
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            SwapBuffers();
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}
