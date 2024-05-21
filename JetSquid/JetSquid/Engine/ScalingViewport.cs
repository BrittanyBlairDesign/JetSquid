


// Ignore Spelling: Viewport , Viewports

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Engine.Viewports
{
    public class ScalingViewport
    {
        private readonly GameWindow _Window;
        public GraphicsDeviceManager _Graphics { get; }
        public Viewport _Viewport => _Graphics.GraphicsDevice.Viewport;

        int _virtualWidth;
        int _virtualHeight;

        bool useBlackBars = true;
        int _barWidth;
        int _barHeight;

        bool isResizing;

        int DESIGNED_RESOLUTION_WIDTH;
        int DESIGNED_RESOLUTION_HEIGHT;
        float DESIGNED_RESOLUTION_ASPECT_RATIO;

        public ScalingViewport(GameWindow window, GraphicsDeviceManager graphics, int DesignedWidth, int DesignedHeight, float DesignedRatio)
        {
            _Window = window;
            _Graphics = graphics;
            
            DESIGNED_RESOLUTION_WIDTH = DesignedWidth;
            DESIGNED_RESOLUTION_HEIGHT = DesignedHeight;
            DESIGNED_RESOLUTION_ASPECT_RATIO = DesignedRatio;

            isResizing = false;
            window.ClientSizeChanged += OnClientSizeChanged;

            _Graphics.HardwareModeSwitch = true;
            _Graphics.IsFullScreen = false;
            _Graphics.ApplyChanges();
        }

        public Rectangle GetDestinationRectangle()
        {
            return new Rectangle(0, 0, _virtualWidth, _virtualHeight);
        }

        private void OnClientSizeChanged(object sender, EventArgs e)
        {
            if (!isResizing && _Window.ClientBounds.Width > 0 && _Window.ClientBounds.Height > 0)
            {
                isResizing = true;
                RefreshViewport();
                isResizing = false;
            }
        }

        public virtual void RefreshViewport()
        {
            _Graphics.GraphicsDevice.Viewport = GetViewportScale();

        }

        protected virtual Viewport GetViewportScale()
        {
            var variance = 0.5;
            int windowWidth = _Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int windowHeight = _Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            var actualAspectRatio = (float)windowWidth / windowHeight;
            _barHeight = 0;
            _barWidth = 0;

            if (actualAspectRatio <= DESIGNED_RESOLUTION_ASPECT_RATIO)
            {
                var presentHeight = (int)(windowWidth / DESIGNED_RESOLUTION_ASPECT_RATIO + variance);
                _barHeight = (int)(windowHeight - presentHeight) / 2;

                _virtualWidth = windowWidth;
                _virtualHeight = presentHeight;

            }
            else
            {
                var presentWidth = (int)(windowHeight * DESIGNED_RESOLUTION_ASPECT_RATIO + variance);
                _barWidth = (int)(windowWidth - presentWidth) / 2;

                _virtualWidth = presentWidth;
                _virtualHeight = windowHeight;

            }


            int x = _barWidth;
            int y = _barHeight;

            if(!useBlackBars)
            {
                _Graphics.PreferredBackBufferWidth = _virtualWidth;
                _Graphics.PreferredBackBufferHeight = _virtualHeight;
                _Graphics.ApplyChanges();

                x = 0;
                y = 0;
            }

            return new Viewport
            {
                X = x ,
                Y = y ,
                Width = _virtualWidth,
                Height = _virtualHeight,
                MinDepth = 0,
                MaxDepth = 1,
            };
            
        }

        public virtual Matrix GetScaleMatrix()
        {
            float Scale = (float)_virtualWidth / (float)DESIGNED_RESOLUTION_WIDTH;
            return Matrix.CreateScale(Scale);
        }

        public Point PointToScreen(Point point)
        {
            return PointToScreen(point.X, point.Y);
        }

        public virtual Point PointToScreen(int x, int y)
        {
            Matrix matrix = Matrix.Invert(GetScaleMatrix());


            return Vector2.Transform(new Vector2(x - _barWidth, y - _barHeight), matrix).ToPoint();
        }

        public Point GetScaledMousePosition()
        {
            return PointToScreen(Mouse.GetState().Position);
        }

    }
}
 