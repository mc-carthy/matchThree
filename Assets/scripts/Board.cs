using UnityEngine;
using UnityEngine.Assertions;
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
	private GameObject tileNormalPrefab;	
	[SerializeField]
	private GameObject tileObstaclePrefab;
	[SerializeField]
	private GameObject[] gamePiecePrefabs;

	public StartingTile[] startingTiles;

	private Tile[,] allTiles;
	private GamePiece[,] allGamePieces;
	private Tile clickedTile;
	private Tile targetTile;
	private ParticleManager particleManager;
	private float swapTime = 0.5f;
	private bool isPlayerInputEnabled = true;

	[System.Serializable]
	public class StartingTile {
		[SerializeField]
		private GameObject tilePrefab;
		public GameObject TilePrefab {
			get {
				return tilePrefab;
			}
		}

		[SerializeField]
		private int x;
		public int X {
			get {
				return x;
			}
		}

		[SerializeField]
		private int y;
		public int Y {
			get {
				return y;
			}
		}
		
		[SerializeField]
		private int z;
		public int Z {
			get {
				return z;
			}
		}
		
	}

	private void Start () {
		allTiles = new Tile[width, height];
		allGamePieces = new GamePiece[width, height];
		SetupTiles();
		FillBoard(15, 0.5f);
		particleManager = GameObject.FindGameObjectWithTag("particleManager").GetComponent<ParticleManager>();
		Assert.IsNotNull(particleManager);
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
		foreach (StartingTile sTile in startingTiles) {
			if (sTile != null) {
				MakeTile(sTile.TilePrefab, sTile.X, sTile.Y, sTile.Z);
			}
		}

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (allTiles[i, j] == null) {
					MakeTile(tileNormalPrefab, i, j);
				}
			}
		}
	}

	private void MakeTile (GameObject prefab, int x, int y, int z = 0) {
		if (prefab != null) {
			GameObject newTile = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
			newTile.name = "Tile (" + x + ", " + y + ")";
			newTile.transform.parent = transform;
			allTiles[x, y] = newTile.GetComponent<Tile>();
			allTiles[x,y ].Init(x, y, this);	
		}
	}

	private GameObject GetRandomGamePiece () {
		int randomIndex = Random.Range(0, gamePiecePrefabs.Length);

		if (gamePiecePrefabs[randomIndex] == null) {
			Debug.LogWarning("BOARD: " + randomIndex + "does not contain a valid GamePiece prefab!");
		}

		return gamePiecePrefabs[randomIndex];
	}

	private void FillBoard (int falseYOffset = 0, float moveTime = 0.1f) {

		List<GamePiece> addedPieces = new List<GamePiece>();

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (allGamePieces[i, j] == null && allTiles[i, j].TType != TileType.Obstacle) {
					GamePiece piece = (falseYOffset == 0) ? FillRandomAt(i, j) : FillRandomAt(i, j, falseYOffset, moveTime);
					addedPieces.Add(piece);
				}
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
				matches = matches.Intersect(addedPieces).ToList();
				if (falseYOffset == 0) {
					ReplaceWithRandom(matches);
				} else {
					ReplaceWithRandom(matches, falseYOffset, moveTime);
				}
			}

			if (currentInteration > maxIterations) {
				isFilled = true;
				Debug.Log("BOARD: Could not generate board with no matches");
			}
			currentInteration++;
		}
	}

	private GamePiece FillRandomAt (int x, int y, int falseYOffset = 0, float moveTime = 0.1f) {
		GameObject randomPiece = Instantiate(GetRandomGamePiece(), Vector3.zero, Quaternion.identity) as GameObject;
		
		if (randomPiece != null) {
			randomPiece.GetComponent<GamePiece>().Init(this);
			PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), x, y);
			if (falseYOffset != 0) {
				randomPiece.transform.position = new Vector3(x, y + falseYOffset, 0);
				randomPiece.GetComponent<GamePiece>().Move(x, y, moveTime);
			}
			randomPiece.transform.parent = transform;
		}

		return randomPiece.GetComponent<GamePiece>();
	}

	private void ReplaceWithRandom(List<GamePiece> gamePieces, int falseYOffset = 0, float moveTime = 0.1f) {
		foreach (GamePiece piece in gamePieces) {
			ClearPieceAt(piece.XIndex, piece.YIndex);
			if (falseYOffset == 0) {
				FillRandomAt(piece.XIndex, piece.YIndex);
			} else {
				FillRandomAt(piece.XIndex, piece.YIndex, falseYOffset, moveTime);
			}
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
		if (isPlayerInputEnabled) {

			GamePiece clickedPiece = allGamePieces[clickedTile.XIndex, clickedTile.YIndex];
			GamePiece targetPiece = allGamePieces[targetTile.XIndex, targetTile.YIndex];

			if (clickedPiece != null && targetPiece != null) {

				clickedPiece.Move(targetTile.XIndex, targetTile.YIndex, swapTime);
				targetPiece.Move(clickedTile.XIndex, clickedTile.YIndex, swapTime);

				yield return new WaitForSeconds(swapTime);

				List<GamePiece> clickedPieceMatches = FindMatchesAt(clickedTile.XIndex, clickedTile.YIndex);
				List<GamePiece> targetPieceMatches = FindMatchesAt(targetTile.XIndex, targetTile.YIndex);

				if (targetPieceMatches.Count == 0 && clickedPieceMatches.Count == 0) {
					clickedPiece.Move(clickedTile.XIndex, clickedTile.YIndex, swapTime);
					targetPiece.Move(targetTile.XIndex, targetTile.YIndex, swapTime);
				} else {
					yield return new WaitForSeconds(swapTime);

					ClearAndRefillBoard(clickedPieceMatches.Union(targetPieceMatches).ToList());
				}
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

	List<GamePiece> FindMatchesAt (List<GamePiece> gamePieces, int minLength = 3)
	{
		List<GamePiece> matches = new List<GamePiece>();

		foreach (GamePiece piece in gamePieces)
		{
			matches = matches.Union(FindMatchesAt(piece.XIndex, piece.YIndex, minLength)).ToList();
		}
		return matches;

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

	private void HighlightTileOff (int x, int y) {
		if (allTiles[x, y].TType != TileType.Breakable) {
			SpriteRenderer spriteRenderer = allTiles[x, y].GetComponent<SpriteRenderer>();
			spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
		}
	}

	private void HighlightTileOn (int x, int y, Color col) {
		if (allTiles[x, y].TType != TileType.Breakable) {
			SpriteRenderer spriteRenderer = allTiles[x, y].GetComponent<SpriteRenderer>();
			spriteRenderer.color = col;
		}
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

	private void HighlightMatches () {
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				HighlightMatchesAt(i, j);
			}
		}
	}

	private void HighlightPieces (List<GamePiece> gamePieces) {
		foreach (GamePiece piece in gamePieces) {
			if (piece) {
				HighlightTileOn(piece.XIndex, piece.YIndex, piece.GetComponent<SpriteRenderer>().color);
			}
		}
	}

	private void ClearPieceAt (int x, int y) {
		GamePiece pieceToClear = allGamePieces[x, y];

		if (pieceToClear) {
			allGamePieces[x, y] = null;
			Destroy(pieceToClear.gameObject);
		}

		// HighlightTileOff(x, y);
	}

	private void ClearBoard () {
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				ClearPieceAt(i, j);
			}
		}
	}

	private void ClearPieceAt (List<GamePiece> gamePieces) {
		foreach (GamePiece piece in gamePieces) {
			if (piece != null) {
				ClearPieceAt(piece.XIndex, piece.YIndex);
				if (particleManager != null) {
					particleManager.ClearPieceFxAt(piece.XIndex, piece.YIndex);
				}
			}
		}
	}

	private void BreakTileAt (int x, int y) {
		Tile tileToBreak = allTiles[x, y];

		if (tileToBreak != null && tileToBreak.TType == TileType.Breakable) {
			if (particleManager != null) {
				particleManager.BreakTileFxAt(tileToBreak.BreakableValue, x, y);
			}
			tileToBreak.BreakTile();
		}
	}

	private void BreakTileAt (List<GamePiece> gamePieces) {
		foreach(GamePiece piece in gamePieces) {
			if (piece != null) {
				BreakTileAt(piece.XIndex, piece.YIndex);
			}
		}
	}

	private List<GamePiece> CollapseColumn (int column, float collapseTime = 0.1f) {
		List<GamePiece> movingPieces = new List<GamePiece>();

		for (int i = 0; i < height - 1; i++) {
			if (allGamePieces[column, i] == null && allTiles[column, i].TType != TileType.Obstacle) {
				for (int j = i + 1; j < height; j++) {
					if (allGamePieces[column, j] != null) {
						allGamePieces[column, j].Move(column, i, collapseTime * (j - i));
						allGamePieces[column, i] = allGamePieces[column, j];
						allGamePieces[column, i].SetCoord(column, i);

						if (!movingPieces.Contains(allGamePieces[column, i])) {
							movingPieces.Add(allGamePieces[column, i]);
						}

						allGamePieces[column, j] = null;
						
						break;
					}
				}
			}
		}

		return movingPieces;
	}

	private List<GamePiece> CollapseColumn (List<GamePiece> gamePieces) {
		List<GamePiece> movingPieces = new List<GamePiece>();
		List<int> columnsToCollapse = GetColumns(gamePieces);

		foreach (int column in columnsToCollapse) {
			movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
		}

		return movingPieces;
	}

	private List<int> GetColumns (List<GamePiece> gamePieces) {
		List<int> columns = new List<int>();

		foreach (GamePiece piece in gamePieces) {
			if (!columns.Contains(piece.XIndex)) {
				columns.Add(piece.XIndex);
			}
		}

		return columns;
	}

	private void ClearAndRefillBoard (List<GamePiece> gamePieces) {
		StartCoroutine(ClearAndRefillBoardRoutine(gamePieces));
	}

	private IEnumerator ClearAndRefillBoardRoutine (List<GamePiece> gamePieces) {
		isPlayerInputEnabled = false;

		List<GamePiece> matches = gamePieces;

		do {
			yield return StartCoroutine(ClearAndCollapseRoutine(matches));
			
			yield return null;

			yield return StartCoroutine(RefillRoutine());

			matches = FindAllMatches();

			yield return new WaitForSeconds (0.5f);
		} while (matches.Count != 0);

		isPlayerInputEnabled = true;
	}

	private IEnumerator ClearAndCollapseRoutine (List<GamePiece> gamePieces) {

		List<GamePiece> movingPieces = new List<GamePiece>();
		List<GamePiece> matches = new List<GamePiece>();


		// HighlightPieces(gamePieces);

		yield return new WaitForSeconds(0.2f);

		bool isFinished = false;

		while (!isFinished) {
			ClearPieceAt(gamePieces);
			BreakTileAt(gamePieces);

			yield return new WaitForSeconds(0.25f);

			movingPieces = CollapseColumn(gamePieces);

			while (!IsCollapsed(movingPieces)) {
				yield return null;
			}

			yield return new WaitForSeconds(0.2f);

			matches = FindMatchesAt(movingPieces);

			if (matches.Count == 0) {
				isFinished = true;
				break;
			} else {
				yield return StartCoroutine(ClearAndCollapseRoutine(matches));
			}
		}

		yield return null;
	}

	private IEnumerator RefillRoutine () {
		FillBoard(10, 0.5f);
		yield return null;
	}

	private bool IsCollapsed (List<GamePiece> gamePieces) {
		foreach (GamePiece piece in gamePieces) {
			if (piece) {
				if (piece.transform.position.y - (float)piece.YIndex > 0.001f) {
					return false;
				}
			}
		}
		return true;
	}
}
