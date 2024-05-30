
using Engine.Components.Collision;
using Engine.Components.Physics;
using Engine.Input;
using Engine.Objects;
using Engine.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Serialization;
using SpriteSheetAnimationContentPipeline;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace JetSquid
{
    // Main Gameplay State.
    public class JetSquidGameplayState : GameplayState
    {

        // Scrolling backgorunds
        private string backgroundWall = "JetSquid/Objects/BrickWallBackground";
        private Terrain _wall;

        private string Floor = "JetSquid/Objects/Floor";
        private Terrain _floor;

        // Obstacles
        private string fishBucket = "JetSquid/Objects/FishBucketObstacle";
        private Texture2D fishBucketTex;
        private string crate = "JetSquid/Objects/Crate";
        private Texture2D crateTex;
        private string ceilingLight = "JetSquid/Objects/HangingLight";
        private Texture2D ceilingLightTex;
        private string airDucts = "JetSquid/Objects/AirDucts";
        private Texture2D airDuctsTex;
        private string ceilingFan = "JetSquid/Animation/Objects/FanSpriteAnim";
        private SpriteSheetAnimation ceilingFanTex;

        private List<Obstacle> floorObstacleList = new List<Obstacle>();
        private List<Obstacle> ceilingObstacleList = new List<Obstacle>();
        private List<AnimatedObstacle> fanObstacleList = new List<AnimatedObstacle>();

        private BoundingBox2D _CeilingSpawner;
        private BoundingBox2D _FloorSpawner;

        private float SpawnTimer;
        private float SpawnCoolDown;
        private int obstacleCount { get { return floorObstacleList.Count + ceilingObstacleList.Count + fanObstacleList.Count; } }
        private int maxObstacles = 6;

        // Level Speed
        private float levelSpeed = 3.0f;
        private Direction levelDirection = Direction.RIGHT;
        // Player
        private string playerTexture = "JetSquid/Animation/Player/SquidAnimatedSpriteSheet";
        private PlayerSquid _player;

        private string inkParticleTexture = "JetSquid/Particles/InkParticle";
        private InkJetEmitter _inkEmitter;

        public override void LoadContent(GraphicsDevice graphics = null)
        {
            // Background
            _wall = new Terrain(LoadTexture(backgroundWall), levelSpeed, levelDirection, _viewportWidth, _viewportHeight);
            AddGameObject(_wall);

            _floor = new Terrain(LoadTexture(Floor), levelSpeed, levelDirection, _viewportWidth, _viewportHeight);
            AddGameObject(_floor);

            // Player
            SpriteSheetAnimation animSheet = LoadAnimation(playerTexture);
            int spriteHeight = animSheet.Animations[0].SpriteHeight;
            Vector2 playerStartPos;
            playerStartPos.X = _viewportWidth / 3;
            playerStartPos.Y = _viewportHeight - ((spriteHeight * 0.5f) / 2);

            _inkEmitter = new InkJetEmitter(LoadTexture(inkParticleTexture), playerStartPos);
            AddGameObject(_inkEmitter);

            _player = new PlayerSquid(animSheet, playerStartPos, true, 0.5f, _inkEmitter);
            _playerSprite = _player;
            AddGameObject(_player);


            // Obstacle Spawners
            _CeilingSpawner = new BoundingBox2D(new Vector2(_viewportWidth + 500, 500 / 2), 500, 700);
            _FloorSpawner = new BoundingBox2D(new Vector2(_viewportWidth + 500, _viewportHeight - 500 / 2), 250, 250);

            // Obstacle Textures
            fishBucketTex = LoadTexture(fishBucket);
            crateTex = LoadTexture(crate);
            ceilingLightTex = LoadTexture(ceilingLight);
            airDuctsTex = LoadTexture(airDucts);
            ceilingFanTex = LoadAnimation(ceilingFan);

            if(graphics != null)
            {
                isDebug = true;
                this.graphics = graphics;
            }
        }

        public override void UpdateGameState(GameTime gameTime)
        {
            _player.Update(gameTime);
            KeepPlayerInBounds();
            
            foreach(Obstacle FO in floorObstacleList)
            {
                FO.Update(gameTime);
                if(FO.Position.X < 0 - FO.Width)
                {
                    FO.OnNotify(new JetSquidGameplayEvents.DamageObstacle());
                }
            }

            foreach(Obstacle CO in ceilingObstacleList)
            {
                CO.Update(gameTime);
                if (CO.Position.X < 0 - CO.Width)
                {
                    CO.OnNotify(new JetSquidGameplayEvents.DamageObstacle());
                }
            }
            
            foreach(AnimatedObstacle Fan in fanObstacleList)
            {
                Fan.Update(gameTime);
                if (Fan.Position.X < 0 - Fan.SpriteWidth)
                {
                    Fan.OnNotify(new JetSquidGameplayEvents.DamageObstacle());
                }
            }

            floorObstacleList = CleanObjects(floorObstacleList);
            ceilingObstacleList = CleanObjects(ceilingObstacleList);
            fanObstacleList = CleanObjects(fanObstacleList);
            
            if(SpawnTimer > 0.0f)
            {
                SpawnTimer -= (float)gameTime.TotalGameTime.TotalSeconds;
            }
            else
            {
                if (obstacleCount < maxObstacles)
                { AttemptSpawn(); }
            }
            
            base.UpdateGameState(gameTime);
        }

        private void AttemptSpawn()
        {
            bool spawnComplete = false;
            int FloorHeight = 0;

            RandomNumberGenerator rand = new RandomNumberGenerator();

            int floorSpawns = rand.NextRandom(3);
            for (int i = 0; i < floorSpawns; i++)
            {
                switch (rand.NextRandom(2))
                {
                    case 0:
                        spawnComplete = SpawnFloorObstacle(fishBucketTex, FloorHeight);
                        FloorHeight += fishBucketTex.Height;
                        break;
                    case 1:
                        spawnComplete = SpawnFloorObstacle(crateTex, FloorHeight);
                        FloorHeight += crateTex.Height;
                        break;
                    default:
                        break;
                }
            }

            Rectangle box2D;
            int ceilingOffest = rand.NextRandom(75);
            switch(rand.NextRandom(3))
            {
                case 0:
                    box2D = new Rectangle((int)_CeilingSpawner.Position.X,(int)_CeilingSpawner.Position.Y + (ceilingLightTex.Height - 100) , ceilingLightTex.Width, 100);
                    spawnComplete = SpawnCeilingObstacle(ceilingLightTex, box2D, ceilingOffest);
                    break;
                case 1:
                    box2D = new Rectangle((int)_CeilingSpawner.Position.X, (int)_CeilingSpawner.Position.Y, airDuctsTex.Width, airDuctsTex.Height);
                    spawnComplete = SpawnCeilingObstacle(airDuctsTex,box2D, ceilingOffest);
                    break;
                case 3:
                    box2D = new Rectangle((int)_CeilingSpawner.Position.X, (int)_CeilingSpawner.Position.Y +( (ceilingFanTex.Height - 100) - ceilingOffest), ceilingFanTex.Width/2, 100);
                    Vector2 position = _CeilingSpawner.Position;
                    position.Y -= ceilingOffest;
                    AnimatedObstacle FanObstacle = new AnimatedObstacle(ceilingFanTex, position, isDebug, 1, levelSpeed, levelDirection, new BoundingBox2D(position, ceilingFanTex.Width/2, 100));
                    AddGameObject(FanObstacle);
                    fanObstacleList.Add(FanObstacle);
                    break;
                default:
                    break;
            }

            if (spawnComplete)
            {
                SpawnTimer = SpawnCoolDown;
            }
        }

        private bool SpawnFloorObstacle(Texture2D tex, int offsetHeight = 0)
        {
            Vector2 position = _FloorSpawner.Position;
            position.Y -= offsetHeight;

            Obstacle floorObstacle = new Obstacle(tex, position, isDebug, levelSpeed, levelDirection);
            AddGameObject(floorObstacle);
            floorObstacleList.Add(floorObstacle);
            return true;
        }

        private bool SpawnCeilingObstacle(Texture2D tex,Rectangle box2D, int offsetHeight = 0)
        {
            box2D.Y -= offsetHeight;
            Vector2 position = _CeilingSpawner.Position;
            position.Y -= offsetHeight;

            BoundingBox2D box = new BoundingBox2D(position, box2D.Width, box2D.Height);
            Obstacle ceilingObstacel = new Obstacle(tex, position, isDebug, levelSpeed, levelDirection, box);
            AddGameObject(ceilingObstacel);
            floorObstacleList.Add(ceilingObstacel);
            return true;
        }

        public override void HandleInput(GameTime gameTime, Point MousePosition)
        {
            _inputManager.GetCommands(cmd =>
            {
                if (cmd is JetSquidGameInputCommand.PlayerExit)
                {
                    NotifyEvent(new BaseGameStateEvent.GameQuit());
                }

                if(cmd is JetSquidGameInputCommand.PlayerJump)
                {
                    NotifyEvent(new JetSquidGameplayEvents.PlayerJump());
                }
                else if(cmd is JetSquidGameInputCommand.PlayerHover)
                {
                    NotifyEvent(new JetSquidGameplayEvents.PlayerHover());
                }

                if ( cmd is JetSquidGameInputCommand.PlayerFall)
                {
                    NotifyEvent(new JetSquidGameplayEvents.PlayerFall());
                }

                if (cmd is JetSquidGameInputCommand.PlayerPause)
                {
                    NotifyEvent(new BaseGameStateEvent.GamePause());
                }
            });
        }

        protected override void KeepPlayerInBounds()
        {

            int aWidth = _player.SpriteWidth;
            int aHeight = _player.SpriteHeight;

            if (_player.Position.X < 0)
            {
                _player.Position = new Vector2(0, _player.Position.Y);
            }

            if (_player.Position.X > _viewportWidth - aWidth)
            {
                _player.Position = new Vector2(_viewportWidth - aWidth , _player.Position.Y);
            }

            if (_player.Position.Y < 0)
            {
                _player.Position = new Vector2(_player.Position.X, 0);
            }

            if (_player.Position.Y > _viewportHeight - (aHeight + aHeight / 2))
            {
                _player.Position = new Vector2(_player.Position.X, _viewportHeight - (aHeight + aHeight/ 2));
                if(_player._animationState != ePlayerAnimState.WALKING)
                {
                    NotifyEvent(new JetSquidGameplayEvents.PlayerFloorCollide());
                }
            }

            if (isDebug)
            {
                //Trace.WriteLine("Player Sprite Width = " + aWidth);
                //Trace.WriteLine("Player Sprite Height = " + aHeight);
                //Trace.WriteLine("Player Position X = " + _player.Position.X);
                //Trace.WriteLine("Player Position Y = " + _player.Position.Y);
            }
        }

        protected override void SetInputManager()
        {
            _inputManager = new InputManager(new JetSquidGameInputMapper());
        }

        protected override void DetectCollisions()
        {
            JetSquidGameplayEvents hitEvent;
            var floorObstacleCollisionDetector = new AABBCollisionDetector<Obstacle, PlayerSquid>(floorObstacleList);

            floorObstacleCollisionDetector.DetectCollisions(_player, (Obstacle, PlayerSquid) =>
            {
                if (PlayerSquid.Position.Y > Obstacle.Position.Y)
                {
                    hitEvent = new JetSquidGameplayEvents.PlayerFloorCollide();
                    PlayerSquid.OnNotify(hitEvent);
                }
                else if (PlayerSquid.Position.X > Obstacle.Position.X + Obstacle.Width / 2)
                {
                    hitEvent = new JetSquidGameplayEvents.PlayerFall();
                    if (PlayerSquid._animationState == ePlayerAnimState.WALKING)
                    { PlayerSquid.OnNotify(hitEvent); }
                }
                else
                {
                    hitEvent = new JetSquidGameplayEvents.PlayerTakeDamage();
                    PlayerSquid.OnNotify(hitEvent);
                    PlayerSquid.setPosition(new Vector2(PlayerSquid.Position.X, Obstacle.Position.Y - (Obstacle.Height / 2)));
                }
            });

            var ceilingObstacelCollisionDetector = new AABBCollisionDetector<Obstacle, PlayerSquid>(ceilingObstacleList);

            ceilingObstacelCollisionDetector.DetectCollisions(_player, (Obstacle, PlayerSquid) =>
            {
                if (PlayerSquid.Position.Y > Obstacle.Position.Y)
                {
                    hitEvent = new JetSquidGameplayEvents.PlayerTakeDamage();
                    PlayerSquid.OnNotify(hitEvent);
                    PlayerSquid.setPosition(new Vector2(PlayerSquid.Position.X,Obstacle.Position.Y + (Obstacle.Height / 2)));

                }
            });

            var fanObstacleCollisionDetor = new AABBCollisionDetector<AnimatedObstacle, PlayerSquid>(fanObstacleList);

            fanObstacleCollisionDetor.DetectCollisions(_player, (Obstacle, PlayerSquid) =>
            {
                hitEvent = new JetSquidGameplayEvents.PlayerTakeDamage();
                PlayerSquid.OnNotify(hitEvent);
                PlayerSquid.setPosition(new Vector2(PlayerSquid.Position.X, Obstacle.BoundingBoxes[0].Rectangle.Y + 100));
            });

            hitEvent = new JetSquidGameplayEvents.DamageObstacle();
            foreach (var fanObstacle in fanObstacleList)
            {   
                if(_inkEmitter.CheckForParticleCollisions(fanObstacle.BoundingBoxes[0]))
                {
                    fanObstacle.OnNotify(hitEvent);
                }
            }

            foreach (var ceilingObstacle in ceilingObstacleList)
            {
                if (_inkEmitter.CheckForParticleCollisions(ceilingObstacle.BoundingBoxes[0]))
                {
                    ceilingObstacle.OnNotify(hitEvent);
                }
            }

            foreach(var floorObstacle in floorObstacleList)
            {
                if (_inkEmitter.CheckForParticleCollisions(floorObstacle.BoundingBoxes[0]))
                {
                    floorObstacle.OnNotify(hitEvent);
                    
                }
            }
        }

    }

}

