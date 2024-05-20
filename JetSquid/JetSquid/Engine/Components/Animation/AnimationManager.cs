using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSheetAnimationContentPipeline;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.Components.Animation
{
    public class AnimationManager2D
    {
        protected List<SpriteSheetAnimation> _animatedSpriteSheets = new List<SpriteSheetAnimation>();
        protected Dictionary<string, SpriteAnim> _animations = new Dictionary<string, SpriteAnim>();
        public Anim2D _currentAnimation;
        public string _nextAnimation;
        public bool _doesNextLoop;

        public Vector2 _spriteSize;
        public int _averageFrameRate;

        private bool _Debug;

        public AnimationManager2D( int averageFrameRate, bool debug = false)
        {
            
            _averageFrameRate = averageFrameRate;
            _Debug = debug;
        }

        public void AddSpriteSheet(SpriteSheetAnimation spriteSheet, float scale)
        {
           
            _animatedSpriteSheets.Add(spriteSheet);

            for(int i = 0; i < spriteSheet.Animations.Count; i++)
            {
                
                SpriteAnim anim = new SpriteAnim(spriteSheet.TextureSheet, spriteSheet.Animations[i],scale);

                _animations.Add(spriteSheet.Animations[i].Name, anim);

            }

            _spriteSize = _animations["Walk"]._spriteSize;

        }

        public virtual void Update(float deltaTime)
        {
            _currentAnimation.Update(deltaTime);

            if(_currentAnimation.GetPlayState() == PlayState.PAUSE)
            {
                SwitchAnimations(_nextAnimation, _doesNextLoop);
            }
        }


        public virtual void SwitchAnimations(string animationName, bool loop = false)
        {
            if (_currentAnimation != _animations[animationName])
            {
                PlayState previousState = PlayState.PLAY;
                if (_currentAnimation != null && _currentAnimation.isLooping)
                {
                    previousState = _currentAnimation.GetPlayState();
                    _currentAnimation.ChangePlayState(PlayState.PAUSE);
                    _currentAnimation.Reset();
                }
                else if ( _currentAnimation != null && !_currentAnimation.isLooping)
                {
                    SetNextAnimation(animationName);
                }

                if (_animations.ContainsKey(animationName))
                {
                    
                    _currentAnimation = _animations[animationName];
                    _currentAnimation.ChangePlayState(PlayState.PLAY);
                    _currentAnimation.isLooping = loop;
                }
                else
                {
                    //Trace.WriteLine(" Animation " + animationName + " could not be found.");
                    foreach (string key in _animations.Keys)
                    {
                        //Trace.WriteLine(key + ": " + " animation\n");
                    }
                    if (_currentAnimation != null)
                    { _currentAnimation.ChangePlayState(previousState); }
                }
            }
            else if (_currentAnimation != null && !_currentAnimation.isLooping)
            {
                _currentAnimation.Reset();
            }
        }

        public virtual void SetNextAnimation(string NextAnimationName, bool Loop = false)
        {
            if(_animations.ContainsKey(NextAnimationName) && _nextAnimation != NextAnimationName)
            {
                _nextAnimation = NextAnimationName;
                _doesNextLoop = Loop;

                if (_Debug)
                {
                    Trace.WriteLine(" Next Animation : " + NextAnimationName);
                }
                
            }
        }
        public virtual string[] GetAnimationNames()
        {
            string[] names = new string[_animations.Keys.Count];
            int index = 0;
            foreach(string i in  _animations.Keys)
            {
                names[index] = i;
                index++;
            }

            return names;
        }

        public virtual bool CheckCurrentAnimation(string animationName)
        {
            return _currentAnimation == _animations[animationName];
        }
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects effects = SpriteEffects.None)
        {
            if(_currentAnimation != null)
            _currentAnimation.Draw(spriteBatch, position, effects);
        }
    }
}
