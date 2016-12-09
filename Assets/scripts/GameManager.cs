﻿using UnityEngine;
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

	private Board board;
	private int movesLeft = 30;
	private int scoreGoal = 10000;
	private bool isReadyToBegin;
	private bool isGameOver;
	private bool isWinner;

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
		while (!isReadyToBegin) {
			yield return null;
			yield return new WaitForSeconds(2f); // TODO - Remove this hardcoded pause
			isReadyToBegin = true;
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
			if (movesLeft <= 0) {
				isGameOver = true;
				isWinner = false;
			}
			yield return null;
		}
	}

	private IEnumerator EndGameRoutine () {
		if (screenFader != null) {
			screenFader.FadeOn();
		}
		
		if (isWinner) {
			Debug.Log("A winner is you!");
		} else {
			Debug.Log("Loser...");
		}
		yield return null;
	}

}
