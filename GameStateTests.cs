using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O;
using PACMAN_R.E.P.O.Systems;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class GameStateTests
    {
        [TestMethod]
        public void GameStateManager_ShouldStartInLoginState()
        {
            GameStateManager gameStateManager = new GameStateManager();

            Assert.AreEqual(GameState.Login, gameStateManager.CurrentState);
        }

        [TestMethod]
        public void ChangeState_ShouldChangeToMainMenu()
        {
            GameStateManager gameStateManager = new GameStateManager();

            gameStateManager.ChangeState(GameState.MainMenu);

            Assert.AreEqual(GameState.MainMenu, gameStateManager.CurrentState);
        }

        [TestMethod]
        public void StartGame_ShouldChangeStateToPlaying()
        {
            GameStateManager gameStateManager = new GameStateManager();

            gameStateManager.StartGame();

            Assert.AreEqual(GameState.Playing, gameStateManager.CurrentState);
        }

        [TestMethod]
        public void PauseGame_ShouldChangeStateToPaused_WhenCurrentlyPlaying()
        {
            GameStateManager gameStateManager = new GameStateManager();

            gameStateManager.StartGame();
            gameStateManager.PauseGame();

            Assert.AreEqual(GameState.Paused, gameStateManager.CurrentState);
        }

        [TestMethod]
        public void PauseGame_ShouldNotPause_WhenNotPlaying()
        {
            GameStateManager gameStateManager = new GameStateManager();

            gameStateManager.ChangeState(GameState.MainMenu);
            gameStateManager.PauseGame();

            Assert.AreEqual(GameState.MainMenu, gameStateManager.CurrentState);
        }

        [TestMethod]
        public void ResumeGame_ShouldChangeStateToPlaying_WhenPaused()
        {
            GameStateManager gameStateManager = new GameStateManager();

            gameStateManager.StartGame();
            gameStateManager.PauseGame();
            gameStateManager.ResumeGame();

            Assert.AreEqual(GameState.Playing, gameStateManager.CurrentState);
        }

        [TestMethod]
        public void EnterShop_ShouldChangeStateToShop()
        {
            GameStateManager gameStateManager = new GameStateManager();

            gameStateManager.EnterShop();

            Assert.AreEqual(GameState.Shop, gameStateManager.CurrentState);
        }

        [TestMethod]
        public void GameOver_ShouldChangeStateToGameOver()
        {
            GameStateManager gameStateManager = new GameStateManager();

            gameStateManager.SetGameOver();

            Assert.AreEqual(GameState.GameOver, gameStateManager.CurrentState);
        }
    }
}
