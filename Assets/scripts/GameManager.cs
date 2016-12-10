using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : Singleton<GameManager> {

	[SerializeField]
	private ScreenFader screenFader;
	[SerializeField]
	private Text levelNameText;
	[SerializeField]
	private Text movesLeftText;
	[SerializeField]
	private MessageWindow messageWindow;
	[SerializeField]
	private Sprite goalIcon;
	[SerializeField]
	private Sprite loseIcon;
	[SerializeField]
	private Sprite winIcon;

	private Board board;
	private int movesLeft = 30;
	private int scoreGoal = 10000;
	private bool isReadyToBegin;
	private bool isGameOver;
	private bool isWinner;
	private bool isReadyToReload;

	private void Start () {
		board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();
		Scene scene = SceneManager.GetActiveScene();
		if (levelNameText != null) {
			levelNameText.text = scene.name;
		}
		UpdateMoves();
		StartCoroutine(ExecuteGameLoop());
	}

	public void DecrementMoves (int moves = 1) {
		movesLeft--;
		UpdateMoves();
	}

	public void BeginGame () {
		isReadyToBegin = true;
	}

	private void UpdateMoves () {
		if (movesLeftText != null) {
			movesLeftText.text = movesLeft.ToString();
		}
	}

	private IEnumerator ExecuteGameLoop () {
		yield return StartCoroutine(StartGameRoutine());
		yield return StartCoroutine(PlayGameRoutine());
		yield return StartCoroutine(EndGameRoutine());
	}

	private IEnumerator StartGameRoutine () {

		if (messageWindow != null) {
			messageWindow.GetComponent<RectTransformMover>().MoveOn();
			messageWindow.ShowMessage(goalIcon, "Score Goal\n" + scoreGoal.ToString(), "Start!");
		}

		while (!isReadyToBegin) {
			yield return null;
		}

		if (screenFader != null) {
			screenFader.FadeOff();
		}

		yield return new WaitForSeconds(0.5f);

		if (board != null) {
			board.SetupBoard();
		}
	}

	private IEnumerator PlayGameRoutine() {
		while (!isGameOver) {
			
			if (ScoreManager.Instance != null) {
				if (ScoreManager.Instance.CurrentScore > scoreGoal) {
					isGameOver = true;
					isWinner = true;
				}
			}

			if (movesLeft <= 0) {
				isGameOver = true;
				isWinner = false;
			}
			yield return null;
		}
	}

	private IEnumerator EndGameRoutine () {

		isReadyToReload = false;

		if (isWinner) {
			if (messageWindow != null) {
				messageWindow.GetComponent<RectTransformMover>().MoveOn();
				messageWindow.ShowMessage(winIcon, "You Win!", "Play Again!");
			}
		} else {
			if (messageWindow != null) {
				messageWindow.GetComponent<RectTransformMover>().MoveOn();
				messageWindow.ShowMessage(loseIcon, "You Lose!", "Play Again!");
			}		
		}

		yield return new WaitForSeconds(1f);

		if (screenFader != null) {	
			screenFader.FadeOn();
		}
		
		while (!isReadyToReload) {
			yield return null;
		}

		SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
	}

	public void ReloadScene () {
		isReadyToReload = true;
	}

}
