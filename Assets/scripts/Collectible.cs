using UnityEngine;

public class Collectible : GamePiece {

	private bool clearedByBomb;
	private bool clearedAtBottom;

	private void Start () {
		MatchVal = MatchValue.None;
	}
}
