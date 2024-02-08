using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingPopup : MonoBehaviour {

    public Sprite[] underSprite = new Sprite[2];

    public void OpenEndingPopup() {
        if (Game.gameManager.crYear >= 4) transform.GetChild(1).GetComponent<Image>().sprite = underSprite[1];
        else transform.GetChild(1).GetComponent<Image>().sprite = underSprite[0];

        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text 
            = (Game.gameManager.crYear + 1) + "년 " + (Game.gameManager.crWeek + 1) + "주";

        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = Game.gameManager.goodsTotal[0] + "";
        transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = Game.gameManager.goodsTotal[1] + "";
        transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = Game.gameManager.goodsTotal[2] + "";

        transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = Game.gameManager.inCount + "명";
        transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = Game.gameManager.outCount + "명";
    }

}
