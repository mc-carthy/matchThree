using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : Singleton<ScoreManager> {

	[SerializeField]
	private Text scoreText;

	private int currentScore;
	private int counterValue;
	private int increment = 5;

	private void Start () {
		UpdateScoreText(currentScore);
	}

	public void AddScore (int value) {
		currentScore += value;
		StartCoroutine(CountScoreRoutine());
	}

	private void UpdateScoreText (int scoreValue) {
		if (scoreText != null) {
			scoreText.text = scoreValue.ToString();
		}
	}

	private IEnumerator CountScoreRoutine () {
		int maxIterations = 10000;
		int iterations = 0;

		while (counterValue < currentScore && iterations < maxIterations) {
			counterValue += increment;
			UpdateScoreText(counterValue);
			iterations++;
			yield return null;
		}

		counterValue = currentScore;
		UpdateScoreText(counterValue);
	}

}
