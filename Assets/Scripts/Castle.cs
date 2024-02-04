using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

[Serializable]
struct ImagesInfo {
    public Sprite[] images;
}

public class Castle : MonoBehaviour {

    public int level;

    private Image image0, image1;
    [SerializeField] private ImagesInfo[] imagesInfos;
    [SerializeField] private Sprite[] upgradeThm;

    [SerializeField] private bool isFading, ifInsufficient = false;

    [SerializeField] private int crCost;
    private GameObject castlePopup;
    private TextMeshProUGUI titleText;
    private Image thmImage, btnImage;
    public TextMeshProUGUI[] goodsTexts = new TextMeshProUGUI[3];
    public bool isEnding = false;

    private void Awake() {
        Game.castle = this;
    }

    void Start() {
        image0 = transform.GetChild(0).GetComponent<Image>();
        image1 = transform.GetChild(1).GetComponent<Image>();

        castlePopup = GameObject.Find("CastlePopup");
        titleText = castlePopup.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        thmImage = castlePopup.transform.GetChild(3).GetComponent<Image>();
        btnImage = castlePopup.transform.GetChild(4).GetComponent<Image>();

        for (int i = 0; i < 3; i++) {
            goodsTexts[i] = castlePopup.transform.GetChild(i + 5).GetComponent<TextMeshProUGUI>();
        }

        UpgradeUIClose();
    }

    public void UpgradeUIOpen() {
        castlePopup.SetActive(true);
        SetPopupInfo();
        CheckLevelUpAvail();
        Game.soundManager.PlaySfx(3);
    }

    public void UpgradeUIClose() {
        castlePopup.SetActive(false);
    }

    public void Upgrade() {
        if (ifInsufficient) return;

        level++;

        if (level == 6) {
            Game.gameManager.endingPopup.SetActive(true);
            Game.gameManager.endingPopup.GetComponent<EndingPopup>().OpenEndingPopup();
            isEnding = true;
            return;
        }

        image0.sprite = imagesInfos[level - 1].images[Game.gameManager.crSec / 20];
        image1.sprite = imagesInfos[level - 1].images[Game.gameManager.crSec / 20];

        for (int i = 0; i < 3; i++) {
            Game.gameManager.goods[i] -= crCost;
        }
        Game.gameManager.SetGoodsText();
        for (int i = 0; i < 3; i++) {
            Game.gauge.levelBtns[i].CheckLocked();
        }

        for (int i = 0; i < Game.levelToSlots[0, Game.castle.level]; i++) {
            Slot crSlot = Game.gameManager.executiveSlots[i].GetComponent<Slot>();
            crSlot.SetImage();
        }
        for (int i = 0; i < Game.levelToSlots[1, Game.castle.level]; i++) {
            Slot crSlot = Game.gameManager.workSlots[i].GetComponent<Slot>();
            crSlot.SetImage();
        }
        for (int i = 0; i < Game.levelToSlots[2, Game.castle.level]; i++) {
            Slot crSlot = Game.gameManager.restSlots[i].GetComponent<Slot>();
            crSlot.SetImage();
        }

        Game.gameManager.crWorkerSlotRemainCnt
            += Game.levelToSlots[1, level] - Game.levelToSlots[1, level - 1];

        Game.gameManager.crResterSlotRemainCnt
            += Game.levelToSlots[2, level] - Game.levelToSlots[2, level - 1];

        SetPopupInfo();
        CheckLevelUpAvail();
        Game.soundManager.PlaySfx(2);
    }

    public void CheckLevelUpAvail() {

        bool ifInsufficient = false;
        for (int i = 0; i < 3; i++) {
            if (Game.gameManager.goods[i] < crCost) {
                goodsTexts[i].color = new(1, 0, 0, 1);
                ifInsufficient = true;
            }
            else goodsTexts[i].color = new(0, 0, 0, 1);
        }

        if (ifInsufficient) {
            for (int i = 0; i < 3; i++) {
                goodsTexts[i].color = new(goodsTexts[i].color.r, goodsTexts[i].color.g, goodsTexts[i].color.b, 0.3f);
            }
            btnImage.color = new(1, 1, 1, 0.3f);
        }
        else {
            for (int i = 0; i < 3; i++) {
                goodsTexts[i].color = new(goodsTexts[i].color.r, goodsTexts[i].color.g, goodsTexts[i].color.b, 1);
            }
            btnImage.color = new(1, 1, 1, 1);
        }

        this.ifInsufficient = ifInsufficient;
    }

    public void SetPopupInfo() {
        crCost = Game.upgradeCost[level - 1];
        for (int i = 0; i < 3; i++) {
            goodsTexts[i].text = crCost + "";
        }
        titleText.text = "Lv." + level + " -> Lv." + (level + 1);
        thmImage.sprite = upgradeThm[level - 1];
    }

    public void ChangeImage(int num) {
        IEnumerator Fade() {
            isFading = true;
            image0.sprite = imagesInfos[level - 1].images[(num + 1) % 2];
            image1.sprite = imagesInfos[level - 1].images[num];
            image0.color = new(1, 1, 1, 1);

            float t = 0, fadeSpeed = 2.5f;
            while (t < 1) {
                image0.sprite = imagesInfos[level - 1].images[(num + 1) % 2];
                image1.sprite = imagesInfos[level - 1].images[num];
                image1.color = new(1, 1, 1, t);
                t += fadeSpeed * Time.deltaTime * Game.gameManager.speedWeight;
                yield return null;
            }

            image1.color = new(1, 1, 1, 1);
            isFading = false;
        }

        StartCoroutine(Fade());
    }
}
