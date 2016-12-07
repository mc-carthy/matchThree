using UnityEngine;

public class Tile : MonoBehaviour {

	[SerializeField]
	private int xIndex;
	[SerializeField]
	private int yIndex;

	private Board board;

	private void Start () {

	}

	public void Init (int x, int y, Board board) {
		xIndex = x;
		yIndex = y;
		this.board = board;
	}

}
