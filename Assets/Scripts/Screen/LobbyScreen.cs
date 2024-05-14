using System.Collections.Generic;
using System.Linq;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Screen
{
    public class LobbyScreen : BaseScreen
    {
        [field: SerializeField]
        public Button PlayBtn { get; private set; }

        [field: SerializeField]
        public TMP_Text[] TextTeams { get; private set; }

        public void SetTeamNames(IList<string> names)
        {
            for (var i = 0; i < this.TextTeams.Length; i++)
            {
                if (i >= names.Count)
                {
                    this.TextTeams[i].gameObject.SetActive(false);
                    continue;
                }

                this.TextTeams[i].gameObject.SetActive(true);
                this.TextTeams[i].text = names[i];
            }
        }

        public override void Show()
        {
            this.SetTeamNames(new List<string>());
            base.Show();
        }

        public override void Hide()
        {
            this.SetTeamNames(new List<string>());
            base.Hide();
        }

        public void UpdateClientInfos()
        {
            this.SetTeamNames(
                GameManager.Instance
                    .TcpServer
                    .PublicClientInfos
                    .Select(info => info.Player ? info.Player.Name : "")
                    .ToList()
            );
        }
    }
}