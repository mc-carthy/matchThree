using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Board : MonoBehaviour {

	[SerializeField]
	private int height;
	public int Height {
		get {
			return height;
		}
	}

	[SerializeField]
	private int width;
	public int Width {
		get {
			return width;
		}
	}

	[SerializeField]
	private GameObject tilePrefab;
	[SerializeField]
	private GameObject[] gamePiecePrefabs;

	private Tile[,] allTiles;
	private GamePiece[,] allGamePieces;
	private Tile clickedTile;
	private Tile targetTile;
	private float swapTime = 0.5f;

	private void Start () {
		allTiles = new Tile[width, height];
		allGamePieces = new GamePiece[width, height];
		SetupTiles();
		FillBoard();
	}

	public void PlaceGamePiece (GamePiece piece, int x, int y) {
		if (piece == null) {
			Debug.LogWarning("BOARD: Invalid GamePiece!");
			return;
		}

		piece.transform.position = new Vector3(x, y, 0);
		piece.transform.rotation = Quaternion.identity;
		if (IsWithinBounds(x, y)) {
			allGamePieces[x, y] = piece;
		}
		piece.SetCoord(x, y);
	}

	private void SetupTiles () {
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				GameObject newTile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
				newTile.name = "Tile (" + i + ", " + j + ")";
				newTile.transform.parent = transform;
				allTiles[i, j] = newTile.GetComponent<Tile>();
				allTiles[i, j].Init(i, j, this);	
			}
		}
	}

	private GameObject GetRandomGamePiece () {
		int randomIndex = Random.Range(0, gamePiecePrefabs.Length);

		if (gamePiecePrefabs[randomIndex] == null) {
			Debug.LogWarning("BOARD: " + randomIndex + "does not contain a valid GamePiece prefab!");
		}

		return gamePiecePrefabs[randomIndex];
	}

	private void FillBoard () {
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				FillRandomAt(i, j);
			}
		}

		bool isFilled = false;
		int maxIterations = 100;
		int currentInteration = 0;
		while (!isFilled) {
			List<GamePiece> matches = FindAllMatches();
			if (matches.Count == 0) {
				isFilled = true;
				break;
			} else {
				ReplaceWithRandom(matches);
			}

			if (currentInteration > maxIterations) {
				isFilled = true;
				Debug.Log("BOARD: Could not generate board with no matches");
			}
			currentInteration++;
		}
	}

	private void FillRandomAt (int x, int y) {
		GameObject randomPiece = Instantiate(GetRandomGamePiece(), Vector3.zero, Quaternion.identity) as GameObject;
		
		if (randomPiece != null) {
			randomPiece.GetComponent<GamePiece>().Init(this);
			PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), x, y);
			randomPiece.transform.parent = transform;
		}
	}

	private void ReplaceWithRandom(List<GamePiece> gamepieces) {
		foreach (GamePiece piece in gamepieces) {
			ClearPieceAt(piece.XIndex, piece.YIndex);
			FillRandomAt(piece.XIndex, piece.YIndex);
		}
	}

	public void ClickTile (Tile tile) {
		if (clickedTile == null) {
			clickedTile = tile;
		}
	}

	public void DragToTile (Tile tile) {
		if (clickedTile != null && IsNextTo(tile, clickedTile)) {
			targetTile = tile;
		}
	}

	public void ReleaseTile () {
		if (clickedTile && targetTile) {
			SwitchTiles(clickedTile, targetTile);
		}
		this.clickedTile = null;
		this.targetTile = null;
	}

	private void SwitchTiles (Tile clickedTile, Tile targetTile) {
		StartCoroutine(SwitchTilesRoutine(clickedTile, targetTile));
	}

	private IEnumerator SwitchTilesRoutine (Tile clickedTile, Tile targetTile) {
		GamePiece clickedPiece = allGamePieces[clickedTile.XIndex, clickedTile.YIndex];
		GamePiece targetPiece = allGamePieces[targetTile.XIndex, targetTile.YIndex];

		if (clickedPiece != null && targetPiece != null) {

			clickedPiece.Move(targetTile.XIndex, targetTile.YIndex, swapTime);
			targetPiece.Move(clickedTile.XIndex, clickedTile.YIndex, swapTime);

			yield return new WaitForSeconds(swapTime);

			List<GamePiece> clickedPieceMatches = FindMatchesAt(clickedTile.XIndex, clickedTile.YIndex);
			List<GamePiece> targetTileMatches = FindMatchesAt(targetTile.XIndex, targetTile.YIndex);

			if (targetTileMatches.Count == 0 && clickedPieceMatches.Count == 0) {
				clickedPiece.Move(clickedTile.XIndex, clickedTile.YIndex, swapTime);
				targetPiece.Move(targetTile.XIndex, targetTile.YIndex, swapTime);
			} else {
				yield return new WaitForSeconds(swapTime);

				ClearPieceAt(clickedPieceMatches);
				ClearPieceAt(targetTileMatches);
				// HighlightMatchesAt(clickedTile.XIndex, clickedTile.YIndex);
				// HighlightMatchesAt(targetTile.XIndex, targetTile.YIndex);
			}
		}
	}

	private bool IsWithinBounds (int x, int y) {
		return (x >= 0 && x < width && y >= 0 && y < height);
	}

	private bool IsNextTo (Tile start, Tile end) {
		return (Mathf.Abs(start.XIndex - end.XIndex) + Mathf.Abs(start.YIndex - end.YIndex)) == 1;
	}

	private List<GamePiece> FindMatches (int startX, int startY, Vector2 searchDirection, int minLength = 3) {
		List<GamePiece> matches = new List<GamePiece>();
		GamePiece startPiece = null;

		if (IsWithinBounds(startX, startY)) {
			startPiece = allGamePieces[startX, startY];
		}

		if (startPiece) {
			matches.Add(startPiece);
		} else {
			return null;
		}

		int nextX;
		int nextY;

		int maxValue = (width > height) ? width : height;

		for (int i = 1; i < maxValue - 1; i++) {
			nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
			nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

			if (!IsWithinBounds(nextX, nextY)) {
				break;
			}

			GamePiece nextPiece = allGamePieces[nextX, nextY];

			if (nextPiece == null) {
				break;
			} else {
				if (nextPiece.MatchVal == startPiece.MatchVal && !matches.Contains(nextPiece)) {
					matches.Add(nextPiece);
				} else {
					break;
				}
			}

		}

		return (matches.Count >= minLength) ? matches : null;
	}

	private List<GamePiece> FindVerticalMatches (int startX, int startY, int minLength = 3) {
		List<GamePiece> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 2);
		List<GamePiece> downwardMatches = FindMatches(startX, startY, new Vector2(0, -1), 2);

		if (upwardMatches == null) {
			upwardMatches = new List<GamePiece>();
		}

		if (downwardMatches == null) {
			downwardMatches = new List<GamePiece>();
		}

		List<GamePiece> combinedMatches = upwardMatches.Union(downwardMatches).ToList();

		return (combinedMatches.Count >= minLength) ? combinedMatches : null;
	}

	private List<GamePiece> FindHorizontalMatches (int startX, int startY, int minLength = 3) {
		List<GamePiece> rightMatches = FindMatches(startX, startY, new Vector2(1, 0), 2);
		List<GamePiece> leftWatches = FindMatches(startX, startY, new Vector2(-1, 0), 2);

		if (rightMatches == null) {
			rightMatches = new List<GamePiece>();
		}

		if (leftWatches == null) {
			leftWatches = new List<GamePiece>();
		}

		List<GamePiece> combinedMatches = rightMatches.Union(leftWatches).ToList();

		return (combinedMatches.Count >= minLength) ? combinedMatches : null;
	}

	private void HighlightMatches () {
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				HighlightMatchesAt(i, j);
			}
		}
	}

	private List<GamePiece> FindMatchesAt(int x, int y, int minLength = 3) {
		List<GamePiece> horMatches = FindHorizontalMatches(x, y, minLength);
		List<GamePiece> verMatches = FindVerticalMatches(x, y, minLength);

		if (horMatches == null) {
			horMatches = new List<GamePiece>();
		}
		if (verMatches == null) {
			verMatches = new List<GamePiece>();
		}

		List<GamePiece> combinedMatches = horMatches.Union(verMatches).ToList();

		return combinedMatches;
	}

	private List<GamePiece> FindAllMatches () {
		List<GamePiece> combinedMatches = new List<GamePiece>();

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				List<GamePiece> matches = FindMatchesAt(i, j);
				combinedMatches = combinedMatches.Union(matches).ToList();
			}
		}

		return combinedMatches;
	}

	private void HighlightMatchesAt (int x, int y) {
		HighlightTileOff(x, y);

		List<GamePiece> combinedMatches = FindMatchesAt(x, y);

		if (combinedMatches.Count > 0) {
			foreach(GamePiece piece in combinedMatches) {
				HighlightTileOn(piece.XIndex, piece.YIndex, piece.GetComponent<SpriteRenderer>().color);
			}
		}
	}

	private void HighlightTileOff (int x, int y) {
		SpriteRenderer spriteRenderer = allTiles[x, y].GetComponent<SpriteRenderer>();
		spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
	}

	private void HighlightTileOn (int x, int y, Color col) {
		SpriteRenderer spriteRenderer = allTiles[x, y].GetComponent<SpriteRenderer>();
		spriteRenderer.color = col;
	}

	private void ClearPieceAt (int x, int y) {
		GamePiece pieceToClear = allGamePieces[x, y];

		if (pieceToClear) {
			allGamePieces[x, y] = null;
			Destroy(pieceToClear.gameObject);
		}

		HighlightTileOff(x, y);
	}

	private void ClearBoard () {
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				ClearPieceAt(i, j);
			}
		}
	}

	private void ClearPieceAt(List<GamePiece> gamepieces) {
		foreach (GamePiece piece in gamepieces) {
			ClearPieceAt(piece.XIndex, piece.YIndex);
		}
	}
}
