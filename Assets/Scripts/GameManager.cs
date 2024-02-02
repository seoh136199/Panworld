using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game {
    public static GameManager gameManager;
    public static BgImage bgImage;
    public static Castle castle;
}

public class GameManager : MonoBehaviour {

    [SerializeField] private const int YEAR_TO_DAY = 20;
    [SerializeField] private const int DAY_TO_SEC = 60;
    [SerializeField] private readonly int[] BOUNDARY = { 0, 15, 30, 45 };
    [SerializeField] private int crYear = 0;
    [SerializeField] private int crDay = 0;
    [SerializeField] private int preSec = 0;
    [SerializeField] private int crSec = 0;
    [SerializeField] private double time = 0;

    private void CalTime() {
        time += Time.deltaTime;
        crSec = (int)time;

        if (crSec >= DAY_TO_SEC) {
            int deltaDay = crSec / DAY_TO_SEC;
            crDay += deltaDay;
            crSec -= deltaDay * DAY_TO_SEC;
            time -= deltaDay * DAY_TO_SEC;
        }
        if (crDay >= YEAR_TO_DAY) {
            int deltaYear = crDay / YEAR_TO_DAY;
            crYear += deltaYear;
            crDay -= deltaYear * YEAR_TO_DAY;
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
