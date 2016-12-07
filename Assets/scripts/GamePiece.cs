using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {

	private int xIndex;
	private int yIndex;
	private bool isMoving;

	public void SetCoord (int x, int y) {
		xIndex = x;
		yIndex = y;
	}

	public void Move (int destX, int destY, float timeToMove) {
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
			float t = Mathf.Clamp(elapsedTime / timeToMove, 0, 1);
			transform.position = Vector3.Lerp(startPos, destination, t);
			yield return null;
		}
		isMoving = false;
	}
}
