using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummonBtn : MonoBehaviour {



    public void Summon() {
        Debug.Log("��ȯ ��ư ����");

        //��ȯ ����

        Game.gauge.crInputCnt = 0;
        for (int i = 0; i < 3; i++) Game.gauge.inputGoodsCnt[i] = 0;

        Game.gauge.bar1.targetScale = 0;
        Game.gauge.bar2.targetScale = 0;
        Game.gauge.bar3.targetScale = 0;
        Game.gauge.bar2.targetPos = 0;
        Game.gauge.bar3.targetPos = 0;

        for (int i = 0; i < 3; i++) {
            Game.gauge.levelBtns[i].SetActive();
            Game.gauge.goodsStanding[i].color = new(1, 1, 1, 0);
        }

        GetComponent<Image>().enabled = false;
        GetComponent<Button>().enabled = false;
    }
}
