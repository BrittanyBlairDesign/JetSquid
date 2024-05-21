
using Microsoft.Xna.Framework;

namespace Engine.Components.Physics
{
    public class Movement
    {
        protected float _speed;
        protected float _startSpeed;
        protected Direction dir;
        protected Vector2 _direction;
        protected Vector2 _velocity;
        public bool _decelerate { get; set; }
        public Movement(float speed = 0.0f, Direction direction = Direction.STOP)
        {
            _speed = speed;
            _startSpeed = speed;
            ChangeDirection(direction);
        }
        public Movement(float speed, Vector2 direction)
        {
            _speed = speed;
            _startSpeed = speed;
            _direction = direction;
        }

        public Movement()
        {
            _speed = 0.0f;
        }

        public virtual Vector2 Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_decelerate)
            {
                _direction = DecayVector(deltaTime, _direction, new Directions().GetDirection(dir));
            }

            return CalculateVelocity(gameTime);
        }

        protected virtual Vector2 CalculateVelocity(GameTime gameTime)
        {
            _velocity = (_speed * _direction);
            return _velocity;
        }

        public void ChangeDirection(Direction direction)
        {
            if (!_decelerate)
            {
                _direction = new Directions().GetDirection(direction);
            }
            else
            {
                dir = direction;
            }
        }

        public void ChangSpeed(float speed)
        {
            _speed = speed;
        }

        public void ResetSpeed()
        {
            _speed = _startSpeed;
        }

        // Slowly changes a vector's X and Y values to equal 0.0f
        protected virtual Vector2 DecayVector(float deltaTime, Vector2 V)
        {
            if (V.X < 0)
            {
                V.X += deltaTime;
            }
            else if (V.X > 0)
            {
                V.X -= deltaTime;
            }

            if (V.Y < 0)
            {
                V.Y += deltaTime;
            }
            else if (V.Y > 0)
            {
                V.Y -= deltaTime;
            }

            return V;
        }
        // slowly changes a vector's X and Y values to match the X and Y values of the target;
        protected virtual Vector2 DecayVector(float deltaTime, Vector2 V, Vector2 target)
        {
            if (V.X < target.X)
            {
                V.X += deltaTime;
            }
            else if (V.X > target.X)
            {
                V.X -= deltaTime;
            }

            if (V.Y < target.Y)
            {
                V.Y += deltaTime;
            }
            else if (V.Y > target.Y)
            {
                V.Y -= deltaTime;
            }

            return V;
        }
    }

    public class PhysicsMovement : Movement
    {

        protected Vector2 _acceleration = Vector2.Zero;
        protected Vector2 _maxAcceleration;
        protected float _mass;
        protected Vector2 _gravity = Vector2.Zero;
        protected Vector2 _force = Vector2.Zero;

        public PhysicsMovement(Vector2 maxAcceleration, float mass, Vector2 gravity)
        {
            _maxAcceleration = maxAcceleration;
            _mass = mass;
            _gravity = gravity;
        }

        protected override Vector2 CalculateVelocity(GameTime gameTime)
        {
            // Get the deltaTime.
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate the amount of thrust being applied to the object.
            if (_decelerate)
            {
                DecayVector(deltaTime, _force);
            }

            Vector2 thrust = _gravity + _force;

            // Don't go above maxAcceleration or below -maxAcceleration
            CalculateAcceleration(thrust);

            // Accelerate in a direction.
            Vector2 speed = _direction * _acceleration;

            // Set Velocity
            _velocity += (speed + (_mass * _gravity)) * deltaTime;

            return _velocity;
        }

        protected virtual void CalculateAcceleration(Vector2 force)
        {
            _acceleration += (force / _mass);

            if (_acceleration.X > _maxAcceleration.X)
            {
                _acceleration.X = _maxAcceleration.X;
            }
            else if (_acceleration.X < -_maxAcceleration.X)
            {
                _acceleration.X = -_maxAcceleration.X;
            }

            if (_acceleration.Y > _maxAcceleration.Y)
            {
                _acceleration.Y = _maxAcceleration.Y;
            }
            else if (_acceleration.Y < -_maxAcceleration.Y)
            {
                _acceleration.Y = -_maxAcceleration.Y;
            }
        }

        public virtual void SetForce(Vector2 force)
        {
            _force = force;
        }
    }
}
