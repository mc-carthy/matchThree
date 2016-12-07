using UnityEngine;

public class Board : MonoBehaviour {

	[SerializeField]
	private GameObject tilePrefab;
	[SerializeField]
	private int height;
	[SerializeField]
	private int width;

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
