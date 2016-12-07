using UnityEngine;

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

	private void Start () {
		allTiles = new Tile[width, height];
		allGamePieces = new GamePiece[width, height];
		SetupTiles();
		FillRandom();
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

	private void PlaceGamePiece (GamePiece piece, int x, int y) {
		if (piece == null) {
			Debug.LogWarning("BOARD: Invalid GamePiece!");
			return;
		}

		piece.transform.position = new Vector3(x, y, 0);
		piece.transform.rotation = Quaternion.identity;
		piece.SetCoord(x, y);
	}

	private void FillRandom () {
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				GameObject randomPiece = Instantiate(GetRandomGamePiece(), Vector3.zero, Quaternion.identity) as GameObject;
				
				if (randomPiece != null) {
					PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), i, j);
				}
			}
		}
	}

	public void ClickTile (Tile tile) {
		if (clickedTile == null) {
			clickedTile = tile;
		}
	}

	public void DragToTile (Tile tile) {
		if (clickedTile != null) {
			targetTile = tile;
		}
	}

	public void ReleaseTile () {
		if (clickedTile && targetTile) {
			SwitchTiles(clickedTile, targetTile);
		}
	}

	private void SwitchTiles (Tile clickedTile, Tile targetTile) {
		this.clickedTile = null;
		this.targetTile = null;
	}

}
