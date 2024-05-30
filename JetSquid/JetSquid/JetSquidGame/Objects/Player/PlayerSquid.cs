
using Microsoft.Xna.Framework;

using Engine.Objects;
using Engine.States;
using SpriteSheetAnimationContentPipeline;
using Engine.Components.Physics;
using Engine.Stats;
using System.Diagnostics;
using Engine.Particles;
using System.Runtime.CompilerServices;


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
        public Color damageColor = Color.Lerp(Color.White, Color.Transparent, 0.5f);
        public float damageColorTimer;
        public float DamageColorDuration = 0.5f;

        // ink Refill
        private float inkCooldownTimer;
        private float inkCooldownDuration = 0.5f;

        // Constructors 
        public PlayerSquid(SpriteSheetAnimation sheetAnimation, bool debug = false, float scale = 1.0f, Emitter emitter = null)
                : base(sheetAnimation, debug, scale)
        {
            SetManager(frameRate);
            SetAnimations(sheetAnimation);

            _jetEmitter = emitter;

        }

        public PlayerSquid(SpriteSheetAnimation sheetAnimation, Vector2 startPos, bool debug = false, float scale = 1.0f, Emitter emitter = null)
            : base(sheetAnimation, startPos, debug, scale)
        {
            SetManager(frameRate);
            SetAnimations(sheetAnimation);

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

            Vector2 newPos = Position + _movement.Update(gameTime);
            if(_animManager.CheckCurrentAnimation("JumpNoInk"))
            {
                float spriteHeight = _animManager._spriteSize.Y * Scale;
                if(newPos.Y < 1080 - spriteHeight + (spriteHeight /2)) 
                {
                    if(_Debug)
                    {
                        Trace.WriteLine("Sprite Y Position = " + newPos.Y.ToString());
                    }
                    newPos.Y = 1080 - spriteHeight + (spriteHeight / 2);
                    
                    _movement.ChangeDirection(Direction.DOWN);
                    _animationState = ePlayerAnimState.FALLING;
                }
            }

            Position = newPos;
            _jetEmitter.Position = new Vector2(Position.X + (SpriteWidth - SpriteWidth /3), Position.Y + SpriteHeight/3);
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
            else
            {
                Ink.isDecaying = false;
            }
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
                case JetSquidGameplayEvents.PlayerFall:
                    if (_animationState != ePlayerAnimState.WALKING)
                    { 
                        if(Ink._value > 0 && _animationState != ePlayerAnimState.JUMPING)
                        {  _animationState = ePlayerAnimState.FALLING;}
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
                    Health.DecreaseValue(1);
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
    

