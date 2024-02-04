using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DetailArea : MonoBehaviour {

    private Image Portrait;
    [SerializeField] private RectTransform[] bar = new RectTransform[3];
    private Member crMember;

    private void Start() {
        //Portrait = transform.GetChild(1).GetComponent<Image>();
        //for (int i = 0; i < 3; i++) {
        //    bar[i] = transform.GetChild(i + 2).GetComponent<RectTransform>();
        //}
    }

    public void SetInfo(Member crMember) {
        Portrait = transform.GetChild(1).GetComponent<Image>();
        for (int i = 0; i < 3; i++) {
            bar[i] = transform.GetChild(i + 2).GetComponent<RectTransform>();
        }

        this.crMember = crMember;
        Portrait.sprite = crMember.face;
        bar[0].sizeDelta = new(crMember.pVisual, 100);
        bar[1].sizeDelta = new(crMember.dVisual, 100);
        bar[2].sizeDelta = new(crMember.aVisual, 100);

        string text = "이름: " + crMember.myName + "\r\n";
        if (crMember.part == Game.Part.programming) text += "파트: 프로그래밍\r\n";
        if (crMember.part == Game.Part.design) text += "파트: 디자인\r\n";
        if (crMember.part == Game.Part.art) text += "파트: 아트\r\n";

        if (crMember.memberType == Game.MemberType.probationary) text += "분류: 수습회원\r\n";
        if (crMember.memberType == Game.MemberType.regular) text += "파트: 정회원\r\n";

        text += "레벨: " + crMember.level + "\r\n";
        transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = text;
    }

    public void Kill() {
        crMember.mySlot.RemoveMember();
        Game.gameManager.goods[0] += (int)(crMember.pVisual * crMember.level * 0.7f);
        Game.gameManager.goods[1] += (int)(crMember.dVisual * crMember.level * 0.7f);
        Game.gameManager.goods[2] += (int)(crMember.aVisual * crMember.level * 0.7f);
        Destroy(crMember.gameObject);
        Game.gameManager.SetGoodsText();
        Game.gameManager.SetDetailAreaOff();

        Game.getPopup2.Appear((int)(crMember.pVisual * crMember.level * 0.7f),
            (int)(crMember.dVisual * crMember.level * 0.7f),
            (int)(crMember.aVisual * crMember.level * 0.7f));

        Game.soundManager.PlaySfx(1);

        Game.gameManager.outCount++;
    }

}
