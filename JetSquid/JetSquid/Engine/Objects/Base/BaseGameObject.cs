
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BaseGameObject
{
    // Vars
    protected Texture2D _texture;
    protected Texture2D _boundingBoxTexture;
    protected Vector2 _position = Vector2.One;
    
    public int zIndex;
    
    public int Width { get { return _texture.Width; } }
    public int Height { get { return _texture.Height; } }

    protected List<BoundingBox2D> _boundingBoxes = new List<BoundingBox2D>();
    public List<BoundingBox2D> BoundingBoxes{ get { return _boundingBoxes; } }

    protected bool _Debug = false;
    public virtual Vector2 Position
    {
        get { return _position; }
        set 
        {
            var deltaX = value.X - _position.X;
            var deltaY = value.Y - _position.Y;
            _position = value; 

            foreach(var box in BoundingBoxes)
            {
                box.Position = new Vector2(box.Position.X + deltaX, box.Position.Y + deltaY);
            }
        }
    }

    // Methods
    public virtual void OnNotify(BaseGameStateEvent gameEvent) { }
    public virtual void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, _position, Color.White);
        
        if(_Debug)
        {
            RenderBoundingBoxes(spriteBatch);
        }
    }

    public void setPosition(Vector2 newPos)
    {
        this._position = newPos;
    }

    public void AddBoundingBox(BoundingBox2D box)
    {
        _boundingBoxes.Add(box);
    }

    public virtual void RenderBoundingBoxes(SpriteBatch spriteBatch)
    {

        if (_boundingBoxTexture == null)
        {
            CreateBoundingBoxTexture(spriteBatch.GraphicsDevice);
        }
        foreach (var b in _boundingBoxes)
        {
            spriteBatch.Draw(_boundingBoxTexture, b.Rectangle, Color.Red);
        }
    
    }

    protected virtual void CreateBoundingBoxTexture(GraphicsDevice graphics) 
    {
        _boundingBoxTexture = new Texture2D(graphics, 1, 1);
        _boundingBoxTexture.SetData<Color>(new Color[] { Color.White });
    }

}

