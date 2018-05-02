using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateMineDropdown : MonoBehaviour {

    List<string> mineOptions = new List<string> {};
    Dropdown dropdown;

	// Use this for initialization
	void Start () {
        Transform manager = GameObject.Find("MineManager").transform;

        // iterate over mines, and add unlocked mines to dropdown
        foreach (Transform child in manager) {
            MineStats mine = child.GetComponent<MineStats>();
            if (!mine.locked) {
                mineOptions.Add(mine.mineName);
            }
        }

        // update the dropdown UI object
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(mineOptions);
	}
}
