using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
struct ImagesInfo {
    public Sprite[] images;
}

public class Castle : MonoBehaviour {

    public int level;

    private Image image0, image1;
    [SerializeField] private ImagesInfo[] imagesInfos;

    [SerializeField] private bool isFading;

    private void Awake() {
        Game.castle = this;
    }

    void Start() {
        image0 = transform.GetChild(0).GetComponent<Image>();
        image1 = transform.GetChild(1).GetComponent<Image>();
    }

    public void LevelUp() {

    }

    public void ChangeImage(int num) {
        IEnumerator Fade() {
            isFading = true;
            image0.sprite = imagesInfos[level - 1].images[(num + 1) % 2];
            image1.sprite = imagesInfos[level - 1].images[num];
            image0.color = new(1, 1, 1, 1);

            float t = 0, fadeSpeed = 2.5f;
            while (t < 1) {
                image1.color = new(1, 1, 1, t);
                t += fadeSpeed * Time.deltaTime;
                yield return null;
            }

            image1.color = new(1, 1, 1, 1);
            isFading = false;
        }

        StartCoroutine(Fade());
    }
}
