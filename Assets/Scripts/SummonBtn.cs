using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SummonBtn : MonoBehaviour {

    private GameObject result;
    private void Start() {
        result = GameObject.Find("Result");
    }

    public void Summon() {
        Debug.Log("소환 버튼 누름");

        int pCnt = Game.gauge.inputGoodsCnt[0] * Game.gauge.inputUnit + Random.Range(-30, 40 + 1);
        int dCnt = Game.gauge.inputGoodsCnt[1] * Game.gauge.inputUnit + Random.Range(-30, 40 + 1);
        int aCnt = Game.gauge.inputGoodsCnt[2] * Game.gauge.inputUnit + Random.Range(-30, 40 + 1);
        pCnt = Mathf.Max(0, pCnt);
        dCnt = Mathf.Max(0, dCnt);
        aCnt = Mathf.Max(0, aCnt);

        Debug.Log(pCnt + ", " + dCnt + ", " + aCnt);

        int[] cntNew = new int[3];
        cntNew[0] = (int)((float)pCnt / (pCnt + dCnt + aCnt) * 100);
        cntNew[1] = (int)((float)dCnt / (pCnt + dCnt + aCnt) * 100);
        cntNew[2] = (int)((float)aCnt / (pCnt + dCnt + aCnt) * 100);

        int loss = 100 - cntNew[0] - cntNew[1] - cntNew[2];
        cntNew[Random.Range(0, 3)] += loss;

        Debug.Log(cntNew[0] + ", " + cntNew[1] + ", " + cntNew[2]);

        result.SetActive(true);
        result.GetComponent<Result>().SetInfo(cntNew[0], cntNew[1], cntNew[2], Game.gauge.crLevel);

        Game.gauge.crInputCnt = 0;
        for (int i = 0; i < 3; i++) Game.gauge.inputGoodsCnt[i] = 0;

        Game.gauge.bar1.targetScale = 0;
        Game.gauge.bar2.targetScale = 0;
        Game.gauge.bar3.targetScale = 0;
        Game.gauge.bar2.targetPos = 0;
        Game.gauge.bar3.targetPos = 0;

        for (int i = 0; i < 3; i++) {
            Game.gauge.levelBtns[i].SetActive();
            Game.gauge.goodsStandings[i].color = new(1, 1, 1, 0);
        }

        GetComponent<Image>().enabled = false;
        GetComponent<Button>().enabled = false;
    }
}
