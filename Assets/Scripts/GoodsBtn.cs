using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoodsBtn : MonoBehaviour {

    public Game.Part type;
    private bool isActive = true;
    private Image myImage;
    private void Start() {
        myImage = GetComponent<Image>();
    }

    public void OnClick() {
        if (!isActive) return;
        Game.gauge.InputGoods(type);
    }

    public void SetUnit(int unit) {
        
    }

    public void SetUnitAvail() {

    }

    public void SetActive() {
        isActive = true;
        myImage.color = new(1, 1, 1, 1);
    }

    public void Deactive() {
        isActive = false;
        myImage.color = new(1, 1, 1, 0.3f);
    }
}
