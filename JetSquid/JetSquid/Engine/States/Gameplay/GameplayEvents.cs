

public class GameplayEvents : BaseGameStateEvent
{
    public class PlayerJumps : GameplayEvents { }
    public class PlayerLoose : GameplayEvents { }
    public class PlayerPoints : GameplayEvents { }
}
