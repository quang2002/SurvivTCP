namespace Game
{
    public class GamePlayingState : IGameState
    {

        public void Enter()
        {
            GameManager.Instance.GameScreen.Show();
        }

        public void Exit()
        {
            GameManager.Instance.GameScreen.Hide();
        }

        public void Tick()
        {
        }
    }
}