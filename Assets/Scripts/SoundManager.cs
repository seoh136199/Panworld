using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {

    [SerializeField] private AudioSource crBgm, crSfx;
    public AudioMixer audioMixer;

    [System.Serializable]
    public class SoundData {
        public AudioClip clip;
        public float volume = 1;
    }

    [SerializeField]
    private SoundData[] bgms;

    [SerializeField]
    private SoundData[] sfxs;

    private void Awake() {
        Game.soundManager = this;
    }

    public void PlayBgm(int idx) {
        if (crSfx != null) crBgm.Stop();
        crBgm.clip = bgms[idx].clip;
        crBgm.volume = bgms[idx].volume;
        crBgm.Play();
    }

    public void PlaySfx(int idx) {
        if (crSfx != null) crSfx.Stop();
        crSfx.clip = sfxs[idx].clip;
        crSfx.volume = sfxs[idx].volume;
        crSfx.Play();
    }

    public IEnumerator BgmFadeIn(float speed) {
        float crVolume = crBgm.volume;
        crBgm.volume = 0;
        float t = 0;
        while (t < 1) {
            crBgm.volume = crVolume * t;
            t += speed * Time.deltaTime;
            yield return null;
        }
        crBgm.volume = crVolume;
    }

    public IEnumerator BgmFadeOut(float speed) {
        float crVolume = crBgm.volume;
        float t = 1;
        while (t > 0) {
            crBgm.volume = crVolume * t;
            t -= speed * Time.deltaTime;
            yield return null;
        }
        crBgm.volume = 0;
    }

    public IEnumerator BgmFadeOutAndStart(float speed, int num) {
        float crVolume = crBgm.volume;
        float t = 1;
        while (t > 0) {
            crBgm.volume = crVolume * t;
            t -= speed * Time.deltaTime;
            yield return null;
        }
        crBgm.volume = 0;
        StartBgm(num);
    }

    public void StartBgm(int idx) {
        crBgm.Stop();
        PlayBgm(idx);
        crBgm.loop = true;
        StartCoroutine(BgmFadeIn(1f));
    }
}