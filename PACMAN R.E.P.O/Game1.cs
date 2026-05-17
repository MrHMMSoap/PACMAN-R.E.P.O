using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PACMAN_R.E.P.O.Items;
using PACMAN_R.E.P.O.Map;
using PACMAN_R.E.P.O.Monsters;
using PACMAN_R.E.P.O.Systems;
using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;
using System;
using System.Collections.Generic;
using System.IO;

namespace PACMAN_R.E.P.O
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D pixel;
        private SpriteFont hudFont;

        private TileMap tileMap;
        private MapFileHandler mapFileHandler;

        private const int TileSize = 32;
        private const int ViewRadius = 4;

        private VisibilitySystem visibilitySystem;

        private PlayerEntity playerStats;
        private Inventory inventory;

        private StaminaSystem staminaSystem;
        private ExtractionSystem extractionSystem;
        private RoundManager roundManager;
        private ShopManager shopManager;
        private GameStateManager gameStateManager;

        private bool extractionGoalComplete;

        private KeyboardState previousKeyboardState;

        private Random random;

        private Vector2 playerPosition;
        private Vector2 playerSize = new Vector2(24, 24);
        private float playerSpeed = 150f;

        private float sprintMultiplier = 1.7f;
        private float staminaDrainPerSecond = 40f;
        private float staminaRegenPerSecond = 8f;

        private Vector2 cameraPosition;

        private string mapFilePath;

        private bool[,] discoveredTiles;

        private Wraith wraith;
        private Vector2 wraithPosition;
        private Vector2 wraithSize = new Vector2(22, 22);

        private Point wraithTargetTile;
        private bool wraithHasTargetTile = false;

        private float wraithSlowSpeed = 45f;
        private float wraithRageSpeed = 115f;

        private float wraithDamageCooldown = 0f;
        private float wraithDamageCooldownTime = 1.0f;
        private int wraithDamage = 20;

        private HealthSystem healthSystem;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize()
        {
            mapFileHandler = new MapFileHandler();
            visibilitySystem = new VisibilitySystem(ViewRadius);

            mapFilePath = Path.Combine(AppContext.BaseDirectory, "Maps", "test_map.txt");

            tileMap = mapFileHandler.LoadMap(mapFilePath);
            discoveredTiles = new bool[tileMap.Width, tileMap.Height];

            playerStats = new PlayerEntity();
            inventory = new Inventory(playerStats);
            staminaSystem = new StaminaSystem();
            healthSystem = new HealthSystem();

            roundManager = new RoundManager();
            extractionSystem = new ExtractionSystem();
            extractionSystem.RequiredValue = roundManager.GetExtractionRequirement();

            shopManager = new ShopManager();

            gameStateManager = new GameStateManager();
            gameStateManager.StartGame();

            extractionGoalComplete = false;

            random = new Random();
            previousKeyboardState = Keyboard.GetState();

            playerPosition = FindSpawnPosition();
            wraith = new Wraith();
            wraithPosition = FindWraithSpawnPosition();
            wraithHasTargetTile = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            hudFont = Content.Load<SpriteFont>("HudFont");
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (gameStateManager.CurrentState == GameState.Playing)
            {
                UpdatePlaying(gameTime, keyboard);
            }
            else if (gameStateManager.CurrentState == GameState.Shop)
            {
                UpdateShop(keyboard);
            }

            previousKeyboardState = keyboard;

            base.Update(gameTime);
        }

        private void UpdatePlaying(GameTime gameTime, KeyboardState keyboard)
        {
            UpdatePlayerMovement(gameTime, keyboard);
            HandleInteraction(keyboard);

            UpdateWraith(gameTime);

            UpdateDiscoveredTiles();
            UpdateCamera();
        }

        private void UpdateWraith(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (wraithDamageCooldown > 0f)
            {
                wraithDamageCooldown -= deltaTime;

                if (wraithDamageCooldown < 0f)
                {
                    wraithDamageCooldown = 0f;
                }
            }

            int playerTileX = (int)((playerPosition.X + playerSize.X / 2f) / TileSize);
            int playerTileY = (int)((playerPosition.Y + playerSize.Y / 2f) / TileSize);

            int wraithTileX = (int)((wraithPosition.X + wraithSize.X / 2f) / TileSize);
            int wraithTileY = (int)((wraithPosition.Y + wraithSize.Y / 2f) / TileSize);

            bool wraithVisibleToPlayer = visibilitySystem.IsTileVisible(
                wraithTileX,
                wraithTileY,
                playerTileX,
                playerTileY
            );

            if (wraithVisibleToPlayer && wraith.State != WraithState.Rage)
            {
                wraith.OnSeenByPlayer();
            }

            wraith.UpdateRageTimer(deltaTime);

            float speed = wraith.State == WraithState.Rage
                ? wraithRageSpeed
                : wraithSlowSpeed;

            MoveWraithTowardsPlayer(deltaTime, speed);

            CheckWraithPlayerCollision();
        }

        private void MoveWraithTowardsPlayer(float deltaTime, float speed)
        {
            Point wraithTile = new Point(
                (int)((wraithPosition.X + wraithSize.X / 2f) / TileSize),
                (int)((wraithPosition.Y + wraithSize.Y / 2f) / TileSize)
            );

            Point playerTile = new Point(
                (int)((playerPosition.X + playerSize.X / 2f) / TileSize),
                (int)((playerPosition.Y + playerSize.Y / 2f) / TileSize)
            );

            if (wraithTile == playerTile)
            {
                return;
            }

            if (!wraithHasTargetTile)
            {
                List<Point> path = FindPath(wraithTile, playerTile);

                if (path.Count == 0)
                {
                    return;
                }

                wraithTargetTile = path[0];
                wraithHasTargetTile = true;
            }

            Vector2 targetPosition = GetTileCenter(
                wraithTargetTile.X,
                wraithTargetTile.Y,
                wraithSize
            );

            Vector2 direction = targetPosition - wraithPosition;
            float distanceToTarget = direction.Length();

            if (distanceToTarget <= 1f)
            {
                wraithPosition = targetPosition;
                wraithHasTargetTile = false;
                return;
            }

            float moveDistance = speed * deltaTime;

            if (moveDistance >= distanceToTarget)
            {
                wraithPosition = targetPosition;
                wraithHasTargetTile = false;
                return;
            }

            direction.Normalize();

            wraithPosition += direction * moveDistance;
        }

        private bool CanMonsterMoveTo(Vector2 newPosition, Vector2 monsterSize)
        {
            Rectangle monsterRectangle = new Rectangle(
                (int)newPosition.X,
                (int)newPosition.Y,
                (int)monsterSize.X,
                (int)monsterSize.Y
            );

            Point topLeft = new Point(monsterRectangle.Left, monsterRectangle.Top);
            Point topRight = new Point(monsterRectangle.Right - 1, monsterRectangle.Top);
            Point bottomLeft = new Point(monsterRectangle.Left, monsterRectangle.Bottom - 1);
            Point bottomRight = new Point(monsterRectangle.Right - 1, monsterRectangle.Bottom - 1);

            return IsPixelPositionWalkable(topLeft) &&
                   IsPixelPositionWalkable(topRight) &&
                   IsPixelPositionWalkable(bottomLeft) &&
                   IsPixelPositionWalkable(bottomRight);
        }

        private void CheckWraithPlayerCollision()
        {
            Rectangle playerRectangle = new Rectangle(
                (int)playerPosition.X,
                (int)playerPosition.Y,
                (int)playerSize.X,
                (int)playerSize.Y
            );

            Rectangle wraithRectangle = new Rectangle(
                (int)wraithPosition.X,
                (int)wraithPosition.Y,
                (int)wraithSize.X,
                (int)wraithSize.Y
            );

            if (!playerRectangle.Intersects(wraithRectangle))
            {
                return;
            }

            if (wraithDamageCooldown > 0f)
            {
                return;
            }

            healthSystem.TakeDamage(playerStats, wraithDamage);
            wraithDamageCooldown = wraithDamageCooldownTime;

            Window.Title = "Wraith hit you! HP: " + playerStats.Health + "/" + playerStats.MaxHealth;

            if (healthSystem.IsDead(playerStats))
            {
                gameStateManager.ChangeState(GameState.GameOver);
                Window.Title = "GAME OVER - Wraith killed you.";
            }
        }

        private Vector2 GetTileCenter(int tileX, int tileY, Vector2 entitySize)
        {
            float x = tileX * TileSize + (TileSize - entitySize.X) / 2f;
            float y = tileY * TileSize + (TileSize - entitySize.Y) / 2f;

            return new Vector2(x, y);
        }

        private List<Point> FindPath(Point start, Point goal)
        {
            Queue<Point> frontier = new Queue<Point>();
            Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();

            frontier.Enqueue(start);
            cameFrom[start] = start;

            Point[] directions =
            {
        new Point(1, 0),
        new Point(-1, 0),
        new Point(0, 1),
        new Point(0, -1)
    };

            while (frontier.Count > 0)
            {
                Point current = frontier.Dequeue();

                if (current == goal)
                {
                    break;
                }

                foreach (Point direction in directions)
                {
                    Point next = new Point(
                        current.X + direction.X,
                        current.Y + direction.Y
                    );

                    if (next.X < 0 || next.X >= tileMap.Width ||
                        next.Y < 0 || next.Y >= tileMap.Height)
                    {
                        continue;
                    }

                    if (!tileMap.Tiles[next.X, next.Y].IsWalkable)
                    {
                        continue;
                    }

                    if (cameFrom.ContainsKey(next))
                    {
                        continue;
                    }

                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }

            if (!cameFrom.ContainsKey(goal))
            {
                return new List<Point>();
            }

            List<Point> path = new List<Point>();

            Point pathCurrent = goal;

            while (pathCurrent != start)
            {
                path.Add(pathCurrent);
                pathCurrent = cameFrom[pathCurrent];
            }

            path.Reverse();

            return path;
        }

        private bool WasKeyPressed(KeyboardState keyboard, Keys key)
        {
            return keyboard.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
        }


        private bool WasAnyKeyPressed(KeyboardState keyboard, Keys keyOne, Keys keyTwo)
        {
            return WasKeyPressed(keyboard, keyOne) || WasKeyPressed(keyboard, keyTwo);
        }



        private void UpdatePlayerMovement(GameTime gameTime, KeyboardState keyboard)
        {
            Vector2 movement = Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.W))
            {
                movement.Y -= 1;
            }

            if (keyboard.IsKeyDown(Keys.S))
            {
                movement.Y += 1;
            }

            if (keyboard.IsKeyDown(Keys.A))
            {
                movement.X -= 1;
            }

            if (keyboard.IsKeyDown(Keys.D))
            {
                movement.X += 1;
            }

            if (movement != Vector2.Zero)
            {
                movement.Normalize();
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float currentSpeed = playerStats.CalculateSpeed();

            bool wantsToSprint = keyboard.IsKeyDown(Keys.LeftShift);
            bool isMoving = movement != Vector2.Zero;
            bool canSprint = staminaSystem.CanSprint(playerStats);

            if (wantsToSprint && isMoving && canSprint)
            {
                currentSpeed *= sprintMultiplier;

                float staminaToUse = staminaDrainPerSecond * deltaTime;
                staminaSystem.UseStamina(playerStats, staminaToUse);
            }
            else
            {
                float staminaToRegenerate = staminaRegenPerSecond * deltaTime;
                staminaSystem.RegenerateStamina(playerStats, staminaToRegenerate);
            }

            playerSpeed = currentSpeed;

            Vector2 newPosition = playerPosition + movement * playerSpeed * deltaTime;

            if (CanMoveTo(newPosition))
            {
                playerPosition = newPosition;
            }
        }

        private bool CanMoveTo(Vector2 newPosition)
        {
            Rectangle playerRectangle = new Rectangle(
                (int)newPosition.X,
                (int)newPosition.Y,
                (int)playerSize.X,
                (int)playerSize.Y
            );

            Point topLeft = new Point(playerRectangle.Left, playerRectangle.Top);
            Point topRight = new Point(playerRectangle.Right - 1, playerRectangle.Top);
            Point bottomLeft = new Point(playerRectangle.Left, playerRectangle.Bottom - 1);
            Point bottomRight = new Point(playerRectangle.Right - 1, playerRectangle.Bottom - 1);

            return IsPixelPositionWalkable(topLeft) &&
                   IsPixelPositionWalkable(topRight) &&
                   IsPixelPositionWalkable(bottomLeft) &&
                   IsPixelPositionWalkable(bottomRight);
        }

        private bool IsPixelPositionWalkable(Point pixelPosition)
        {
            int tileX = pixelPosition.X / TileSize;
            int tileY = pixelPosition.Y / TileSize;

            if (tileX < 0 || tileX >= tileMap.Width || tileY < 0 || tileY >= tileMap.Height)
            {
                return false;
            }

            return tileMap.Tiles[tileX, tileY].IsWalkable;
        }

        private Vector2 FindSpawnPosition()
        {
            for (int x = 0; x < tileMap.Width; x++)
            {
                for (int y = 0; y < tileMap.Height; y++)
                {
                    if (tileMap.Tiles[x, y].Type == TileType.Spawn)
                    {
                        float spawnX = x * TileSize + (TileSize - playerSize.X) / 2f;
                        float spawnY = y * TileSize + (TileSize - playerSize.Y) / 2f;

                        return new Vector2(spawnX, spawnY);
                    }
                }
            }

            return new Vector2(TileSize, TileSize);
        }

        private Vector2 FindWraithSpawnPosition()
        {
            // Första versionen: spawna Wraith långt från spawn, nära nedre högra delen av kartan.
            for (int x = tileMap.Width - 2; x >= 1; x--)
            {
                for (int y = tileMap.Height - 2; y >= 1; y--)
                {
                    if (tileMap.Tiles[x, y].IsWalkable &&
                        tileMap.Tiles[x, y].Type != TileType.Spawn &&
                        tileMap.Tiles[x, y].Type != TileType.Extraction)
                    {
                        float spawnX = x * TileSize + (TileSize - wraithSize.X) / 2f;
                        float spawnY = y * TileSize + (TileSize - wraithSize.Y) / 2f;

                        return new Vector2(spawnX, spawnY);
                    }
                }
            }

            return new Vector2(TileSize * 5, TileSize * 5);
        }

        private void UpdateCamera()
        {
            float screenCenterX = graphics.PreferredBackBufferWidth / 2f;
            float screenCenterY = graphics.PreferredBackBufferHeight / 2f;

            cameraPosition = new Vector2(
                playerPosition.X - screenCenterX,
                playerPosition.Y - screenCenterY
            );
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (gameStateManager.CurrentState == GameState.Playing)
            {
                DrawMap();
                DrawPlayer();
                DrawWraith();
                DrawGameplayHUD();
            }
            else if (gameStateManager.CurrentState == GameState.Shop)
            {
                DrawShopHUD();
            }
            else if (gameStateManager.CurrentState == GameState.GameOver)
            {
                DrawGameOverHUD();
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGameOverHUD()
        {
            string text = "GAME OVER\nThe Wraith caught you.\nPress ESC to quit.";

            Vector2 position = new Vector2(
                graphics.PreferredBackBufferWidth / 2f - 170,
                graphics.PreferredBackBufferHeight / 2f - 50
            );

            spriteBatch.DrawString(hudFont, text, position, Color.Red);
        }

        private void DrawWraith()
        {
            int playerTileX = (int)((playerPosition.X + playerSize.X / 2f) / TileSize);
            int playerTileY = (int)((playerPosition.Y + playerSize.Y / 2f) / TileSize);

            int wraithTileX = (int)((wraithPosition.X + wraithSize.X / 2f) / TileSize);
            int wraithTileY = (int)((wraithPosition.Y + wraithSize.Y / 2f) / TileSize);

            bool isVisible = visibilitySystem.IsTileVisible(
                wraithTileX,
                wraithTileY,
                playerTileX,
                playerTileY
            );

            if (!isVisible)
            {
                return;
            }

            Rectangle wraithRectangle = new Rectangle(
                (int)(wraithPosition.X - cameraPosition.X),
                (int)(wraithPosition.Y - cameraPosition.Y),
                (int)wraithSize.X,
                (int)wraithSize.Y
            );

            Color color = wraith.State == WraithState.Rage
                ? Color.Red
                : Color.Purple;

            spriteBatch.Draw(pixel, wraithRectangle, color);
        }

        private void DrawGameplayHUD()
        {
            int x = 15;
            int y = 15;
            int lineHeight = 22;

            string roundText = "Round: " + roundManager.CurrentRound;
            string moneyText = "Money: " + playerStats.Money;
            string hpText = "HP: " + playerStats.Health + "/" + playerStats.MaxHealth;
            string staminaText = "Stamina: " + ((int)playerStats.Stamina) + "/" + ((int)playerStats.MaxStamina);
            string weightText = "Weight: " + playerStats.CarriedWeight.ToString("0.0") + "/" + playerStats.MaxCarryWeight.ToString("0.0");
            string extractionText = "Extraction: " + extractionSystem.ExtractedValue + "/" + extractionSystem.RequiredValue;

            spriteBatch.DrawString(hudFont, roundText, new Vector2(x, y), Color.White);
            spriteBatch.DrawString(hudFont, moneyText, new Vector2(x, y + lineHeight), Color.White);
            spriteBatch.DrawString(hudFont, hpText, new Vector2(x, y + lineHeight * 2), Color.White);
            spriteBatch.DrawString(hudFont, staminaText, new Vector2(x, y + lineHeight * 3), Color.White);
            spriteBatch.DrawString(hudFont, weightText, new Vector2(x, y + lineHeight * 4), Color.White);
            spriteBatch.DrawString(hudFont, extractionText, new Vector2(x, y + lineHeight * 5), Color.White);

            string controlsText = "WASD: Move | E: Interact | ESC: Quit";
            spriteBatch.DrawString(hudFont, controlsText, new Vector2(x, graphics.PreferredBackBufferHeight - 35), Color.LightGray);

            if (extractionGoalComplete)
            {
                string returnText = "Extraction goal complete! Return to spawn.";
                spriteBatch.DrawString(hudFont, returnText, new Vector2(x, y + lineHeight * 7), Color.Gold);
            }
        }

        private void DrawMap()
        {
            int playerTileX = (int)(playerPosition.X / TileSize);
            int playerTileY = (int)(playerPosition.Y / TileSize);

            for (int x = 0; x < tileMap.Width; x++)
            {
                for (int y = 0; y < tileMap.Height; y++)
                {
                    Rectangle tileRectangle = new Rectangle(
                        x * TileSize - (int)cameraPosition.X,
                        y * TileSize - (int)cameraPosition.Y,
                        TileSize,
                        TileSize
                    );

                    float brightness = GetVisibilityBrightness(
                        tileX: x,
                        tileY: y,
                        playerTileX: playerTileX,
                        playerTileY: playerTileY
                    );

                    Tile tile = tileMap.Tiles[x, y];
                    Color baseColor = GetTileColor(tile.Type);

                    if (brightness > 0f)
                    {
                        Color visibleColor = DarkenColor(baseColor, brightness);
                        spriteBatch.Draw(pixel, tileRectangle, visibleColor);
                        continue;
                    }

                    if (discoveredTiles[x, y])
                    {
                        Color rememberedColor = DarkenColor(baseColor, 0.12f);
                        spriteBatch.Draw(pixel, tileRectangle, rememberedColor);
                        continue;
                    }

                    spriteBatch.Draw(pixel, tileRectangle, Color.Black);
                }
            }
        }

        private void UpdateDiscoveredTiles()
        {
            int playerTileX = (int)(playerPosition.X / TileSize);
            int playerTileY = (int)(playerPosition.Y / TileSize);

            for (int x = playerTileX - ViewRadius; x <= playerTileX + ViewRadius; x++)
            {
                for (int y = playerTileY - ViewRadius; y <= playerTileY + ViewRadius; y++)
                {
                    if (x < 0 || x >= tileMap.Width || y < 0 || y >= tileMap.Height)
                    {
                        continue;
                    }

                    if (visibilitySystem.IsTileVisible(x, y, playerTileX, playerTileY))
                    {
                        discoveredTiles[x, y] = true;
                    }
                }
            }
        }

        private void DrawShopHUD()
        {
            Rectangle background = new Rectangle(
                0,
                0,
                graphics.PreferredBackBufferWidth,
                graphics.PreferredBackBufferHeight
            );

            spriteBatch.Draw(pixel, background, new Color(10, 10, 15));

            int x = 80;
            int y = 70;
            int lineHeight = 34;

            int speedCost = shopManager.GetUpgradeCost(playerStats.SpeedLevel);
            int strengthCost = shopManager.GetUpgradeCost(playerStats.StrengthLevel);
            int staminaCost = shopManager.GetUpgradeCost(playerStats.StaminaLevel);
            int healthCost = shopManager.GetUpgradeCost(playerStats.HealthLevel);
            int healthPackCost = shopManager.GetHealthPackCost();

            spriteBatch.DrawString(hudFont, "ITEM SHOP", new Vector2(x, y), Color.Gold);

            y += lineHeight * 2;

            spriteBatch.DrawString(hudFont, "Money: " + playerStats.Money, new Vector2(x, y), Color.White);
            y += lineHeight;

            spriteBatch.DrawString(hudFont, "HP: " + playerStats.Health + "/" + playerStats.MaxHealth, new Vector2(x, y), Color.White);
            y += lineHeight * 2;

            spriteBatch.DrawString(hudFont, "[1] Speed Upgrade       Cost: " + speedCost + "   Level: " + playerStats.SpeedLevel, new Vector2(x, y), Color.White);
            y += lineHeight;

            spriteBatch.DrawString(hudFont, "[2] Strength Upgrade    Cost: " + strengthCost + "   Level: " + playerStats.StrengthLevel, new Vector2(x, y), Color.White);
            y += lineHeight;

            spriteBatch.DrawString(hudFont, "[3] Stamina Upgrade     Cost: " + staminaCost + "   Level: " + playerStats.StaminaLevel, new Vector2(x, y), Color.White);
            y += lineHeight;

            spriteBatch.DrawString(hudFont, "[4] Max HP Upgrade      Cost: " + healthCost + "   Level: " + playerStats.HealthLevel, new Vector2(x, y), Color.White);
            y += lineHeight;

            spriteBatch.DrawString(hudFont, "[5] Health Pack         Cost: " + healthPackCost, new Vector2(x, y), Color.White);
            y += lineHeight * 2;

            spriteBatch.DrawString(hudFont, "[Enter/Space] Start next round", new Vector2(x, y), Color.LightGreen);
        }

        private float GetVisibilityBrightness(int tileX, int tileY, int playerTileX, int playerTileY)
        {
            int distanceX = Math.Abs(tileX - playerTileX);
            int distanceY = Math.Abs(tileY - playerTileY);

            int distance = Math.Max(distanceX, distanceY);

            if (distance > ViewRadius)
            {
                return 0f;
            }

            switch (distance)
            {
                case 0:
                    return 1.0f;

                case 1:
                    return 0.9f;

                case 2:
                    return 0.75f;

                case 3:
                    return 0.55f;

                case 4:
                    return 0.35f;

                default:
                    return 0f;
            }
        }

        private Color DarkenColor(Color color, float brightness)
        {
            return new Color(
                (int)(color.R * brightness),
                (int)(color.G * brightness),
                (int)(color.B * brightness)
            );
        }

        private void TryPickupItem(Tile currentTile)
        {
            Item item = GenerateItemFromTile();

            bool added = inventory.AddItem(item);

            if (!added)
            {
                Window.Title = "Item too heavy! Current weight: " +
                               playerStats.CarriedWeight + "/" + playerStats.MaxCarryWeight;
                return;
            }

            currentTile.Type = TileType.Road;

            Window.Title = "Picked up " + item.Name +
                           " | Value: " + item.Value +
                           " | Weight: " + item.Weight +
                           " | Carrying: " + playerStats.CarriedWeight + "/" + playerStats.MaxCarryWeight;
        }

        private Item GenerateItemFromTile()
        {
            float weight = random.Next(1, 6); // 1 till 5
            int value = (int)(weight * random.Next(20, 41));

            string name = "Scrap Item";

            if (value >= 160)
            {
                name = "Rare Artifact";
            }
            else if (value >= 100)
            {
                name = "Valuable Object";
            }
            else if (value >= 60)
            {
                name = "Metal Scrap";
            }

            return new Item(name, weight, value);
        }

        private void TryExtractItems()
        {
            if (inventory.Items.Count == 0)
            {
                Window.Title = "No items to extract. Required: " +
                               extractionSystem.ExtractedValue + "/" +
                               extractionSystem.RequiredValue;
                return;
            }

            int extractedThisTime = 0;

            List<Item> itemsToExtract = new List<Item>(inventory.Items);

            foreach (Item item in itemsToExtract)
            {
                extractedThisTime += item.Value;
                playerStats.Money += item.Value;
                extractionSystem.AddExtractedValue(item.Value);
                inventory.RemoveItem(item);
            }

            Window.Title = "Extracted value: +" + extractedThisTime +
                           " | Total: " + extractionSystem.ExtractedValue +
                           "/" + extractionSystem.RequiredValue +
                           " | Money: " + playerStats.Money +
                           " | Weight: " + playerStats.CarriedWeight +
                           "/" + playerStats.MaxCarryWeight;

            if (extractionSystem.IsGoalComplete())
            {
                extractionGoalComplete = true;

                Window.Title = "Extraction goal complete! Return to spawn and press E to finish the round. Money: " +
                               playerStats.Money;
            }
        }

        private void TryFinishRoundAtSpawn()
        {
            if (!extractionGoalComplete)
            {
                Window.Title = "You need to extract more value first. Progress: " +
                               extractionSystem.ExtractedValue + "/" +
                               extractionSystem.RequiredValue;
                return;
            }

            if (inventory.Items.Count > 0)
            {
                Window.Title = "You still have items. Extract them before finishing the round.";
                return;
            }

            FinishRound();
        }

        private void FinishRound()
        {
            gameStateManager.EnterShop();

            Window.Title = GetShopTitle("Round " + roundManager.CurrentRound + " complete!");
        }

        private void UpdateShop(KeyboardState keyboard)
        {
            if (WasAnyKeyPressed(keyboard, Keys.D1, Keys.NumPad1))
            {
                bool bought = shopManager.BuySpeedUpgrade(playerStats);

                Window.Title = bought
                    ? GetShopTitle("Bought Speed Upgrade.")
                    : GetShopTitle("Not enough money for Speed Upgrade.");
            }

            if (WasAnyKeyPressed(keyboard, Keys.D2, Keys.NumPad2))
            {
                bool bought = shopManager.BuyStrengthUpgrade(playerStats);

                Window.Title = bought
                    ? GetShopTitle("Bought Strength Upgrade.")
                    : GetShopTitle("Not enough money for Strength Upgrade.");
            }

            if (WasAnyKeyPressed(keyboard, Keys.D3, Keys.NumPad3))
            {
                bool bought = shopManager.BuyStaminaUpgrade(playerStats);

                Window.Title = bought
                    ? GetShopTitle("Bought Stamina Upgrade.")
                    : GetShopTitle("Not enough money for Stamina Upgrade.");
            }

            if (WasAnyKeyPressed(keyboard, Keys.D4, Keys.NumPad4))
            {
                bool bought = shopManager.BuyHealthUpgrade(playerStats);

                Window.Title = bought
                    ? GetShopTitle("Bought Max Health Upgrade.")
                    : GetShopTitle("Not enough money for Max Health Upgrade.");
            }

            if (WasAnyKeyPressed(keyboard, Keys.D5, Keys.NumPad5))
            {
                bool bought = shopManager.BuyHealthPack(playerStats);

                Window.Title = bought
                    ? GetShopTitle("Bought Health Pack.")
                    : GetShopTitle("Cannot buy Health Pack. Either not enough money or already full HP.");
            }

            if (WasAnyKeyPressed(keyboard, Keys.Enter, Keys.Space))
            {
                StartNextRoundFromShop();
            }
        }

        private string GetShopTitle(string message)
        {
            int speedCost = shopManager.GetUpgradeCost(playerStats.SpeedLevel);
            int strengthCost = shopManager.GetUpgradeCost(playerStats.StrengthLevel);
            int staminaCost = shopManager.GetUpgradeCost(playerStats.StaminaLevel);
            int healthCost = shopManager.GetUpgradeCost(playerStats.HealthLevel);
            int healthPackCost = shopManager.GetHealthPackCost();

            return message +
                   " | SHOP" +
                   " | Money: " + playerStats.Money +
                   " | HP: " + playerStats.Health + "/" + playerStats.MaxHealth +
                   " | 1 Speed: " + speedCost +
                   " | 2 Strength: " + strengthCost +
                   " | 3 Stamina: " + staminaCost +
                   " | 4 Max HP: " + healthCost +
                   " | 5 Health Pack: " + healthPackCost +
                   " | Enter: Next Round";
        }

        private void StartNextRoundFromShop()
        {
            roundManager.NextRound();

            extractionSystem = new ExtractionSystem();
            extractionSystem.RequiredValue = roundManager.GetExtractionRequirement();

            extractionGoalComplete = false;

            inventory = new Inventory(playerStats);
            playerStats.CarriedWeight = 0f;

            tileMap = mapFileHandler.LoadMap(mapFilePath);
            discoveredTiles = new bool[tileMap.Width, tileMap.Height];

            playerPosition = FindSpawnPosition();

            gameStateManager.StartGame();

            Window.Title = "Round " + roundManager.CurrentRound +
                           " started. Required extraction value: " +
                           extractionSystem.RequiredValue +
                           " | Money: " + playerStats.Money +
                           " | HP: " + playerStats.Health + "/" + playerStats.MaxHealth;
        }

        private void HandleInteraction(KeyboardState keyboard)
        {
            bool pressedEThisFrame = WasKeyPressed(keyboard, Keys.E);

            if (!pressedEThisFrame)
            {
                return;
            }

            int playerTileX = (int)((playerPosition.X + playerSize.X / 2f) / TileSize);
            int playerTileY = (int)((playerPosition.Y + playerSize.Y / 2f) / TileSize);

            if (playerTileX < 0 || playerTileX >= tileMap.Width ||
                playerTileY < 0 || playerTileY >= tileMap.Height)
            {
                return;
            }

            Tile currentTile = tileMap.Tiles[playerTileX, playerTileY];

            if (currentTile.Type == TileType.Item)
            {
                TryPickupItem(currentTile);
                return;
            }

            if (currentTile.Type == TileType.Extraction)
            {
                TryExtractItems();
                return;
            }

            if (currentTile.Type == TileType.Spawn)
            {
                TryFinishRoundAtSpawn();
                return;
            }

            Window.Title = "Nothing to interact with here.";
        }
        private Color GetTileColor(TileType type)
        {
            switch (type)
            {
                case TileType.Wall:
                    return new Color(35, 38, 48);

                case TileType.Road:
                    return new Color(70, 76, 92);

                case TileType.Spawn:
                    return new Color(70, 120, 90);

                case TileType.Extraction:
                    return new Color(180, 150, 60);

                case TileType.Item:
                    return new Color(120, 90, 180);

                default:
                    return Color.Magenta;
            }
        }

        private void DrawPlayer()
        {
            Rectangle playerRectangle = new Rectangle(
                (int)(playerPosition.X - cameraPosition.X),
                (int)(playerPosition.Y - cameraPosition.Y),
                (int)playerSize.X,
                (int)playerSize.Y
            );

            spriteBatch.Draw(pixel, playerRectangle, Color.Cyan);
        }
    }
}
