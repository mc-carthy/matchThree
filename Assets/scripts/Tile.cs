using UnityEngine;

public class Tile : MonoBehaviour {

	private int xIndex;
	public int XIndex {
		get {
			return xIndex;
		}
	}

	private int yIndex;
	public int YIndex {
		get {
			return yIndex;
		}
	}
	
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
