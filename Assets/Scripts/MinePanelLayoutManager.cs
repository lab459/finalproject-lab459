using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinePanelLayoutManager : MonoBehaviour {

    // based on http://www.studica.com/blog/unity-ui-tutorial-scroll-grid

    public GameObject minePanelPrefab;
    public GameObject mineManager;
    public GameObject myUIManager;

    private void OnEnable()
    {
        // repopulate the panel every time it is opened
        Populate();
    }

    private void OnDisable()
    {
        // empty the panel every time it closes
        Empty();
    }

    void Populate()
    {
        // empty the unit panel of existing entries
        Empty();

        // iterate over army to populate unit panel with all unlocked unit types
        var mineStats = mineManager.GetComponentsInChildren<MineStats>();
        foreach (var mine in mineStats)
        {
            if (!mine.locked)
            {
                AddMinePanelItem(mine);
            }
        }
    }

    // clear existing children so the wallet can be refilled
    void Empty()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    //
    void AddMinePanelItem(MineStats mine)
    {
        var newObj = (GameObject)Instantiate(minePanelPrefab, transform);
        newObj.name = mine.resourceName + "MinePanel";
        string newText;

        // iterate over object's children, replacing placeholders with values from statblock
        foreach (Transform child in newObj.transform) 
        {
            switch(child.name)
            {
                case "MineName":
                    child.GetComponent<Text>().text = mine.mineName;
                    break;

                case "MineDoor":
                    child.GetComponent<Image>().sprite = mine.mineDoor;
                    break;

                case "MineDesc":
                    child.GetComponent<Text>().text = mine.mineDesc;
                    break;

                case "NumProduction":
                    newText = "Producing " + mine.CalculateTotalGather() + " " + mine.resourceName + " every " + mine.mineSpeed + " seconds";
                    child.GetComponent<Text>().text = newText;
                    break;

                case "NumWorkers":
                    newText = "";

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

                    child.GetComponent<Text>().text = newText;
                    break;

                case "EmployButton":
                    //TODO FIRST THING TOMORROW!
                    break;

                default:
                    print("unexpected child in unitpanel prefab");
                    break;
            }
        }
    }
}
