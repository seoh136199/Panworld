using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Game {

    public static int YEAR_TO_WEEK = 12;
    public static int WEEK_TO_SEC = 40;
    public static int[] BOUNDARY = { 0, 20 };

    public static GameManager gameManager;
    public static BgImage bgImage;
    public static Castle castle;
    public static BtnActions btnActions;
    public static Gauge gauge;
    public static GetPopup getPopup1, getPopup2;
    public static SoundManager soundManager;

    public enum Part { programming, design, art }
    public enum MemberType { probationary, regular, honorary }
    public enum SlotType { executive, work, rest }

    public static int[,] levelToSlots = {
        { 0, 0, 0, 3, 3, 3 },
        { 0, 3, 6, 9, 14, 20 },
        { 0, 2, 2, 4, 6, 8 }
    };

    public static int[,] levelToThroughput = { { 0, 5, 8, 11 }, { 0, 7, 10, 13 } };
    public static int[,] levelToBonusThroughput = { { 0, 1, 2, 3 }, { 0, 2, 3, 4 } };
    public static int exePluesThroughput = 3, exePluesBonusThroughput = 1;

    public static int[,] levelToWorktime = { { 0, 2, 4, 6 }, { 0, 4, 6, 8 } };
    public static int exePluesWorktime = 4;

    public static int[] levelToInterval = { 0, 10, 8, 6, 4, 2 };

    public static int[] upgradeCost = { 80, 300, 1000, 3800, 10000 };

}

public class GameManager : MonoBehaviour {

    public int crYear = 0;
    public int crWeek = 0;
    [SerializeField] private int preSec = 0, preWeek;
    public int crSec = 0;
    [SerializeField] private int crSecAfterLevelup = 0;
    [SerializeField] private double crSecDouble = 0;

    public double timeSecDouble = 0;
    public int timeWeek = 0;
    [SerializeField] private GameObject memberPrefab;

    public Slot[] executiveSlots = new Slot[3];
    public Slot[] workSlots = new Slot[20];
    public Slot[] restSlots = new Slot[8];

    [SerializeField] private GameObject slotArea, detailArea;
    public GameObject endingPopup;
    [SerializeField] private Member mouseDownMember, mouseUpMember, detailMember;
    [SerializeField] private Slot mouseUpSlot, mouseDownSlot;
    [SerializeField] private bool isDraging = false, isDetailAreaOn = false;
    private TextMeshProUGUI[] goodsTexts = new TextMeshProUGUI[3];
    private TextMeshProUGUI dateText;

    public int inCount = 0, outCount = 0;

    public int crWorkerSlotRemainCnt = 0, crResterSlotRemainCnt = 0;

    public int[] goods = new int[3] {0, 0, 0};
    public int[] goodsTotal = new int[3] {0, 0, 0};

    public int speedWeight = 1;

    [SerializeField] private string nameDebug; //
    [SerializeField] private int levelDebug, pRatioDebug, dRatioDebug, aRatioDebug; //

    private void CalTime() {
        if (Game.castle.isEnding) return;

        timeSecDouble += Time.deltaTime * speedWeight;
        crSecDouble += Time.deltaTime * speedWeight;
        crSec = (int)crSecDouble;

        if (crSec >= Game.WEEK_TO_SEC) {
            int deltaWeek = crSec / Game.WEEK_TO_SEC;
            crWeek += deltaWeek;
            timeWeek += deltaWeek;
            crSec -= deltaWeek * Game.WEEK_TO_SEC;
            crSecDouble -= deltaWeek * Game.WEEK_TO_SEC;
        }
        if (crWeek >= Game.YEAR_TO_WEEK) {
            int deltaYear = crWeek / Game.YEAR_TO_WEEK;
            crYear += deltaYear;
            crWeek -= deltaYear * Game.YEAR_TO_WEEK;
        }

        if (preSec == crSec) return;
        else {
            EverySecEvent();
            preSec = crSec;
        }

        if (preWeek == timeWeek) return;
        else {
            EveryWeekEvent();
            preWeek = timeWeek;
        }
        
    }

