using UnityEngine;

public class ParticlePlayer : MonoBehaviour {

	[SerializeField]
	private ParticleSystem[] allParticles;
	private float lifeTime = 1f;

	private void Awake () {
		allParticles = GetComponentsInChildren<ParticleSystem>();
		Destroy(gameObject, lifeTime);
	}

	public void Play () {
		foreach (ParticleSystem ps in allParticles) {
			ps.Stop();
			ps.Play();
		}
	}
}
