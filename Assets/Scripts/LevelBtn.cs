using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBtn : MonoBehaviour {

    public int level;
    public bool thisLevelOn = false, isActive = true;
    [SerializeField] private int maxGoodsCnt = 10, inputUnit = 10;
    private Image bgImage, gaugeLineImage, myImage;
    [SerializeField] private Sprite bgSprite, gaugeLineSprite, btnOnSprite, btnOffSprite;

    private void Start() {
        myImage = GetComponent<Image>();
        bgImage = GameObject.Find("AlterPopup").GetComponent<Image>();
        gaugeLineImage = GameObject.Find("GaugeLine").GetComponent<Image>();
    }

    public void OnClick() {
        if (thisLevelOn) return;
        if (!isActive) return;

        thisLevelOn = true;
        Game.gauge.crLevel = level;
        Game.gauge.maxGoodsCnt = maxGoodsCnt;
        Game.gauge.inputUnit = inputUnit;

        myImage.sprite = btnOnSprite;
        bgImage.sprite = bgSprite;
        gaugeLineImage.sprite = gaugeLineSprite;

        for (int i = 0; i < 3; i++) {
            if (Game.gauge.levelBtns[i] != this) Game.gauge.levelBtns[i].SetOff();
        }

        for (int i = 0; i < 3; i++) {
            Game.gauge.goodsBtns[i].SetUnit(inputUnit);
        }
        Game.gauge.CheckGoodsBtnAvail();
    }

    public void SetOff() {
        thisLevelOn = false;
        myImage.sprite = btnOffSprite;
    }

    public void SetActive() {
        isActive = true;
        myImage.color = new(1, 1, 1, 1f);
    }

    public void Deactive() {
        isActive = false;
        myImage.color = new(1, 1, 1, 0.3f);
    }
}
