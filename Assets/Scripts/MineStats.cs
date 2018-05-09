using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MineStats : MonoBehaviour {

    public string mineName;
    public bool locked;
    public string resourceName;
    public string mineDesc;
    public float resourceNum;
    public float mineSpeed;
    public float mineSpeedMultiplier;
    public string[] workersAllowed;
    public Dictionary<UnitStats, int> workerList = new Dictionary<UnitStats, int>();
    public Sprite mineDoor;

    public IEnumerator GatherResources()
    {

        // wait for speed to elapse
        yield return new WaitForSeconds(mineSpeed * mineSpeedMultiplier);

        // ensure mine is unlocked
        if (locked)
        {
            print("gathering failed; " + mineName + " is locked");
        }
        // calculate total worker gathering power
        else
        {
            var totalGather = TotalGatherOfAllWorkers();

            if (totalGather > 0) {
                // succeed
                print("Workers gathered " + totalGather + " " + resourceName);
                resourceNum += totalGather;
            }
            else {
                print("Tried to gather " + resourceName + ", but total quantity was 0");
            }
        }

        StartCoroutine(GatherResources());
    }
	
    public int TotalGatherOfAllWorkers() {
        var totalGather = 0;


        foreach (KeyValuePair<UnitStats, int> worker in workerList)
        {
            // verify that unit is allowed and unlocked
            if (worker.Key.locked)
            {
                print(worker.Key.unitName + " tried to contribute to " + resourceName + " gathering, but that unit type is locked");
            }
            else if (!workersAllowed.Contains(worker.Key.unitName))
            {
                print(worker.Key.unitName + " tried to contribute to " + resourceName + " gathering, but it is not allowed to work in that mine");
            }
            else
            {
                // add unit's gathering score to total gathering score
                totalGather += worker.Value * worker.Key.TotalGather();
            }
        }

        return totalGather;
    }
}