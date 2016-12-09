using UnityEngine;

public class CameraController : MonoBehaviour {

	[SerializeField]
	private Board board;
	private int boardHeight;
	private int boardWidth;
	private float borderSize = 1f;

	private void Start () {
		boardHeight = board.Height;
		boardWidth = board.Width;

		transform.position = new Vector3(((float)boardWidth - 1) / 2, ((float)boardHeight - 1) / 2, -10);

		GetCameraSize();
	}

	private void GetCameraSize () {
		float aspectRatio = (float)Screen.width / (float)Screen.height;
		float verticalSize = (float)boardHeight / 2 + (float)borderSize;
		float horizontalSize = ((float)boardWidth / 2 + (float)borderSize) / aspectRatio;

		Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;
	}
}
