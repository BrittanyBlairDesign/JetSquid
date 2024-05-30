
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Components.Collision;
using Engine.States;
using System.Diagnostics;
using Engine.Components.Physics;


namespace Engine.Objects
{
    public class BaseGameObject
    {
        // Vars
        protected Texture2D _texture;
        protected Texture2D _boundingBoxTexture;
        protected Vector2 _position = Vector2.One;
        protected float _scale = 1.0f;
        protected Color _color = Color.White;

        public int zIndex;

        public bool Destroyed = false;

        public int Width { get { return _texture.Width; } }
        public int Height { get { return _texture.Height; } }

        protected List<BoundingBox2D> _boundingBoxes = new List<BoundingBox2D>();
        public List<BoundingBox2D> BoundingBoxes { get { return _boundingBoxes; } }

        protected bool _Debug = false;
        public virtual Vector2 Position
        {
            get { return _position; }
            set
            {
                var deltaX = value.X - _position.X;
                var deltaY = value.Y - _position.Y;
                _position = value;

                foreach (var box in BoundingBoxes)
                {
                    box.Position = new Vector2(box.Position.X + deltaX, box.Position.Y + deltaY);
                }
            }
        }

        // Methods
        public virtual void OnNotify(BaseGameStateEvent gameEvent) { }                         
        public virtual void Render(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, _color);

            if (_Debug)
            {
                RenderBoundingBoxes(spriteBatch);
            }
        }
        public void Destroy()
        {
            Destroyed = true;
        }

        public void setPosition(Vector2 newPos)
        {
            this._position = newPos;
        }
        
        protected virtual void UpdatePosition(Vector2 positionDelta)
        {
            Position += positionDelta;
            //foreach (BoundingBox2D box in BoundingBoxes)
            //{
            //    box.Position += positionDelta;
            //}
        }
        public void AddBoundingBox(BoundingBox2D box)
        {
            _boundingBoxes.Add(box);
        }

        public virtual void RenderBoundingBoxes(SpriteBatch spriteBatch)
        {
            foreach(var box in BoundingBoxes)
            {
                if (box.BoundingBoxTexture == null)
                {
                    box.BoundingBoxTexture = CreateBoundingBoxTexture(spriteBatch.GraphicsDevice, box.Rectangle);
                }
                
                Rectangle scaleRect = box.Rectangle;
                scaleRect.Height = (int)(scaleRect.Height * _scale);
                scaleRect.Width = (int)(scaleRect.Width * _scale);
                
                spriteBatch.Draw(box.BoundingBoxTexture, scaleRect, box.Rectangle, Color.Red);
            }
        }

        protected virtual Texture2D CreateBoundingBoxTexture(GraphicsDevice graphics, Rectangle r)
        {
            int rWidth = r.Width;
            int rHeight = r.Height;
            Trace.Write(" WIDTH : " + rWidth);
            Trace.Write(" Height : " + rHeight);

            Texture2D Texture = new Texture2D(graphics, rWidth, rHeight);
            var colors = new List<Color>();

            for (int i = 0; i < rHeight; i++)
            {
                for (int j = 0; j < rWidth; j++)
                {
                    if (i == 0 || i == rWidth - 1
                     || j == 0 || j == rHeight - 1)
                    {
                        colors.Add(Color.DarkRed);
                    }
                    else
                    {
                        colors.Add(Color.Red);
                    }
                }
            }

            if (colors != null)
            {
                Texture.SetData<Color>(colors.ToArray());
                return Texture;
            }
            else
            {
                return null;
            }
        }

        public virtual float GetScale()
        {
            return _scale;
        }
        public virtual bool CollidesWith(BoundingBox2D otherBox)
        {
            if(otherBox == null) return false;

            bool isColliding = false;

            foreach(BoundingBox2D box in BoundingBoxes)
            {
                if(box.CollidesWith(otherBox))
                {
                    isColliding = true;
                    break;
                }
            }

            return isColliding;
        }
    }
}

