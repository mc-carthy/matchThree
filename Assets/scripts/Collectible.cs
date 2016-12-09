using UnityEngine;

public class Collectible : GamePiece {

	[SerializeField]
	private bool clearedByBomb;
	public bool ClearedByBomb {
		get {
			return clearedByBomb;
		}
	}

	[SerializeField]
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
