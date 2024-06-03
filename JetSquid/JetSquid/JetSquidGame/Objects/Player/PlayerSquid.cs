
using Microsoft.Xna.Framework;

using Engine.Objects;
using Engine.States;
using SpriteSheetAnimationContentPipeline;
using Engine.Components.Physics;
using Engine.Stats;
using System.Diagnostics;
using Engine.Particles;
using Engine.Components.Collision;
using Microsoft.Xna.Framework.Content;



namespace JetSquid
{
    public enum ePlayerAnimState
    {
        WALKING,
        JUMPING,
        HOVERING,
        FALLING,
    }
    public class PlayerSquid : AnimatedSpriteObject
    {
        // Player Squid Components
        protected Movement _movement = new Movement(10.0f, Direction.DOWN);
        protected Emitter _jetEmitter;
        protected BaseStat Health = new BaseStat(3, 3, 0);
        protected DecayStat Ink = new DecayStat(1, 0.2f, 20, 20, 0);

        // Animation vars
        public ePlayerAnimState _animationState = ePlayerAnimState.FALLING;
        public int frameRate = 12;

        // Rendering and scale vars
        public float Scale { get { return _scale; } }

        // Damage
        public Color damageColor = new Color(Color.Red, 0.5f);
        public float damageColorTimer;
        public float DamageColorDuration = 0.5f;

        // ink Refill
        private float inkCooldownTimer;
        private float inkCooldownDuration = 0.2f;

        public bool isOnObstacle = false;
        private Obstacle _collidingObstacle;

        // Constructors 
        public PlayerSquid(SpriteSheetAnimation sheetAnimation, bool debug = false, float scale = 1.0f, Emitter emitter = null)
                : base(sheetAnimation, debug, scale)
        {
            SetManager(frameRate);
            SetAnimations(sheetAnimation);
            AddBoundingBox(new BoundingBox2D(Position,SpriteWidth, SpriteHeight));
            _jetEmitter = emitter;

        }

        public PlayerSquid(SpriteSheetAnimation sheetAnimation, Vector2 startPos, bool debug = false, float scale = 1.0f, Emitter emitter = null)
            : base(sheetAnimation, startPos, debug, scale)
        {
            SetManager(frameRate);
            SetAnimations(sheetAnimation);

            boxOffsetX = SpriteWidth / 4;
            boxOffsetY = 0;
            AddBoundingBox(new BoundingBox2D(startPos, SpriteWidth - (SpriteWidth / 4), SpriteHeight));
            _jetEmitter = emitter;
        }

        // Methods
        public virtual void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
           
            
            AnimateCharacter(deltaTime);
            _animManager.Update(deltaTime);
            
            if(_animationState == ePlayerAnimState.HOVERING)
            {
                _jetEmitter.Update(gameTime, true);
            }
            else
            {
                _jetEmitter.Update(gameTime, false);
            }
            
            if(damageColorTimer > 0.0f)
            {
                damageColorTimer -= deltaTime;
                _color = damageColor;
            }
            else
            {
                _color = Color.White;
            }

            UpdatePosition(_movement.Update(gameTime));
            if (_collidingObstacle != null && isOnObstacle)
            {
                Rectangle collidingRect = _collidingObstacle.BoundingBoxes[0].Rectangle;
                collidingRect.Width = (int)(collidingRect.Width * _collidingObstacle.GetScale());
                collidingRect.Height = (int)(collidingRect.Height * _collidingObstacle.GetScale());
                if (_Debug )
                {
                    Trace.WriteLine(" is Position x  : " + BoundingBoxes[0].Rectangle.X);
                    Trace.WriteLine(" is Obstacle x  : " + _collidingObstacle.Position.X);
                    Trace.WriteLine(" obstacle rect x: " + collidingRect.X);
                    Trace.WriteLine(" Obstacle rect w: " + _collidingObstacle.BoundingBoxes[0].Rectangle.Width);
                    Trace.WriteLine(" scaled rect w  : " + collidingRect.Width);
                    Trace.WriteLine(" Obstacle Width : " + (_collidingObstacle.Position.X + (int)collidingRect.Width));
                    Trace.WriteLine(" distance x     : " + ((_collidingObstacle.Position.X + (collidingRect.Width)) - (Position.X + boxOffsetX)));
                }

                int distance = (int)((_collidingObstacle.Position.X + (collidingRect.Width)) - (Position.X + boxOffsetX));
                if (distance <= 0)
                {
                    OnNotify(new JetSquidGameplayEvents.PlayerFall());
                    _collidingObstacle = null;
                }
            }
        }

        protected override void UpdatePosition(Vector2 positionDelta)
        {
            Vector2 newDelta = positionDelta;
            if (_animManager.CheckCurrentAnimation("JumpNoInk"))
            {
                float spriteHeight = _animManager._spriteSize.Y * Scale;
                if (Position.Y + positionDelta.Y < 1080 - spriteHeight * 2)
                {
                    if (_Debug)
                    {
                        Trace.WriteLine("Sprite Y Position = " + Position.Y.ToString());
                    }
                    float yPos = 1080 - spriteHeight + (spriteHeight / 2);
                    newDelta.Y = positionDelta.Y - yPos;
                    _movement.ChangeDirection(Direction.DOWN);
                    _animationState = ePlayerAnimState.FALLING;
                }
            }

            Position += newDelta;
            _jetEmitter.Position = new Vector2(Position.X + (SpriteWidth - SpriteWidth / 3), Position.Y + SpriteHeight / 3);
        }

