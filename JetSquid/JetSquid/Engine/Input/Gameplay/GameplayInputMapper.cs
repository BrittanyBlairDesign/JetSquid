
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class GameplayInputMapper : BaseInputMapper
{
    public override IEnumerable<BaseInputCommand> GetKeyboardState(KeyboardState state)
    {
        var commands = new List<GameplayInputCommand>();

        if( state.IsKeyDown(Keys.Escape) )
        {
            commands.Add(new GameplayInputCommand.GameExit());
        }

        if(state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.W))
        {
            commands.Add(new GameplayInputCommand.PlayerJump());
        }
        return commands;
    }
}