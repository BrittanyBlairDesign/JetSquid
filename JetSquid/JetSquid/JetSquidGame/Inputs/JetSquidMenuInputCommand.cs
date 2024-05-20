using Engine.Input;
namespace JetSquid;

public class JetSquidMenuInputCommand : BaseInputCommand
{
    public class StartGame : JetSquidMenuInputCommand { }
    public class ExitGame : JetSquidMenuInputCommand { }
    public class RetryGame : JetSquidMenuInputCommand { }
    public class MouseClick : JetSquidMenuInputCommand { }
    public class MouseMovement : JetSquidMenuInputCommand { }
}