using UnityEngine;

public class Collectible : GamePiece {

	private bool clearedByBomb;
	public bool ClearedByBomb {
		get {
			return clearedByBomb;
		}
	}

	private bool clearedAtBottom;
	public bool ClearedAtBottom {
		get {
			return clearedAtBottom;
		}
	}

	private void Start () {
		MatchVal = MatchValue.None;
	}
}
