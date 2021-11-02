using System.Collections.Generic;
using AnttiStarterKit.ScriptableObjects;
using UnityEngine;

namespace AnttiStarterKit.Managers
{
	public class AudioManager : ObjectPool<SoundEffect> {

		public AudioSource curMusic;
		public AudioSource[] musics;

		public float volume = 0.5f;
		private float musVolume = 0.5f;
		public AudioClip[] effects;

		private AudioLowPassFilter lowpass;
		private AudioHighPassFilter highpass;

		public float TargetPitch { get; set; } = 1f;

		// private AudioReverbFilter reverb;
		// private AudioReverbPreset fromReverb, toReverb;
		
		private AudioSource prevMusic;

		private float fadeOutPos = 0f, fadeInPos = 0f;
		private float fadeOutDuration = 1f, fadeInDuration = 3f;

		private bool doingLowpass, doingHighpass;

		[SerializeField] private List<SoundCollection> soundCollections;
		

		/******/

		private static AudioManager instance = null;
		public static AudioManager Instance {
			get { return instance; }
		}

		void Awake() {
			if (instance != null && instance != this) {
				Destroy (this.gameObject);
				return;
			} else {
				instance = this;
			}

			// reverb = GetComponent<AudioReverbFilter> ();
			// fromReverb = AudioReverbPreset.Hallway;
			// toReverb = AudioReverbPreset.Off;

			DontDestroyOnLoad(instance.gameObject);
		}

		public void BackToDefaultMusic()
		{
			if (curMusic != musics [0]) {
				ChangeMusic (0, 0.5f, 2f, 1f);
			}
		}

		public void Lowpass(bool state = true)
		{
			if (!lowpass) lowpass = Camera.main.GetComponent<AudioLowPassFilter>();

			doingLowpass = state;
			doingHighpass = false;
		}

		public void Highpass(bool state = true)
		{
			if (!highpass) highpass = Camera.main.GetComponent<AudioHighPassFilter>();
			doingHighpass = state;
			doingLowpass = false;
		}

		private void ChangeMusic(int next, float fadeOutDur, float fadeInDur, float startDelay)
		{
			if (musics[next] == curMusic) return;
		
			fadeOutPos = 0f;
			fadeInPos = -1f;

			fadeOutDuration = fadeOutDur;
			fadeInDuration = fadeInDur;

			prevMusic = curMusic;
			curMusic = musics [next];

			prevMusic.time = 0f;

			Invoke (nameof(StartNext), startDelay);
		}

		private void StartNext()
		{
			fadeInPos = 0f;
			curMusic.time = 0f;
			curMusic.volume = 0f;
			curMusic.Play ();
		}

		private void Update()
		{
			var targetLowpass = (doingLowpass) ? 5000f : 22000;
			var targetHighpass = (doingHighpass) ? 400f : 10f;
			const float changeSpeed = 0.075f;

			curMusic.pitch = Mathf.MoveTowards (curMusic.pitch, TargetPitch, 0.005f * changeSpeed);
			if(lowpass) lowpass.cutoffFrequency = Mathf.MoveTowards (lowpass.cutoffFrequency, targetLowpass, 750f * changeSpeed);
			if (highpass) highpass.cutoffFrequency = Mathf.MoveTowards (highpass.cutoffFrequency, targetHighpass, 50f * changeSpeed);
		
			if (fadeInPos < 1f) fadeInPos += Time.unscaledDeltaTime / fadeInDuration;

			if (fadeOutPos < 1f) fadeOutPos += Time.unscaledDeltaTime / fadeOutDuration;

			if (curMusic && fadeInPos >= 0f) curMusic.volume = Mathf.Lerp(0f, musVolume * 1.5f, fadeInPos);

			if (prevMusic)
			{
				prevMusic.volume = Mathf.Lerp(musVolume, 0f, fadeOutPos);

				if (prevMusic.volume <= 0f) prevMusic.Stop();
			}
		}

		public void PlayEffectFromCollection(int collection, Vector3 pos, float volume = 1f)
		{
			var clip = soundCollections[collection].Random();
			PlayEffectAt(clip, pos, volume);
		}
		
		public void PlayEffectFromCollection(SoundCollection collection, Vector3 pos, float volume = 1f)
		{
			var clip = collection.Random();
			PlayEffectAt(clip, pos, volume);
		}

		public void PlayEffectAt(AudioClip clip, Vector3 pos, float volume, bool pitchShift = true) {
			var se = Get();
			se.transform.position = pos;
			se.Play (clip, volume, pitchShift);
			se.transform.parent = transform;
		}

		public void PlayEffectAt(AudioClip clip, Vector3 pos, bool pitchShift = true) {
			PlayEffectAt (clip, pos, 1f, pitchShift);
		}

		public void PlayEffectAt(int effect, Vector3 pos, bool pitchShift = true) {
			PlayEffectAt (effects [effect], pos, 1f, pitchShift);
		}

		public void PlayEffectAt(int effect, Vector3 pos, float volume, bool pitchShift = true) {
			PlayEffectAt (effects [effect], pos, volume, pitchShift);
		}

		public void ChangeMusicVolume(float vol) {
			curMusic.volume = vol * 1.5f;
			musVolume = vol * 1.5f;
		}

		public void ToMenu()
		{
			ChangeMusic(1, 0.2f, 0.2f, 0f);
		}

		public void ToMain()
		{
			ChangeMusic(0, 0.2f, 0.2f, 0f);
		}

		public static void BaseSound(Vector3 p, float vol = 1f)
		{
			Instance.PlayEffectAt(4, p, 0.212f * vol);
			Instance.PlayEffectAt(22, p, 0.277f * vol);
			Instance.PlayEffectAt(24, p, 0.644f * vol);
			Instance.PlayEffectAt(9, p, 0.286f * vol);
			Instance.PlayEffectAt(0, p, 4f * vol);
		}
	}
}
