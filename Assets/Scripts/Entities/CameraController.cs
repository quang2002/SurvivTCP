using Game;
using UnityEngine;
namespace Entities
{
    public class CameraController : MonoBehaviour
    {

        [field: SerializeField]
        public Transform FollowTarget { get; set; }

        private Camera Camera { get; set; }
        private float TargetSize { get; set; }
        private Vector2 TargetPos { get; set; }

        private int FollowIndex { get; set; }

        private void Awake()
        {
            this.Camera = this.gameObject.GetComponent<Camera>();
        }

        private void Update()
        {
            // Follow
            if (this.FollowTarget)
            {
                this.TargetSize = 10f;
                this.TargetPos = this.FollowTarget.position;
            }

            GameManager.Instance.GameScreen.TextName.text = this.FollowTarget ? this.FollowTarget.GetComponent<Player>().Name : "Player ???";

            // Move
            var hor = Input.GetAxis("Horizontal");
            var ver = Input.GetAxis("Vertical");
            var speed = 200 / this.Camera.orthographicSize;

            this.TargetPos += new Vector2(hor, ver) * (Time.deltaTime * speed);

            // Zoom
            var scr = Input.mouseScrollDelta.y;
            this.TargetSize = Mathf.Clamp(
                this.TargetSize - scr * Time.deltaTime * 100,
                5f,
                50f
            );

            // Cancel Target
            if (Mathf.Abs(hor) > 0.5f || Mathf.Abs(ver) > 0.5f || Mathf.Abs(scr) > 0.5f)
            {
                this.FollowTarget = null;
            }

            // Apply
            this.Camera.transform.position = Vector3.Lerp(
                this.Camera.transform.position,
                (Vector3)this.TargetPos + Vector3.forward * this.Camera.transform.position.z,
                Time.deltaTime * 10
            );

            this.Camera.orthographicSize = Mathf.Lerp(
                this.Camera.orthographicSize,
                this.TargetSize,
                Time.deltaTime * 10
            );
        }

        public void Next()
        {
            var players = GameManager.Instance.TcpServer.PublicClientInfos;

            if (++this.FollowIndex >= players.Count) this.FollowIndex = 0;

            var player = players[this.FollowIndex % players.Count].Player;
            this.FollowTarget = player.transform;
        }

        public void Prev()
        {
            var players = GameManager.Instance.TcpServer.PublicClientInfos;

            if (--this.FollowIndex < 0) this.FollowIndex = players.Count - 1;

            var player = players[this.FollowIndex % players.Count].Player;
            this.FollowTarget = player.transform;
        }
    }
}