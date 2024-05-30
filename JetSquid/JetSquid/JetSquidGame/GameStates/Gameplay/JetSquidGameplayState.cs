
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
using static System.Net.Mime.MediaTypeNames;

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
        private float fishBucketScale = 1.0f;
        private float fishBucketPoints = 10;

        private string crate = "JetSquid/Objects/Crate";
        private Texture2D crateTex;
        private float crateScale = .5f;
        private float cratePoints = 50;

        private string ceilingLight = "JetSquid/Objects/HangingLight";
        private Texture2D ceilingLightTex;
        private float ceilingLightScale = .75f;
        private float ceilingLightPoints = 75;

        private string airDucts = "JetSquid/Objects/AirDucts";
        private Texture2D airDuctsTex;
        private float airDuctScale = 1.0f;
        private float airDuctPoints = 200;

        private string ceilingFan = "JetSquid/Animation/Objects/FanSpriteAnim";
        private SpriteSheetAnimation ceilingFanTex;
        private float ceilingFanScale = 1.0f;
        private float ceilingFanPoints = 125;

        private List<Obstacle> floorObstacleList = new List<Obstacle>();
        private List<Obstacle> ceilingObstacleList = new List<Obstacle>();
        private List<AnimatedObstacle> fanObstacleList = new List<AnimatedObstacle>();

        private BoundingBox2D _CeilingSpawner;
        private BoundingBox2D _FloorSpawner;

        private float SpawnTimer;
        private float SpawnCoolDown = 5.0f;
        private int obstacleCount { get { return floorObstacleList.Count + ceilingObstacleList.Count + fanObstacleList.Count; } }
        private int maxObstacles = 20;


        // Collectables
        int floorHeight = 50;

        private string inkCollectable = "JetSquid/Particle/InkParticle";
        private Texture2D inkTex;
        float inkScale = 1.0f;
        private float inkPoints = 1;

        private float collectableTimer;
        private float collectableDurration = 1.5f;
        private List<Obstacle> collectables = new List<Obstacle>();

        private BoundingBox2D _collectableSpawner;

        // Score
        public int HighScore;
        public int Score;
   
        // Level Speed
        private float levelSpeed = 3.0f;
        private Direction levelDirection = Direction.RIGHT;

        // Player
        private string playerTexture = "JetSquid/Animation/Player/SquidAnimatedSpriteSheet";
        private PlayerSquid _player;
        private float playerScale = .5f;

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
            playerStartPos.Y = _viewportHeight - ((spriteHeight * playerScale) / 2);

            _inkEmitter = new InkJetEmitter(LoadTexture(inkParticleTexture), playerStartPos);
            AddGameObject(_inkEmitter);

            _player = new PlayerSquid(animSheet, playerStartPos, isDebug, playerScale, _inkEmitter);
            _playerSprite = _player;
            AddGameObject(_player);


            // Obstacle Spawners
            _CeilingSpawner = new BoundingBox2D(new Vector2(_viewportWidth + 500, 0), 500, 700);
            _FloorSpawner = new BoundingBox2D(new Vector2(_viewportWidth + 500, _viewportHeight), 250, 250);

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
                SpawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                if (obstacleCount < maxObstacles)
                { floorHeight = AttemptSpawn(); }
            }
            
            if(collectableTimer > 0.0f)
            {
                collectableTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                SpawnCollectable(floorHeight);
            }

            base.UpdateGameState(gameTime);
        }

        private void SpawnCollectable(int floorHeight)
        {
            _collectableSpawner.Position = FindSpawnPosition(_collectableSpawner);

            Vector2 position = _collectableSpawner.Position;
            Obstacle collectable = new Obstacle(inkTex, position, isDebug, levelSpeed, levelDirection, null, inkScale);
             
        }

        private Vector2 FindSpawnPosition(BoundingBox2D spawner)
        {
            bool searchingForSpawnPoint = true;
            BoundingBox2D box = spawner;
            box.Position = new Vector2(spawner.Position.X, _viewportHeight - 50);

            Vector2 newSpawnLocation = Vector2.Zero;

            while (searchingForSpawnPoint)
            {
                newSpawnLocation = DetectSpawnCollisions(box);

                if(newSpawnLocation == box.Position)
                {
                    searchingForSpawnPoint = false;
                }
                else
                {
                    box.Position = newSpawnLocation;
                }
            }

            return newSpawnLocation;
        }

        private int AttemptSpawn()
        {
            bool spawnComplete = false;
            int FloorHeight = 50;

            RandomNumberGenerator rand = new RandomNumberGenerator();
            int floor_Or_Ceiling = rand.NextRandom(1, 3);
            
            if(floor_Or_Ceiling == 1 && DetectSpawnCollisions(_FloorSpawner) == _FloorSpawner.Position)
            {
                int floorSpawns = rand.NextRandom(3);
                float scale;
                for (int i = 0; i < floorSpawns; i++)
                {
                    switch (rand.NextRandom(2))
                    {
                        case 0:
                            scale = rand.NextRandom(.5f, fishBucketScale);
                            FloorHeight += (int)(fishBucketTex.Height * scale);
                            spawnComplete = SpawnFloorObstacle(fishBucketTex, FloorHeight);
                            FloorHeight -= (int)((fishBucketTex.Height * scale) /3);
                            break;
                        case 1:
                            scale = rand.NextRandom(.5f, crateScale);
                            FloorHeight += (int)(crateTex.Height * scale);
                            spawnComplete = SpawnFloorObstacle(crateTex, FloorHeight);
                            FloorHeight -= (int)(crateTex.Height * scale)/4;
                            break;
                        default:
                            break;
                    }
                }
            }
            else if(floor_Or_Ceiling == 2 && DetectSpawnCollisions(_CeilingSpawner) == _CeilingSpawner.Position)
            {
                Vector2 collider;
                float scale;
                int ceilingOffest = rand.NextRandom(300);
                switch (rand.NextRandom(3))
                {
                    case 0:
                        collider = new Vector2(ceilingLightTex.Width, 200);
                        scale = rand.NextRandom(.5f, ceilingLightScale);
                        spawnComplete = SpawnCeilingObstacle(ceilingLightTex, collider, ceilingOffest, scale);
                        break;
                    case 1:
                        collider = new Vector2(airDuctsTex.Width, airDuctsTex.Height);
                        scale = rand.NextRandom(.5f, airDuctScale);
                        spawnComplete = SpawnCeilingObstacle(airDuctsTex, collider, ceilingOffest);
                        break;
                    case 2:
                        collider = new Vector2(ceilingFanTex.Width / 2, 150);
                        scale = rand.NextRandom(.5f, ceilingFanScale);
                        Vector2 position = _CeilingSpawner.Position;
                        position.Y -= ceilingOffest;

                        Vector2 colliderPos = position;
                        colliderPos.Y = (ceilingFanTex.Height * ceilingFanScale) - (collider.Y * ceilingFanScale);

                        BoundingBox2D box = new BoundingBox2D(colliderPos, collider.X, collider.Y);

                        AnimatedObstacle FanObstacle = new AnimatedObstacle(ceilingFanTex, position, isDebug,ceilingFanScale, levelSpeed, levelDirection, box);
                        AddGameObject(FanObstacle);
                        fanObstacleList.Add(FanObstacle);
                        break;
                    default:
                        break;
                }
            }

            if (spawnComplete)
            {
                SpawnTimer = rand.NextRandom(1.0f, SpawnCoolDown);
            }
            return FloorHeight;
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

        private bool SpawnCeilingObstacle(Texture2D tex,Vector2 ColliderSize, int offsetHeight = 0, float scale = 1.0f)
        {
            
            Vector2 position = _CeilingSpawner.Position;
            position.Y -= offsetHeight;

            Vector2 colliderPos = position;

            if(ColliderSize.Y != tex.Height)
            {
                colliderPos.Y = (tex.Height * scale) - (ColliderSize.Y * scale);
            }
        

            BoundingBox2D box = new BoundingBox2D(colliderPos, ColliderSize.X, ColliderSize.Y);
            Obstacle ceilingObstacel = new Obstacle(tex, position, isDebug, levelSpeed, levelDirection, box, scale);
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

            var collectableCollisiondetector = new AABBCollisionDetector<Obstacle, PlayerSquid>(collectables);

            collectableCollisiondetector.DetectCollisions(_player, (Obstacle, PlayerSquid) =>
            {
                hitEvent = new JetSquidGameplayEvents.PlayerEarnPoints();
                collectables.Remove(Obstacle);
                _player.OnNotify(hitEvent);
                NotifyEvent(hitEvent);
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

        protected Vector2 DetectSpawnCollisions(BoundingBox2D spawner)
        {
            Vector2 nextPosition = spawner.Position;

            foreach(Obstacle FO in floorObstacleList)
            {
                if (FO.CollidesWith(spawner))
                {
                    nextPosition.Y = FO.Position.Y - (FO.Height * FO.GetScale());
                }
            }

            foreach(Obstacle CO in ceilingObstacleList)
            {
                if(CO.CollidesWith(spawner))
                {
                    
                    float Y = CO.Position.Y - (CO.Height * CO.GetScale());
                    if(Y <= 0.0f)
                    {
                        Y = CO.Position.Y + (CO.Height * CO.GetScale());
                    }

                    nextPosition.Y = Y;
                }
            }

            foreach (AnimatedObstacle Fan in fanObstacleList)
            {
                if (Fan.CollidesWith(spawner))
                {

                    float Y = Fan.Position.Y - (Fan.SpriteHeight);
                    if (Y <= 0.0f)
                    {
                        Y = Fan.Position.Y + (Fan.SpriteHeight);
                    }

                    nextPosition.Y = Y;
                }
            }

            foreach (BaseGameObject GO in collectables)
            {
                if(GO.CollidesWith(spawner))
                {
                    float Y = GO.Position.Y - (GO.Height * GO.GetScale());
                    if (Y <= 0.0f)
                    {
                        Y = GO.Position.Y + (GO.Height * GO.GetScale());
                    }

                    nextPosition.Y = Y;
                }
            }

            return nextPosition;
        }
    }

}

