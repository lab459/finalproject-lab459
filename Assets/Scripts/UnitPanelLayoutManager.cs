using UnityEngine;
using UnityEngine.UI;

public class UnitPanelLayoutManager : MonoBehaviour
{

    // based on http://www.studica.com/blog/unity-ui-tutorial-scroll-grid

    public GameObject unitPanelPrefab;
    public GameObject armyManager;
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
        var unitStats = armyManager.GetComponentsInChildren<UnitStats>();
        foreach (var unit in unitStats)
        {
            if (!unit.locked)
            {
                AddUnitPanelItem(unit);
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
    void AddUnitPanelItem(UnitStats unit)
    {
        var newObj = (GameObject)Instantiate(unitPanelPrefab, transform);
        newObj.name = unit.unitName + "Panel";
        string newText;

        // iterate over object's children, replacing placeholders with values from statblock
        foreach (Transform child in newObj.transform)
        {
            switch(child.name) 
            {
                case "UnitName":
                    child.GetComponent<Text>().text = unit.unitName;
                    break;

                case "UnitPortrait":
                    child.GetComponent<Image>().sprite = unit.unitSprite;
                    break;

                case "UnitDesc":
                    child.GetComponent<Text>().text = unit.unitDesc;
                    break;

                case "UnitStats":
                    newText = "Strength: " + (unit.baseStrength * unit.modStrength) + "\nGather: " + (unit.baseGather * unit.modGather);
                    newText = newText.Replace("\\n", "\n"); // ensure that newlines are properly escaped
                    child.GetComponent<Text>().text = newText;
                    break;

                case "RecruitButton":
                    newText = "Recruit " + unit.recruitNum + " ";
                    if (unit.recruitNum == 1) { newText += unit.unitName; } else { newText += unit.unitNamePlural; }
                    child.transform.GetChild(0).GetComponent<Text>().text = newText;

                    // hook up onclick function
                    child.GetComponent<Button>().onClick.AddListener(delegate{ myUIManager.GetComponent<UIManager>().Recruit(GameObject.Find(unit.unitName)); });
                    break;

                case "HireButton":
                    newText = "Hire " + unit.unitName + " Recruiter\nCost: " + (int)unit.recruiterCost + " ";
                    if ((int)unit.recruiterCost == 1) { newText += unit.unitName; } else { newText += unit.unitNamePlural; }
                    child.transform.GetChild(0).GetComponent<Text>().text = newText;

                    // hook up onclick function
                    child.GetComponent<Button>().onClick.AddListener(delegate { myUIManager.GetComponent<UIManager>().Hire(GameObject.Find(unit.unitName)); });
                    break;

                case "NumRecruiters":
                    // TODO: this was buggy, so I just neutralized the default and let UIManager handle it. Come back if you have time.
                    break;

                default:
                    print("unexpected child in unitpanel prefab");
                    break;
            }
        }

    }
}
