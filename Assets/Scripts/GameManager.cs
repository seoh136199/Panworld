using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game {
    public static GameManager gameManager;
    public static BgImage bgImage;
    public static Castle castle;

    public enum Part {
        programming, design, art
    }

    public enum MemberType {
        probationary, regular, honorary
    }

    public enum SlotType {
        executive, work, rest
    }

}

public class GameManager : MonoBehaviour {

    [SerializeField] private const int YEAR_TO_WEEK = 12;
    [SerializeField] private const int WEEK_TO_SEC = 40;
    [SerializeField] private readonly int[] BOUNDARY = { 0, 30 };
    [SerializeField] private int crYear = 0;
    [SerializeField] private int crWeek = 0;
    [SerializeField] private int preSec = 0;
    [SerializeField] private int crSec = 0;

    [SerializeField] private double timeSec = 0;
    [SerializeField] private int timeWeek = 0;
    [SerializeField] private GameObject memberPrefab;

    public Slot[] executiveSlots = new Slot[3];
    public Slot[] workSlots = new Slot[20];
    public Slot[] restSlots = new Slot[8];

    [SerializeField] private GameObject slotArea;

    public int crWorkerSlotRemainCnt = 0, crResterSlotRemainCnt = 0;

    public int[,] levelToSlots = {
        { 0, 0, 0, 3, 3, 3 },
        { 0, 3, 6, 9, 14, 20 },
        { 0, 2, 2, 4, 6, 8 }
    };

    [SerializeField] private string nameDebug; //
    [SerializeField] private int pRatioDebug, dRatioDebug, aRatioDebug; //

private void CalTime() {
        timeSec += Time.deltaTime;
        crSec = (int)timeSec;

        if (crSec >= WEEK_TO_SEC) {
            int deltaWeek = crSec / WEEK_TO_SEC;
            crWeek += deltaWeek;
            timeWeek += deltaWeek;
            crSec -= deltaWeek * WEEK_TO_SEC;
            timeSec -= deltaWeek * WEEK_TO_SEC;
        }
        if (crWeek >= YEAR_TO_WEEK) {
            int deltaYear = crWeek / YEAR_TO_WEEK;
            crYear += deltaYear;
            crWeek -= deltaYear * YEAR_TO_WEEK;
        }

        for (int i = 0; i < BOUNDARY.Length; i++) {
            if (preSec == crSec) continue;
            if (crSec != BOUNDARY[i]) continue;
            TimeBoundaryEvent(i);
            break;
        }
        preSec = crSec;
    }

    private void TimeBoundaryEvent(int num) {
        Game.bgImage.ChangeImage(num);
        Game.castle.ChangeImage(num);
    }

    [ContextMenu("AddMemberDebug")]
    private void AddMemberDebug() {
        AddMember(nameDebug, pRatioDebug, dRatioDebug, aRatioDebug);
    }

    private void AddMember(string name, int pRatio, int dRatio, int aRatio) {
        GameObject newMember = Instantiate(memberPrefab);
        newMember.transform.parent = slotArea.transform;
        newMember.GetComponent<Member>().Init(name, timeWeek, pRatio, dRatio, aRatio);

        if (crWorkerSlotRemainCnt > 0) {
            for (int i = 0; i < Game.gameManager.levelToSlots[1, Game.castle.level]; i++) {
                Slot crSlot = Game.gameManager.workSlots[i].GetComponent<Slot>();
                if (crSlot.isFill || crSlot.isLocked) continue;
                crSlot.SetMember(newMember);
                break;
            }
        }
        else {
            for (int i = 0; i < Game.gameManager.levelToSlots[2, Game.castle.level]; i++) {
                Slot crSlot = Game.gameManager.restSlots[i].GetComponent<Slot>();
                if (crSlot.isFill || crSlot.isLocked) continue;
                crSlot.SetMember(newMember);
                break;
            }
        }
    }

    private void Awake() {
        Game.gameManager = this;    
    }

    void Start() {
        slotArea = GameObject.Find("SlotArea");
        crWorkerSlotRemainCnt = Game.gameManager.levelToSlots[1, Game.castle.level];
        crResterSlotRemainCnt = Game.gameManager.levelToSlots[2, Game.castle.level];
    }

    void Update() {
        CalTime();
    }
}
