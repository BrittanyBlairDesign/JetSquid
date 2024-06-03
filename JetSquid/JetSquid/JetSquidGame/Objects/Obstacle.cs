

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Objects;
using Engine.Components.Physics;
using Engine.Components.Collision;
using Engine.Stats;
using SpriteSheetAnimationContentPipeline;
using Engine.States;

namespace JetSquid;

public class Obstacle : SpriteObject
{
    public Movement _movement;
    protected BaseStat _durability = new BaseStat(10, 10);
   
    protected float _damageTimer;
    protected float _damageCooldown = 0.2f;

    protected float colorFlashTimer;
    protected float colorFlashDuration;
    public Obstacle(Texture2D texture, bool debug = false) :
              base(texture, debug)
    { }
    public Obstacle(Texture2D texture, Vector2 startPos, bool debug = false) :
                  base(texture, startPos, debug)
    {  }
    public Obstacle(Texture2D texture, Vector2 startPos, bool debug = false, float speed = 0.0f, Direction dir = Direction.STOP, BoundingBox2D box2D = null, float scale = 1.0f )
    : base(texture, startPos, debug, scale)
    {
        _movement = new Movement(speed, dir);
        if(box2D != null)
        {
            AddBoundingBox(box2D);
        }
        else
        {
            AddBoundingBox(new BoundingBox2D(startPos, texture.Width, texture.Height));
        }
    }

    public override void OnNotify(BaseGameStateEvent gameEvent)
    {
        switch (gameEvent)
        {
            case JetSquidGameplayEvents.DamageObstacle:
                if (!TakeDamage())
                { Destroy(); }
                break;
            case JetSquidGameplayEvents.DestroyObstacle:
                Destroy();
                break;
        }
    }

    public virtual void Update(GameTime gameTime)
    {
        UpdatePosition(_movement.Update(gameTime));

        if(_damageTimer > 0)
        {
            _damageTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        if(colorFlashTimer > 0)
        {
            colorFlashTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            _color = new Color(_color, 0.5f);
        }
        else
        {
            _color = Color.White;
        }
    }

    public bool TakeDamage(int amount = 1)
    {
        if(_damageTimer <= 0.0f)
        {
            _durability.DecreaseValue(amount);
            _damageTimer = _damageCooldown;

            if (_durability._value <= _durability._minValue)
            {
                colorFlashTimer = colorFlashDuration;
                return false;
            }
        }

        return true;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        if(!Destroyed)
        {
            spriteBatch.Draw(_texture, _position, null, _color, 0.0f, Vector2.Zero, _scale, SpriteEffects.None, 1);

            if (_Debug)
            {
                RenderBoundingBoxes(spriteBatch);
            }
        }
    }
}

public class AnimatedObstacle : AnimatedSpriteObject
{

    public Movement _movement;
    protected BaseStat _durability = new BaseStat(10, 10);

    protected float _damageTimer;
    protected float _damageCooldown = 0.2f;

    protected float colorFlashTimer;
    protected float colorFlashDuration = 0.2f;

    public int frameRate = 3;

    public AnimatedObstacle(SpriteSheetAnimation animationSheet, bool debug = false, float scale = 1) 
        : base(animationSheet, debug, scale)
    {
        SetManager(frameRate);
        SetAnimations(animationSheet);
    }

    public AnimatedObstacle(SpriteSheetAnimation animationSheet, Vector2 startPos, bool debug = false, float scale = 1.0f, float speed = 0.0f, Direction dir = Direction.STOP, BoundingBox2D box2D = null) 
        : base(animationSheet, startPos, debug, scale)
    {
        SetManager(frameRate);
        SetAnimations(animationSheet);

        _movement = new Movement(speed, dir);

        if (box2D != null)
        {
            AddBoundingBox(box2D);
        }
        else
        {
            AddBoundingBox(new BoundingBox2D(startPos, SpriteWidth, SpriteHeight));
        }
    }

    public override void SetAnimations(SpriteSheetAnimation spriteSheet)
    {
        _animManager.AddSpriteSheet(spriteSheet, _scale);
        _animManager.SwitchAnimations("FanOn", true);
        _animManager._currentAnimation.ChangePlayState(Engine.Components.Animation.PlayState.PLAY);
    }

    public virtual void Update(GameTime gameTime)
    {
        UpdatePosition(_movement.Update(gameTime));

        float deltaTime = (float)gameTime.TotalGameTime.TotalSeconds;
        _animManager.Update(deltaTime);

        if (_damageTimer > 0.0f)
        {
            _damageTimer -= deltaTime;
        }

        if(colorFlashTimer > 0.0f)
        {
            _color = Color.Black;
            colorFlashTimer -= deltaTime;
        }
        else
        {
            _color = Color.White;
        } 
    }

    public override void OnNotify(BaseGameStateEvent gameEvent)
    {
        switch (gameEvent)
        {
            case JetSquidGameplayEvents.DamageObstacle:
                if (!TakeDamage())
                { Destroy(); }
                break;
            case JetSquidGameplayEvents.DestroyObstacle:
                Destroy();
                break;
        }
    }

    public bool TakeDamage(int amount = 1)
    {
        if (_damageTimer <= 0.0f)
        {
            _durability.DecreaseValue(amount);
            _damageTimer = _damageCooldown;

            if (_durability._value <= _durability._minValue)
            {
                colorFlashTimer = colorFlashDuration;
                return false;
            }
        }

        return true;
    }
}
