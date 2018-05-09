using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConquerPanelManager : MonoBehaviour {

    public GameObject villageDisplay;
    public GameObject armyDisplay;
    public GameObject myUIManager;
    private UIManager myUIManagerScript;
    private int strengthLength;
    private int villageStrength;

	// Use this for initialization
	void Start () {
        // display village strength
        myUIManagerScript = myUIManager.GetComponent<UIManager>();
        villageStrength = myUIManagerScript.villageStrength;
        villageDisplay.GetComponent<Text>().text = villageStrength.ToString();

        // figure out how many leading zeroes to use in army strength display
        strengthLength = villageStrength.ToString("D").Length;
	}
	
	// Update is called once per frame
	void Update () {
        // display army strength with leading zeroes
        var armyStrength = myUIManagerScript.CalculateArmyStrength();
        armyDisplay.GetComponent<Text>().text = armyStrength.ToString("D" + strengthLength);
	}
}
