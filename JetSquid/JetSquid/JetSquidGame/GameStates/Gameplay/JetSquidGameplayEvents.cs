
using Engine.States;
namespace JetSquid;
public class JetSquidGameplayEvents: GameplayEvents
{
     //  All the Player Specific events
     //     Input or Animation Events
    public class PlayerJump : JetSquidGameplayEvents { }
    public class PlayerHover : JetSquidGameplayEvents { }
    public class PlayerFall : JetSquidGameplayEvents { }
    public class PlayerFloorCollide : JetSquidGameplayEvents { }

    //      Damage events
    public class PlayerDie : JetSquidGameplayEvents { }
    public class PlayerTakeDamage : JetSquidGameplayEvents { }

    //      collection events
    public class PlayerEarnPoints : JetSquidGameplayEvents { }
    public class PlayerRefillInk : JetSquidGameplayEvents { }
    public class PlayerCollectHearts : JetSquidGameplayEvents { }


    // All the object specific events
    public class DamageObstacle : JetSquidGameplayEvents { }
}
