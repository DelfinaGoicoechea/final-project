using Contents;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ContentPlayer : MonoBehaviour {
    public AudioSource audioSource;
    private GameObject sphere;
	private Content _currentlyPlaying;
    public Content currentlyPlaying { get { return _currentlyPlaying; } set{ ; } } // sin setter
	private float configuredVolume = 1.0f;
    public static ContentPlayer Instance { get; private set; }
    private void Awake() {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
	/// Takes a Content as an input, and starts playing it.
	/// </summary>
	/// <param name="scene"></param>
	/// <returns></returns>
    public void play(Content content) {
		if(this._currentlyPlaying != null && this._currentlyPlaying != content){
			// me aseguro de parar la reproduccion del anterior,
			// antes de reproducir el nuevo contenido
			_currentlyPlaying.stop();
		}
		this._currentlyPlaying = content;
        Time.timeScale = 1.0f;
        content.play();
		StartCoroutine(fadeIn(2.5f));
	}

    /// <summary>
	/// Stops content reproduction and returns to the "Main" scene.
	/// </summary>
	/// <returns></returns>
    public IEnumerator stop(Content mainScene) {
        // Deberia pasarme la escena Main
        float currentTime = 0.0f;
        float start = configuredVolume;
        float end = 0.0f;
        float duration = 5f;
        //fadeout
        while (currentTime < duration) {
            currentTime += Time.fixedDeltaTime;
            audioSource.volume = Mathf.Lerp(start, end, currentTime / duration);
            yield return new WaitForFixedUpdate();
        }
        // Por que no hago currentlyPlaying.Stop?
        // Solo voy a Play si me tiran Stop y actualmente no estoy en la escena Main
        bool alreadyInMain = this._currentlyPlaying == null || this._currentlyPlaying.nombre == "Main";
        if (!alreadyInMain)
            this.play(mainScene);
        yield return null;
    }

    /// <summary>
	/// pauses any currently playing content.
	/// Exactly what that means depends on the kind of
	/// content currently playing (image/video/scene).
	/// </summary>
	/// <returns></returns>
    public IEnumerator pause() {
        float currentTime = 0.0f;
        float start = configuredVolume;
        float end = 0.0f;
        float duration = 2.5f;
        if (_currentlyPlaying != null) {
            _currentlyPlaying.pause();
        }
        while (currentTime < duration) {
            currentTime += Time.fixedDeltaTime;
            audioSource.volume = Mathf.Lerp(start, end, currentTime / duration);
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    public void setVolume(float vol) {
        if (vol > 100.0f) {
            vol = 100.0f;
        }
        else if (vol < 0.0f) {
            vol = 0.0f;
        }
        vol = vol / 100.0f;

        if (audioSource.clip != null) {
            audioSource.volume = vol;
        }
        configuredVolume = vol;

        // El video solo esta presente en la escena de la esfera
        if (SceneManager.GetActiveScene().name == "SphereScene") {
            VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayer").GetComponent<VideoPlayer>();
            if (videoPlayer.clip != null)
            {
                if (videoPlayer.clip.audioTrackCount > 0)
                {
                    videoPlayer.SetDirectAudioVolume(0, 0);
                }
            }
        }
    }

    private IEnumerator fadeIn(float duration) {
		float currentTime = 0;
		float start = 0.0f;
		float end = configuredVolume;
		while (currentTime < duration) {
			currentTime += Time.fixedDeltaTime;
			audioSource.volume = Mathf.Lerp(start, end, currentTime / duration);
			yield return new WaitForFixedUpdate();
		}
		yield break;
	}
}