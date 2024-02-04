using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailArea : MonoBehaviour {

    private Image Portrait;
    [SerializeField] private RectTransform[] bar = new RectTransform[3];
    private Member crMember;

    private void Start() {
        Portrait = transform.GetChild(1).GetComponent<Image>();
        for (int i = 0; i < 3; i++) {
            bar[i] = transform.GetChild(i + 2).GetComponent<RectTransform>();
        }
    }

    public void SetInfo(Member crMember) {
        this.crMember = crMember;
        Portrait.sprite = crMember.face;
        bar[0].sizeDelta = new(crMember.pVisual, 100);
        bar[1].sizeDelta = new(crMember.dVisual, 100);
        bar[2].sizeDelta = new(crMember.aVisual, 100);
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
    }

}
