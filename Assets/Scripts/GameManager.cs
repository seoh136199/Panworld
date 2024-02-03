using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game {

    public static int YEAR_TO_WEEK = 12;
    public static int WEEK_TO_SEC = 40;
    public static int[] BOUNDARY = { 0, 30 };

    public static GameManager gameManager;
    public static BgImage bgImage;
    public static Castle castle;

    public enum Part { programming, design, art }
    public enum MemberType { probationary, regular, honorary }
    public enum SlotType { executive, work, rest }

    public static int[,] levelToSlots = {
        { 0, 0, 0, 3, 3, 3 },
        { 0, 3, 6, 9, 14, 20 },
        { 0, 2, 2, 4, 6, 8 }
    };

    public static int[,] levelToThroughput = { { 0, 5, 10, 15 }, { 0, 10, 15, 20 } };
    public static int[,] levelToBonusThroughput = { { 0, 3, 6, 8 }, { 0, 6, 9, 12 } };
    public static int exePluesThroughput = 15, exePluesBonusThroughput = 9;

    public static int[,] levelToWorktime = { { 0, 2, 4, 6 }, { 0, 4, 6, 8 } };
    public static int exePluesWorktime = 4;

    public static int[] levelToInterval = { 0, 10, 8, 6, 4, 2 };

}

public class GameManager : MonoBehaviour {

    [SerializeField] private int crYear = 0;
    [SerializeField] private int crWeek = 0;
    [SerializeField] private int preSec = 0;
    [SerializeField] private int crSec = 0;
    [SerializeField] private int crSecAfterLevelup = 0;

    [SerializeField] private double timeSec = 0;
    [SerializeField] private int timeWeek = 0;
    [SerializeField] private GameObject memberPrefab;

    public Slot[] executiveSlots = new Slot[3];
    public Slot[] workSlots = new Slot[20];
    public Slot[] restSlots = new Slot[8];

    [SerializeField] private GameObject slotArea;
    [SerializeField] private Member mouseDownMember;
    [SerializeField] private Slot mouseDownSlot;
    [SerializeField] private bool isDraging = false;

    public int crWorkerSlotRemainCnt = 0, crResterSlotRemainCnt = 0;

    [SerializeField] private int[] goods = new int[3] {0, 0, 0};

    [SerializeField] private string nameDebug; //
    [SerializeField] private int levelDebug, pRatioDebug, dRatioDebug, aRatioDebug; //

    private void CalTime() {
        timeSec += Time.deltaTime;
        crSec = (int)timeSec;

        if (crSec >= Game.WEEK_TO_SEC) {
            int deltaWeek = crSec / Game.WEEK_TO_SEC;
            crWeek += deltaWeek;
            timeWeek += deltaWeek;
            crSec -= deltaWeek * Game.WEEK_TO_SEC;
            timeSec -= deltaWeek * Game.WEEK_TO_SEC;
            EveryWeekEvent();
        }
        if (crWeek >= Game.YEAR_TO_WEEK) {
            int deltaYear = crWeek / Game.YEAR_TO_WEEK;
            crYear += deltaYear;
            crWeek -= deltaYear * Game.YEAR_TO_WEEK;
            EveryYearEvent();
        }

        if (preSec == crSec) return;
        EverySecEvent();
        preSec = crSec;
    }

