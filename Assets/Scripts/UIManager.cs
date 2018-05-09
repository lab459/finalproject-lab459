using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameObject activeTab;
    public GameObject walkerPrefab;
    public GameObject armyManager;
    public GameObject mineManager;
    public GameObject conquerButton;
    public int villageStrength;
    private const int MAX_SPRITES_ON_SCREEN = 30;
    private const float ACTIVE_TAB_ALPHA = 1f;
    private const float INACTIVE_TAB_ALPHA = 0.3f;
    private Color notificationColor = new Color (0.871f, 0.753f, 0.322f, 1);
    private bool victoryNotified = false;

	private void Update()
	{
        // check victory condition
        if (!victoryNotified && CalculateArmyStrength() >= villageStrength)
        {
            NotifyVictory();
        }
	}

	public void Recruit (GameObject unitType) {
        UnitStats unit = unitType.GetComponent<UnitStats>();

        // ensure unit is unlocked
        if (unit.locked)
        {
            print("active recruit failed; " + unit.unitName + " is locked");
        }
        // recruit unit
        else {
            print("actively recruiting " + unit.recruitNum + " " + unit.unitName + "s");
            unit.unitNum += unit.recruitNum;

            // spawn a walker
            SpawnWalker(unit);
        }
    }

    // TODO: write an update loop with a switch statement to handle updating the currently active panel?

    public void Employ (UnitStats unit, MineStats mine, int workerNum) {

        // ensure unit and mine are unlocked
        if (mine.locked)
        {
            print("employ workers failed; " + mine.mineName + " is locked");
        }
        else if (unit.locked)
        {
            print("employ workers failed; " + unit.unitName + " is locked");
        }
        // ensure unit is a valid worker
        else if (!mine.workersAllowed.Contains(unit.unitName))
        {
            print("employ workers failed; " + unit.unitName + " can not work in the " + mine.mineName);
        }
        // ensure desired number of workers exist
        else if (unit.unitNum < workerNum)
        {
            print("employ workers failed; fewer than " + workerNum + " " + unit.unitName + "s in army");
        }
        // employ workers
        else {
            print("employing " + workerNum + " " + unit.unitName + "s in the " + mine.mineName);

            // if this is the first worker of ANY type, add it to the workerlist and start gathering coroutine
            if (mine.workerList.Count == 0)
            {
                print("starting " + mine.mineName + " gathering coroutine");
                mine.workerList.Add(unit, workerNum);
                StartCoroutine(mine.GatherResources());
            }
            // if it's just the first worker of THIS type, add it to the workerlist
            else if (!mine.workerList.ContainsKey(unit))
            {
                mine.workerList.Add(unit, workerNum);
            }
            // otherwise, add to existing workerlist entry
            else {
                mine.workerList[unit] += workerNum;
            }
            // remove workers from army
            unit.unitNum -= workerNum;

            // update UI
            UpdateMineWorkerDisplay(mine);
        }
    }

    public void Hire (GameObject unitType) {
        UnitStats unit = unitType.GetComponent<UnitStats>();
        int cost = unit.RecruiterCost();

        // ensure unit is unlocked
        if (unit.locked)
        {
            print("hire failed; " + unit.unitName + " is locked");
        }
        // ensure enough units in army
        else if (unit.unitNum < cost)
        {
            print("hire failed; fewer than " + cost + " " + unit.unitNamePlural + " in army");
        }
        // hire recruiter
        else
        {
            print("hiring " + unit.unitName + " recruiter for " + cost + " units");

            // subtract units from army and increase recruiters
            unit.unitNum -= cost;
            unit.recruiterNum++;

            // if this is the first recruiter of this type, start the passive recruitment coroutine
            if (unit.recruiterNum == 1)
            {
                print("starting passive recruitment of " + unit.unitNamePlural);
                StartCoroutine(unit.PassiveRecruit());
            }

            // get new recruiter cost
            cost = unit.RecruiterCost();

            // update UI
            UpdateHireButtonDisplay(unit, cost);
            UpdateRecruiterNumDisplay(unit);
        }
    }


    public void SpawnWalker (UnitStats unit) {
        // when a unit is recruited, spawn a sprite that walks across the screen
        // based on https://answers.unity.com/questions/1164719/2d-spawn-across-screen.html

        // check that there aren't too many units on the screen at once
        if (GameObject.FindGameObjectsWithTag("unit sprite").Length < MAX_SPRITES_ON_SCREEN) {
            // set starting position
            Vector3 spawnPosition = new Vector3(-10f, 2.8f, 0);
            Quaternion spawnRotation = new Quaternion(0, 180, 0, 0);
            // spawn the walker with appropriate sprite
            GameObject walker = Instantiate(walkerPrefab, spawnPosition, spawnRotation) as GameObject;
            walker.GetComponent<SpriteRenderer>().sprite = unit.unitSprite;
            // randomize sort order
            walker.GetComponent<SpriteRenderer>().sortingOrder = UnityEngine.Random.Range(0, 5);
        }
    }

    private void UpdateHireButtonDisplay(UnitStats unit, int cost) {
        // whenever the price of hiring a recruiter changes, call this to find and update the cost display in the button
        Text buttonText = GameObject.Find(unit.unitName + "Panel").transform.Find("HireButton").transform.Find("Text").GetComponent<Text>();
        var newText = "Hire " + unit.unitName + " Recruiter\nCost: " + cost + " " + unit.unitNamePlural;
        newText = newText.Replace("\\n", "\n"); // ensure that newlines are properly escaped
        buttonText.text = newText;
    }

    private void UpdateRecruiterNumDisplay(UnitStats unit)
    {
        // whenever the number, speed, or efficiency of recruiters changes, call this to find and update the recruiter display
        // TODO: this is an awkward construction. Can I make it neater?
        Text displayText = GameObject.Find(unit.unitName + "Panel").transform.Find("NumRecruiters").GetComponent<Text>();
        var newText = unit.recruiterNum + " ";
        if (unit.recruiterNum == 1) { newText += "recruiter "; } else { newText += "recruiters "; }
        var recruitingNum = (int)(unit.recruiterNum * unit.passiveRecruitNum);
        newText += "generating " + recruitingNum + " ";
        if (recruitingNum == 1) { newText += unit.unitName; } else { newText += unit.unitNamePlural; }
        if ((int)unit.recruiterSpeed == 1) { newText += " every second"; } else { newText += " every " + unit.recruiterSpeed + " seconds"; }
        displayText.text = newText;
    }

    public void UpdateMineWorkerDisplay(MineStats mine)
    {
        // whenever the number of workers in a given mine changes, call this to find and update the worker display
        Transform minePanel = GameObject.Find(mine.resourceName + "MinePanel").transform;

        // update worker list first
        Text displayText = minePanel.Find("NumWorkers").GetComponent<Text>();

        var newText = "";

        if (mine.workerList.Count == 0)
        {
            newText = "none";
        }
        else
        {
            foreach (KeyValuePair<UnitStats, int> worker in mine.workerList)
            {
                newText += worker.Key.unitNamePlural + ": " + worker.Value + "\n";
            }
        }

        displayText.text = newText;

        // then update production readout
        displayText = minePanel.Find("NumProduction").GetComponent<Text>();

        newText = "Producing " + mine.TotalGatherOfAllWorkers() + " " + mine.resourceName + " every " + mine.mineSpeed + " seconds";
        displayText.text = newText;
    }


    // activate conquer button and notify player that victory conditions have been met
    public void NotifyVictory()
    {
        // activate conquer button -- has to be direct reference rather than find, because it's inactive @ start
        conquerButton.SetActive(true);

        // highlight tab
        GameObject.Find("ConquerTab").GetComponent<Image>().color = notificationColor;

    }

    public void SwitchTabs(GameObject newTab) {
        if (newTab != activeTab) {
            // switch tab colors
            Color c = activeTab.GetComponent<Image>().color;
            c.a = INACTIVE_TAB_ALPHA;
            activeTab.GetComponent<Image>().color = c;

            c = newTab.GetComponent<Image>().color;
            c.a = ACTIVE_TAB_ALPHA;
            newTab.GetComponent<Image>().color = c;

            // get panels associated with tabs
            var newPanel = newTab.GetComponent<TabProperties>().panel;
            var activePanel = activeTab.GetComponent<TabProperties>().panel;

            // switch active panels
            activePanel.SetActive(false);
            newPanel.SetActive(true);

            // store currently active tab
            activeTab = newTab;
        }
    }

    public void AdminCheat1K() {
        // cheat to give yourself 1K of everything you've unlocked
        print("CHEATING -- granting 1K unlocked units & resources");

        var units = armyManager.GetComponentsInChildren<UnitStats>();
        var mines = mineManager.GetComponentsInChildren<MineStats>();

        foreach (var unit in units)
        {
            if (!unit.locked) { unit.unitNum += 1000; }
        }
        foreach (var mine in mines)
        {
            if (!mine.locked) { mine.resourceNum += 1000; }
        }


    }

    public void AdminCheatUnlockAll() {
        // cheat to unlock everything
       print("CHEATING -- unlocking all unit & resource types");

        var units = armyManager.GetComponentsInChildren<UnitStats>();
        var mines = mineManager.GetComponentsInChildren<MineStats>();

        foreach (var unit in units)
        {
            unit.locked = false;
        }
        foreach (var mine in mines)
        {
            mine.locked = false;
        }
    }

    public int CalculateArmyStrength()
    {
        // iterate over objects in army manager and aggregate army strength (rounded to int)
        var army = armyManager.GetComponentsInChildren<UnitStats>();
        var armyStrength = 0;

        foreach (UnitStats unit in army)
        {
            if (!unit.locked)
            {
                armyStrength += (int)(unit.baseStrength * unit.strengthMultiplier * unit.unitNum);
            }
        }

        return armyStrength;
    }

    public void Victory()
    {
        // TODO: make this suck way less

        // double-check victory condition
        if (CalculateArmyStrength() >= villageStrength)
        {
            // reset data and prepare for final swarm
            var army = armyManager.GetComponentsInChildren<UnitStats>();
            var mines = mineManager.GetComponentsInChildren<MineStats>();
            Dictionary<Sprite, int> finalSwarm = new Dictionary<Sprite, int>();

            foreach (MineStats mine in mines)
            {
                mine.resourceNum = 0;
                mine.workerList = new Dictionary<UnitStats, int>();
                if (mine.resourceName != "Iron") { mine.locked = true; }
            }

            foreach (UnitStats unit in army)
            {
                
                if (!unit.locked && unit.unitNum > 0)
                {
                    finalSwarm.Add(unit.unitSprite, unit.unitNum);
                }
                unit.unitNum = 0;
                unit.strengthMultiplier = 1;
                unit.gatherMultiplier = 1;
                unit.recruiterNum = 0;
                if (unit.unitName != "Orc") { unit.locked = true; }
            }

            // TODO: release final swarm
            // TODO: display victory screen
        }
    }
}
