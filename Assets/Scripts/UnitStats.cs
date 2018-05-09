﻿using UnityEngine;
using System.Collections;

public class UnitStats : MonoBehaviour {
    
    public string unitName;
    public string unitNamePlural;
    public bool locked;
    public string unitDesc;
    public int unitNum;
    public int recruitNum;
    public float baseStrength;
    public float modStrength;
    public int baseGather;
    public float modGather;
    public int recruiterNum;
    public float recruiterCost;
    public float recruiterCostMultiplier;
    public float recruiterSpeed;
    public int passiveRecruitNum;
    public Sprite unitSprite;
    private const int MAX_BATCH = 10;

    public IEnumerator PassiveRecruit() {

        // wait for speed to elapse
        yield return new WaitForSeconds(recruiterSpeed);

        // ensure unit is unlocked
        if (locked)
        {
            print("passive recruit failed; " + unitName + " is locked");
        }
        // recruit units
        else
        {
            var recruitingNow = recruiterNum * passiveRecruitNum;
            print("passively recruiting " + recruitingNow + " " + unitName + "s");
            unitNum += recruitingNow;

            // spawn up to max-batch walkers
            recruitingNow = Mathf.Min(recruitingNow, MAX_BATCH);
            for (int i = 0; i < recruitingNow; i++) {
                GameObject.Find("UIManager").GetComponent<UIManager>().SpawnWalker(this);
            }
        }

        StartCoroutine(PassiveRecruit());
    }

}