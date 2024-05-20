
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Diagnostics;
using System.IO;
using System.Text.Json;

using Engine.States;
using Engine.System;
using System;

enum Scenes
{
    START,
    GAME,
    LOSE    
}
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
    private Rectangle _renderScaleRectangle;
    private bool isResizing = false;



//  Desired resolution for the game.
    private int DESIGNED_RESOLUTION_WIDTH;
    private int DESIGNED_RESOLUTION_HEIGHT;
    private float DESIGNED_RESOLUTION_ASPECT_RATIO;

    public MainGame(int width, int height, BaseGameState firstGameState)
    {
        Content.RootDirectory = "Content";
        _graphics = new GraphicsDeviceManager(this);

        _firstGameState = firstGameState;
        DESIGNED_RESOLUTION_WIDTH = width;
        DESIGNED_RESOLUTION_HEIGHT = height;
        DESIGNED_RESOLUTION_ASPECT_RATIO = width / (float)height;

        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnClientSizeChanged;
    }

    public MainGame(int width, int height, BaseGameState firstGameState, bool Debug)
    {
        Content.RootDirectory = "Content";
        _graphics = new GraphicsDeviceManager(this);

        _firstGameState = firstGameState;
        DESIGNED_RESOLUTION_WIDTH = width;
        DESIGNED_RESOLUTION_HEIGHT = height;
        DESIGNED_RESOLUTION_ASPECT_RATIO = width / (float)height;

        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnClientSizeChanged;

        isDebug = Debug;
    }

    private void OnClientSizeChanged(object sender, EventArgs e)
    {
        if (!isResizing && Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0)
        {
            isResizing = true;
            _renderScaleRectangle = GetScaleRectangle();
            isResizing = false;
        }
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = DESIGNED_RESOLUTION_WIDTH;
        _graphics.PreferredBackBufferHeight = DESIGNED_RESOLUTION_HEIGHT;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();

        _renderTarget = new RenderTarget2D(_graphics.GraphicsDevice, DESIGNED_RESOLUTION_WIDTH, DESIGNED_RESOLUTION_HEIGHT, false,
                        SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);

        _renderScaleRectangle = GetScaleRectangle();

        this.IsMouseVisible = true;
        base.Initialize();
    }

    private Rectangle GetScaleRectangle()
    {
        var variance = 0.5;
        var actualAspectRatio = Window.ClientBounds.Width / (float)Window.ClientBounds.Height;

        Rectangle scaleRectangle;

        if (actualAspectRatio <= DESIGNED_RESOLUTION_ASPECT_RATIO)
        {
            var presentHeight = (int)(Window.ClientBounds.Width / DESIGNED_RESOLUTION_ASPECT_RATIO + variance);
            var barHeight = (Window.ClientBounds.Height - presentHeight) / 2;

            _graphics.PreferredBackBufferHeight = presentHeight ;
            _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;

            scaleRectangle = new Rectangle(0, 0, Window.ClientBounds.Width, presentHeight);
        }
        else
        {
            var presentWidth = (int)(Window.ClientBounds.Height * DESIGNED_RESOLUTION_ASPECT_RATIO + variance);
            var barWidth = (Window.ClientBounds.Width - presentWidth) / 2;

            _graphics.PreferredBackBufferWidth = presentWidth;
            _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

            scaleRectangle = new Rectangle(0, 0, presentWidth, Window.ClientBounds.Height);
        }

        _graphics.ApplyChanges();
        return scaleRectangle;
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

        _currentGameState.Initialize(Content, DESIGNED_RESOLUTION_WIDTH, DESIGNED_RESOLUTION_HEIGHT);
        GetScaleRectangle();


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
        _currentGameState.HandleInput(gameTime);

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
        

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _currentGameState.Render(_spriteBatch);

        _spriteBatch.End();

        // Now render the scaled content
        _graphics.GraphicsDevice.SetRenderTarget(null);

        _graphics.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

        _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, samplerState: SamplerState.PointClamp) ;

        _spriteBatch.Draw(_renderTarget, _renderScaleRectangle, Color.White);

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
