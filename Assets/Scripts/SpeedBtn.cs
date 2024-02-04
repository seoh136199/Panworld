using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedBtn : MonoBehaviour {
    
    public Sprite[] images = new Sprite[2];

    public void OnClick() {
        if (Game.gameManager.speedWeight == 1) {
            Game.gameManager.speedWeight = 2;
            GetComponent<Image>().sprite = images[1];
        }
        else {
            Game.gameManager.speedWeight = 1;
            GetComponent<Image>().sprite = images[0];
        }
    }

}
