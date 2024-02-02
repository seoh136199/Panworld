using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game {
    public static GameManager gameManager;
    public static BgImage bgImage;
    public static Castle castle;

    public enum Part {
        programming, design, art
    }

    public enum MemberType {
        probationary, regular, honorary
    }

}

public class GameManager : MonoBehaviour {

    [SerializeField] private const int YEAR_TO_WEEK = 12;
    [SerializeField] private const int WEEK_TO_SEC = 40;
    [SerializeField] private readonly int[] BOUNDARY = { 0, 30 };
    [SerializeField] private int crYear = 0;
    [SerializeField] private int crWeek = 0;
    [SerializeField] private int preSec = 0;
    [SerializeField] private int crSec = 0;

    [SerializeField] private double timeSec = 0;
    [SerializeField] private int timeWeek = 0;

    private void CalTime() {
        timeSec += Time.deltaTime;
        crSec = (int)timeSec;

        if (crSec >= WEEK_TO_SEC) {
            int deltaWeek = crSec / WEEK_TO_SEC;
            crWeek += deltaWeek;
            timeWeek += deltaWeek;
            crSec -= deltaWeek * WEEK_TO_SEC;
            timeSec -= deltaWeek * WEEK_TO_SEC;
        }
        if (crWeek >= YEAR_TO_WEEK) {
            int deltaYear = crWeek / YEAR_TO_WEEK;
            crYear += deltaYear;
            crWeek -= deltaYear * YEAR_TO_WEEK;
        }

        for (int i = 0; i < BOUNDARY.Length; i++) {
            if (preSec == crSec) continue;
            if (crSec != BOUNDARY[i]) continue;
            TimeBoundaryEvent(i);
            break;
        }
        preSec = crSec;
    }

    private void TimeBoundaryEvent(int num) {
        Game.bgImage.ChangeImage(num);
        Game.castle.ChangeImage(num);
    }

    private void Awake() {
        Game.gameManager = this;    
    }

    void Start() {
        
    }

    void Update() {
        CalTime();
    }
}
