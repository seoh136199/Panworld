using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnActions : MonoBehaviour {

    private bool isMooving = false, isWorkPopupOn;
    public GameObject alterPopup;
    private RectTransform workPopupTransform;

    private void Awake() {
        Game.btnActions = this;
    }

    private void Start() {
        workPopupTransform = GameObject.Find("WorkPopup").GetComponent<RectTransform>();
        alterPopup = GameObject.Find("AlterPopup");
        alterPopup.SetActive(false);
    }


    public void ToggleWorkPopup() {
        void SetWorkPopupOn() {
            float Func(float t) {
                return Mathf.Sin(t * (Mathf.PI * 0.57f)) / Mathf.Sin(Mathf.PI * 0.57f);
            }

            IEnumerator StartOpenAnimation() {
                isMooving = true;

                float t = 0, openSpeed = 4.2f;
                while (t < 1) {
                    workPopupTransform.localScale = new(Func(t) * 0.9f, Func(t) * 0.9f, 1);
                    t += openSpeed * Time.deltaTime;
                    yield return null;
                }
                workPopupTransform.localScale = new(0.9f, 0.9f, 1);
                isMooving = false;
            }

            StartCoroutine(StartOpenAnimation());
            isWorkPopupOn = true;
        }

        void SetWorkPopupOff() {
            float Func(float t) {
                return Mathf.Sin(t * (Mathf.PI * 0.5f));
            }

            IEnumerator StartCloseAnimation() {
                isMooving = true;

                float t = 0, openSpeed = 5.5f;
                while (t < 1) {
                    workPopupTransform.localScale = new(Func(1 - t) * 0.9f, Func(1 - t) * 0.9f, 1);
                    t += openSpeed * Time.deltaTime;
                    yield return null;
                }
                workPopupTransform.localScale = new(0, 0, 1);
                isMooving = false;
                Game.gameManager.SetDetailAreaOff();
            }

            StartCoroutine(StartCloseAnimation());
            isWorkPopupOn = false;
        }

        if (isMooving) return;

        if (isWorkPopupOn) SetWorkPopupOff();
        else SetWorkPopupOn();
    }

    public void OpenAlterPopup() {
        Game.btnActions.alterPopup.SetActive(true);
    }

}
