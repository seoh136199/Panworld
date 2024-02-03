using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeBar : MonoBehaviour {

    public float crScale = 0, targetScale = 0, crPos = 0, targetPos = 0;
    private RectTransform myTransform;

    void Start() {
        myTransform = GetComponent<RectTransform>();
    }

    private void FixedUpdate() {
        crScale = crScale * 0.88f + targetScale * 0.12f;
        crPos = crPos * 0.90f + targetPos * 0.1f;
        myTransform.localScale = new(crScale, 100);
        myTransform.anchoredPosition = new(crPos, 0); ;
    }
}
