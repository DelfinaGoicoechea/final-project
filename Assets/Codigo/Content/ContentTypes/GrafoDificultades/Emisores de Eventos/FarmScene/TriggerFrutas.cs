using UnityEngine;
using System.Collections;

public class TriggerFrutas : MonoBehaviour {

    public string fruitObjectName = "AsignarValor";
    public AudioSource fxAudioSource;
    public AudioClip sonidoBien;
    public AudioClip sonidoMal;
    public EmisorCompletaFrutas emisor;

    private IEnumerator playSonido(AudioClip sonido){
        if (fxAudioSource != null && fxAudioSource.isPlaying) {
            yield return new WaitForSeconds(fxAudioSource.clip.length);
        }
        fxAudioSource.clip = sonido;
        fxAudioSource.Play();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Fruta") {
            if (other.gameObject.name.Contains(fruitObjectName)) {
                // Emitir sonido bien
                StartCoroutine(playSonido(sonidoBien));
                emisor.agregarFruta();
            } else {
                // Emitir sonido mal
                StartCoroutine(playSonido(sonidoMal));
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Fruta") {
            if (other.gameObject.name.Contains(fruitObjectName)) {
                emisor.restarFruta();
            }
        }
    }
}