
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

public class SplashInputMapper : BaseInputMapper
{
    public override IEnumerable<BaseInputCommand> GetKeyboardState(KeyboardState state)
    {
        var commands = new List<SplashInputCommand>();

        if (state.IsKeyDown(Keys.Enter))
        {
            commands.Add(new SplashInputCommand.GameSelect());
        }
        if (state.IsKeyDown(Keys.Escape))
        {
            commands.Add(new SplashInputCommand.GameExit());
        }

        return commands;
    }

    public override IEnumerable<BaseInputCommand> GetMouseState(MouseState state)
    {
        var Commands = new List<SplashInputCommand>();

        if(state.LeftButton == ButtonState.Pressed)
        {
            Commands.Add(new SplashInputCommand.GameSelect());
            Trace.WriteLine("\t LEFT MOUSE BUTTON PRESSED.");
        }

        return Commands;
    }
}