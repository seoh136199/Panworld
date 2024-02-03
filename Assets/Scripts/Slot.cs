using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour {

    [SerializeField] private int id;
    public bool isFill = false, isLocked = false, isLimited = false;
    public Game.SlotType slotType;
    [SerializeField] private Sprite emptyImage, lockImage, fillImage, limitImage;
    [SerializeField] private Image myImage;
    public Member myMember;

    void Start() {
        if (slotType == Game.SlotType.executive) Game.gameManager.executiveSlots[id] = this;
        else if (slotType == Game.SlotType.work) Game.gameManager.workSlots[id] = this;
        else if (slotType == Game.SlotType.rest) Game.gameManager.restSlots[id] = this;

        myImage = GetComponent<Image>();

        SetImage();
    }

    public void SetImage() {
        if (id >= Game.levelToSlots[(int)slotType, Game.castle.level]) {
            isLocked = true;
            myImage.sprite = lockImage;
        }
        else if (isFill) {
            isLocked = false;
            myImage.sprite = fillImage;
        }
        else {
            isLocked = false;
            myImage.sprite = emptyImage;
        }
    }

    Vector2 GetCombinedAnchoredPosition() {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 myAnchoredPosition = rectTransform.anchoredPosition;
        Vector2 parentAnchoredPosition = (rectTransform.parent as RectTransform).anchoredPosition;
        Vector2 ans = myAnchoredPosition + parentAnchoredPosition;
        return ans;
    }

    public void PutMember(GameObject target) {
        isFill = true;
        myMember = target.GetComponent<Member>();
        myMember.mySlot = this;
        myMember.GetComponent<RectTransform>().anchoredPosition = GetCombinedAnchoredPosition();
        if (slotType == Game.SlotType.work) Game.gameManager.crWorkerSlotRemainCnt--;
        else if (slotType == Game.SlotType.rest) Game.gameManager.crResterSlotRemainCnt--;

        SetImage();
    }

    public void RemoveMember() {
        isFill = false;
        myMember.GetComponent<Member>().mySlot = null;
        myMember = null;
        if (slotType == Game.SlotType.work) Game.gameManager.crWorkerSlotRemainCnt++;
        else if (slotType == Game.SlotType.rest) Game.gameManager.crResterSlotRemainCnt++;

        SetImage();
    }

    public void SetLimited(bool isLimited) {
        if (isFill) return;

        this.isLimited = isLimited;
        if (!isLocked && isLimited) myImage.sprite = limitImage;
        else SetImage();
    }


}
