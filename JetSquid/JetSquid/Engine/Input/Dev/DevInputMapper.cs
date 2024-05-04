
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class DevInputMapper : BaseInputMapper
{
    public override IEnumerable<BaseInputCommand> GetKeyboardState(KeyboardState state)
    {
        var commands = new List<DevInputCommand>();

        if(state.IsKeyDown(Keys.Escape))
        {
            commands.Add(new DevInputCommand.DevQuit());
        }

        if(state.IsKeyDown(Keys.Space))
        {
            commands.Add(new DevInputCommand.DevShoot());
        }

        if(state.IsKeyDown(Keys.Tab))
        {
            commands.Add(new DevInputCommand.DevPause());
        }

        if(state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift))
        {
            commands.Add(new DevInputCommand.DevJump());
        }

        if(state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Up) ||
           state.IsKeyDown(Keys.A) || state.IsKeyDown(Keys.Left) ||
           state.IsKeyDown(Keys.S) || state.IsKeyDown(Keys.Right) ||
           state.IsKeyDown(Keys.D) || state.IsKeyDown(Keys.Down))
        {
            commands.Add(new DevInputCommand.DevMove());
        }

        return commands;
    }
}