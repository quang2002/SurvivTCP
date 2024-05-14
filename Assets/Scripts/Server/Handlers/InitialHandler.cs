using System;
using System.Runtime.InteropServices;
using Game;
namespace Server.Handlers
{
    public class InitialHandler : INonSchemaRequestHandler
    {

        public unsafe int Handler(ClientInfo info, byte* packet)
        {
            var count = *(int*)packet;
            var name = Marshal.PtrToStringUTF8((IntPtr)(packet + sizeof(int)), count);

            if (GameManager.Instance.CurrentState is not GameLobbyState) goto exit;

            GameManager.Instance.RunOnMainThread(() =>
            {
                info.Player.Name = name;
                GameManager.Instance.LobbyScreen.UpdateClientInfos();
            });

            exit:
            return count + sizeof(int);
        }
    }
}