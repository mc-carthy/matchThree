using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransformMover))]
public class MessageWindow : MonoBehaviour {

	[SerializeField]
	private Image messageIcon;
	[SerializeField]
	private Text messageText;
	[SerializeField]
	private Text buttonText;

	public void ShowMessage (Sprite sprite = null, string message = "", string buttonMessage = "Start") {
		if (messageIcon != null) {
			messageIcon.sprite = sprite;
		}
		if (messageText != null) {
			messageText.text = message;
		}
		if (buttonText != null) {
			buttonText.text = buttonMessage;
		}
	}
}
