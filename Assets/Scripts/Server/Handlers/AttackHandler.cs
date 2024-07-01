using Game;
using UnityEngine;
namespace Server.Handlers
{
    public class AttackHandler : IRequestHandler<AttackProto>
    {

        public void Handler(ClientInfo info, AttackProto packet)
        {
            Debug.Log("Attack: ()");

            if (GameManager.Instance.CurrentState is not GamePlayingState) return;
            if (info.Player.IsDead) return;
            
            info.Player.Attack();
        }
    }
}