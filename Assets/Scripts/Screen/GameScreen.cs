using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

        private void Awake()
        {
            this.PrevBtn.onClick.RemoveAllListeners();
            this.NextBtn.onClick.RemoveAllListeners();

            this.PrevBtn.onClick.AddListener(this.OnClickPrevBtn);
            this.NextBtn.onClick.AddListener(this.OnClickNextBtn);
        }

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