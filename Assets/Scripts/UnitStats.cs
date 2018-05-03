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
    public Sprite unitSprite;

    public IEnumerator PassiveRecruit() {

        // wait for speed to elapse
        yield return new WaitForSeconds(recruiterSpeed);

        // ensure unit is unlocked
        if (locked)
        {
            print("passive recruit failed; " + unitName + " is locked");
        }
        // recruit unit
        else
        {
            var recruitingNow = recruiterNum * passiveRecruitNum;
            print("passively recruiting " + recruitingNow + " " + unitName + "s");
            unitNum += recruitingNow;
        }

        // update UI
        UIManager.updateUnitNumDisplay(this);

        StartCoroutine(PassiveRecruit());
    }

}