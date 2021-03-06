﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalletManager : MonoBehaviour {

    // based on http://www.studica.com/blog/unity-ui-tutorial-scroll-grid

    public GameObject mineManager;
    public GameObject armyManager;
    public GameObject textPrefab;
    public GameObject spacerPrefab;
    public int fontLarge = 24;
    public int fontSmall = 20;

	// Use this for initialization
	void Start () {
        Populate();

	}
	
	// Update is called once per frame
	void Update () {
        Populate();
		
	}

    void Populate() {
        // empty the wallet of existing entries
        Empty();

        // populate the list with army units first, then resources
        AddSpacer(0);
        AddWalletItem("ArmyWalletTitle", "ARMY", fontLarge);

        // iterate over manager's children
        var unitStats = armyManager.GetComponentsInChildren<UnitStats>();
        foreach (var unit in unitStats)
        {
            if (!unit.locked) {
                AddWalletItem(
                    "ArmyWallet" + unit.unitName, //name
                    unit.unitName + ": " + unit.unitNum.ToString(), //text
                    fontSmall
                );
            }
        }

        AddSpacer(5);
        AddWalletItem("ResourceWalletTitle", "RESOURCES", fontLarge);

        var mineStats = mineManager.GetComponentsInChildren<MineStats>();
        foreach (var mine in mineStats)
        {
            if (!mine.locked) {
                AddWalletItem(
                    "ResourceWallet" + mine.resourceName, //name
                    mine.resourceName + ": " + mine.resourceNum.ToString(), //text
                    fontSmall
                );
            }
        }
    }

    // clear existing children so the wallet can be refilled
    void Empty() {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    void AddWalletItem(string objName, string text, int fontSize) {
        var newObj = (GameObject)Instantiate(textPrefab, transform);
        var objText = newObj.GetComponent<Text>();
        newObj.name = objName;
        objText.text = text;
        objText.resizeTextMaxSize = fontSize;
    }

    void AddSpacer(int size)
    {
        var newObj = (GameObject)Instantiate(spacerPrefab, transform);
        newObj.name = "WalletSpacer";
        newObj.GetComponent<RectTransform>().sizeDelta = new Vector2(160, size);
    }
}
