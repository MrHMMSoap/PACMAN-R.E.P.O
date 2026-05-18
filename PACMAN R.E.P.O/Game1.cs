using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PACMAN_R.E.P.O.Items;
using PACMAN_R.E.P.O.Map;
using PACMAN_R.E.P.O.Monsters;
using PACMAN_R.E.P.O.Systems;
using PACMAN_R.E.P.O.Save;
using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;
using System;
using System.Collections.Generic;
using System.IO;

namespace PACMAN_R.E.P.O
{
    /// <summary>
    /// Main game class for PACMAN R.E.P.O - an extraction-based roguelike game.
    /// Manages game state, rendering, player movement, monster AI, and user authentication.
    /// </summary>
    public class Game1 : Game
    {
        #region Graphics and Rendering Fields

        /// <summary>Graphics device manager for handling display settings and resolution.</summary>
        private GraphicsDeviceManager graphics;

        /// <summary>Sprite batch for efficient 2D rendering.</summary>
        private SpriteBatch spriteBatch;

        /// <summary>Single white pixel texture used for drawing colored rectangles.</summary>
        private Texture2D pixel;

        /// <summary>Font used for rendering all HUD text elements.</summary>
        private SpriteFont hudFont;

        #endregion

        #region Map and World Fields

        /// <summary>The tile-based map representing the game world.</summary>
        private TileMap tileMap;

        /// <summary>Handler for loading and saving map files.</summary>
        private MapFileHandler mapFileHandler;

        /// <summary>Size of each tile in pixels.</summary>
        private const int TileSize = 32;

        /// <summary>How many tiles away from the player are visible.</summary>
        private const int ViewRadius = 4;

        /// <summary>System for calculating tile visibility based on player position.</summary>
        private VisibilitySystem visibilitySystem;

        /// <summary>Path to the current map file being used.</summary>
        private string mapFilePath;

        /// <summary>2D array tracking which tiles the player has discovered (fog of war).</summary>
        private bool[,] discoveredTiles;

        /// <summary>Camera position in world space for rendering.</summary>
        private Vector2 cameraPosition;

        #endregion

        #region Player Fields

        /// <summary>Player entity containing health, stamina, stats, and upgrade levels.</summary>
        private PlayerEntity playerStats;

        /// <summary>Player's inventory system for carrying items.</summary>
        private Inventory inventory;

        /// <summary>Player's position in world space (pixels).</summary>
        private Vector2 playerPosition;

        /// <summary>Player's collision box size.</summary>
        private Vector2 playerSize = new Vector2(24, 24);

        /// <summary>Current movement speed of the player (modified by weight and sprinting).</summary>
        private float playerSpeed = 150f;

        /// <summary>Speed multiplier applied when sprinting.</summary>
        private float sprintMultiplier = 1.7f;

        /// <summary>How much stamina is consumed per second while sprinting.</summary>
        private float staminaDrainPerSecond = 40f;

        /// <summary>How much stamina regenerates per second when not sprinting.</summary>
        private float staminaRegenPerSecond = 8f;

        #endregion

        #region Game Systems

        /// <summary>System for managing player stamina consumption and regeneration.</summary>
        private StaminaSystem staminaSystem;

        /// <summary>System for handling item extraction and extraction goals.</summary>
        private ExtractionSystem extractionSystem;

        /// <summary>Manages round progression and difficulty scaling.</summary>
        private RoundManager roundManager;

        /// <summary>Manages shop purchases and upgrade costs.</summary>
        private ShopManager shopManager;

        /// <summary>Controls game state transitions (login, playing, shop, etc.).</summary>
        private GameStateManager gameStateManager;

        /// <summary>System for managing player health and damage.</summary>
        private HealthSystem healthSystem;

        /// <summary>Tracks if the player has extracted enough value to complete the round.</summary>
        private bool extractionGoalComplete;

        #endregion

        #region Monster Fields

        /// <summary>The wraith enemy that chases the player.</summary>
        private Wraith wraith;

        /// <summary>Wraith's position in world space (pixels).</summary>
        private Vector2 wraithPosition;

        /// <summary>Wraith's collision box size.</summary>
        private Vector2 wraithSize = new Vector2(22, 22);

        /// <summary>Current tile the wraith is pathfinding towards.</summary>
        private Point wraithTargetTile;

        /// <summary>Whether the wraith has a valid target tile for pathfinding.</summary>
        private bool wraithHasTargetTile = false;

        /// <summary>Wraith's movement speed when in normal chase mode.</summary>
        private float wraithSlowSpeed = 45f;

        /// <summary>Wraith's movement speed when enraged after being seen.</summary>
        private float wraithRageSpeed = 180f;

        /// <summary>Cooldown timer to prevent continuous damage from the wraith.</summary>
        private float wraithDamageCooldown = 0f;

        /// <summary>Duration in seconds between wraith damage instances.</summary>
        private float wraithDamageCooldownTime = 1.0f;

        /// <summary>Amount of damage the wraith deals per hit.</summary>
        private int wraithDamage = 20;

        #endregion

        #region Input and Authentication Fields

        /// <summary>Previous frame's keyboard state for detecting key presses.</summary>
        private KeyboardState previousKeyboardState;

        /// <summary>Random number generator for item generation and spawning.</summary>
        private Random random;

        /// <summary>Database handler for user accounts and save files.</summary>
        private SQLHandler sqlHandler;

        /// <summary>Currently logged-in username.</summary>
        private string currentUsername = "";

        /// <summary>Username input field for login screen.</summary>
        private string usernameInput = "";

        /// <summary>Password input field for login screen.</summary>
        private string passwordInput = "";

        /// <summary>Whether the user is currently typing in the password field.</summary>
        private bool typingPassword = false;

        /// <summary>Message displayed on the login screen.</summary>
        private string loginMessage = "Enter username and password. Enter = Login | F2 = Create Account";

        /// <summary>Message displayed on the main menu screen.</summary>
        private string mainMenuMessage = "N = New Save | L = Load Save";

        #endregion
     

        /// <summary>
        /// Initializes a new instance of the Game1 class.
        /// Sets up graphics manager, content directory, and initial window settings.
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Set window resolution
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        /// <summary>
        /// Initializes all game systems, loads the map, sets up the database,
        /// and prepares the game for the login screen.
        /// Called once when the game starts, before the first frame.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize map loading system
            mapFileHandler = new MapFileHandler();
            visibilitySystem = new VisibilitySystem(ViewRadius);

            // Load the main game map
            mapFilePath = Path.Combine(AppContext.BaseDirectory, "Maps", "test_map.txt");
            tileMap = mapFileHandler.LoadMap(mapFilePath);
            discoveredTiles = new bool[tileMap.Width, tileMap.Height];

            // Initialize player and game systems
            playerStats = new PlayerEntity();
            inventory = new Inventory(playerStats);
            staminaSystem = new StaminaSystem();
            healthSystem = new HealthSystem();

            // Initialize round and progression systems
            roundManager = new RoundManager();
            extractionSystem = new ExtractionSystem();
            extractionSystem.RequiredValue = roundManager.GetExtractionRequirement();

            shopManager = new ShopManager();

            // Start in the login state
            gameStateManager = new GameStateManager();
            gameStateManager.ChangeState(GameState.Login);

            extractionGoalComplete = false;

            // Initialize utilities
            random = new Random();
            previousKeyboardState = Keyboard.GetState();

