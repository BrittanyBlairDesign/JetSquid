using Engine.Input;

namespace JetSquid;

public class JetSquidGameInputCommand : BaseInputCommand
{
    public class PlayerJump : JetSquidGameInputCommand { }
    public class PlayerHover : JetSquidGameInputCommand { }
    public class PlayerPause : JetSquidGameInputCommand { }
    public class PlayerExit : JetSquidGameInputCommand { }
    public class PlayerFall : JetSquidGameInputCommand { }

}
