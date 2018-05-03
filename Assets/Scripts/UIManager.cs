using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public void Recruit (GameObject unitType) {
        UnitStats unit = unitType.GetComponent<UnitStats>();

        // ensure unit is unlocked
        if (unit.locked) {
            print("recruit failed; " + unit.unitName + " is locked");
        }
        // recruit unit
        else {
            print("recruiting " + unit.recruitNum + " " + unit.unitName + "s");
            unit.unitNum += unit.recruitNum;
        }

        // update UI
        updateUnitNumDisplay(unit);
	}

    public void Employ (GameObject button) {
        EmployButtonProperties properties = button.GetComponent<EmployButtonProperties>();
        UnitStats unit = properties.unitType.GetComponent<UnitStats>();
        MineStats mine = properties.mineType.GetComponent<MineStats>();

        // ensure unit and mine are unlocked
        if (mine.locked) {
            print("employ workers failed; " + mine.mineName + " is locked");
        }
        else if (unit.locked) {
            print("employ workers failed; " + unit.unitName + " is locked");
        }
        // ensure unit is a valid worker
        else if (!mine.workersAllowed.Contains(unit.unitName)) {
            print("employ workers failed; " + unit.unitName + " can not work in the " + mine.mineName);
        }
        // ensure desired number of workers exist
        else if (unit.unitNum < properties.workerNum) {
            print("employ workers failed; fewer than " + properties.workerNum + " " + unit.unitName + "s in army");
        }
        // employ workers
        else {
            print("employing " + properties.workerNum + " " + unit.unitName + "s in the " + mine.mineName);

            // if this is the first worker of this type, add the type to the workerlist first
            if (!mine.workerList.ContainsKey(unit.unitName)) {
                mine.workerList.Add(unit.unitName, properties.workerNum);
            }
            // otherwise, add to existing workerlist entry
            else {
                mine.workerList[unit.unitName] += properties.workerNum;
            }
            // remove workers from army
            unit.unitNum -= properties.workerNum;

            // update UI
            updateUnitNumDisplay(unit);
        }
    }

    public void Hire (GameObject unitType) {
        UnitStats unit = unitType.GetComponent<UnitStats>();
        int cost = (int)Math.Floor(unit.recruiterCost);

        // ensure unit is unlocked
        if (unit.locked) {
            print("hire failed; " + unit.unitName + " is locked");
        }
        // ensure enough units in army
        else if (unit.unitNum < cost) {
            print("hire failed; fewer than " + cost + " " + unit.unitName + "s in army");
        }
        // hire recruiter
        else {
            print("hiring " + unit.unitName + " recruiter for " + cost + " units");
            // subtract units from army and increase recruiters
            unit.unitNum -= cost;
            unit.recruiterNum++;

            // modify recruiter cost
            unit.recruiterCost *= unit.recruiterCostMultiplier;
            cost = (int)Math.Floor(unit.recruiterCost);

            // update UI
            updateUnitNumDisplay(unit);
            updateHireButtonDisplay(unit, cost);
        }
    }

    private void updateUnitNumDisplay (UnitStats unit) {
        // whenever the number of units in the army is changed, call this to find and update the num display
        Text displayText = GameObject.Find(unit.unitName + "Panel").transform.Find("NumInArmy").GetComponent<Text>();
        displayText.text = unit.unitNum.ToString();
    }

    private void updateHireButtonDisplay(UnitStats unit, int cost) {
        // whenever the price of hiring a recruiter changes, call this to find and update the cost display in the button
        Text buttonText = GameObject.Find(unit.unitName + "Panel").transform.Find("HireButton").transform.Find("Text").GetComponent<Text>();
        var newText = "Hire " + unit.unitName + " Recruiter\nCost: " + cost + " " + unit.unitName + "s";
        // ensure that newlines are properly escaped
        newText = newText.Replace("\\n", "\n");
        buttonText.text = newText;
    }
}
