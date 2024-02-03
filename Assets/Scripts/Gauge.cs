using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gauge : MonoBehaviour {

    public int maxGoodsCnt = 10, inputUnit = 10, crInputCnt = 0;
    public int[] inputGoodsCnt = new int[3] { 0, 0, 0 };
    public GaugeBar bar1, bar2, bar3;
    public LevelBtn[] levelBtns = new LevelBtn[3];
    public Image[] goodsStanding = new Image[3];
    public GameObject summonBtn;

    public int debugType = 0;

    private void Awake() {
        Game.gauge = this;
    }

    void Start() {
        GameObject gaugeBars = GameObject.Find("GaugeBars");
        bar1 = gaugeBars.transform.GetChild(0).GetComponent<GaugeBar>();
        bar2 = gaugeBars.transform.GetChild(1).GetComponent<GaugeBar>();
        bar3 = gaugeBars.transform.GetChild(2).GetComponent<GaugeBar>();

        GameObject uiBtns = GameObject.Find("UIBtns");
        for (int i = 0; i < 3; i++) {
            levelBtns[i] = uiBtns.transform.GetChild(i + 1).GetComponent<LevelBtn>();
        }

        GameObject goodsStandings = GameObject.Find("GoodsStandings");
        for (int i = 0; i < 3; i++) {
            goodsStanding[i] = goodsStandings.transform.GetChild(i).GetComponent<Image>();
        }

        summonBtn = GameObject.Find("SummonBtn");
    }

    void Update() {
        
    }


    [ContextMenu("InputGoodsDebug")]
    public void InputGoodsDebug() {
        InputGoods((Game.Part)debugType);
    }

    public void InputGoods(Game.Part type) {
        if (crInputCnt >= maxGoodsCnt) return;

        if (crInputCnt == 0) {
            for (int i = 0; i < 3; i++) {
                if (!levelBtns[i].thisLevelOn) levelBtns[i].Deactive();
            }
        }

        crInputCnt++;
        inputGoodsCnt[(int)type] += 1;
        goodsStanding[(int)type].color = new(1, 1, 1, 1);
        SetBars();

        if (crInputCnt == maxGoodsCnt) {
            summonBtn.GetComponent<Image>().enabled = true;
            summonBtn.GetComponent<Button>().enabled = true;
        }
    }

    public void SetBars() {
        float ratio1 = (float)inputGoodsCnt[0] / maxGoodsCnt * 100;
        float ratio2 = (float)inputGoodsCnt[1] / maxGoodsCnt * 100;
        float ratio3 = (float)inputGoodsCnt[2] / maxGoodsCnt * 100;

        bar1.targetScale = ratio1;
        bar2.targetScale = ratio2;
        bar3.targetScale = ratio3;

        bar2.targetPos = ratio1;
        bar3.targetPos = ratio1 + ratio2;
    }

    public void CloseAlterPopup() {
        Game.btnActions.alterPopup.SetActive(false);
    }
}
