using System;
using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;
namespace Screen
{
    public class GameScreen : BaseScreen
    {
        [field: SerializeField]
        public Button PrevBtn { get; private set; }

        [field: SerializeField]
        public Button NextBtn { get; private set; }

        [field: SerializeField]
        public TMP_Text TextName { get; private set; }

        [field: SerializeField]
        public TMP_Text TextTime { get; private set; }

        [field: SerializeField]
        public GameObject RankingContainer { get; private set; }

        [field: SerializeField]
        public Transform RankingContentTf { get; private set; }

        [field: SerializeField]
        public GameScreenRankingRow GameScreenRankingRowPrefab { get; private set; }

        public TimeSpan RemainingTime { get; set; }

        private List<GameScreenRankingRow> Rows { get; set; } = new ();

        private void Awake()
        {
            this.PrevBtn.onClick.RemoveAllListeners();
            this.NextBtn.onClick.RemoveAllListeners();

            this.PrevBtn.onClick.AddListener(this.OnClickPrevBtn);
            this.NextBtn.onClick.AddListener(this.OnClickNextBtn);

            // Duplicate rows
            for (var i = 0; i < 15; i++)
            {
                var row = Instantiate(
                    this.GameScreenRankingRowPrefab,
                    this.RankingContentTf
                );

                this.Rows.Add(row);
            }
        }

        private void Update()
        {
            if (this.RemainingTime.TotalSeconds > 0)
            {
                this.RemainingTime = this.RemainingTime.Subtract(TimeSpan.FromSeconds(Time.deltaTime));
                var ts = this.RemainingTime;
                this.TextTime.text = $"{ts.Minutes:00}:{ts.Seconds:00}";

                if (this.RemainingTime.TotalSeconds <= 0)
                {
                    this.OnTimeout?.Invoke();
                }
            }

            if (Input.GetButtonUp("Jump"))
            {
                this.RankingContainer.SetActive(!this.RankingContainer.activeSelf);
            }

            {
                var clients = GameManager.Instance.TcpServer.PublicClientInfos;

                for (var i = 0; i < this.Rows.Count; i++)
                {
                    if (i < clients.Count)
                    {
                        this.Rows[i].gameObject.SetActive(true);
                        this.Rows[i].Name = clients[i].Player.Name;
                        this.Rows[i].Point = clients[i].Player.Point;
                        continue;
                    }

                    this.Rows[i].gameObject.SetActive(false);
                }
            }
        }

        public event Action OnTimeout;

        private void OnClickNextBtn()
        {
            GameManager.Instance.CameraController.Next();
        }

        private void OnClickPrevBtn()
        {
            GameManager.Instance.CameraController.Prev();
        }
    }
}