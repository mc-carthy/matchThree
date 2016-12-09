using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(MaskableGraphic))]
public class ScreenFader : MonoBehaviour {

	private MaskableGraphic graphic;
	private Color color;
	private float currentAlpha;
	private float increment;
	private float solidAlpha = 1f;
	private float clearAlpha = 0f;
	private float delay = 0.5f;
	private float timeToFade = 1f;

	private void Start () {
		graphic = GetComponent<MaskableGraphic>();
		color = graphic.color;
	}

	public void FadeOn () {
		StartCoroutine(FadeRoutine(clearAlpha, solidAlpha));
	}

	public void FadeOff () {
		StartCoroutine(FadeRoutine(solidAlpha, clearAlpha));
	}

	private IEnumerator FadeRoutine (float startAlpha, float endAlpha) {
		
		graphic.color = new Color(color.r, color.g, color.b, startAlpha);
		currentAlpha = startAlpha;

		increment = (endAlpha - startAlpha) / timeToFade * Time.deltaTime;
		
		yield return new WaitForSeconds(delay);

		while (Mathf.Abs(endAlpha - currentAlpha) > 0.01f) {
			yield return null;
			currentAlpha += increment;
			graphic.color = new Color (color.r, color.g, color.b, currentAlpha);
		}

		currentAlpha = endAlpha;
		graphic.color = new Color (color.r, color.g, color.b, currentAlpha);
	}
}
