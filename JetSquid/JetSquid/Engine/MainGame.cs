
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Diagnostics;


using Engine.States;
using Engine.System;
using Engine.Viewports;



public class MainGame : Game
{
    protected BaseGameState _currentGameState;
    protected BaseGameState _firstGameState;
    protected GraphicsDeviceManager _graphics;
    protected SpriteBatch _spriteBatch;

//  Game Settings 
    protected bool isDebug = false;
    protected bool isPaused = false;

//  Render Target
    private RenderTarget2D _renderTarget;

//  Desired resolution for the game.
    private int DESIGNED_RESOLUTION_WIDTH;
    private int DESIGNED_RESOLUTION_HEIGHT;
    private float DESIGNED_RESOLUTION_ASPECT_RATIO;

//  Viewport that Scales With The Window
    private ScalingViewport _Viewport;

    public MainGame(int width, int height, BaseGameState firstGameState, bool Debug = false)
    {
        Content.RootDirectory = "Content";
        _graphics = new GraphicsDeviceManager(this);

        _firstGameState = firstGameState;
        DESIGNED_RESOLUTION_WIDTH = width;
        DESIGNED_RESOLUTION_HEIGHT = height;
        DESIGNED_RESOLUTION_ASPECT_RATIO = width / (float)height;

        Window.AllowUserResizing = true;
        isDebug = Debug;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = DESIGNED_RESOLUTION_WIDTH;
        _graphics.PreferredBackBufferHeight = DESIGNED_RESOLUTION_HEIGHT;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();

        _renderTarget = new RenderTarget2D(_graphics.GraphicsDevice, DESIGNED_RESOLUTION_WIDTH, DESIGNED_RESOLUTION_HEIGHT, false,
                        SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);

        _Viewport = new ScalingViewport(Window, _graphics, DESIGNED_RESOLUTION_WIDTH, DESIGNED_RESOLUTION_HEIGHT, DESIGNED_RESOLUTION_ASPECT_RATIO);
        _Viewport.RefreshViewport();
        this.IsMouseVisible = true;
        base.Initialize();
    }


    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        SwitchGameState(_firstGameState);
    }

    private void CurrentGameState_OnStateSwitched(object sender, BaseGameState e)
    {
        SwitchGameState(e);
    }

    protected void SwitchGameState(BaseGameState gameState)
    {
        if (_currentGameState != null)
        {
            _currentGameState.OnStateSwitched -= CurrentGameState_OnStateSwitched;
            _currentGameState.OnEventNotification -= _currentGameState_OnEventNotification;
            _currentGameState.UnloadContent();
        }

        
        _currentGameState = gameState;
        _Viewport.RefreshViewport();
        _currentGameState.Initialize(Content, DESIGNED_RESOLUTION_WIDTH, DESIGNED_RESOLUTION_HEIGHT, _Viewport);
 


        if (isDebug)
        {
            Trace.WriteLine(" DESIGNED_RESOLUTION_WIDTH : " + DESIGNED_RESOLUTION_WIDTH);
            Trace.WriteLine(" DESIGNED_RESOLUTION_HEIGHT : " + DESIGNED_RESOLUTION_HEIGHT);

            _currentGameState.LoadContent(_graphics.GraphicsDevice);
        }
        else
        {
            _currentGameState.LoadContent();
        }
        _currentGameState.OnStateSwitched += CurrentGameState_OnStateSwitched;
        _currentGameState.OnEventNotification += _currentGameState_OnEventNotification;

        if(isPaused)
        {
            isPaused = false;
        }
    }

    protected virtual void _currentGameState_OnEventNotification(object sender, BaseGameStateEvent e)
    {

        switch (e)
        {
            case BaseGameStateEvent.GameStart:
                SwitchGameState(new GameplayState());
                break;
            case BaseGameStateEvent.GameQuit:
                Exit();
                break;
            case BaseGameStateEvent.GamePause:
                isPaused = !isPaused;
                break;
        }
    }

    protected override void UnloadContent()
    {
        _currentGameState?.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        _currentGameState.HandleInput(gameTime, _Viewport.GetScaledMousePosition());

        if(!isPaused)
        {
            _currentGameState.Update(gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Render to the Render Target
        GraphicsDevice.SetRenderTarget(_renderTarget);

        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        _currentGameState.Render(_spriteBatch);

        _spriteBatch.End();

        // Now render the scaled content
        _graphics.GraphicsDevice.SetRenderTarget(null);

        _Viewport.RefreshViewport();

        _graphics.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

        _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, transformMatrix:_Viewport.GetScaleMatrix()) ;

        _spriteBatch.Draw(_renderTarget, Vector2.Zero, Color.White);
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    public void Quit()
    {
        this.Exit();
    }

    protected virtual void Save(SaveFile save) { }

    protected virtual SaveFile Load() { return null; }

}
