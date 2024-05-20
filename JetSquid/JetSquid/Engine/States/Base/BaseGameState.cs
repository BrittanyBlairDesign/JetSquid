using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Engine.Objects;
using Engine.Input;
using Engine.Sound;
using Microsoft.Xna.Framework.Input;
using Engine.Components.Collision;



namespace Engine.States;
public abstract class BaseGameState
{
    protected readonly List<BaseGameObject> _gameObjects = new List<BaseGameObject>();

    private const string FallbackTexture = "SplashScreens/Empty";

    protected ContentManager _contentManager;
    protected int _viewportHeight;
    protected int _viewportWidth;

    protected GraphicsDevice graphics;
    protected bool isDebug = false;
    protected InputManager _inputManager { get; set; }
    protected SoundManager _soundManager = new SoundManager();

    // Loading and unloading content
    public abstract void LoadContent();
    public virtual void LoadContent(GraphicsDevice graphics)
    { isDebug = true; this.graphics = graphics; }
    public void UnloadContent()
    {
        _contentManager.Unload();
    }

    public virtual void Initialize(ContentManager contentManager, int viewportWidth, int viewportHeight)
    {
        _contentManager = contentManager;
        _viewportHeight = viewportHeight;
        _viewportWidth = viewportWidth;

        SetInputManager();
    }


    protected Texture2D LoadTexture(string textureName)
    {
        var texture = _contentManager.Load<Texture2D>(textureName);
        return texture ?? _contentManager.Load<Texture2D>(FallbackTexture);
    }
    protected SoundEffect LoadSound(string soundName)
    {
        return _contentManager.Load<SoundEffect>(soundName);
    }

    // Input
    protected abstract void SetInputManager();
    public abstract void UpdateGameState(GameTime gameTime);
    public abstract void HandleInput(GameTime gameTime);
    public virtual void Update(GameTime gameTime) 
    {
        _soundManager.PlaySoundTrack();
        UpdateGameState(gameTime);
        
    }
            
    // Events
    public event EventHandler<BaseGameStateEvent> OnEventNotification;
    protected void NotifyEvent(BaseGameStateEvent gameEvent)
    {
        OnEventNotification?.Invoke(this, gameEvent);

        foreach(var gameObject in _gameObjects)
        {
            gameObject.OnNotify(gameEvent);
        }

        _soundManager.OnNotify(gameEvent);
    }

    public event EventHandler<BaseGameState> OnStateSwitched;
    protected void SwitchState(BaseGameState gameState)
    {
        OnStateSwitched?.Invoke(this, gameState);
    }

    // Objects
    protected void AddGameObject(BaseGameObject gameObject)
    {
        _gameObjects.Add(gameObject);
    }

    protected void RemoveGameObject(BaseGameObject gameObject)
    {
        _gameObjects.Remove(gameObject);
    }

    public virtual void Render(SpriteBatch spriteBatch)
    {
        foreach (var gameObject in _gameObjects.OrderBy(a => a.zIndex))
        {
            gameObject.Render(spriteBatch);
        }     
    }

}
