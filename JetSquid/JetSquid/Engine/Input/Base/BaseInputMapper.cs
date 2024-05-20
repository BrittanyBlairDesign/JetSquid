using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace Engine.Input;

public class BaseInputMapper 
{
    public virtual IEnumerable<BaseInputCommand> GetKeyboardState(KeyboardState state)
    {
        return new List<BaseInputCommand>();
    }

    public virtual IEnumerable<BaseInputCommand> GetMouseState(MouseState state)
    {
        return new List<BaseInputCommand>();
    }

    public virtual IEnumerable<BaseInputCommand> GetGamePadState(GamePadState state)
    {
        return new List<BaseInputCommand>();
    }

    public virtual IEnumerable<BaseInputCommand> GetTouchPanelState(TouchPanelState state)
    {
        return new List<BaseInputCommand>();
    }
}

