
namespace Engine.Stats
{
    public class BaseStat
    {
        public int _value { get; private set; } // current value of a stat
        public int _maxValue { get; private set; } // max possible value
        public int _minValue { get; private set; } // min possible value


        public BaseStat(int value = 0, int max = 0, int min = 0)
        {
            _value = value;
            _maxValue = max;
            _minValue = min;
        }

        public virtual void IncreaseValue ( int amount)
        {
            if( _value + amount > _maxValue ) { _value = _maxValue;}
            else { _value += amount; }
        }

        public virtual void DecreaseValue(int amount)
        {
            if(_value - amount < _minValue) { _value = _minValue;}
            else { _value -= amount; }
        }

        public virtual void ResetValue()
        {
            _value = _maxValue;
        }
    }

    /// <summary>
    /// TO DO : Upgrade this when you get a CSV custom importer or  JSON Importer.
    ///     include a data table & Row reference so it can preform a Level Up () function. 
    /// </summary>
    public class LevelingStat : BaseStat
    {
        public int _level;
        public LevelingStat(int value = 0, int max = 0, int min = 0, int Level = 0)
            : base(value, max, min) { _level = Level; }
    }
}
