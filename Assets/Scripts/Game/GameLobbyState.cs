namespace Game
{
    public class GameLobbyState : IGameState
    {

        public void Enter()
        {
            GameManager.Instance.TcpServer.IsOpenForConnection = true;
            GameManager.Instance.LobbyScreen.Show();
            GameManager.Instance.TcpServer.OnConnected += this.OnConnected;
            GameManager.Instance.TcpServer.OnDisconnected += this.OnDisconnected;
            GameManager.Instance.LobbyScreen.PlayBtn.onClick.AddListener(this.OnClickPlayBtn);
        }

        public void Exit()
        {
            GameManager.Instance.TcpServer.IsOpenForConnection = false;
            GameManager.Instance.LobbyScreen.Hide();
            GameManager.Instance.TcpServer.OnConnected -= this.OnConnected;
            GameManager.Instance.TcpServer.OnDisconnected -= this.OnDisconnected;
            GameManager.Instance.LobbyScreen.PlayBtn.onClick.RemoveListener(this.OnClickPlayBtn);
        }

        public void Tick()
        {
        }

        private void OnClickPlayBtn()
        {
            GameManager.Instance.TransitionTo<GameWaitingState>();
        }

        private void OnDisconnected(ClientInfo info)
        {
            GameManager.Instance.RunOnMainThread(GameManager.Instance.LobbyScreen.UpdateClientInfos);
        }

        private void OnConnected(ClientInfo info)
        {
            GameManager.Instance.RunOnMainThread(GameManager.Instance.LobbyScreen.UpdateClientInfos);
        }
    }
}