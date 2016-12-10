using UnityEngine;

public class SoundManager : Singleton<SoundManager> {

	[SerializeField]
	private AudioClip[] musicClips;
	[SerializeField]
	private AudioClip[] winClips;
	[SerializeField]
	private AudioClip[] loseClips;
	[SerializeField]
	private AudioClip[] bonusClips;

	[Range(0, 1)]
	[SerializeField]
	private float musicVolume = 0.5f;
	[Range(0, 1)]
	[SerializeField]
	private float sfxVolume = 1.0f;

	private float lowPitch = 0.95f;
	private float highPitch = 1.05f;

	private void Start () {
		PlayRandomMusic();
	}

	private AudioSource PlayClipAtPoint (AudioClip clip, Vector3 position, float volume = 1f) { // TODO - Make position default to Vector3.zero
		if (clip != null) {
			GameObject go = new GameObject("SoundFX " + clip.name);
			go.transform.position = position;

			AudioSource source = go.AddComponent<AudioSource>();
			source.clip = clip;

			float randomPitch = Random.Range(lowPitch, highPitch);
			source.pitch = randomPitch;

			source.volume = volume;

			source.Play();
			Destroy(go, clip.length);
			return source;
		}
		return null;
	}

	public AudioSource PlayRandom (AudioClip[] clips, Vector3 position, float volume = 1f) {
		if (clips != null) {
			if (clips.Length > 0) {
				int randomIndex = Random.Range(0, clips.Length);

				if (clips[randomIndex] != null) {
					return PlayClipAtPoint(clips[randomIndex], position, volume);
				}
			}
		}
		return null;
	}

	public void PlayRandomMusic () {
		PlayRandom(musicClips, Vector3.zero, musicVolume);
	}

	public void PlayRandomWinSound() {
		PlayRandom(winClips, Vector3.zero, sfxVolume);
	}

	public void PlayRandomLoseSound () {
		PlayRandom(loseClips, Vector3.zero, sfxVolume);
	}

	public void PlayRandomBonusSound () {
		PlayRandom(bonusClips, Vector3.zero, sfxVolume);
	}

}
