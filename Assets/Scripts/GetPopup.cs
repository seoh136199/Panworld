using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GetPopup : MonoBehaviour {
    public int id;
    private Image[] iconImages = new Image[3];
    private TextMeshProUGUI[] getTexts = new TextMeshProUGUI[3];

    private void Awake() {
        if (id == 1) Game.getPopup1 = this;
        if (id == 2) Game.getPopup2 = this;
    }

    void Start() {
        for (int i = 0; i < 3; i++) {
            iconImages[i] = transform.GetChild(i).GetComponent<Image>();
            getTexts[i] = transform.GetChild(i + 3).GetComponent<TextMeshProUGUI>();
        }
    }

    public void Appear(int pGet, int dGet, int aGet) {
        getTexts[0].text = "+" + pGet;
        getTexts[1].text = "+" + dGet;
        getTexts[2].text = "+" + aGet;

        IEnumerator Fade() {
            float t = 0, fadeSpeed = 0.6f;
            float alpha;

            while (t < 1) {
                alpha = Mathf.Min(Mathf.Sin(t * Mathf.PI) * 1.3f, 0.9f);

                getTexts[0].color = new(getTexts[0].color.r, getTexts[0].color.g, getTexts[0].color.b, alpha);
                getTexts[1].color = new(getTexts[1].color.r, getTexts[1].color.g, getTexts[1].color.b, alpha);
                getTexts[2].color = new(getTexts[2].color.r, getTexts[2].color.g, getTexts[2].color.b, alpha);
                iconImages[0].color = new(1, 1, 1, alpha);
                iconImages[1].color = new(1, 1, 1, alpha);
                iconImages[2].color = new(1, 1, 1, alpha);
                t += fadeSpeed * Time.deltaTime * Game.gameManager.speedWeight;
                yield return null;
            }

            alpha = 0;
            getTexts[0].color = new(getTexts[0].color.r, getTexts[0].color.g, getTexts[0].color.b, alpha);
            getTexts[1].color = new(getTexts[1].color.r, getTexts[1].color.g, getTexts[1].color.b, alpha);
            getTexts[2].color = new(getTexts[2].color.r, getTexts[2].color.g, getTexts[2].color.b, alpha);
            iconImages[0].color = new(1, 1, 1, alpha);
            iconImages[1].color = new(1, 1, 1, alpha);
            iconImages[2].color = new(1, 1, 1, alpha);
        }

        StartCoroutine(Fade());
    }
}