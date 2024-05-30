// Ignore Spelling: Gameplay

using Microsoft.Xna.Framework;

using Engine.Objects;
using Engine.Input;
using SpriteSheetAnimationContentPipeline;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace Engine.States;
public class GameplayState : BaseGameState
{
    protected string Player = "Sprites/Player";
    protected string BackgroundTexture = "Backgrounds/Barren";

    protected SpriteObject _playerSprite;
 
    public override void LoadContent(GraphicsDevice graphics = null)
    {
        _playerSprite = new SpriteObject(LoadTexture(Player));
        
        AddGameObject(new SplashImage(LoadTexture(BackgroundTexture)));
        AddGameObject(_playerSprite);

        var playerXPos = _viewportWidth / 2 - _playerSprite.Width / 2;
        var playerYPos = _viewportHeight - _playerSprite.Height - 30;
        _playerSprite.Position = new Vector2(playerXPos, playerYPos);

        if (graphics != null)
        {
            isDebug = true;
            this.graphics = graphics;
        }
    }

    public virtual SpriteSheetAnimation LoadAnimation(string SpriteSheetName)
    {
        var SpriteSheet = _contentManager.Load<SpriteSheetAnimation>(SpriteSheetName);
        return SpriteSheet;
    }

    public override void HandleInput(GameTime gameTime, Point MousePosition)
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

    protected List<T> CleanObjects<T>(List<T> objectList) where T : BaseGameObject
    {
        List<T> listOfItemsToKeep = new List<T>();
        foreach (T item in objectList)
        {
            var offScreen = item.Position.Y < -50;

            if (offScreen || item.Destroyed)
            {
                RemoveGameObject(item);
            }
            else
            {
                listOfItemsToKeep.Add(item);
            }
        }

        return listOfItemsToKeep;
    }

    protected virtual void KeepPlayerInBounds()
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
