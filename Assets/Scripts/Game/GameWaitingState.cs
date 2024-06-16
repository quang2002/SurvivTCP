namespace Game
{
    public class GameWaitingState : IGameState
    {

        public void Enter()
        {
            GameManager.Instance.GameScreen.Show();
            GameManager.Instance.TransitionTo<GamePlayingState>();
            GameManager.Instance.RandomSpawnWeapons(20);
            GameManager.Instance.RandomSpawnRocks(30);
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