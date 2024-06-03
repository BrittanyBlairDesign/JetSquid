
using Engine.Components.Collision;
using Engine.Components.Physics;
using Engine.Input;
using Engine.Objects;
using Engine.Objects.UI;
using Engine.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Serialization;
using SpriteSheetAnimationContentPipeline;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
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
        private int fishBucketPoints = 10;

        private string crate = "JetSquid/Objects/Crate";
        private Texture2D crateTex;
        private float crateScale = .5f;
        private int cratePoints = 50;

        private string ceilingLight = "JetSquid/Objects/HangingLight";
        private Texture2D ceilingLightTex;
        private float ceilingLightScale = .75f;
        private int ceilingLightPoints = 75;

        private string airDucts = "JetSquid/Objects/AirDucts";
        private Texture2D airDuctsTex;
        private float airDuctScale = 1.0f;
        private int airDuctPoints = 200;

        private string ceilingFan = "JetSquid/Animation/Objects/FanSpriteAnim";
        private SpriteSheetAnimation ceilingFanTex;
        private float ceilingFanScale = 1.0f;
        private int ceilingFanPoints = 125;

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
        private int inkPoints = 1;

        private float collectableTimer;
        private float collectableDurration = 1.0f;
        private List<Obstacle> collectables = new List<Obstacle>();

        private BoundingBox2D _collectableSpawner;

        // Score
        public double HighScore;
        public double Score;
   
        // Level Speed
        private float levelSpeed = 3.0f;
        private Direction levelDirection = Direction.RIGHT;

        // Player
        private string playerTexture = "JetSquid/Animation/Player/SquidAnimatedSpriteSheet";
        private PlayerSquid _player;
        private float playerScale = .5f;

        private string inkParticleTexture = "JetSquid/Particles/InkParticle";
        private InkJetEmitter _inkEmitter;

        public string scoreTexture = "JetSquid/UI/ScoreFrame";
        private string scoreFont = "JetSquid/UI/Font"; 
        public TextObject gameScore;

        public override void LoadContent(GraphicsDevice graphics = null)
        {
            if (graphics != null)
            {
                isDebug = true;
                this.graphics = graphics;
            }

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

            inkTex = LoadTexture(inkParticleTexture);
            _inkEmitter = new InkJetEmitter(inkTex, playerStartPos);
            AddGameObject(_inkEmitter);

            _player = new PlayerSquid(animSheet, playerStartPos, isDebug, playerScale, _inkEmitter);
            _playerSprite = _player;
            AddGameObject(_player);

            // score
            gameScore = new TextObject(LoadTexture(scoreTexture), "Score " ,_contentManager.Load<SpriteFont>(scoreFont));
            AddGameObject(gameScore);

            // Obstacle Spawners
            _CeilingSpawner = new BoundingBox2D(new Vector2(_viewportWidth + 500, 0), 500, 700);
            _FloorSpawner = new BoundingBox2D(new Vector2(_viewportWidth + 500, _viewportHeight), 250, 250);
            _collectableSpawner = new BoundingBox2D(new Vector2(_viewportWidth + 500, _viewportHeight), 250, 250);
           
            // Obstacle Textures
            fishBucketTex = LoadTexture(fishBucket);
            crateTex = LoadTexture(crate);
            ceilingLightTex = LoadTexture(ceilingLight);
            airDuctsTex = LoadTexture(airDucts);
            ceilingFanTex = LoadAnimation(ceilingFan);

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
                    FO.OnNotify(new JetSquidGameplayEvents.DestroyObstacle());
                }
            }

            foreach(Obstacle CO in ceilingObstacleList)
            {
                CO.Update(gameTime);
                if (CO.Position.X < 0 - CO.Width)
                {
                    CO.OnNotify(new JetSquidGameplayEvents.DestroyObstacle());
                }
            }
            
            foreach(AnimatedObstacle Fan in fanObstacleList)
            {
                Fan.Update(gameTime);
                if (Fan.Position.X < 0 - Fan.SpriteWidth)
                {
                    Fan.OnNotify(new JetSquidGameplayEvents.DestroyObstacle());
                }
            }

            foreach(Obstacle Item in collectables)
            {
                Item.Update(gameTime);
                if(Item.Position.X < 0 - Item.Width)
                {
                    Item.OnNotify(new JetSquidGameplayEvents.DestroyObstacle());
                }
            }

            floorObstacleList = CleanObjects(floorObstacleList);
            ceilingObstacleList = CleanObjects(ceilingObstacleList);
            fanObstacleList = CleanObjects(fanObstacleList);
            collectables = CleanObjects(collectables);
            
            if(SpawnTimer > 0.0f)
            {
                SpawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                
            }
            else
            {
                if (obstacleCount < maxObstacles)
                {
                    floorHeight = 50;
                    floorHeight = AttemptSpawn(); 
                }
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
            Vector2 position = new Vector2(_collectableSpawner.Position.X, _collectableSpawner.Position.Y - (floorHeight + (inkTex.Height * inkScale)));

            Obstacle collectable = new Obstacle(inkTex, position, isDebug, levelSpeed, levelDirection, null, inkScale);
            collectables.Add(collectable);
            AddGameObject(collectable);

            collectableTimer = collectableDurration;
             
        }

        private int AttemptSpawn()
        {
            bool spawnComplete = false;
            int FloorHeight = 50;

            RandomNumberGenerator rand = new RandomNumberGenerator();
            int floor_Or_Ceiling = rand.NextRandom(1, 3);
            
            if(floor_Or_Ceiling == 1 )
            {
                int floorSpawns = rand.NextRandom(3);
                float scale;
                for (int i = 0; i < floorSpawns; i++)
                {
                    switch (rand.NextRandom(2))
                    {
                        case 0:
                            scale = rand.NextRandom(.5f, 1.0f);
                            FloorHeight += (int)(fishBucketTex.Height * scale);
                            spawnComplete = SpawnFloorObstacle(fishBucketTex, FloorHeight, scale);
                            FloorHeight -= (int)((fishBucketTex.Height * scale) /3);
                            break;
                        case 1:
                            scale = rand.NextRandom(.5f, 1.0f);
                            FloorHeight += (int)(crateTex.Height * scale);
                            spawnComplete = SpawnFloorObstacle(crateTex, FloorHeight, scale);
                            FloorHeight -= (int)(crateTex.Height * scale)/4;
                            break;
                        default:
                            break;
                    }
                }
            }
            else if(floor_Or_Ceiling == 2 )
            {
                Vector2 collider;
                float scale;
                int ceilingOffest = rand.NextRandom(300);
                switch (rand.NextRandom(3))
                {
                    case 0:
                        collider = new Vector2(ceilingLightTex.Width, 150);
                        scale = rand.NextRandom(.5f, ceilingLightScale);
                        spawnComplete = SpawnCeilingObstacle(ceilingLightTex, collider, ceilingOffest, scale);
                        break;
                    case 1:
                        collider = new Vector2(airDuctsTex.Width, airDuctsTex.Height);
                        scale = rand.NextRandom(.5f, airDuctScale);
                        spawnComplete = SpawnCeilingObstacle(airDuctsTex, collider, ceilingOffest);
                        break;
                    case 2:
                        collider = new Vector2(ceilingFanTex.Width / 2, 100);
                        scale = rand.NextRandom(.5f, ceilingFanScale);
                        Vector2 position = _CeilingSpawner.Position;
                        position.Y -= ceilingOffest;

                        Vector2 colliderPos = position;
                        colliderPos.Y = (ceilingFanTex.Height * ceilingFanScale) - (collider.Y);

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

        private bool SpawnFloorObstacle(Texture2D tex, int offsetHeight = 0, float scale = 1.0f)
        {
            Vector2 position = _FloorSpawner.Position;
            position.Y -= offsetHeight;

            BoundingBox2D box = new BoundingBox2D(position, tex.Width , tex.Height);

            Obstacle floorObstacle = new Obstacle(tex, position, isDebug, levelSpeed, levelDirection, box, scale: scale);
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
                
                Rectangle playerCollider = PlayerSquid.BoundingBoxes[0].Rectangle;
                playerCollider.X += PlayerSquid.boxOffsetX;
                playerCollider.Y += PlayerSquid.boxOffsetY;

                Rectangle obstacleCollider = Obstacle.BoundingBoxes[0].Rectangle;
                obstacleCollider.Width = (int)(obstacleCollider.Width * Obstacle.GetScale());
                obstacleCollider.Height = (int)(obstacleCollider.Height * Obstacle.GetScale());
                
                PlayerSquid.SetCollidingObstacle(Obstacle);

                if (isDebug)
                {   
                    Trace.WriteLine("Player Collision End Position   : " + (playerCollider.X + playerCollider.Width) + ", " + (playerCollider.Y + playerCollider.Height));
                    Trace.WriteLine("Obstacle Collision Start Position : " + obstacleCollider.X + ", " + obstacleCollider.Y);
                }

                if (playerCollider.Y + (playerCollider.Height) <= obstacleCollider.Y) // If squid is taller than the object its colliding with.
                {
                    // if squid is on top of the object.
                    if (playerCollider.X + playerCollider.Width >= obstacleCollider.X ||
                        playerCollider.X < (obstacleCollider.X + obstacleCollider.Width))
                    {
                        // if squid is not already walking or is not hovering then we can let the squid walk on the box.
                        if (PlayerSquid._animationState != ePlayerAnimState.HOVERING && PlayerSquid._animationState != ePlayerAnimState.WALKING)
                        {
                            hitEvent = new JetSquidGameplayEvents.PlayerObstacleCollide();
                            PlayerSquid.OnNotify(hitEvent);

                            if (isDebug)
                            {
                                Trace.WriteLine(" Player is walking on a Floor Obstacle. ");
                            }
                        }
                    }
                }
                else if (playerCollider.X < (obstacleCollider.X + obstacleCollider.Width) ) // if squid hit the left side of the object.
                {
                    if((playerCollider.Y + playerCollider.Height) > obstacleCollider.Y)
                    {
                        hitEvent = new JetSquidGameplayEvents.PlayerTakeDamage();
                        PlayerSquid.OnNotify(hitEvent);

                        PlayerSquid.Position = new Vector2(PlayerSquid.Position.X, Obstacle.Position.Y - (PlayerSquid.SpriteHeight));
                        hitEvent = new JetSquidGameplayEvents.PlayerObstacleCollide();
                        PlayerSquid.OnNotify(hitEvent);

                        if (isDebug)
                        {
                            Trace.WriteLine(" Player Hit the side of an Obstacle.");
                        }
                    }

                }
            });
            
            var collectableCollisiondetector = new AABBCollisionDetector<Obstacle, PlayerSquid>(collectables);

            collectableCollisiondetector.DetectCollisions(_player, (Obstacle, PlayerSquid) =>
            {
                hitEvent = new JetSquidGameplayEvents.PlayerEarnPoints();
                _player.OnNotify(hitEvent);
                NotifyEvent(hitEvent);

                hitEvent = new JetSquidGameplayEvents.DestroyObstacle();
                Obstacle.OnNotify(hitEvent);

                AddScorePoints(inkPoints);
            });

            var ceilingObstacelCollisionDetector = new AABBCollisionDetector<Obstacle, PlayerSquid>(ceilingObstacleList);

            ceilingObstacelCollisionDetector.DetectCollisions(_player, (Obstacle, PlayerSquid) =>
            {
                if (PlayerSquid.Position.Y > Obstacle.Position.Y)
                {
                    hitEvent = new JetSquidGameplayEvents.PlayerTakeDamage();
                    PlayerSquid.OnNotify(hitEvent);
                    PlayerSquid.Position = new Vector2(PlayerSquid.Position.X , Obstacle.Position.Y + (Obstacle.Height * Obstacle.GetScale()));

                }
            });

            var fanObstacleCollisionDetor = new AABBCollisionDetector<AnimatedObstacle, PlayerSquid>(fanObstacleList);

            fanObstacleCollisionDetor.DetectCollisions(_player, (Obstacle, PlayerSquid) =>
            {
                hitEvent = new JetSquidGameplayEvents.PlayerTakeDamage();
                PlayerSquid.OnNotify(hitEvent);
                PlayerSquid.Position = new Vector2(PlayerSquid.Position.X, Obstacle.BoundingBoxes[0].Rectangle.Y + 100);
            });

            hitEvent = new JetSquidGameplayEvents.DamageObstacle();
            foreach (var fanObstacle in fanObstacleList)
            {   
                if(_inkEmitter.CheckForParticleCollisions(fanObstacle.BoundingBoxes[0]))
                {
                    fanObstacle.OnNotify(hitEvent);
                    if(fanObstacle.Destroyed)
                    {
                        AddScorePoints(ceilingFanPoints);
                    }
                }
            }

            foreach (var ceilingObstacle in ceilingObstacleList)
            {
                if (_inkEmitter.CheckForParticleCollisions(ceilingObstacle.BoundingBoxes[0]))
                {
                    ceilingObstacle.OnNotify(hitEvent);

                    AddScorePoints(airDuctPoints);
                }
            }

            foreach(var floorObstacle in floorObstacleList)
            {
                if (_inkEmitter.CheckForParticleCollisions(floorObstacle.BoundingBoxes[0]))
                {
                    floorObstacle.OnNotify(hitEvent);
                    AddScorePoints(cratePoints);
                }
            }
        }

        public void AddScorePoints(int amount)
        {
            Score += amount;
            gameScore.setText("Score " + Score.ToString());
        }
    }

    
}

