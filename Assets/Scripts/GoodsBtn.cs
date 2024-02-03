using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsBtn : MonoBehaviour {

    public Game.Part type;

    public void OnClick() {
        Game.gauge.InputGoods(type);
    }
}
