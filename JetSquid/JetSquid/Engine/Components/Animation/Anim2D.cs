

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSheetAnimationContentPipeline;
using System.Data;
using System.Diagnostics;

namespace Engine.Components.Animation
{
    public class Anim2D : IAnimation
    {
        protected Texture2D _spriteSheet; // sprite sheet for the animation
        public Vector2 _spriteSize { get; private set; } // the width and height of each sprite in the sheet
        protected float _scale; // How large the character is rendered

        protected int _frameCount; // How many frames are in the animation starts at 0
        protected int _frameIndex; // Our current frame index
        protected Rectangle _frame; // The location of our frame index on the sheet

        protected int _frameRate; // How many frames are rendered per second
        protected float _frameTimer; // count down for loading the next frame
        protected float _frameDuration; // how long frames are held before changing

        protected PlayState _playState; // Current PlayState of the animation
        public bool isLooping { get; set; } // Whether or not the animation is a looping animation
        public bool isRewind { get; set; } // is the animation supposed to play backwards
        public bool _Debug { get; private set; }

        public Anim2D(Texture2D spriteSheet, Vector2 spriteSize, int frameRate, int frameCount, float scale = 1.0f, bool looping = false, PlayState startState = PlayState.PAUSE, bool debug = false)
        {
            _spriteSheet = spriteSheet;
            _spriteSize = spriteSize;

            _frameCount = frameCount;
            isLooping = looping;

            ChangePlayState(startState);
            ChangeFrameRate(frameRate);
            _Debug = debug;
        }

        public virtual void ChangeFrameRate(int rate)
        {
            _frameRate = rate;
            _frameDuration = 1.0f / _frameRate;
        }

        public virtual void ChangePlayState(PlayState state)
        {
            _playState = state;

            if (_playState == PlayState.REWIND)
            {
                isRewind = true;
            }
            else
            {
                isRewind = false;
            }
        }

        public virtual void Reset()
        {
            if (isRewind)
            {
                _frameIndex = _frameCount - 1;
            }
            else
            {
                _frameIndex = 0;
            }

            // Reset the frame position to the start / end of the animation
            _frame.X = (int)(_frameIndex * _spriteSize.X);


            // If the animation loops keep playing the animation. otherwise stop the animation.
            if (isLooping && isRewind) { _playState = PlayState.REWIND; }
            else if (isLooping && !isRewind) { _playState = PlayState.PLAY; }
            else { _playState = PlayState.PAUSE; }
        }

        public virtual void Update(float deltaTime)
        {

            if (_playState == PlayState.RESET)
            {
                Reset();
            }

            if (_playState != PlayState.PAUSE)
            {

                if (_frameTimer <= 0.0f)
                {
                    switch (_playState)
                    {
                        case PlayState.PLAY: // Add 1 to the frame index
                            _frameIndex++;
                            if (_frameIndex + 1 >= _frameCount)
                            {
                                _playState = PlayState.RESET;
                            }
                            break;
                        case PlayState.REWIND: // subtract 1 from the frameIndex
                            _frameIndex--;
                            if (_frameIndex - 1 < 0)
                            {
                                _playState = PlayState.RESET;
                            }
                            break;
                    }

                    // Move the X position of the rectangle to the next frame
                    _frame.X = (int)(_frameIndex * _spriteSize.X);
                    _frameTimer = _frameDuration;
                }
                else
                {
                    // subtract deltaTime from the timer
                    _frameTimer -= deltaTime;
                }

            }
        }

        public PlayState GetPlayState()
        {
            return _playState;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 location, SpriteEffects effects, Color color)
        {
            spriteBatch.Draw(_spriteSheet, location, _frame, color, 0.0f, Vector2.Zero, 0.5f, effects, 1.0f); ;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 location, SpriteEffects effects)
        {
            spriteBatch.Draw(_spriteSheet, location, _frame, Color.White, 0.0f, Vector2.Zero, 0.5f, effects, 1.0f); ;
        }

    }

    public class SpriteAnim : Anim2D
    {
        SpriteAnimation _animationData;
        public SpriteAnim(Texture2D spriteSheet, SpriteAnimation animation, float scale = 1.0f, bool loop = false, PlayState state = PlayState.PAUSE ):
                     base(spriteSheet, new Vector2(animation.SpriteWidth, animation.SpriteHeight), animation.FrameRate, animation.FrameCount, scale ,loop, state)
        {
            _animationData = animation;
            _frame = new Rectangle(0, _animationData.Row * _animationData.SpriteHeight, _animationData.SpriteWidth, _animationData.SpriteHeight);
           
        }

        public string GetAnimationName()
        {
            return _animationData.Name;
        }

        public override void Update(float deltaTime)
        {
            if (_playState == PlayState.RESET)
            {
                Reset();
            }

            if (_playState != PlayState.PAUSE)
            {

                if (_frameTimer <= 0.0f)
                {
                    switch (_playState)
                    {
                        case PlayState.PLAY: // Add 1 to the frame index
                            _frameIndex++;
                            if (_frameIndex + 1 >= _frameCount)
                            {
                                _playState = PlayState.RESET;
                            }
                            break;
                        case PlayState.REWIND: // subtract 1 from the frameIndex
                            _frameIndex--;
                            if (_frameIndex - 1 < _animationData.StartFrame)
                            {
                                _playState = PlayState.RESET;
                            }
                            break;
                    }

                    // Move the X position of the rectangle to the next frame
                    _frame.X = (int)(_frameIndex * _spriteSize.X);

                    if(_Debug)
                    {
                        Trace.WriteLine(" Animation Row = " + _animationData.Row);
                        Trace.WriteLine(" Rectangle Y Position = " + _frame.Y);
                    }

                    _frameTimer = _frameDuration;
                }
                else
                {
                    // subtract deltaTime from the timer
                    _frameTimer -= deltaTime;
                }

            }
        }

        public override void Reset()
        {
            if (isRewind)
            {
                _frameIndex = _frameCount - 1;
            }
            else
            {
                _frameIndex = _animationData.StartFrame ;
            }

            // Reset the frame position to the start / end of the animation
            _frame.X = (int)(_frameIndex * _spriteSize.X);

            // If the animation loops keep playing the animation. otherwise stop the animation.
            if (isLooping && isRewind) { _playState = PlayState.REWIND; }
            else if (isLooping && !isRewind) { _playState = PlayState.PLAY; }
            else { _playState = PlayState.PAUSE; }
        }

    }
    
}