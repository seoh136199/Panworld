using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Member : MonoBehaviour {

    [SerializeField] private string name;
    [SerializeField] private int entryWeek;
    [SerializeField] private int level, throughput;
    [SerializeField] private int pRatio, dRatio, aRatio;
    [SerializeField] private Game.Part part;
    [SerializeField] private Game.MemberType memberType;
    [SerializeField] private Sprite[] face = new Sprite[3];
    private Image myImage;
    private BoxCollider2D myCollider;

    public Slot mySlot;

    public void Init(string name, int crWeek, int pRatio, int dRatio, int aRatio) {
        this.name = name;
        this.entryWeek = crWeek;

        this.pRatio = pRatio;
        this.dRatio = dRatio;
        this.aRatio = aRatio;

        if (Random.Range(0, 100) < pRatio) {
            part = Game.Part.programming;
        }
        else if (Random.Range(0, dRatio + aRatio) < dRatio) {
            part = Game.Part.design;
        }
        else {
            part = Game.Part.art;
        }

        //myImage.sprite = face[(int)part];
        transform.localScale = new(1, 1, 1);
        memberType = Game.MemberType.probationary;
    }

    public void ChangeName(string newName) {
        name = newName;
    }

    public void SetDragging(bool isDragging) {
        if (isDragging) {
            myCollider.enabled = false;
            myImage.color = new(myImage.color.r, myImage.color.g, myImage.color.b, 0.5f);
        }
        else {
            myCollider.enabled = true;
            myImage.color = new(myImage.color.r, myImage.color.g, myImage.color.b, 1);
        }
    }

    public Game.Part GetCube() {
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

    void Start() {
        myImage = GetComponent<Image>();
        myCollider = GetComponent<BoxCollider2D>();
    }

    void Update() {
        
    }
}
