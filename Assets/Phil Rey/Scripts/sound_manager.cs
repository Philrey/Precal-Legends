using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class sound_manager : MonoBehaviour
{
    private static sound_manager instance;

    [Header("Background Music")]
    public AudioClip[] bgm;
    [Header("UI Sounds")]
    public AudioClip[] ui_snd;
    [Header("SFX Sounds")]
    public AudioClip[] sfxs;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

}
