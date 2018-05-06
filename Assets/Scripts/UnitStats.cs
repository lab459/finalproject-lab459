using UnityEngine;
using System.Collections;

public class UnitStats : MonoBehaviour {
    
    public string unitName;
    public bool locked;
    public int unitNum;
    public int recruitNum;
    public int baseStrength;
    public float modStrength;
    public int baseGather;
    public float modGather;
    public int recruiterNum;
    public float recruiterCost;
    public float recruiterCostMultiplier;
    public float recruiterSpeed;
    public int passiveRecruitNum;
    public GameObject unitSprite;
    private const int MAX_BATCH = 5;

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
                UIManager.SpawnWalker(this);
            }
        }

        StartCoroutine(PassiveRecruit());
    }

}