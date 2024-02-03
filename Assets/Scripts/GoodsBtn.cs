using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoodsBtn : MonoBehaviour {

    public Game.Part type;
    private int crUnit = 10;
    private bool isActiveByUnit = true, isActiveBySummon = true;
    private Image myImage;
    private TextMeshProUGUI myText;

    private void Start() {
        myImage = GetComponent<Image>();
        Debug.Log(transform.childCount);
        myText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        CheckUnitAvail();
    }

    public void OnClick() {
        if (!isActiveByUnit || !isActiveBySummon) return;
        Game.gauge.InputGoods(type);
        Game.gameManager.goods[(int)type] -= crUnit;
        Debug.Log(crUnit);

        CheckUnitAvail();
        Game.castle.CheckLevelUpAvail();
        Game.gameManager.SetGoodsText();
    }

    public void SetUnit(int unit) {
        crUnit = unit;
        myText.text = unit + "";
        CheckUnitAvail();
    }

    public void CheckUnitAvail() {
        if (Game.gameManager.goods[(int)type] < crUnit) {
            myText.color = new(1, 0, 0, myText.color.a);
            DeactiveByUnit();
        }
        else {
            myText.color = new(1, 1, 1, myText.color.a);
            SetActiveByUnit();
        }
    }

    public void SetActiveByUnit() {
        isActiveByUnit = true;
        if (isActiveBySummon) myImage.color = new(1, 1, 1, 1);
        if (isActiveBySummon) myText.color = new(myText.color.r, myText.color.g, myText.color.b, 1);
    }
    public void SetActiveBySummon() {
        isActiveBySummon = true;
        if (isActiveByUnit) myImage.color = new(1, 1, 1, 1);
        if (isActiveByUnit) myText.color = new(myText.color.r, myText.color.g, myText.color.b, 1);
    }

    public void DeactiveByUnit() {
        isActiveByUnit = false;
        myImage.color = new(1, 1, 1, 0.3f);
        myText.color = new(myText.color.r, myText.color.g, myText.color.b, 0.3f);
    }

    public void DeactiveBySummon() {
        isActiveBySummon = false;
        myImage.color = new(1, 1, 1, 0.3f);
        myText.color = new(myText.color.r, myText.color.g, myText.color.b, 0.3f);
    }
}
