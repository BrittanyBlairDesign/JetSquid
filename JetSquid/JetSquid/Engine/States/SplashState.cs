
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Engine.Objects;
using Engine.Input;

namespace Engine.States;
public class SplashState : BaseGameState
{
    public override void LoadContent()
    {
        AddGameObject(new SplashImage(LoadTexture("SplashScreens/Splash")));
    }

    public override void HandleInput(GameTime gameTime)
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

