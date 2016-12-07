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

	private Tile[,] allTiles;

	private void Start () {
		allTiles = new Tile[width, height];
		SetupTiles();
	}

	private void SetupTiles () {
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				GameObject newTile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
				newTile.name = "Tile (" + i + ", " + j + ")";
				newTile.transform.parent = transform;
				allTiles[i, j] = newTile.GetComponent<Tile>();
			}
		}
	}
}
