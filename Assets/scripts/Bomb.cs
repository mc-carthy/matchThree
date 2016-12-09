using UnityEngine;

public enum BombType {
	None,
	Column,
	Row,
	Adjacent,
	Color
}

public class Bomb : GamePiece {

	[SerializeField]
	private BombType bType;
	public BombType BType {
		get {
			return bType;
		}
	}

}
