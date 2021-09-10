using System;
using UnityEngine;
using UnityEngine.Audio;


public class sound_manager : MonoBehaviour
{
    private static sound_manager instance;

    public Sound[] bgSounds;
    public Sound[] uiSounds;
    public Sound[] sfxSounds;

    private AudioSource bgSource;
    private AudioSource uiSource;
    private AudioSource sfxSource;

    private void Awake() {
        if(instance == null) {
            sound_manager.instance = this;
            GameObject.DontDestroyOnLoad(gameObject);

            foreach(Sound bg in bgSounds) {
                bg.source = gameObject.AddComponent<AudioSource>();
                
                bg.source.clip = bg.clip;
                bg.source.volume = bg.volume;
                bg.source.pitch = 1f;
                bg.source.loop = bg.loop;
            }

            foreach (Sound ui in uiSounds) {
                ui.source = gameObject.AddComponent<AudioSource>();

                ui.source.clip = ui.clip;
                ui.source.volume = ui.volume;
                ui.source.pitch = 1f;
                ui.source.loop = ui.loop;
            }

            foreach (Sound sfx in sfxSounds) {
                sfx.source = gameObject.AddComponent<AudioSource>();

                sfx.source.clip = sfx.clip;
                sfx.source.volume = sfx.volume;
                sfx.source.pitch = 1f;
                sfx.source.loop = sfx.loop;
            }
            playBgSound("bgm");
        } else {
            Destroy(gameObject);
            return;
        }
    }
    public void playBgSound(string name,bool stop = false) {
        foreach(Sound s in bgSounds) {
            if(s.name == name) {
                if (stop) {
                    s.source.Stop();
                } else {
                    s.source.Play();
                }
                return;
            }
        }
        Debug.LogWarning(name + " sound Was not found in Bg Sounds.");
    }

    public void playUiSound(string name, bool stop = false, float pitch = 1f) {
        foreach (Sound s in uiSounds) {
            if (s.name == name) {
                if (stop) {
                    s.source.Stop();
                } else {
                    s.source.pitch = pitch;
                    s.source.Play();
                }
                return;
            }
        }
        Debug.LogWarning(name + " sound Was not found in Ui Sounds.");
    }

    public void playSfxSound(string name, bool stop = false, float pitch = 1f) {
        foreach (Sound s in sfxSounds) {
            if (s.name == name) {
                if (stop) {
                    s.source.Stop();
                } else {
                    s.source.pitch = pitch;
                    s.source.Play();
                }
                return;
            }
        }
        Debug.LogWarning(name + " sound Was not found in SFX Sounds.");
    }
}