        public override void SetAnimations(SpriteSheetAnimation spriteSheet)
        {
            _animManager.AddSpriteSheet(spriteSheet, _scale);
            _animManager.SwitchAnimations("Walk", true);
            _animationState = ePlayerAnimState.WALKING;
        }

        public virtual void AnimateCharacter(float deltaTime)
        {
            switch (_animationState)
            {
                case ePlayerAnimState.WALKING:
                    Walk();
                    Ink.Update(deltaTime);
                    break;
                case ePlayerAnimState.JUMPING:
                    Jump();
                    break;
                case ePlayerAnimState.HOVERING:
                    Hover();
                    Ink.Update(deltaTime);
                    break;
                case ePlayerAnimState.FALLING:
                    Fall();
                    break;
            }
        }

        public virtual void Jump()
        {

            if(Ink._value > 0)
            {
                _animManager.SwitchAnimations("JumpWithInk", false);
                _animManager.SetNextAnimation("Hover", true);
                _movement.ResetSpeed();
                _movement.ChangeDirection(Direction.UP);
            }
            else
            {
                if (_animManager.CheckCurrentAnimation("Walk"))
                {
                    _animManager.SwitchAnimations("JumpNoInk", false);
                    // TO DO:
                    _movement.ResetSpeed();
                    _movement.ChangeDirection(Direction.UP);

                }
            }
        }

        public virtual void Hover()
        {
            if(_animManager.CheckCurrentAnimation("Hover"))
            {
                Ink.isDecaying = true;
                if(Ink._value > 0)
                {
                    // TODO: Emit particles from the squid for the jet propultion.
                    _movement.ChangSpeed(3.0f);
                    _movement.ChangeDirection(Direction.UP);
                }
                else
                {
                    _animationState = ePlayerAnimState.FALLING;
                }
            }
        }

        public virtual void Fall()
        {
            if(!_animManager.CheckCurrentAnimation("JumpNoInk"))
            {
                _movement.ResetSpeed();
                _movement.ChangeDirection(Direction.DOWN);
            }
        }

        public virtual void Walk()
        {
            if(!_animManager.CheckCurrentAnimation("Walk") && !_animManager.CheckCurrentAnimation("Land"))
            {
                if(!_animManager.CheckCurrentAnimation("JumpNoInk"))
                {
                    _animManager.SwitchAnimations("Land", false);
                } 
                _animManager.SetNextAnimation("Walk", true);
                _movement.ChangeDirection(Direction.STOP);
            }
            else if (_animManager.CheckCurrentAnimation("Walk"))
            {
                _movement.ChangeDirection(Direction.STOP);
            }
            else
            {
                Ink.isDecaying = false;
            }
        }

        public virtual void SetCollidingObstacle(Obstacle other)
        {
            _collidingObstacle = other;
        }

        public override void OnNotify(BaseGameStateEvent gameEvent)
        {

            switch (gameEvent)
            {
                case JetSquidGameplayEvents.PlayerFloorCollide:
                    if ((_animationState != ePlayerAnimState.WALKING) 
                    || (_animationState == ePlayerAnimState.FALLING))
                       { _animationState = ePlayerAnimState.WALKING; }
                    break;
                case JetSquidGameplayEvents.PlayerObstacleCollide:
                    _animationState = ePlayerAnimState.WALKING;
                    isOnObstacle = true;
                    break;
                case JetSquidGameplayEvents.PlayerFall:
                    if (_animationState != ePlayerAnimState.FALLING)
                    { 
                        if(_animationState != ePlayerAnimState.JUMPING || isOnObstacle)
                        {   
                            _animationState = ePlayerAnimState.FALLING;
                            isOnObstacle = false;
                        }
                    }
                    break;
                case JetSquidGameplayEvents.PlayerJump:
                    if (_animationState == ePlayerAnimState.FALLING)
                    { 
                        if (Ink._value > 0)
                        { _animationState = ePlayerAnimState.HOVERING; }
                    }
                    else if(_animationState == ePlayerAnimState.WALKING)
                    { _animationState = ePlayerAnimState.JUMPING; }
                    break;
                case JetSquidGameplayEvents.PlayerHover:
                    if (Ink._value > 0)
                         { _animationState = ePlayerAnimState.HOVERING; }
                    else { _animationState = ePlayerAnimState.FALLING; }
                    break;
                case JetSquidGameplayEvents.PlayerRefillInk:
                    Ink.ResetValue();
                    break;
                case JetSquidGameplayEvents.PlayerTakeDamage:
                    if(TakeDamage())
                    {
                        Trace.WriteLine("Player Dead");
                        Destroy();
                    }
                    break;
                case JetSquidGameplayEvents.PlayerCollectHearts:
                    Health.IncreaseValue(1);
                    break;
            }

            if (_Debug)
            { 
                Trace.WriteLine(" Animation State : " + (string)_animationState.ToString());
                Trace.WriteLine(" Event Notified to Player Squid");
            }
        }
       
        public bool TakeDamage(int amount = 1)
        {
            if (damageColorTimer <= 0.0f)
            {
                Health.DecreaseValue(amount);
                damageColorTimer = DamageColorDuration;

                if (Health._value <= Health._minValue)
                {
                    return false;
                }
            } 
            return true;
        }

    }
}
    

