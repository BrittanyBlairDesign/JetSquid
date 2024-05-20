using Engine.Input;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.ComponentModel.Design;
namespace JetSquid;

public class JetSquidGameInputMapper : BaseInputMapper
{
    private bool isJumping = false;
    private bool isPaused = false;

    public override IEnumerable<BaseInputCommand> GetKeyboardState(KeyboardState state)
    {
        var commands = new List<BaseInputCommand>();

        // Jumping or Hovering in the air
        if (state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Up))
        {
            if (isJumping)
            {
                commands.Add(new JetSquidGameInputCommand.PlayerHover());
            }
            else
            {
                commands.Add(new JetSquidGameInputCommand.PlayerJump());
                isJumping = true;
            }
        }
        else if (state.IsKeyUp(Keys.W) && state.IsKeyUp(Keys.Up) && isJumping)
        {
            isJumping = false;
            commands.Add(new JetSquidGameInputCommand.PlayerFall());
        }

        if(state.IsKeyDown(Keys.Escape))
        {
            commands.Add(new JetSquidGameInputCommand.PlayerExit());
        }

        if( state.IsKeyDown(Keys.Tab))
        {
            if(!isPaused)
            {
                isPaused = true;
            }    
            else
            {
                isPaused = false;
            }
            commands.Add(new JetSquidGameInputCommand.PlayerPause());
        }

        return commands;
    }
}