            // Spawn player and wraith at appropriate locations
            playerPosition = FindSpawnPosition();
            wraith = new Wraith();
            wraithPosition = FindWraithSpawnPosition();
            wraithHasTargetTile = false;

            // Initialize SQLite database for user accounts and saves
            string databasePath = Path.Combine(AppContext.BaseDirectory, "save.db");
            sqlHandler = new SQLHandler(databasePath);
            sqlHandler.InitializeDatabase();

            base.Initialize();
        }

        /// <summary>
        /// Loads all game content (textures, fonts, etc.).
        /// Called once after Initialize.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create a single white pixel texture for drawing colored rectangles
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            // Load the font for HUD text
            hudFont = Content.Load<SpriteFont>("HudFont");
        }

        /// <summary>
        /// Main update loop called every frame.
        /// Routes to appropriate update method based on current game state.
        /// </summary>
        /// <param name="gameTime">Provides timing information for the current frame.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            // Allow exit at any time
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Route to appropriate update method based on current state
            if (gameStateManager.CurrentState == GameState.Login)
            {
                UpdateLogin(keyboard);
            }
            else if (gameStateManager.CurrentState == GameState.MainMenu)
            {
                UpdateMainMenu(keyboard);
            }
            else if (gameStateManager.CurrentState == GameState.Playing)
            {
                // Allow saving during gameplay
                if (WasKeyPressed(keyboard, Keys.F5))
                {
                    SaveCurrentGame();
                }

                UpdatePlaying(gameTime, keyboard);
            }
            else if (gameStateManager.CurrentState == GameState.Shop)
            {
                // Allow saving in shop
                if (WasKeyPressed(keyboard, Keys.F5))
                {
                    SaveCurrentGame();
                }

                UpdateShop(keyboard);
            }

            // Store keyboard state for next frame's input detection
            previousKeyboardState = keyboard;

            base.Update(gameTime);
        }

        #region Login System

        /// <summary>
        /// Updates the login screen, handling username/password input and account creation.
        /// </summary>
        /// <param name="keyboard">Current keyboard state.</param>
        private void UpdateLogin(KeyboardState keyboard)
        {
            // Switch between username and password fields
            if (WasKeyPressed(keyboard, Keys.Tab))
            {
                typingPassword = !typingPassword;
            }

            // Handle backspace for deleting characters
            if (WasKeyPressed(keyboard, Keys.Back))
            {
                if (typingPassword)
                {
                    if (passwordInput.Length > 0)
                    {
                        passwordInput = passwordInput.Substring(0, passwordInput.Length - 1);
                    }
                }
                else
                {
                    if (usernameInput.Length > 0)
                    {
                        usernameInput = usernameInput.Substring(0, usernameInput.Length - 1);
                    }
                }
            }

            // Get any typed character and add it to the active field
            string typedCharacter = GetTypedCharacter(keyboard);

            if (typedCharacter != "")
            {
                if (typingPassword)
                {
                    passwordInput += typedCharacter;
                }
                else
                {
                    usernameInput += typedCharacter;
                }
            }

            // Attempt login
            if (WasKeyPressed(keyboard, Keys.Enter))
            {
                TryLogin();
            }

            // Attempt account creation
            if (WasKeyPressed(keyboard, Keys.F2))
            {
                TryCreateAccount();
            }
        }

        /// <summary>
        /// Attempts to log in with the entered username and password.
        /// On success, transitions to the main menu.
        /// </summary>
        private void TryLogin()
        {
            bool success = sqlHandler.ValidateLogin(usernameInput, passwordInput);

            if (!success)
            {
                loginMessage = "Login failed. Check username/password or press F2 to create account.";
                return;
            }

            currentUsername = usernameInput;
            loginMessage = "Logged in as " + currentUsername;

            gameStateManager.ChangeState(GameState.MainMenu);
        }

        /// <summary>
        /// Attempts to create a new user account with the entered credentials.
        /// On success, logs in automatically and transitions to the main menu.
        /// </summary>
        private void TryCreateAccount()
        {
            bool success = sqlHandler.CreateUser(usernameInput, passwordInput);

            if (!success)
            {
                loginMessage = "Could not create account. Username may already exist or fields are empty.";
                return;
            }

            currentUsername = usernameInput;
            loginMessage = "Account created. Logged in as " + currentUsername;

            gameStateManager.ChangeState(GameState.MainMenu);
        }

        /// <summary>
        /// Detects which character key was pressed this frame, if any.
        /// Supports letters, numbers, and basic punctuation.
        /// </summary>
        /// <param name="keyboard">Current keyboard state.</param>
        /// <returns>The typed character as a string, or empty string if none.</returns>
        private string GetTypedCharacter(KeyboardState keyboard)
        {
            Keys[] pressedKeys = keyboard.GetPressedKeys();

            foreach (Keys key in pressedKeys)
            {
                // Only process if this is a new press (not held from previous frame)
                if (!WasKeyPressed(keyboard, key))
                {
                    continue;
                }

                // Handle letter keys (A-Z)
                if (key >= Keys.A && key <= Keys.Z)
                {
                    char character = (char)('a' + (key - Keys.A));
                    return character.ToString();
                }

                // Handle number row keys (0-9)
                if (key >= Keys.D0 && key <= Keys.D9)
                {
                    char character = (char)('0' + (key - Keys.D0));
                    return character.ToString();
                }

                // Handle numpad keys (0-9)
                if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
                {
                    char character = (char)('0' + (key - Keys.NumPad0));
                    return character.ToString();
                }

                // Handle special characters
                if (key == Keys.OemMinus)
                {
                    return "-";
                }

                if (key == Keys.OemPeriod)
                {
                    return ".";
                }
            }

            return "";
        }

        #endregion
        

        #region Main Menu and Save/Load System

        /// <summary>
        /// Updates the main menu screen, handling new game and load game inputs.
        /// </summary>
        /// <param name="keyboard">Current keyboard state.</param>
        private void UpdateMainMenu(KeyboardState keyboard)
        {
            // Start a new save file
            if (WasKeyPressed(keyboard, Keys.N))
            {
                StartNewSave();
            }

            // Load existing save file
            if (WasKeyPressed(keyboard, Keys.L))
            {
                LoadExistingSave();
            }
        }

        /// <summary>
        /// Initializes a new game save with default values and transitions to gameplay.
        /// Resets all player stats, round counter, and creates a fresh map.
        /// </summary>
        private void StartNewSave()
        {
            // Reset player stats to defaults
            playerStats = new PlayerEntity();
            inventory = new Inventory(playerStats);

            // Start at round 1
            roundManager = new RoundManager();

            // Set extraction goal for round 1
            extractionSystem = new ExtractionSystem();
            extractionSystem.RequiredValue = roundManager.GetExtractionRequirement();

            extractionGoalComplete = false;

            // Load a fresh map
            tileMap = mapFileHandler.LoadMap(mapFilePath);
            discoveredTiles = new bool[tileMap.Width, tileMap.Height];

            playerPosition = FindSpawnPosition();

            // Spawn a new wraith
            wraith = new Wraith();
            wraithPosition = FindWraithSpawnPosition();
            wraithHasTargetTile = false;
            wraithDamageCooldown = 0f;

            // Transition to gameplay
            gameStateManager.StartGame();

            // Save immediately so the player has a save file
            SaveCurrentGame();

            Window.Title = "New save started for " + currentUsername;
        }

        /// <summary>
        /// Loads an existing save file from the database and applies it to the game state.
        /// If no save exists, displays an error message.
        /// </summary>
        private void LoadExistingSave()
        {
            SaveData saveData = sqlHandler.LoadGame(currentUsername);

            if (saveData == null)
            {
                mainMenuMessage = "No save found. Press N to start a new save.";
                return;
            }

            ApplySaveData(saveData);

            gameStateManager.StartGame();

            Window.Title = "Loaded save for " + currentUsername +
                           " | Round: " + roundManager.CurrentRound +
                           " | Money: " + playerStats.Money;
        }

        /// <summary>
        /// Saves the current game state (stats, round, upgrades) to the database.
        /// </summary>
        private void SaveCurrentGame()
        {
            if (string.IsNullOrWhiteSpace(currentUsername))
            {
                return;
            }

            // Package all relevant data into a save object
            SaveData saveData = new SaveData();

            saveData.Round = roundManager.CurrentRound;
            saveData.Money = playerStats.Money;
            saveData.Health = playerStats.Health;

            saveData.SpeedLevel = playerStats.SpeedLevel;
            saveData.StrengthLevel = playerStats.StrengthLevel;
            saveData.StaminaLevel = playerStats.StaminaLevel;
            saveData.HealthLevel = playerStats.HealthLevel;

            saveData.MapFile = "Maps/test_map.txt";

            // Save to database
            sqlHandler.SaveGame(currentUsername, saveData);

            Window.Title = "Game saved for " + currentUsername;
        }

        /// <summary>
        /// Applies loaded save data to the current game state.
        /// Restores player stats, upgrades, round number, and recalculates derived values.
        /// </summary>
        /// <param name="saveData">The save data loaded from the database.</param>
        private void ApplySaveData(SaveData saveData)
        {
            // Create a new player entity and restore saved values
            playerStats = new PlayerEntity();

            playerStats.Money = saveData.Money;
            playerStats.Health = saveData.Health;

            playerStats.SpeedLevel = saveData.SpeedLevel;
            playerStats.StrengthLevel = saveData.StrengthLevel;
            playerStats.StaminaLevel = saveData.StaminaLevel;
            playerStats.HealthLevel = saveData.HealthLevel;

            // Recalculate max values based on upgrade levels
            playerStats.BaseSpeed = 120f + playerStats.SpeedLevel * 10f;
            playerStats.MaxCarryWeight = 10f + playerStats.StrengthLevel * 2f;
            playerStats.MaxStamina = 100f + playerStats.StaminaLevel * 20f;
            playerStats.MaxHealth = 100 + playerStats.HealthLevel * 20;

            // Ensure health doesn't exceed max after an upgrade
            if (playerStats.Health > playerStats.MaxHealth)
            {
                playerStats.Health = playerStats.MaxHealth;
            }

            // Restore stamina to full
            playerStats.Stamina = playerStats.MaxStamina;
            playerStats.CarriedWeight = 0f;

            // Recreate inventory (empty at start of round)
            inventory = new Inventory(playerStats);

            // Restore round number
            roundManager = new RoundManager();
            roundManager.SetRound(saveData.Round);

            // Set extraction goal for the current round
            extractionSystem = new ExtractionSystem();
            extractionSystem.RequiredValue = roundManager.GetExtractionRequirement();

            extractionGoalComplete = false;

            // Reload the map (fresh state for the round)
            tileMap = mapFileHandler.LoadMap(mapFilePath);
            discoveredTiles = new bool[tileMap.Width, tileMap.Height];

            playerPosition = FindSpawnPosition();

            // Spawn a new wraith for this round
            wraith = new Wraith();
            wraithPosition = FindWraithSpawnPosition();
            wraithHasTargetTile = false;
            wraithDamageCooldown = 0f;
        }

        #endregion
        

        #region Gameplay Update

        /// <summary>
        /// Main gameplay update loop. Handles player movement, interactions, wraith AI,
        /// visibility updates, and camera following.
        /// </summary>
        /// <param name="gameTime">Provides timing information.</param>
        /// <param name="keyboard">Current keyboard state.</param>
        private void UpdatePlaying(GameTime gameTime, KeyboardState keyboard)
        {
            UpdatePlayerMovement(gameTime, keyboard);
            HandleInteraction(keyboard);

            UpdateWraith(gameTime);

            UpdateDiscoveredTiles();
            UpdateCamera();
        }

        #endregion

        #region Wraith AI System

        /// <summary>
        /// Updates the wraith's AI behavior including state management, pathfinding,
        /// movement, and collision with the player.
        /// </summary>
        /// <param name="gameTime">Provides timing information.</param>
        private void UpdateWraith(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update damage cooldown timer
            if (wraithDamageCooldown > 0f)
            {
                wraithDamageCooldown -= deltaTime;

                if (wraithDamageCooldown < 0f)
                {
                    wraithDamageCooldown = 0f;
                }
            }

            // Determine player and wraith tile positions
            int playerTileX = (int)((playerPosition.X + playerSize.X / 2f) / TileSize);
            int playerTileY = (int)((playerPosition.Y + playerSize.Y / 2f) / TileSize);

            int wraithTileX = (int)((wraithPosition.X + wraithSize.X / 2f) / TileSize);
            int wraithTileY = (int)((wraithPosition.Y + wraithSize.Y / 2f) / TileSize);

            // Check if wraith is visible to the player
            bool wraithVisibleToPlayer = visibilitySystem.IsTileVisible(
                wraithTileX,
                wraithTileY,
                playerTileX,
                playerTileY
            );

            // Trigger rage mode if the wraith sees the player (and not already raging)
            if (wraithVisibleToPlayer && wraith.State != WraithState.Rage)
            {
                wraith.OnSeenByPlayer();
            }

            // Update rage timer (will revert to slow chase when timer expires)
            wraith.UpdateRageTimer(deltaTime);

            // Determine movement speed based on wraith state
            float speed = wraith.State == WraithState.Rage
                ? wraithRageSpeed
                : wraithSlowSpeed;

            // Move wraith towards player
            MoveWraithTowardsPlayer(deltaTime, speed);

            // Check for collision with player
            CheckWraithPlayerCollision();
        }

        /// <summary>
        /// Moves the wraith towards the player using breadth-first search pathfinding.
        /// Calculates a new path when needed and moves one step at a time.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame in seconds.</param>
        /// <param name="speed">Movement speed in pixels per second.</param>
        private void MoveWraithTowardsPlayer(float deltaTime, float speed)
        {
            // Get current tile positions
            Point wraithTile = new Point(
                (int)((wraithPosition.X + wraithSize.X / 2f) / TileSize),
                (int)((wraithPosition.Y + wraithSize.Y / 2f) / TileSize)
            );

            Point playerTile = new Point(
                (int)((playerPosition.X + playerSize.X / 2f) / TileSize),
                (int)((playerPosition.Y + playerSize.Y / 2f) / TileSize)
            );

            // Already on player's tile
            if (wraithTile == playerTile)
            {
                return;
            }

            // Calculate new path if needed
            if (!wraithHasTargetTile)
            {
                List<Point> path = FindPath(wraithTile, playerTile);

                // No valid path found
                if (path.Count == 0)
                {
                    return;
                }

                // Set first step in path as target
                wraithTargetTile = path[0];
                wraithHasTargetTile = true;
            }

            // Get pixel position of target tile center
            Vector2 targetPosition = GetTileCenter(
                wraithTargetTile.X,
                wraithTargetTile.Y,
                wraithSize
            );

            // Calculate direction and distance to target
            Vector2 direction = targetPosition - wraithPosition;
            float distanceToTarget = direction.Length();

            // Reached target tile
            if (distanceToTarget <= 1f)
            {
                wraithPosition = targetPosition;
                wraithHasTargetTile = false;
                return;
            }

            // Calculate movement this frame
            float moveDistance = speed * deltaTime;

            // Will reach target this frame
            if (moveDistance >= distanceToTarget)
            {
                wraithPosition = targetPosition;
                wraithHasTargetTile = false;
                return;
            }

            // Move towards target
            direction.Normalize();
            wraithPosition += direction * moveDistance;
        }

        /// <summary>
        /// Checks if a monster can move to a given position without colliding with walls.
        /// Tests all four corners of the monster's bounding box.
        /// </summary>
        /// <param name="newPosition">The proposed position to test.</param>
        /// <param name="monsterSize">The size of the monster's collision box.</param>
        /// <returns>True if all corners are in walkable tiles.</returns>
        private bool CanMonsterMoveTo(Vector2 newPosition, Vector2 monsterSize)
        {
            Rectangle monsterRectangle = new Rectangle(
                (int)newPosition.X,
                (int)newPosition.Y,
                (int)monsterSize.X,
                (int)monsterSize.Y
            );

            // Test all four corners of the bounding box
            Point topLeft = new Point(monsterRectangle.Left, monsterRectangle.Top);
            Point topRight = new Point(monsterRectangle.Right - 1, monsterRectangle.Top);
            Point bottomLeft = new Point(monsterRectangle.Left, monsterRectangle.Bottom - 1);
            Point bottomRight = new Point(monsterRectangle.Right - 1, monsterRectangle.Bottom - 1);

            return IsPixelPositionWalkable(topLeft) &&
                   IsPixelPositionWalkable(topRight) &&
                   IsPixelPositionWalkable(bottomLeft) &&
                   IsPixelPositionWalkable(bottomRight);
        }

        /// <summary>
        /// Checks for collision between wraith and player. If colliding and cooldown expired,
        /// deals damage to the player and resets the cooldown.
        /// </summary>
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

            // No collision
            if (!playerRectangle.Intersects(wraithRectangle))
            {
                return;
            }

            // Still on cooldown
            if (wraithDamageCooldown > 0f)
            {
                return;
            }

            // Deal damage to player
            healthSystem.TakeDamage(playerStats, wraithDamage);
            wraithDamageCooldown = wraithDamageCooldownTime;

            Window.Title = "Wraith hit you! HP: " + playerStats.Health + "/" + playerStats.MaxHealth;

            // Check for player death
            if (healthSystem.IsDead(playerStats))
            {
                gameStateManager.ChangeState(GameState.GameOver);
                Window.Title = "GAME OVER - Wraith killed you.";
            }
        }

        /// <summary>
        /// Calculates the center position of a tile in pixel coordinates,
        /// adjusted for an entity's size so the entity appears centered in the tile.
        /// </summary>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <param name="entitySize">The size of the entity to center.</param>
        /// <returns>Pixel position for the entity to be centered in the tile.</returns>
        private Vector2 GetTileCenter(int tileX, int tileY, Vector2 entitySize)
        {
            float x = tileX * TileSize + (TileSize - entitySize.X) / 2f;
            float y = tileY * TileSize + (TileSize - entitySize.Y) / 2f;

            return new Vector2(x, y);
        }

        #endregion
        

        #region Pathfinding System

        /// <summary>
        /// Finds a path from start to goal using breadth-first search algorithm.
        /// Returns the path as a list of tile positions (does not include the start tile).
        /// </summary>
        /// <param name="start">Starting tile position.</param>
        /// <param name="goal">Target tile position.</param>
        /// <returns>List of tile positions from start to goal, or empty list if no path found.</returns>
        private List<Point> FindPath(Point start, Point goal)
        {
            // Initialize BFS queue and visited tracking
            Queue<Point> frontier = new Queue<Point>();
            Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();

            frontier.Enqueue(start);
            cameFrom[start] = start;

            // Four cardinal directions for pathfinding
            Point[] directions =
            {
                new Point(1, 0),   // Right
                new Point(-1, 0),  // Left
                new Point(0, 1),   // Down
                new Point(0, -1)   // Up
            };

            // Breadth-first search main loop
            while (frontier.Count > 0)
            {
                Point current = frontier.Dequeue();

                // Found the goal
                if (current == goal)
                {
                    break;
                }

                // Explore all neighbors
                foreach (Point direction in directions)
                {
                    Point next = new Point(
                        current.X + direction.X,
                        current.Y + direction.Y
                    );

                    // Out of bounds
                    if (next.X < 0 || next.X >= tileMap.Width ||
                        next.Y < 0 || next.Y >= tileMap.Height)
                    {
                        continue;
                    }

                    // Wall or non-walkable tile
                    if (!tileMap.Tiles[next.X, next.Y].IsWalkable)
                    {
                        continue;
                    }

                    // Already visited
                    if (cameFrom.ContainsKey(next))
                    {
                        continue;
                    }

                    // Add to frontier
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }

            // No path found
            if (!cameFrom.ContainsKey(goal))
            {
                return new List<Point>();
            }

            // Reconstruct path by backtracking from goal to start
            List<Point> path = new List<Point>();

            Point pathCurrent = goal;

            while (pathCurrent != start)
            {
                path.Add(pathCurrent);
                pathCurrent = cameFrom[pathCurrent];
            }

            // Reverse to get path from start to goal
            path.Reverse();

            return path;
        }

        /// <summary>
        /// Checks if a key was pressed this frame (down now, up last frame).
        /// </summary>
        /// <param name="keyboard">Current keyboard state.</param>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key was just pressed this frame.</returns>
        private bool WasKeyPressed(KeyboardState keyboard, Keys key)
        {
            return keyboard.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if either of two keys was pressed this frame.
        /// Useful for supporting both number row and numpad inputs.
        /// </summary>
        /// <param name="keyboard">Current keyboard state.</param>
        /// <param name="keyOne">First key to check.</param>
        /// <param name="keyTwo">Second key to check.</param>
        /// <returns>True if either key was just pressed this frame.</returns>
        private bool WasAnyKeyPressed(KeyboardState keyboard, Keys keyOne, Keys keyTwo)
        {
            return WasKeyPressed(keyboard, keyOne) || WasKeyPressed(keyboard, keyTwo);
        }

        #endregion

        #region Player Movement System

        /// <summary>
        /// Updates player movement based on keyboard input.
        /// Handles WASD movement, sprinting with Shift, stamina consumption/regeneration,
        /// and collision detection.
        /// </summary>
        /// <param name="gameTime">Provides timing information.</param>
        /// <param name="keyboard">Current keyboard state.</param>
        private void UpdatePlayerMovement(GameTime gameTime, KeyboardState keyboard)
        {
            // Build movement vector from WASD input
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

            // Normalize diagonal movement to prevent faster diagonal speed
            if (movement != Vector2.Zero)
            {
                movement.Normalize();
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate speed based on weight and upgrades
            float currentSpeed = playerStats.CalculateSpeed();

            bool wantsToSprint = keyboard.IsKeyDown(Keys.LeftShift);
            bool isMoving = movement != Vector2.Zero;
            bool canSprint = staminaSystem.CanSprint(playerStats);

            // Handle sprinting and stamina
            if (wantsToSprint && isMoving && canSprint)
            {
                // Apply sprint multiplier
                currentSpeed *= sprintMultiplier;

                // Drain stamina
                float staminaToUse = staminaDrainPerSecond * deltaTime;
                staminaSystem.UseStamina(playerStats, staminaToUse);
            }
            else
            {
                // Regenerate stamina when not sprinting
                float staminaToRegenerate = staminaRegenPerSecond * deltaTime;
                staminaSystem.RegenerateStamina(playerStats, staminaToRegenerate);
            }

            playerSpeed = currentSpeed;

            // Calculate new position
            Vector2 newPosition = playerPosition + movement * playerSpeed * deltaTime;

            // Only move if new position is valid (no collision)
            if (CanMoveTo(newPosition))
            {
                playerPosition = newPosition;
            }
        }

        /// <summary>
        /// Checks if the player can move to a proposed position without colliding with walls.
        /// Tests all four corners of the player's bounding box.
        /// </summary>
        /// <param name="newPosition">The proposed position to test.</param>
        /// <returns>True if all corners are in walkable tiles.</returns>
        private bool CanMoveTo(Vector2 newPosition)
        {
            Rectangle playerRectangle = new Rectangle(
                (int)newPosition.X,
                (int)newPosition.Y,
                (int)playerSize.X,
                (int)playerSize.Y
            );

            // Test all four corners of the bounding box
            Point topLeft = new Point(playerRectangle.Left, playerRectangle.Top);
            Point topRight = new Point(playerRectangle.Right - 1, playerRectangle.Top);
            Point bottomLeft = new Point(playerRectangle.Left, playerRectangle.Bottom - 1);
            Point bottomRight = new Point(playerRectangle.Right - 1, playerRectangle.Bottom - 1);

            return IsPixelPositionWalkable(topLeft) &&
                   IsPixelPositionWalkable(topRight) &&
                   IsPixelPositionWalkable(bottomLeft) &&
                   IsPixelPositionWalkable(bottomRight);
        }

        /// <summary>
        /// Checks if a pixel position is within a walkable tile.
        /// </summary>
        /// <param name="pixelPosition">The pixel position to check.</param>
        /// <returns>True if the position is walkable.</returns>
        private bool IsPixelPositionWalkable(Point pixelPosition)
        {
            // Convert pixel position to tile coordinates
            int tileX = pixelPosition.X / TileSize;
            int tileY = pixelPosition.Y / TileSize;

            // Out of bounds
            if (tileX < 0 || tileX >= tileMap.Width || tileY < 0 || tileY >= tileMap.Height)
            {
                return false;
            }

            return tileMap.Tiles[tileX, tileY].IsWalkable;
        }

        #endregion
     

        #region Spawning System

        /// <summary>
        /// Finds the spawn tile on the map and returns its center position in pixels.
        /// Falls back to a default position if no spawn tile is found.
        /// </summary>
        /// <returns>Player spawn position in pixels.</returns>
        private Vector2 FindSpawnPosition()
        {
            // Search the entire map for the spawn tile
            for (int x = 0; x < tileMap.Width; x++)
            {
                for (int y = 0; y < tileMap.Height; y++)
                {
                    if (tileMap.Tiles[x, y].Type == TileType.Spawn)
                    {
                        // Center the player in the spawn tile
                        float spawnX = x * TileSize + (TileSize - playerSize.X) / 2f;
                        float spawnY = y * TileSize + (TileSize - playerSize.Y) / 2f;

                        return new Vector2(spawnX, spawnY);
                    }
                }
            }

            // Fallback if no spawn tile found
            return new Vector2(TileSize, TileSize);
        }

        /// <summary>
        /// Finds an optimal spawn position for the wraith.
        /// Searches from far corners of the map for a valid position that:
        /// - Is walkable
        /// - Is not on spawn or extraction tiles
        /// - Has a path to the player
        /// - Is at least 8 tiles away from the player
        /// </summary>
        /// <returns>Wraith spawn position in pixels.</returns>
        private Vector2 FindWraithSpawnPosition()
        {
            Point playerTile = new Point(
                (int)((playerPosition.X + playerSize.X / 2f) / TileSize),
                (int)((playerPosition.Y + playerSize.Y / 2f) / TileSize)
            );

            // Search from bottom-right corner backwards to find distant spawn
            for (int x = tileMap.Width - 2; x >= 1; x--)
            {
                for (int y = tileMap.Height - 2; y >= 1; y--)
                {
                    Tile tile = tileMap.Tiles[x, y];

                    // Must be walkable
                    if (!tile.IsWalkable)
                    {
                        continue;
                    }

                    // Avoid spawning on special tiles
                    if (tile.Type == TileType.Spawn || tile.Type == TileType.Extraction)
                    {
                        continue;
                    }

                    Point wraithTile = new Point(x, y);

                    // Verify path exists to player
                    List<Point> path = FindPath(wraithTile, playerTile);

                    if (path.Count == 0)
                    {
                        continue;
                    }

                    // Check minimum distance requirement
                    float distanceFromPlayer = Vector2.Distance(
                        GetTileCenter(x, y, wraithSize),
                        playerPosition
                    );

                    if (distanceFromPlayer < TileSize * 8)
                    {
                        continue;
                    }

                    return GetTileCenter(x, y, wraithSize);
                }
            }

            // No ideal position found, use fallback search
            return FindFallbackWraithPosition(playerTile);
        }

        /// <summary>
        /// Fallback method for finding wraith spawn when no ideal position exists.
        /// Searches in expanding rings around the player for any valid position.
        /// </summary>
        /// <param name="playerTile">The player's current tile position.</param>
        /// <returns>A valid wraith spawn position, or player position as last resort.</returns>
        private Vector2 FindFallbackWraithPosition(Point playerTile)
        {
            // Search in expanding rings from radius 5 to 15
            for (int radius = 5; radius < 15; radius++)
            {
                for (int x = playerTile.X - radius; x <= playerTile.X + radius; x++)
                {
                    for (int y = playerTile.Y - radius; y <= playerTile.Y + radius; y++)
                    {
                        // Out of bounds
                        if (x < 0 || x >= tileMap.Width || y < 0 || y >= tileMap.Height)
                        {
                            continue;
                        }

                        // Must be walkable
                        if (!tileMap.Tiles[x, y].IsWalkable)
                        {
                            continue;
                        }

                        // Verify path exists
                        Point possibleTile = new Point(x, y);
                        List<Point> path = FindPath(possibleTile, playerTile);

                        if (path.Count > 0)
                        {
                            return GetTileCenter(x, y, wraithSize);
                        }
                    }
                }
            }

            // Last resort: spawn at player position (this should rarely happen)
            return GetTileCenter(playerTile.X, playerTile.Y, wraithSize);
        }

        /// <summary>
        /// Updates the camera position to follow the player, centered on screen.
        /// </summary>
        private void UpdateCamera()
        {
            float screenCenterX = graphics.PreferredBackBufferWidth / 2f;
            float screenCenterY = graphics.PreferredBackBufferHeight / 2f;

            cameraPosition = new Vector2(
                playerPosition.X - screenCenterX,
                playerPosition.Y - screenCenterY
            );
        }

        #endregion
        

        #region Rendering System

        /// <summary>
        /// Main draw loop called every frame.
        /// Routes to appropriate rendering method based on current game state.
        /// </summary>
        /// <param name="gameTime">Provides timing information.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            // Route to appropriate drawing method based on state
            if (gameStateManager.CurrentState == GameState.Login)
            {
                DrawLoginHUD();
            }
            else if (gameStateManager.CurrentState == GameState.MainMenu)
            {
                DrawMainMenuHUD();
            }
            else if (gameStateManager.CurrentState == GameState.Playing)
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

        /// <summary>
        /// Draws the login screen with username/password fields and instructions.
        /// </summary>
        private void DrawLoginHUD()
        {
            GraphicsDevice.Clear(Color.Black);

            int x = 80;
            int y = 80;
            int lineHeight = 32;

            // Hide password with asterisks
            string hiddenPassword = new string('*', passwordInput.Length);

            // Title
            spriteBatch.DrawString(hudFont, "LOGIN", new Vector2(x, y), Color.Gold);

            y += lineHeight * 2;

            // Highlight the currently active field
            Color usernameColor = typingPassword ? Color.White : Color.LightGreen;
            Color passwordColor = typingPassword ? Color.LightGreen : Color.White;

            spriteBatch.DrawString(hudFont, "Username: " + usernameInput, new Vector2(x, y), usernameColor);
            y += lineHeight;

            spriteBatch.DrawString(hudFont, "Password: " + hiddenPassword, new Vector2(x, y), passwordColor);
            y += lineHeight * 2;

            // Instructions
            spriteBatch.DrawString(hudFont, "Tab: Switch field", new Vector2(x, y), Color.LightGray);
            y += lineHeight;

            spriteBatch.DrawString(hudFont, "Enter: Login", new Vector2(x, y), Color.LightGray);
            y += lineHeight;

            spriteBatch.DrawString(hudFont, "F2: Create account", new Vector2(x, y), Color.LightGray);
            y += lineHeight * 2;

            // Status/error message
            spriteBatch.DrawString(hudFont, loginMessage, new Vector2(x, y), Color.White);
        }

        /// <summary>
        /// Draws the main menu screen with new game and load game options.
        /// </summary>
        private void DrawMainMenuHUD()
        {
            GraphicsDevice.Clear(Color.Black);

            int x = 80;
            int y = 80;
            int lineHeight = 36;

            // Title
            spriteBatch.DrawString(hudFont, "PACMAN R.E.P.O", new Vector2(x, y), Color.Gold);

            y += lineHeight * 2;

            // Show current logged in user
            spriteBatch.DrawString(hudFont, "Logged in as: " + currentUsername, new Vector2(x, y), Color.White);
            y += lineHeight * 2;

            // Menu options
            spriteBatch.DrawString(hudFont, "[N] Start New Save", new Vector2(x, y), Color.LightGreen);
            y += lineHeight;

            spriteBatch.DrawString(hudFont, "[L] Load Existing Save", new Vector2(x, y), Color.LightBlue);
            y += lineHeight * 2;

            // Status message
            spriteBatch.DrawString(hudFont, mainMenuMessage, new Vector2(x, y), Color.White);
        }

        /// <summary>
        /// Draws the game over screen.
        /// </summary>
        private void DrawGameOverHUD()
        {
            string text = "GAME OVER\nThe Wraith caught you.\nPress ESC to quit.";

            Vector2 position = new Vector2(
                graphics.PreferredBackBufferWidth / 2f - 170,
                graphics.PreferredBackBufferHeight / 2f - 50
            );

            spriteBatch.DrawString(hudFont, text, position, Color.Red);
        }

        /// <summary>
        /// Draws the wraith enemy if it's visible to the player.
        /// Color changes based on wraith state (rage vs normal).
        /// </summary>
        private void DrawWraith()
        {
            int playerTileX = (int)((playerPosition.X + playerSize.X / 2f) / TileSize);
            int playerTileY = (int)((playerPosition.Y + playerSize.Y / 2f) / TileSize);

            int wraithTileX = (int)((wraithPosition.X + wraithSize.X / 2f) / TileSize);
            int wraithTileY = (int)((wraithPosition.Y + wraithSize.Y / 2f) / TileSize);

            // Check if wraith is within player's view radius
            bool isVisible = visibilitySystem.IsTileVisible(
                wraithTileX,
                wraithTileY,
                playerTileX,
                playerTileY
            );

            // Don't draw if not visible
            if (!isVisible)
            {
                return;
            }

            // Convert world position to screen position
            Rectangle wraithRectangle = new Rectangle(
                (int)(wraithPosition.X - cameraPosition.X),
                (int)(wraithPosition.Y - cameraPosition.Y),
                (int)wraithSize.X,
                (int)wraithSize.Y
            );

            // Red when raging, purple when normal
            Color color = wraith.State == WraithState.Rage
                ? Color.Red
                : Color.Purple;

            spriteBatch.Draw(pixel, wraithRectangle, color);
        }

        /// <summary>
        /// Draws the gameplay HUD showing stats, round info, and extraction progress.
        /// </summary>
        private void DrawGameplayHUD()
        {
            int x = 15;
            int y = 15;
            int lineHeight = 22;

            // Prepare HUD text
            string roundText = "Round: " + roundManager.CurrentRound;
            string moneyText = "Money: " + playerStats.Money;
            string hpText = "HP: " + playerStats.Health + "/" + playerStats.MaxHealth;
            string staminaText = "Stamina: " + ((int)playerStats.Stamina) + "/" + ((int)playerStats.MaxStamina);
            string weightText = "Weight: " + playerStats.CarriedWeight.ToString("0.0") + "/" + playerStats.MaxCarryWeight.ToString("0.0");
            string extractionText = "Extraction: " + extractionSystem.ExtractedValue + "/" + extractionSystem.RequiredValue;

            // Draw stat lines
            spriteBatch.DrawString(hudFont, roundText, new Vector2(x, y), Color.White);
            spriteBatch.DrawString(hudFont, moneyText, new Vector2(x, y + lineHeight), Color.White);
            spriteBatch.DrawString(hudFont, hpText, new Vector2(x, y + lineHeight * 2), Color.White);
            spriteBatch.DrawString(hudFont, staminaText, new Vector2(x, y + lineHeight * 3), Color.White);
            spriteBatch.DrawString(hudFont, weightText, new Vector2(x, y + lineHeight * 4), Color.White);
            spriteBatch.DrawString(hudFont, extractionText, new Vector2(x, y + lineHeight * 5), Color.White);

            // Controls hint at bottom of screen
            string controlsText = "WASD: Move | E: Interact | ESC: Quit";
            spriteBatch.DrawString(hudFont, controlsText, new Vector2(x, graphics.PreferredBackBufferHeight - 35), Color.LightGray);

            // Show extraction complete message
            if (extractionGoalComplete)
            {
                string returnText = "Extraction goal complete! Return to spawn.";
                spriteBatch.DrawString(hudFont, returnText, new Vector2(x, y + lineHeight * 7), Color.Gold);
            }
        }

        /// <summary>
        /// Draws the tile map with fog of war effects.
        /// Tiles are drawn with varying brightness based on distance from player,
        /// previously discovered tiles are dimly visible, unexplored tiles are black.
        /// </summary>
        private void DrawMap()
        {
            int playerTileX = (int)(playerPosition.X / TileSize);
            int playerTileY = (int)(playerPosition.Y / TileSize);

            // Draw every tile in the map
            for (int x = 0; x < tileMap.Width; x++)
            {
                for (int y = 0; y < tileMap.Height; y++)
                {
                    // Convert tile position to screen position
                    Rectangle tileRectangle = new Rectangle(
                        x * TileSize - (int)cameraPosition.X,
                        y * TileSize - (int)cameraPosition.Y,
                        TileSize,
                        TileSize
                    );

                    // Calculate brightness based on distance from player
                    float brightness = GetVisibilityBrightness(
                        tileX: x,
                        tileY: y,
                        playerTileX: playerTileX,
                        playerTileY: playerTileY
                    );

                    Tile tile = tileMap.Tiles[x, y];
                    Color baseColor = GetTileColor(tile.Type);

                    // Currently visible: draw at calculated brightness
                    if (brightness > 0f)
                    {
                        Color visibleColor = DarkenColor(baseColor, brightness);
                        spriteBatch.Draw(pixel, tileRectangle, visibleColor);
                        continue;
                    }

                    // Previously discovered but not currently visible: draw dimly
                    if (discoveredTiles[x, y])
                    {
                        Color rememberedColor = DarkenColor(baseColor, 0.12f);
                        spriteBatch.Draw(pixel, tileRectangle, rememberedColor);
                        continue;
                    }

                    // Never discovered: draw black (fog of war)
                    spriteBatch.Draw(pixel, tileRectangle, Color.Black);
                }
            }
        }

        /// <summary>
        /// Updates the discovered tiles array based on player's current view radius.
        /// Once discovered, tiles remain discovered for the rest of the round.
        /// </summary>
        private void UpdateDiscoveredTiles()
        {
            int playerTileX = (int)(playerPosition.X / TileSize);
            int playerTileY = (int)(playerPosition.Y / TileSize);

            // Check all tiles within view radius
            for (int x = playerTileX - ViewRadius; x <= playerTileX + ViewRadius; x++)
            {
                for (int y = playerTileY - ViewRadius; y <= playerTileY + ViewRadius; y++)
                {
                    // Out of bounds
                    if (x < 0 || x >= tileMap.Width || y < 0 || y >= tileMap.Height)
                    {
                        continue;
                    }

                    // Mark as discovered if visible
                    if (visibilitySystem.IsTileVisible(x, y, playerTileX, playerTileY))
                    {
                        discoveredTiles[x, y] = true;
                    }
                }
            }
        }

        /// <summary>
        /// Draws the shop interface showing available upgrades and their costs.
        /// </summary>
        private void DrawShopHUD()
        {
            // Dark background
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

            // Calculate upgrade costs
            int speedCost = shopManager.GetUpgradeCost(playerStats.SpeedLevel);
            int strengthCost = shopManager.GetUpgradeCost(playerStats.StrengthLevel);
            int staminaCost = shopManager.GetUpgradeCost(playerStats.StaminaLevel);
            int healthCost = shopManager.GetUpgradeCost(playerStats.HealthLevel);
            int healthPackCost = shopManager.GetHealthPackCost();

            // Title
            spriteBatch.DrawString(hudFont, "ITEM SHOP", new Vector2(x, y), Color.Gold);

            y += lineHeight * 2;

            // Player stats
            spriteBatch.DrawString(hudFont, "Money: " + playerStats.Money, new Vector2(x, y), Color.White);
            y += lineHeight;

            spriteBatch.DrawString(hudFont, "HP: " + playerStats.Health + "/" + playerStats.MaxHealth, new Vector2(x, y), Color.White);
            y += lineHeight * 2;

            // Shop items with costs and current levels
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

            // Continue prompt
            spriteBatch.DrawString(hudFont, "[Enter/Space] Start next round", new Vector2(x, y), Color.LightGreen);
        }

        /// <summary>
        /// Calculates the brightness/visibility of a tile based on its distance from the player.
        /// Uses Chebyshev distance (max of x and y distance).
        /// </summary>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <param name="playerTileX">Player's tile X coordinate.</param>
        /// <param name="playerTileY">Player's tile Y coordinate.</param>
        /// <returns>Brightness value from 0.0 (not visible) to 1.0 (fully visible).</returns>
        private float GetVisibilityBrightness(int tileX, int tileY, int playerTileX, int playerTileY)
        {
            int distanceX = Math.Abs(tileX - playerTileX);
            int distanceY = Math.Abs(tileY - playerTileY);

            // Use Chebyshev distance (max of x and y)
            int distance = Math.Max(distanceX, distanceY);

            if (distance > ViewRadius)
            {
                return 0f;
            }

            // Brightness decreases with distance
            switch (distance)
            {
                case 0:
                    return 1.0f;   // Player's tile: fully bright

                case 1:
                    return 0.9f;   // Adjacent tiles: very bright

                case 2:
                    return 0.75f;  // 2 tiles away: bright

                case 3:
                    return 0.55f;  // 3 tiles away: medium

                case 4:
                    return 0.35f;  // 4 tiles away: dim

                default:
                    return 0f;     // Beyond view radius: not visible
            }
        }

        /// <summary>
        /// Darkens a color by multiplying its RGB components by a brightness factor.
        /// </summary>
        /// <param name="color">The base color.</param>
        /// <param name="brightness">Brightness multiplier (0.0 = black, 1.0 = full color).</param>
        /// <returns>The darkened color.</returns>
        private Color DarkenColor(Color color, float brightness)
        {
            return new Color(
                (int)(color.R * brightness),
                (int)(color.G * brightness),
                (int)(color.B * brightness)
            );
        }

        #endregion

        #region Item and Interaction System

        /// <summary>
        /// Attempts to pick up an item from the current tile.
        /// Checks weight capacity before adding to inventory.
        /// </summary>
        /// <param name="currentTile">The tile containing the item.</param>
        private void TryPickupItem(Tile currentTile)
        {
            Item item = GenerateItemFromTile();

            bool added = inventory.AddItem(item);

            // Too heavy to carry
            if (!added)
            {
                Window.Title = "Item too heavy! Current weight: " +
                               playerStats.CarriedWeight + "/" + playerStats.MaxCarryWeight;
                return;
            }

            // Successfully picked up: convert tile to road
            currentTile.Type = TileType.Road;

            Window.Title = "Picked up " + item.Name +
                           " | Value: " + item.Value +
                           " | Weight: " + item.Weight +
                           " | Carrying: " + playerStats.CarriedWeight + "/" + playerStats.MaxCarryWeight;
        }

        /// <summary>
        /// Generates a random item with weight-based value.
        /// Heavier items are more valuable but also harder to carry.
        /// </summary>
        /// <returns>A newly generated item.</returns>
        private Item GenerateItemFromTile()
        {
            // Random weight between 1 and 5
            float weight = random.Next(1, 6);

            // Value scales with weight: weight * (20-40)
            int value = (int)(weight * random.Next(20, 41));

            // Assign name based on value
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

        /// <summary>
        /// Attempts to extract all items in the player's inventory at an extraction point.
        /// Converts item value to money and extraction progress.
        /// </summary>
        private void TryExtractItems()
        {
            // No items to extract
            if (inventory.Items.Count == 0)
            {
                Window.Title = "No items to extract. Required: " +
                               extractionSystem.ExtractedValue + "/" +
                               extractionSystem.RequiredValue;
                return;
            }

            int extractedThisTime = 0;

            // Create a copy of the items list to avoid modification during iteration
            List<Item> itemsToExtract = new List<Item>(inventory.Items);

            // Extract all items
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

            // Check if extraction goal is complete
            if (extractionSystem.IsGoalComplete())
            {
                extractionGoalComplete = true;

                Window.Title = "Extraction goal complete! Return to spawn and press E to finish the round. Money: " +
                               playerStats.Money;
            }
        }

        /// <summary>
        /// Attempts to finish the current round at the spawn point.
        /// Requires extraction goal to be complete and inventory to be empty.
        /// </summary>
        private void TryFinishRoundAtSpawn()
        {
            // Must complete extraction goal first
            if (!extractionGoalComplete)
            {
                Window.Title = "You need to extract more value first. Progress: " +
                               extractionSystem.ExtractedValue + "/" +
                               extractionSystem.RequiredValue;
                return;
            }

            // Must extract all items before finishing
            if (inventory.Items.Count > 0)
            {
                Window.Title = "You still have items. Extract them before finishing the round.";
                return;
            }

            FinishRound();
        }

        /// <summary>
        /// Finishes the current round, saves the game, and enters the shop.
        /// </summary>
        private void FinishRound()
        {
            SaveCurrentGame();

            gameStateManager.EnterShop();

            Window.Title = GetShopTitle("Round " + roundManager.CurrentRound + " complete!");
        }

        #endregion

        #region Shop System

        /// <summary>
        /// Updates the shop screen, handling upgrade purchases and round progression.
        /// </summary>
        /// <param name="keyboard">Current keyboard state.</param>
        private void UpdateShop(KeyboardState keyboard)
        {
            // Speed upgrade (1 key)
            if (WasAnyKeyPressed(keyboard, Keys.D1, Keys.NumPad1))
            {
                bool bought = shopManager.BuySpeedUpgrade(playerStats);

                Window.Title = bought
                    ? GetShopTitle("Bought Speed Upgrade.")
                    : GetShopTitle("Not enough money for Speed Upgrade.");
            }

            // Strength upgrade (2 key)
            if (WasAnyKeyPressed(keyboard, Keys.D2, Keys.NumPad2))
            {
                bool bought = shopManager.BuyStrengthUpgrade(playerStats);

                Window.Title = bought
                    ? GetShopTitle("Bought Strength Upgrade.")
                    : GetShopTitle("Not enough money for Strength Upgrade.");
            }

            // Stamina upgrade (3 key)
            if (WasAnyKeyPressed(keyboard, Keys.D3, Keys.NumPad3))
            {
                bool bought = shopManager.BuyStaminaUpgrade(playerStats);

                Window.Title = bought
                    ? GetShopTitle("Bought Stamina Upgrade.")
                    : GetShopTitle("Not enough money for Stamina Upgrade.");
            }

            // Health upgrade (4 key)
            if (WasAnyKeyPressed(keyboard, Keys.D4, Keys.NumPad4))
            {
                bool bought = shopManager.BuyHealthUpgrade(playerStats);

                Window.Title = bought
                    ? GetShopTitle("Bought Max Health Upgrade.")
                    : GetShopTitle("Not enough money for Max Health Upgrade.");
            }

            // Health pack (5 key)
            if (WasAnyKeyPressed(keyboard, Keys.D5, Keys.NumPad5))
            {
                bool bought = shopManager.BuyHealthPack(playerStats);

                Window.Title = bought
                    ? GetShopTitle("Bought Health Pack.")
                    : GetShopTitle("Cannot buy Health Pack. Either not enough money or already full HP.");
            }

            // Start next round
            if (WasAnyKeyPressed(keyboard, Keys.Enter, Keys.Space))
            {
                StartNextRoundFromShop();
            }
        }

        /// <summary>
        /// Generates a detailed shop window title showing all upgrade costs and player stats.
        /// </summary>
        /// <param name="message">A message to display at the start of the title.</param>
        /// <returns>Complete shop title string.</returns>
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

        /// <summary>
        /// Starts the next round after leaving the shop.
        /// Increments round counter, resets map and extraction system, and spawns new wraith.
        /// </summary>
        private void StartNextRoundFromShop()
        {
            // Increment round number
            roundManager.NextRound();

            // Reset extraction system for new round
            extractionSystem = new ExtractionSystem();
            extractionSystem.RequiredValue = roundManager.GetExtractionRequirement();

            extractionGoalComplete = false;

            // Clear inventory for new round
            inventory = new Inventory(playerStats);
            playerStats.CarriedWeight = 0f;

            // Load a fresh map
            tileMap = mapFileHandler.LoadMap(mapFilePath);
            discoveredTiles = new bool[tileMap.Width, tileMap.Height];

            playerPosition = FindSpawnPosition();

            // Spawn new wraith for this round
            wraith = new Wraith();
            wraithPosition = FindWraithSpawnPosition();
            wraithHasTargetTile = false;
            wraithDamageCooldown = 0f;

            // Return to gameplay
            gameStateManager.StartGame();

            // Save progress
            SaveCurrentGame();

            Window.Title = "Round " + roundManager.CurrentRound +
                           " started. Required extraction value: " +
                           extractionSystem.RequiredValue +
                           " | Money: " + playerStats.Money +
                           " | HP: " + playerStats.Health + "/" + playerStats.MaxHealth;
        }

        /// <summary>
        /// Handles player interaction (E key) with tiles.
        /// Routes to appropriate handler based on tile type (Item, Extraction, Spawn).
        /// </summary>
        /// <param name="keyboard">Current keyboard state.</param>
        private void HandleInteraction(KeyboardState keyboard)
        {
            bool pressedEThisFrame = WasKeyPressed(keyboard, Keys.E);

            if (!pressedEThisFrame)
            {
                return;
            }

            // Get player's current tile
            int playerTileX = (int)((playerPosition.X + playerSize.X / 2f) / TileSize);
            int playerTileY = (int)((playerPosition.Y + playerSize.Y / 2f) / TileSize);

            // Out of bounds
            if (playerTileX < 0 || playerTileX >= tileMap.Width ||
                playerTileY < 0 || playerTileY >= tileMap.Height)
            {
                return;
            }

            Tile currentTile = tileMap.Tiles[playerTileX, playerTileY];

            // Item tile: pick up the item
            if (currentTile.Type == TileType.Item)
            {
                TryPickupItem(currentTile);
                return;
            }

            // Extraction tile: extract all carried items
            if (currentTile.Type == TileType.Extraction)
            {
                TryExtractItems();
                return;
            }

            // Spawn tile: finish round if requirements are met
            if (currentTile.Type == TileType.Spawn)
            {
                TryFinishRoundAtSpawn();
                return;
            }

            // No interaction available on this tile
            Window.Title = "Nothing to interact with here.";
        }

        /// <summary>
        /// Returns the color for a given tile type.
        /// Used for rendering the map with distinct visual tiles.
        /// </summary>
        /// <param name="type">The tile type to get the color for.</param>
        /// <returns>The color representing this tile type.</returns>
        private Color GetTileColor(TileType type)
        {
            switch (type)
            {
                case TileType.Wall:
                    return new Color(35, 38, 48);      // Dark gray

                case TileType.Road:
                    return new Color(70, 76, 92);      // Medium gray

                case TileType.Spawn:
                    return new Color(70, 120, 90);     // Green

                case TileType.Extraction:
                    return new Color(180, 150, 60);    // Gold

                case TileType.Item:
                    return new Color(120, 90, 180);    // Purple

                default:
                    return Color.Magenta;              // Error color
            }
        }

        /// <summary>
        /// Draws the player character as a cyan rectangle.
        /// </summary>
        private void DrawPlayer()
        {
            // Convert world position to screen position
            Rectangle playerRectangle = new Rectangle(
                (int)(playerPosition.X - cameraPosition.X),
                (int)(playerPosition.Y - cameraPosition.Y),
                (int)playerSize.X,
                (int)playerSize.Y
            );

            spriteBatch.Draw(pixel, playerRectangle, Color.Cyan);
        }
        //
        #endregion
    }
}
   