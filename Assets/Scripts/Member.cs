using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Member : MonoBehaviour {

    public string myName;
    [SerializeField] private int entryWeek;
    public int level, throughput, bonusThroughput, maxWorkTime;
    public Game.Part part;
    public Game.MemberType memberType;
    [SerializeField] private int pRatio, dRatio, aRatio;
    public int pVisual, dVisual, aVisual;
    public Sprite face;
    [SerializeField] private Sprite sadMask, happyMask;
    public Slot mySlot;
    public int workingTime = 0, restingTime = 0;
    public bool isExhausted = false, isRecovered = false;
    [SerializeField] private float maskWeight = 0;

    private Image myFace, myBg, myMask;
    private BoxCollider2D myCollider;

    public void Init(string name, Sprite avatar, int level, int timeWeek, 
        int pRatio, int dRatio, int aRatio, int pVisual, int dVisual, int aVisual) {
        myBg = transform.GetChild(0).GetComponent<Image>();
        myFace = transform.GetChild(1).GetComponent<Image>();
        myMask = transform.GetChild(2).GetComponent<Image>();
        myCollider = GetComponent<BoxCollider2D>();

        throughput = Game.levelToThroughput[(int)memberType, level];
        bonusThroughput = Game.levelToBonusThroughput[(int)memberType, level];
        maxWorkTime = Game.levelToWorktime[(int)memberType, level] * Game.WEEK_TO_SEC;

        this.myName = name;
        this.level = level;
        this.entryWeek = timeWeek;
        this.face = avatar;

        this.pRatio = pRatio;
        this.dRatio = dRatio;
        this.aRatio = aRatio;
        this.pVisual = pVisual;
        this.dVisual = dVisual;
        this.aVisual = aVisual;

        if (Random.Range(0, 100) < pRatio) {
            part = Game.Part.programming;
        }
        else if (Random.Range(0, dRatio + aRatio) < dRatio) {
            part = Game.Part.design;
        }
        else {
            part = Game.Part.art;
        }

        myFace.sprite = face;
        transform.localScale = new(1, 1, 1);
        transform.transform.position = new(transform.transform.position.x, transform.transform.position.y, -0.001f);
        memberType = Game.MemberType.probationary;

        ResetBgBar(false);
    }

    public void PromotionCheck(int crWeek) {
        if (memberType != Game.MemberType.probationary) return;
        if (crWeek - entryWeek < 12) return;

        memberType = Game.MemberType.regular;

        throughput = Game.levelToThroughput[(int)memberType, level];
        bonusThroughput = Game.levelToBonusThroughput[(int)memberType, level];
        maxWorkTime = Game.levelToWorktime[(int)memberType, level] * Game.WEEK_TO_SEC;
    }

    public void BeExe() {
        throughput = Game.levelToThroughput[(int)memberType, level] + Game.exePluesThroughput;
        bonusThroughput = Game.levelToBonusThroughput[(int)memberType, level] + Game.exePluesBonusThroughput;
        maxWorkTime = (Game.levelToWorktime[(int)memberType, level] + Game.exePluesWorktime) * Game.WEEK_TO_SEC;

        if (isExhausted) workingTime = maxWorkTime;
    }

    public void QuitExe() {
        throughput = Game.levelToThroughput[(int)memberType, level];
        bonusThroughput = Game.levelToBonusThroughput[(int)memberType, level];
        maxWorkTime = Game.levelToWorktime[(int)memberType, level] * Game.WEEK_TO_SEC;

        if (isExhausted) workingTime = maxWorkTime;
    }

    public void ChangeName(string newName) {
        myName = newName;
    }

    public void SetDragging(bool isDragging) {
        if (isDragging) {
            myCollider.enabled = false;
            myFace.color = new(myFace.color.r, myFace.color.g, myFace.color.b, 0.5f);
            myBg.color = new(myBg.color.r, myBg.color.g, myBg.color.b, 0f);
            myMask.color = new(1, 1, 1, 0f);
        }
        else {
            myCollider.enabled = true;
            myFace.color = new(myFace.color.r, myFace.color.g, myFace.color.b, 1);
            myBg.color = new(myBg.color.r, myBg.color.g, myBg.color.b, 1f);
            myMask.color = new(1, 1, 1, 1f);
        }
    }

    public Game.Part GetGoods() {
        if (Random.Range(0, 100) < pRatio) {
            return Game.Part.programming;
        }
        else if (Random.Range(0, dRatio + aRatio) < dRatio) {
            return Game.Part.design;
        }
        else {
            return Game.Part.art;
        }
    }

    public void SetBgBarAnimation(bool isWorking) {
        int crTime, maxTime;
        if (isWorking) { crTime = workingTime; maxTime = maxWorkTime; }
        else { crTime = restingTime; maxTime = 10; }

        float crRatio = Mathf.Min((float)crTime / (maxTime - 1), 1f);
        float nextRatio = Mathf.Min((float)(crTime + 1) / (maxTime - 1), 1f);

        RectTransform targetTransform = myBg.GetComponent<RectTransform>();
        if (isWorking) myBg.GetComponent<Image>().color = new(255f / 255, 84f / 255, 84f / 255);
        else myBg.GetComponent<Image>().color = new(100f / 255, 255f / 255, 81f / 255);

        IEnumerator Anim() {
            float t = 0, fadeSpeed = 1f;
            while (t < 1) {
                targetTransform.localScale = new(1, Mathf.Lerp(crRatio, nextRatio, t), 1);
                t += fadeSpeed * Time.deltaTime * Game.gameManager.speedWeight;
                yield return null;
            }
            targetTransform.localScale = new(1, nextRatio, 1);
        }

        StartCoroutine(Anim());
    }

    public void ResetBgBar(bool isWorking) {
        StopAllCoroutines();

        RectTransform targetTransform = myBg.GetComponent<RectTransform>();

        int crTime, maxTime;
        if (isWorking) { crTime = workingTime; maxTime = maxWorkTime; }
        else { crTime = restingTime; maxTime = 10; }

        if (isWorking && crTime >= maxTime) {
            crTime = maxTime;
            isExhausted = true;
        }

        float crRatio = Mathf.Min((float)crTime / (maxTime - 1), 1f);

        if (isWorking) myBg.GetComponent<Image>().color = new(255f / 255, 84f / 255, 84f / 255);
        else myBg.GetComponent<Image>().color = new(100f / 255, 255f / 255, 81f / 255);

        targetTransform.localScale = new(1, crRatio, 1);
    }


    void Start() {

    }

    IEnumerator SetMaskWeight() {
        float t = 0, fadeSpeed = 5f;
        while (t < 1) {
            maskWeight = t;
            t += fadeSpeed * Time.deltaTime * Game.gameManager.speedWeight;
            yield return null;
        }
        maskWeight = 1;
    }

    public void SetExhausted() {
        isExhausted = true;
        isRecovered = false;
        myMask.sprite = sadMask;
        StartCoroutine(SetMaskWeight());
    }

    public void SetRecovered() {
        isRecovered = true;
        isExhausted = false;
        myMask.sprite = happyMask;
        StartCoroutine(SetMaskWeight());
    }

    private void SetMask() {
        bool maskOn = isExhausted && mySlot && mySlot.slotType != Game.SlotType.rest;
        maskOn |= isRecovered && mySlot && mySlot.slotType == Game.SlotType.rest;

        if (maskOn) {
            float t = (float)Game.gameManager.timeSecDouble * 1.5f;
            myMask.color = new(1, 1, 1, (Mathf.Sin(t) * Mathf.Sin(t) * 0.7f + 0.1f) * maskWeight);
        }
        else {
            myMask.color = new(1, 1, 1, 0);
        }
    }

    void Update() {
        SetMask();
    }
}
