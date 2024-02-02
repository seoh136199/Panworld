using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour {

    [SerializeField] private int id;
    public bool isFill = false, isLocked = false;
    [SerializeField] private Game.SlotType slotType;
    [SerializeField] private Sprite emptyImage, lockImage, fillImage;
    [SerializeField] private Image myImage;
    [SerializeField] private GameObject myMember;

    void Start() {
        if (slotType == Game.SlotType.executive) Game.gameManager.executiveSlots[id] = this;
        else if (slotType == Game.SlotType.work) Game.gameManager.workSlots[id] = this;
        else if (slotType == Game.SlotType.rest) Game.gameManager.restSlots[id] = this;

        myImage = GetComponent<Image>();

        SetImage();
    }

    private void SetImage() {
        if (id >= Game.gameManager.levelToSlots[(int)slotType, Game.castle.level]) {
            isLocked = true;
            myImage.sprite = lockImage;
        }
        else if (isFill) {
            myImage.sprite = fillImage;
        }
        else {
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
        myMember = target;
        target.GetComponent<RectTransform>().anchoredPosition = GetCombinedAnchoredPosition();
        target.GetComponent<Member>().mySlot = this;
        //타겟 투명도 복구
        Game.gameManager.crWorkerSlotRemainCnt--;
    }

    public void RemoveMember() {
        isFill = false;
        myMember.GetComponent<Member>().mySlot = null;
        myMember = null;
        Game.gameManager.crWorkerSlotRemainCnt++;
    }

    void Update() {
        
    }


}
