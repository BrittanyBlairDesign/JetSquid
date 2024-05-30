

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
    protected float _damageCooldown { get; set; }

    protected float colorFlashTimer;
    protected float colorFlashDuration;
    public Obstacle(Texture2D texture, bool debug = false) :
              base(texture, debug)
    { }
    public Obstacle(Texture2D texture, Vector2 startPos, bool debug = false) :
                  base(texture, startPos, debug)
    {  }
    public Obstacle(Texture2D texture, Vector2 startPos, bool debug = false, float speed = 0.0f, Direction dir = Direction.STOP, BoundingBox2D box2D = null )
    : base(texture, startPos, debug)
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
        }
    }

    public virtual void Update(GameTime gameTime)
    {
        _position +=  _movement.Update(gameTime);      
        
        if(_damageTimer > 0)
        {
            _damageTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
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
}

public class AnimatedObstacle : AnimatedSpriteObject
{

    public Movement _movement;
    protected BaseStat _durability = new BaseStat(10, 10);

    protected float _damageTimer;
    protected float _damageCooldown = 0.2f;

    protected float colorFlashTimer;
    protected float colorFlashDuration = 0.2f;

    public int frameRate = 6;

    public AnimatedObstacle(SpriteSheetAnimation animationSheet, bool debug = false, float scale = 1) 
        : base(animationSheet, debug, scale)
    {
        SetManager(frameRate);
        SetAnimations(animationSheet);
    }

    public AnimatedObstacle(SpriteSheetAnimation animationSheet, Vector2 startPos, bool debug = false, float scale = 1, float speed = 0.0f, Direction dir = Direction.STOP, BoundingBox2D box2D = null) 
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
    }

    public virtual void Update(GameTime gameTime)
    {
        Position = _movement.Update(gameTime);
        float deltaTime = (float)gameTime.TotalGameTime.TotalSeconds;
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
                if(!TakeDamage())
                { Destroy(); }
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
