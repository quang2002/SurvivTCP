namespace Game
{
    public class GameEndState : IGameState
    {

        public void Enter()
        {
            GameManager.Instance.ClearWeapons();
        }

        public void Exit()
        {
        }

        public void Tick()
        {
        }
    }
}