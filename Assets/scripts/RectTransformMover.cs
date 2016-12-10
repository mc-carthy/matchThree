using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class RectTransformMover : MonoBehaviour {

	[SerializeField]
	private Vector3 startPos;
	[SerializeField]
	private Vector3 onScreenPos;
	[SerializeField]
	private Vector3 endPos;

	private float timeToMove = 1f;

	private RectTransform rectXform;
	private bool isMoving;

	private void Awake () {
		rectXform = GetComponent<RectTransform>();
	}

	public void MoveOn () {
		Move(startPos, endPos, timeToMove);
	}

	public void MoveOff () {
		Move(endPos, startPos, timeToMove);
	}

	private void Move (Vector3 startPos, Vector3 endPos, float timeToMove) {
		if (isMoving) {
			StartCoroutine(MoveRoutine(startPos, endPos, timeToMove));
		}
	}

	private IEnumerator MoveRoutine (Vector3 startPos, Vector3 endPos, float timeToMove) {
		
		if (rectXform != null) {
			rectXform.anchoredPosition = startPos;
		}

		bool reachedDestination = false;
		float elapsedTime = 0f;
		isMoving = true;

		while (!reachedDestination) {
			if (Vector3.Distance(rectXform.anchoredPosition, endPos) < 0.01f) {
				reachedDestination = true;
				break;
			}

			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp01(elapsedTime / timeToMove);
			t = Ease(t);

			if (rectXform != null) {
				rectXform.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
				yield return null;
			}

		}
		isMoving = false;		
	}

	private float Ease (float x, float easeAmount = 1.5f) {
		easeAmount = Mathf.Clamp(easeAmount, 0, 2);
		float a = easeAmount + 1;
		return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
	}
}
