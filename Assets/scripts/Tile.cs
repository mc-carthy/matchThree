using UnityEngine;

public class Tile : MonoBehaviour {

	[SerializeField]
	private int xIndex;
	[SerializeField]
	private int yIndex;

	private Board board;

	private void OnMouseDown () {
		if (board) {
			board.ClickTile(this);
		}
	}

	private void OnMouseEnter () {
		if (board) {
			board.DragToTile(this);
		}
	}

	private void OnMouseUp () {
		if (board) {
			board.ReleaseTile();
		}
	}

	public void Init (int x, int y, Board board) {
		xIndex = x;
		yIndex = y;
		this.board = board;
	}

}
