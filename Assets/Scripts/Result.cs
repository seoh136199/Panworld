using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result : MonoBehaviour {

    [SerializeField] private int pRatio, dRatio, aRatio, level;
    [SerializeField] private int pVisual, dVisual, aVisual;
    [SerializeField] private RectTransform[] bar = new RectTransform[3];

    private void Start() {
        for (int i = 0; i < 3; i++) {
            bar[i] = transform.GetChild(i + 3).GetChild(0).GetComponent<RectTransform>();
        }
        gameObject.SetActive(false);
    }

    public void SetInfo(int pRatio, int dRatio, int aRatio, int level) {
        this.pRatio = pRatio;
        this.dRatio = dRatio;
        this.aRatio = aRatio;
        this.level = level;

        pVisual = (int)(100 - (float)(100 - pRatio) / (0.6 + 0.4 * level));
        dVisual = (int)(100 - (float)(100 - dRatio) / (0.6 + 0.4 * level));
        aVisual = (int)(100 - (float)(100 - aRatio) / (0.6 + 0.4 * level));

        bar[0].sizeDelta = new(pVisual, 100);
        bar[1].sizeDelta = new(dVisual, 100);
        bar[2].sizeDelta = new(aVisual, 100);
    }

    public void SendMember() {
        Game.gameManager.AddMember("±è¾Æ¹«°³", level, pRatio, dRatio, aRatio, pVisual, dVisual, aVisual);
        gameObject.SetActive(false);

        for (int i = 0; i < 3; i++) {
            Game.gauge.goodsBtns[i].SetActiveBySummon();
        }
    }

}