    private void EverySecEvent() {
        void CheckWorkingTime(Slot crSlot) {
            Member crMember = crSlot.myMember;
            if (crMember == null) return;
            if (crMember.isExhausted) return;

            crMember.SetBgBarAnimation(true);
            crMember.workingTime++;
            if (crMember.workingTime >= crMember.maxWorkTime) {
                crMember.SetExhausted();
            }
        }

        void CheckRestingTime(Slot crSlot) {
            Member crMember = crSlot.myMember;
            if (crMember == null) return;
            if (crMember.isRecovered) return;

            crMember.SetBgBarAnimation(false);
            crMember.restingTime++;
            if (crMember.restingTime >= 10) {
                crMember.SetRecovered();
                crMember.workingTime = 0;
            }
        }

        for (int i = 0; i < Game.BOUNDARY.Length; i++) {
            if (crSec != Game.BOUNDARY[i]) continue;
            TimeBoundaryEvent(i);
            break;
        }

        crSecAfterLevelup += 1;
        if ((crSecAfterLevelup % Game.levelToInterval[Game.castle.level]) == 0) {
            ProduceGoods();
        }

        for (int i = 0; i < Game.levelToSlots[0, Game.castle.level]; i++) {
            Slot crSlot = Game.gameManager.executiveSlots[i].GetComponent<Slot>();
            CheckWorkingTime(crSlot);
        }

        for (int i = 0; i < Game.levelToSlots[1, Game.castle.level]; i++) {
            Slot crSlot = Game.gameManager.workSlots[i].GetComponent<Slot>();
            CheckWorkingTime(crSlot);
        }

        for (int i = 0; i < Game.levelToSlots[2, Game.castle.level]; i++) {
            Slot crSlot = Game.gameManager.restSlots[i].GetComponent<Slot>();
            CheckRestingTime(crSlot);
        }
    }

    private void EveryWeekEvent() {
        var members = GameObject.FindGameObjectsWithTag("Member");
        foreach (var member in members){
            member.GetComponent<Member>().PromotionCheck(timeWeek);
        }
        SetDateText();

        if (isDetailAreaOn) SetDetailAreaOn(detailMember);
    }

    //private void EveryYearEvent() {
    //    Debug.Log("EveryYearEvent()");
    //}

    private void TimeBoundaryEvent(int num) {
        Game.bgImage.ChangeImage(num);
        Game.castle.ChangeImage(num);
    }

    //[ContextMenu("AddMemberDebug")]
    //private void AddMemberDebug() {
    //    AddMember(nameDebug, levelDebug, pRatioDebug, dRatioDebug, aRatioDebug, pRatioDebug, dRatioDebug, aRatioDebug);
    //}

    public void AddMember(string name, Sprite avatar, int level, int pRatio, int dRatio, int aRatio, int pVisual, int dVisual, int aVisual) {
        GameObject newMember = Instantiate(memberPrefab);
        newMember.transform.SetParent(slotArea.transform);
        newMember.GetComponent<Member>().Init(name, avatar, level, timeWeek, pRatio, dRatio, aRatio, pVisual, dVisual, aVisual);

        if (crWorkerSlotRemainCnt > 0) {
            for (int i = 0; i < Game.levelToSlots[1, Game.castle.level]; i++) {
                Slot crSlot = Game.gameManager.workSlots[i].GetComponent<Slot>();
                if (crSlot.isFill || crSlot.isLocked) continue;
                crSlot.PutMember(newMember);
                break;
            }
        }
        else {
            for (int i = 0; i < Game.levelToSlots[2, Game.castle.level]; i++) {
                Slot crSlot = Game.gameManager.restSlots[i].GetComponent<Slot>();
                if (crSlot.isFill || crSlot.isLocked) continue;
                crSlot.PutMember(newMember);
                break;
            }
        }

        inCount++;
    }

