
using Engine.Input;
using Engine.Particles;
using Engine.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSheetAnimationContentPipeline;
using System.Diagnostics;
using System.Security;

namespace JetSquid
{
    // Main Gameplay State.
    public class JetSquidGameplayState : GameplayState
    {
        private string playerTexture = "JetSquid/Animation/Player/SquidAnimatedSpriteSheet";
        private PlayerSquid _player;

        private string inkParticleTexture = "JetSquid/Particles/InkParticle";
        private InkJetEmitter _inkEmitter;

        public override void LoadContent(GraphicsDevice graphics = null)
        {


            SpriteSheetAnimation animSheet = LoadAnimation(playerTexture);
            int spriteHeight = animSheet.Animations[0].SpriteHeight;
            Vector2 playerStartPos;
            playerStartPos.X = _viewportWidth / 3;
            playerStartPos.Y = _viewportHeight - ((spriteHeight * 0.5f) / 2);

            _inkEmitter = new InkJetEmitter(LoadTexture(inkParticleTexture), playerStartPos);
            AddGameObject(_inkEmitter);

            _player = new PlayerSquid(animSheet, playerStartPos, true, 0.5f, _inkEmitter);
            _playerSprite = _player;
            AddGameObject(_player);

            if(graphics != null)
            {
                isDebug = true;
                this.graphics = graphics;
            }
        }

        public override void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
            KeepPlayerInBounds();
            base.Update(gameTime);
        }

        public override void HandleInput(GameTime gameTime, Point MousePosition)
        {
            _inputManager.GetCommands(cmd =>
            {
                if (cmd is JetSquidGameInputCommand.PlayerExit)
                {
                    NotifyEvent(new BaseGameStateEvent.GameQuit());
                }

                if(cmd is JetSquidGameInputCommand.PlayerJump)
                {
                    NotifyEvent(new JetSquidGameplayEvents.PlayerJump());
                }
                else if(cmd is JetSquidGameInputCommand.PlayerHover)
                {
                    NotifyEvent(new JetSquidGameplayEvents.PlayerHover());
                }

                if ( cmd is JetSquidGameInputCommand.PlayerFall)
                {
                    NotifyEvent(new JetSquidGameplayEvents.PlayerFall());
                }

                if (cmd is JetSquidGameInputCommand.PlayerPause)
                {
                    NotifyEvent(new BaseGameStateEvent.GamePause());
                }
            });
        }

        protected override void KeepPlayerInBounds()
        {

            int aWidth = _player.SpriteWidth;
            int aHeight = _player.SpriteHeight;

            if (_player.Position.X < 0)
            {
                _player.Position = new Vector2(0, _player.Position.Y);
            }

            if (_player.Position.X > _viewportWidth - aWidth)
            {
                _player.Position = new Vector2(_viewportWidth - aWidth , _player.Position.Y);
            }

            if (_player.Position.Y < 0)
            {
                _player.Position = new Vector2(_player.Position.X, 0);
            }

            if (_player.Position.Y > _viewportHeight - aHeight)
            {
                _player.Position = new Vector2(_player.Position.X, _viewportHeight - aHeight );
                if(_player._animationState != ePlayerAnimState.WALKING)
                {
                    NotifyEvent(new JetSquidGameplayEvents.PlayerFloorCollide());
                }
            }

            if (isDebug)
            {
                //Trace.WriteLine("Player Sprite Width = " + aWidth);
                //Trace.WriteLine("Player Sprite Height = " + aHeight);
                //Trace.WriteLine("Player Position X = " + _player.Position.X);
                //Trace.WriteLine("Player Position Y = " + _player.Position.Y);
            }
        }

        protected override void SetInputManager()
        {
            _inputManager = new InputManager(new JetSquidGameInputMapper());
        }
    }
}

