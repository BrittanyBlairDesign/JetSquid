// Ignore Spelling: Gameplay
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
public class GameplayState : BaseGameState
{
    protected string Player = "Sprites/Player";
    protected string BackgroundTexture = "Backgrounds/Barren";

    protected PlayerSprite _playerSprite;
 
    public override void LoadContent()
    {
        _playerSprite = new PlayerSprite(LoadTexture(Player));
        
        AddGameObject(new SplashImage(LoadTexture(BackgroundTexture)));
        AddGameObject(_playerSprite);

        var playerXPos = _viewportWidth / 2 - _playerSprite.Width / 2;
        var playerYPos = _viewportHeight - _playerSprite.Height - 30;
        _playerSprite.Position = new Vector2(playerXPos, playerYPos);
    }

    public override void HandleInput(GameTime gameTime)
    {
        _inputManager.GetCommands(cmd =>
        {
            if (cmd is GameplayInputCommand.GameExit)
            {
                NotifyEvent(new BaseGameStateEvent.GameQuit());
            }           
        });
    }

    public override void UpdateGameState(GameTime gameTime)
    {
        DetectCollisions();
    }

    protected virtual void DetectCollisions()
    {
       
    }

    protected void KeepPlayerInBounds()
    {
        if (_playerSprite.Position.X < 0)
        {
            _playerSprite.Position = new Vector2(0, _playerSprite.Position.Y);
        }

        if (_playerSprite.Position.X > _viewportWidth - _playerSprite.Width)
        {
            _playerSprite.Position = new Vector2(_viewportWidth - _playerSprite.Width, _playerSprite.Position.Y);
        }

        if (_playerSprite.Position.Y < 0)
        {
            _playerSprite.Position = new Vector2(_playerSprite.Position.X, 0);
        }

        if (_playerSprite.Position.Y > _viewportHeight - _playerSprite.Height)
        {
            _playerSprite.Position = new Vector2(_playerSprite.Position.X, _viewportHeight - _playerSprite.Height);
        }
    }

    protected override void SetInputManager()
    {
        _inputManager = new InputManager(new GameplayInputMapper());
    }
}
