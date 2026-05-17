namespace PACMAN_R.E.P.O.Systems
{
    public class GameStateManager
    {
        public GameState CurrentState { get; private set; } = GameState.Login;

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
        }

        public void StartGame()
        {
            CurrentState = GameState.Playing;
        }

        public void PauseGame()
        {
            if (CurrentState == GameState.Playing)
            {
                CurrentState = GameState.Paused;
            }
        }

        public void ResumeGame()
        {
            if (CurrentState == GameState.Paused)
            {
                CurrentState = GameState.Playing;
            }
        }

        public void EnterShop()
        {
            CurrentState = GameState.Shop;
        }

        public void SetGameOver()
        {
            CurrentState = GameState.GameOver;
        }
    }
}
