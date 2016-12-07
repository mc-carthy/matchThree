using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {

	private int xIndex;
	private int yIndex;
	private bool isMoving;
	[Range(0, 2)]
	private float easeAmount = 1.5f;

	private void Update () {
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			Move(xIndex - 1, yIndex);
		}
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			Move(xIndex + 1, yIndex);
		}
	}
	
	public void SetCoord (int x, int y) {
		xIndex = x;
		yIndex = y;
	}

	public void Move (int destX, int destY, float timeToMove = 0.5f) {
		if (!isMoving) {
			StartCoroutine(MoveRoutine(new Vector3(destX, destY, 0), timeToMove));
		}
	}

	private IEnumerator MoveRoutine (Vector3 destination, float timeToMove) {
		Vector3 startPos = transform.position;
		bool isAtDestination = false;
		float elapsedTime = 0;
		isMoving = true;
		while (!isAtDestination) {
			if (Vector3.Distance(transform.position, destination) < 0.01f) {
				isAtDestination = true;
				transform.position = destination;
				SetCoord((int)destination.x, (int)destination.y);
				break;
			}
			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp01(elapsedTime / timeToMove);
			t = Ease(t);


			transform.position = Vector3.Lerp(startPos, destination, t);
			yield return null;
		}
		isMoving = false;
	}

	private float Ease (float x) {
		float a = easeAmount + 1;
		return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
	}
}
