using UnityEngine;
using UnityEngine.UI;

public class UnitPanelLayoutManager : MonoBehaviour
{

    // based on http://www.studica.com/blog/unity-ui-tutorial-scroll-grid

    public GameObject unitPanelPrefab;
    public GameObject armyManager;
    public GameObject myUIManager;

    // Use this for initialization
    void Start()
    {
        Populate();

    }

	private void OnEnable()
	{
        Populate();
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
                    child.GetComponent<SpriteRenderer>().sprite = unit.unitSprite.GetComponent<SpriteRenderer>().sprite;
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
                    // TODO: is there a less ugly way to construct this complex sentence?
                    if (unit.recruiterNum == 0) {
                        newText = "No " + unit.unitName + " recruiters active";
                    }
                    else {
                        if (unit.recruiterNum == 1) {
                            newText = "1 recruiter";
                        }
                        else {
                            newText = unit.recruiterNum + " recruiters";
                        }

                        var recruitingNow = unit.recruiterNum * unit.passiveRecruitNum;
                        newText += " recruiting " + recruitingNow + " ";
                            
                        if (recruitingNow == 1) {
                            newText += unit.unitName;
                        }
                        else {
                            newText += unit.unitNamePlural;
                        }

                        if ((int)unit.recruiterSpeed == 1) {
                            newText += " every second";
                        }
                        else {
                            newText += " every " + (int)unit.recruiterSpeed + " seconds";
                        }
                    }
                    break;

                default:
                    print("unexpected child in unitpanel prefab");
                    break;
            }
        }

    }
}
