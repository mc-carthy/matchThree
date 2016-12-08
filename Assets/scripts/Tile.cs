using UnityEngine;
using System.Collections;

public enum TileType {
	Normal,
	Obstacle,
	Breakable
}

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour {

	[SerializeField]
	private TileType tType; 
	public TileType TType {
		get {
			return tType;
		}
	}

	[SerializeField]
	private int breakableValue = 0;
	public int BreakableValue {
		get {
			return breakableValue;
		}
	}

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

	[SerializeField]
	private Sprite[] breakableSprites;

	private Board board;
	private SpriteRenderer spriteRenderer;
	private Color normalColor;

	private void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void Init (int x, int y, Board board) {
		xIndex = x;
		yIndex = y;
		this.board = board;

		if (tType == TileType.Breakable) {
			if (breakableSprites[breakableValue] != null) {
				spriteRenderer.sprite = breakableSprites[breakableValue];
			}
		}
	}

	public void BreakTile () {
		if (tType != TileType.Breakable) {
			return;
		}

		StartCoroutine(BreakTileRoutine());
	}

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

	private IEnumerator BreakTileRoutine () {
		breakableValue = Mathf.Clamp(breakableValue--, 0, breakableValue);
		yield return new WaitForSeconds(0.25f);

		if (breakableSprites[breakableValue] != null) {
			spriteRenderer.sprite = breakableSprites[breakableValue];
		}

		if (breakableValue == 0) {
			tType = TileType.Normal;
			spriteRenderer.color = normalColor;
		}
	}

}
