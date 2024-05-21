
using Engine.States;
using Engine.Objects;
using Engine.Objects.UI;
using Engine.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JetSquid;

public class JetSquidStartState : SplashState
{
    private static string UIFolderRoot = "JetSquid/UI/";
    private string MenuBackgroundTexture = UIFolderRoot + "MenuBackground";
    private string MenuLogoTexture = UIFolderRoot + "MenuTitleFullSzie";

    private ButtonObject StartButton;
    private string StartButtonTexture = UIFolderRoot + "StartButtton";

    private ButtonObject ExitButton;
    private string ExitButtonTexture = UIFolderRoot + "ExitButtton";

    // Loads all content needed for the Start Menu
    public override void LoadContent(GraphicsDevice graphics = null)
    {
        AddGameObject(new SplashImage(LoadTexture(MenuBackgroundTexture)));
        AddGameObject(new SplashImage(LoadTexture(MenuLogoTexture)));

        Texture2D startTX = LoadTexture(StartButtonTexture);
        Vector2 startPos = new Vector2((_viewportWidth / 2 - startTX.Width / 2), (_viewportHeight / 2 + 100));
        StartButton = new ButtonObject(startTX, startPos, 0, 0, 0, -70);
        AddGameObject(StartButton);

        Texture2D exitTX = LoadTexture(ExitButtonTexture);
        Vector2 EndPos = new Vector2((_viewportWidth / 2 + exitTX.Width / 2 ), (_viewportHeight / 2 + 100));
        ExitButton = new ButtonObject(exitTX, EndPos, 0, 0, 0, -70);
        AddGameObject(ExitButton);

        if(graphics != null)
        {
            isDebug = true;
            this.graphics = graphics;
        }
    }
    protected override void SetInputManager()
    {
        _inputManager = new InputManager(new JetSquidMenuInputMapper());
    }

    public override void HandleInput(GameTime gameTime, Point MousePosition)
    {
        _inputManager.GetCommands(cmd =>
        {
            if (cmd is JetSquidMenuInputCommand.StartGame)
            {
                SwitchState(new JetSquidGameplayState());
            }

            if (cmd is JetSquidMenuInputCommand.ExitGame)
            {
                NotifyEvent(new BaseGameStateEvent.GameQuit());
            }
            
            if (cmd is JetSquidMenuInputCommand.MouseMovement)
            {
               
            }

            if (cmd is JetSquidMenuInputCommand.MouseClick)
            {
                if(StartButton.isHovering(MousePosition))
                {
                    SwitchState(new JetSquidGameplayState());
                }
                else if( ExitButton.isHovering(MousePosition))
                {
                    NotifyEvent(new BaseGameStateEvent.GameQuit());
                }
            }
        });
    }

}