    private void EverySecEvent() {
        void CheckWorkingTime(Slot crSlot, bool isExe) {
            Member crMember = crSlot.myMember;
            if (crMember == null) return;
            if (crMember.isExhausted) return;

            crMember.SetBgBarAnimation(true);
            crMember.workingTime++;
            if (crMember.workingTime >= crMember.maxWorkTime) {
                crMember.isExhausted = true;
            }
        }

        void CheckRestingTime(Slot crSlot) {
            Member crMember = crSlot.myMember;
            if (crMember == null) return;
            if (crMember.isRecovered) return;

            crMember.SetBgBarAnimation(false);
            crMember.restingTime++;
            if (crMember.restingTime >= 10) {
                crMember.isExhausted = false;
                crMember.isRecovered = true;
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
            CheckWorkingTime(crSlot, true);
        }

        for (int i = 0; i < Game.levelToSlots[1, Game.castle.level]; i++) {
            Slot crSlot = Game.gameManager.workSlots[i].GetComponent<Slot>();
            CheckWorkingTime(crSlot, false);
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
    }

    private void EveryYearEvent() {
        Debug.Log("EveryYearEvent()");
    }

    private void TimeBoundaryEvent(int num) {
        Game.bgImage.ChangeImage(num);
        Game.castle.ChangeImage(num);
    }

    [ContextMenu("AddMemberDebug")]
    private void AddMemberDebug() {
        AddMember(nameDebug, levelDebug, pRatioDebug, dRatioDebug, aRatioDebug);
    }

    private void AddMember(string name, int level, int pRatio, int dRatio, int aRatio) {
        GameObject newMember = Instantiate(memberPrefab);
        newMember.transform.SetParent(slotArea.transform);
        newMember.GetComponent<Member>().Init(name, level, timeWeek, pRatio, dRatio, aRatio);

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
    }

    private void MemberClickAndDrop() {
        if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonUp(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (Input.GetMouseButtonDown(0)) {
            if (!hit.collider) return;

            GameObject mouseDownObj = hit.collider.gameObject;
            if (!mouseDownObj.TryGetComponent<Member>(out Member crMember)) return;

            mouseDownMember = crMember;
            mouseDownSlot = crMember.mySlot;
            mouseDownSlot.RemoveMember();
            mouseDownMember.SetDragging(true);

            isDraging = true;
        }
        else if (Input.GetMouseButtonUp(0)) {
            isDraging = false;

            if (mouseDownMember == null) return;

            GameObject mouseUpObj = null;
            if (hit.collider) mouseUpObj = hit.collider.gameObject;

            if (hit.collider == null || !mouseUpObj.TryGetComponent<Slot>(out Slot crSlot)
                || crSlot.isFill || crSlot.isLocked) {
                mouseDownSlot.PutMember(mouseDownMember.gameObject);
            }
            else {
                if (crSlot.slotType == Game.SlotType.work) mouseDownMember.restingTime = 0;
                if (crSlot.slotType == Game.SlotType.work) mouseDownMember.isRecovered = false;

                if ((int)mouseDownSlot.slotType + (int)crSlot.slotType >= 2 
                    && mouseDownSlot.slotType != crSlot.slotType) {
                    mouseDownMember.ResetBgBar(crSlot.slotType == Game.SlotType.work);
                }
                crSlot.PutMember(mouseDownMember.gameObject);
            }
            mouseDownMember.SetDragging(false);
            mouseDownMember = null;
            mouseDownSlot = null;
        }
    }

    private void MemberDrag() {
        if (!isDraging) return;
        Vector3 mousePositionScreen = Input.mousePosition;

        Camera mainCamera = Camera.main;
        Vector3 mousePositionWorld 
            = mainCamera.ScreenToWorldPoint(new Vector3(mousePositionScreen.x, 
            mousePositionScreen.y, 0));

        mousePositionWorld.z = -0.001f;
        mouseDownMember.transform.position = mousePositionWorld;
    }

    private void ProduceGoods() {
        int[] deltaGoods = new int[3] { 0, 0, 0 };

        void ProduceGoodBySlot(Slot crSlot, bool isExe) {
            Member crMember = crSlot.myMember;

            if (crMember == null) return;
            if (crMember.isExhausted) return;
            Game.Part goodsType = crMember.GetGoods();

            deltaGoods[(int)goodsType] += crMember.throughput;
            if (isExe) deltaGoods[(int)goodsType] += Game.exePluesThroughput;
            if (crMember.part == goodsType) {
                deltaGoods[(int)goodsType] += crMember.bonusThroughput;
                if (isExe) deltaGoods[(int)goodsType] += Game.exePluesBonusThroughput;
                //보너스 얻을 땐 효과가 있으면 좋겠는데
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

        string debug = "재화 ";
        for (int i = 0; i < 3; i++) {
            debug += deltaGoods[i] + " ";
            goods[i] += deltaGoods[i];
        }
        debug += "추가됨";
        Debug.Log(debug);
    }

    private void Awake() {
        Game.gameManager = this;    
    }

    void Start() {
        slotArea = GameObject.Find("SlotArea");
        crWorkerSlotRemainCnt = Game.levelToSlots[1, Game.castle.level];
        crResterSlotRemainCnt = Game.levelToSlots[2, Game.castle.level];
    }

    void Update() {
        CalTime();
        MemberClickAndDrop();
        MemberDrag();
    }
}
