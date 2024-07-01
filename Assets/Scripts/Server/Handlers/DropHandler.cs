namespace Server.Handlers
{
    using Game;
    using UnityEngine;

    public class DropHandler : IRequestHandler<DropProto>
    {
        public void Handler(ClientInfo info, DropProto packet)
        {
            Debug.Log("Drop: ()");

            if (GameManager.Instance.CurrentState is not GamePlayingState) return;
            if (info.Player.IsDead) return;

            info.Player.Drop();
        }
    }
}