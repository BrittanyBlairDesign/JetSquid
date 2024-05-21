
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Engine.Objects;
using Engine.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.States;
public class SplashState : BaseGameState
{
    public override void LoadContent(GraphicsDevice graphics = null)
    {
        AddGameObject(new SplashImage(LoadTexture("Empty")));
        
        if (graphics != null)
        {
            isDebug = true;
            this.graphics = graphics;
        }
    }

    public override void HandleInput(GameTime gameTime, Point MousePosition)
    {
        _inputManager.GetCommands(cmd =>
        {
            if (cmd is SplashInputCommand.GameSelect)
            {
                SwitchState(new GameplayState());
            }

            if (cmd is SplashInputCommand.GameExit)
            {
                NotifyEvent(new BaseGameStateEvent.GameQuit());
            }
        });
    }
    protected override void SetInputManager()
    {
        _inputManager = new InputManager(new SplashInputMapper());
    }

    public override void UpdateGameState(GameTime gameTime)
    {
       
    }
}

