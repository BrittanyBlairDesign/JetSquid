
using Engine.Input;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
namespace JetSquid;

public class JetSquidMenuInputMapper : BaseInputMapper
{
    private Point MousePosition;
    public override IEnumerable<BaseInputCommand> GetKeyboardState(KeyboardState state)
    {
        var commands = new List<JetSquidMenuInputCommand>();

        if(state.IsKeyDown(Keys.Enter))
        {
            commands.Add(new JetSquidMenuInputCommand.StartGame());
            commands.Add(new JetSquidMenuInputCommand.RetryGame());
            Trace.WriteLine("\t ENTER KEY PRESSED");
        }
        if(state.IsKeyDown(Keys.Escape))
        {
            commands.Add(new JetSquidMenuInputCommand.ExitGame());
            Trace.WriteLine("\t ESCAPE KEY PRESSED");
        }

        return commands;
    }

    public override IEnumerable<BaseInputCommand> GetMouseState(MouseState state)
    {
        var Commands = new List<JetSquidMenuInputCommand>();

        if(state.LeftButton == ButtonState.Pressed)
        {
            Commands.Add(new JetSquidMenuInputCommand.MouseClick());
            Trace.WriteLine("\t LEFT MOUSE BUTTON PRESSED");
        }
        if(state.X != MousePosition.X || state.Y != MousePosition.Y)
        {
            MousePosition.X = state.X;
            MousePosition.Y = state.Y;

            Commands.Add(new JetSquidMenuInputCommand.MouseMovement());
        }

        return Commands;
    }
}