    private void MemberClickAndDrop() {
        if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonUp(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (Input.GetMouseButtonDown(0)) { //Mouse Down
            if (!hit.collider) return;

            GameObject mouseDownObj = hit.collider.gameObject;
            if (!mouseDownObj.TryGetComponent<Member>(out Member crMember)) return;

            mouseDownMember = crMember;
            mouseDownSlot = crMember.mySlot;
            mouseDownSlot.RemoveMember();
            mouseDownMember.SetDragging(true);

            isDraging = true;
            SetDetailAreaOff();

            if (mouseDownMember.memberType == Game.MemberType.probationary) {
                for (int i = 0; i < Game.levelToSlots[0, Game.castle.level]; i++) {
                    Slot crSlot = Game.gameManager.executiveSlots[i].GetComponent<Slot>();
                    crSlot.SetLimited(true);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0)) { //Mouse Up
            isDraging = false;

            if (mouseDownMember == null) return;

            mouseDownMember.transform.parent = slotArea.transform;

            GameObject mouseUpObj = null;
            if (hit.collider) mouseUpObj = hit.collider.gameObject;

            bool isSlot = false, isMember = false;
            if (mouseUpObj) {
                isSlot = mouseUpObj.TryGetComponent(out mouseUpSlot);
                isMember = mouseUpObj.TryGetComponent(out mouseUpMember);
            }

            if (!mouseUpSlot && mouseUpMember) mouseUpSlot = mouseUpMember.mySlot;

            if (mouseUpObj == null || (!isSlot && !isMember)
                || mouseUpSlot.isLocked || mouseUpSlot.isLimited) {
                mouseDownSlot.PutMember(mouseDownMember.gameObject);
            }
            else {

                //노동 슬롯으로 이동한 경우 휴식 초기화
                if ((int)mouseUpSlot.slotType <= 1) mouseDownMember.restingTime = 0;
                if ((int)mouseUpSlot.slotType <= 1) mouseDownMember.isRecovered = false;
                if (mouseUpMember) {
                    if ((int)mouseDownSlot.slotType <= 1) mouseUpMember.restingTime = 0;
                    if ((int)mouseDownSlot.slotType <= 1) mouseUpMember.isRecovered = false;
                }

                //임원진으로의 설정
                if (mouseUpSlot.slotType == Game.SlotType.executive) mouseDownMember.BeExe();
                else mouseDownMember.QuitExe();
                if (mouseUpMember) {
                    if (mouseDownSlot.slotType == Game.SlotType.executive) mouseUpMember.BeExe();
                    else mouseUpMember.QuitExe();
                }
                
                //슬롯 옵션 바뀐 경우 프로그래스 바 초기화
                if (mouseDownSlot.slotType != mouseUpSlot.slotType) {
                    mouseDownMember.ResetBgBar((int)mouseUpSlot.slotType <= 1);
                    if (mouseUpMember) mouseUpMember.ResetBgBar((int)mouseDownSlot.slotType <= 1);
                }

                if (mouseUpSlot.isFill) {
                    Member preMember = mouseUpSlot.myMember;
                    mouseUpSlot.RemoveMember();
                    mouseUpSlot.PutMember(mouseDownMember.gameObject);
                    mouseDownSlot.PutMember(preMember.gameObject);
                }
                else {
                    mouseUpSlot.PutMember(mouseDownMember.gameObject);
                }
            }
            mouseDownMember.SetDragging(false);

            if (mouseDownMember.memberType == Game.MemberType.probationary) {
                for (int i = 0; i < Game.levelToSlots[0, Game.castle.level]; i++) {
                    Slot crSlot = Game.gameManager.executiveSlots[i].GetComponent<Slot>();
                    crSlot.SetLimited(false);
                }
            }

            mouseDownMember = null;
            mouseDownSlot = null;
        }
    }

    private void MemberDrag() {
        if (!isDraging) return;

        mouseDownMember.transform.parent = slotArea.transform.parent;
        Vector3 mousePositionScreen = Input.mousePosition;

        Camera mainCamera = Camera.main;
        Vector3 mousePositionWorld 
            = mainCamera.ScreenToWorldPoint(new Vector3(mousePositionScreen.x, 
            mousePositionScreen.y, 0));

        mousePositionWorld.z = -0.001f;
        mouseDownMember.transform.position = mousePositionWorld;
    }

    private void SetDetailArea() {
        if (!Input.GetMouseButtonDown(1)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (!hit.collider) return;
        GameObject mouseDownObj = hit.collider.gameObject;
        if (!mouseDownObj.TryGetComponent<Member>(out Member crMember)) return;

        if (!isDetailAreaOn) SetDetailAreaOn(crMember);
        else if (detailMember != crMember) SetDetailAreaOn(crMember);
        else SetDetailAreaOff();
    }

    private void ProduceGoods() {
        int[] deltaGoods = new int[3] { 0, 0, 0 };

        void ProduceGoodBySlot(Slot crSlot, bool isExe) {
            Member crMember = crSlot.myMember;

            if (crMember == null) return;
            if (crMember.isExhausted) return;
            Game.Part goodsType = crMember.GetGoods();

            deltaGoods[(int)goodsType] += crMember.throughput;
            if (crMember.part == goodsType) {
                deltaGoods[(int)goodsType] += crMember.bonusThroughput;
            }
        }

        for (int i = 0; i < Game.levelToSlots[0, Game.castle.level]; i++) {
            Slot crSlot = Game.gameManager.executiveSlots[i].GetComponent<Slot>();
            ProduceGoodBySlot(crSlot, true);
        }

        for (int i = 0; i < Game.levelToSlots[1, Game.castle.level]; i++) {
            Slot crSlot = Game.gameManager.workSlots[i].GetComponent<Slot>();
            ProduceGoodBySlot(crSlot, false);
        }

        //string debug = "재화 ";
        for (int i = 0; i < 3; i++) {
            //debug += deltaGoods[i] + " ";
            goods[i] += deltaGoods[i];
            goodsTotal[i] += deltaGoods[i];
        }
        //debug += "추가됨";
        //Debug.Log(debug);

        Game.gauge.CheckGoodsBtnAvail();
        Game.castle.CheckLevelUpAvail();
        SetGoodsText();
        Game.getPopup1.Appear(deltaGoods[0], deltaGoods[1], deltaGoods[2]);
    }

    public void SetGoodsText() {
        for (int i = 0; i < 3; i++) goodsTexts[i].text = goods[i] + "";
    }

    public void SetDateText() {
        dateText.text = (crYear + 1) + "년 " + (crWeek + 1) + "주차";
    }

    public void SetDetailAreaOn(Member crMember) {
        detailMember = crMember;
        isDetailAreaOn = true;
        detailArea.SetActive(true);
        detailArea.GetComponent<DetailArea>().SetInfo(crMember);
    }

    public void SetDetailAreaOff() {
        isDetailAreaOn = false;
        detailArea.SetActive(false);
    }

    private void Awake() {
        Game.gameManager = this;    
    }

    void Start() {
        slotArea = GameObject.Find("SlotArea");
        detailArea = GameObject.Find("DetailArea");
        endingPopup = GameObject.Find("EndingPopup");
        endingPopup.SetActive(false);

        SetDetailAreaOff();
        crWorkerSlotRemainCnt = Game.levelToSlots[1, Game.castle.level];
        crResterSlotRemainCnt = Game.levelToSlots[2, Game.castle.level];

        GameObject mainInfo = GameObject.Find("MainInfo");
        for (int i = 0; i < 3; i++) {
            goodsTexts[i] = mainInfo.transform.GetChild(i + 1).GetComponent<TextMeshProUGUI>();
        }

        dateText = mainInfo.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        SetGoodsText();

        Game.soundManager.StartBgm(0);
    }

    void Update() {
        MemberClickAndDrop();
        MemberDrag();
        CalTime();
        SetDetailArea();
    }
}
