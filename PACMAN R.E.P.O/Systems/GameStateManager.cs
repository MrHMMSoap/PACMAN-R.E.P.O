namespace PACMAN_R.E.P.O.Systems
{
    /// <summary>
    /// Manages game state transitions.
    /// </summary>
    public class GameStateManager
    {
        /// <summary>Gets the current game state.</summary>
        public GameState CurrentState { get; private set; } = GameState.Login;

        /// <summary>
        /// Changes the game to a new state.
        /// </summary>
        /// <param name="newState">The state to transition to.</param>
        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
        }

        /// <summary>
        /// Starts gameplay (transitions to Playing state).
        /// </summary>
        public void StartGame()
        {
            CurrentState = GameState.Playing;
        }

        /// <summary>
        /// Pauses the game if currently playing.
        /// </summary>
        public void PauseGame()
        {
            if (CurrentState == GameState.Playing)
            {
                CurrentState = GameState.Paused;
            }
        }

        /// <summary>
        /// Resumes the game if currently paused.
        /// </summary>
        public void ResumeGame()
        {
            if (CurrentState == GameState.Paused)
            {
                CurrentState = GameState.Playing;
            }
        }

        /// <summary>
        /// Transitions to the shop state.
        /// </summary>
        public void EnterShop()
        {
            CurrentState = GameState.Shop;
        }

        /// <summary>
        /// Transitions to the game over state.
        /// </summary>
        public void SetGameOver()
        {
            CurrentState = GameState.GameOver;
        }
    }
}
