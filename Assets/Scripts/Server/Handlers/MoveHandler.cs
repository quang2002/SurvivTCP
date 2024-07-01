using Game;
using UnityEngine;
namespace Server.Handlers
{
    public class MoveHandler : IRequestHandler<MoveProto>
    {
        public void Handler(ClientInfo info, MoveProto packet)
        {
            Debug.Log($"Move: (${packet.X}, ${packet.Y})");

            if (GameManager.Instance.CurrentState is not GamePlayingState) return;
            if (info.Player.IsDead) return;

            info.Player.Direction = packet.Direction;
        }
    }
}