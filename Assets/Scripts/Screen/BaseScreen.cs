using UnityEngine;
namespace Screen
{
    public abstract class BaseScreen : MonoBehaviour
    {
        public virtual void Show()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}