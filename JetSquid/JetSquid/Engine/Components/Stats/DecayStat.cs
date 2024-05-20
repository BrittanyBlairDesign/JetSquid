


using Engine.Stats;
using System.Diagnostics;

namespace JetSquid
{
    public class DecayStat : BaseStat
    {
        private int DecayAmount;
        private float DecayRate;
        private float DecayTimer;

        public DecayStat(int decayAmount, float decayRate, int value, int max = 0, int min = 0 ) 
                  : base(value, max, min)
        {
            DecayAmount = decayAmount;
            DecayRate = decayRate;
            DecayTimer = DecayRate;
        }

        public void Update(float deltaTime)
        {
            if(DecayTimer <= 0.0f)
            {
                DecreaseValue(DecayAmount);
                Trace.WriteLine("Decay Stat Value : " + _value);

                if(_value > 0)
                {
                    DecayTimer = DecayRate;
                }
            }
            else
            {
                DecayTimer -= deltaTime;
            }
        }

        public override void ResetValue()
        {
            DecayTimer = DecayRate;
            base.ResetValue();
        }
    }
}
