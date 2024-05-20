using Microsoft.Xna.Framework;
using Engine.States;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Collections;

namespace JetSquid
{
    public class JetSquid : MainGame
    {
        public JetSquid(int width, int height, BaseGameState firstGameState, bool debug) : base(width, height, firstGameState, debug) { }


        protected override void _currentGameState_OnEventNotification(object sender, BaseGameStateEvent e)
        {
            base._currentGameState_OnEventNotification(sender, e);

        }

    }
}
