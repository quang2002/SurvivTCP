using System;
namespace Game
{
    public class GamePlayingState : IGameState
    {

        public void Enter()
        {
            GameManager.Instance.GameScreen.Show();
            GameManager.Instance.GameScreen.RemainingTime = TimeSpan.FromMinutes(10);
            GameManager.Instance.GameScreen.OnTimeout += this.OnTimeout;
        }

        public void Exit()
        {
            GameManager.Instance.GameScreen.Hide();
            GameManager.Instance.GameScreen.OnTimeout -= this.OnTimeout;
        }

        public void Tick()
        {
        }

        private void OnTimeout()
        {
            GameManager.Instance.TransitionTo<GameEndState>();
        }
    }
}