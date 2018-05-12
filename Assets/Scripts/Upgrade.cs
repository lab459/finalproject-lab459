using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct RecipeItem
{
    public string name; // name of gameobject that governs the currency
    public string type; // "unit", "resource", or "other" (other is not in use yet, to be used for prereqs)
    public bool expended; // if false, it's a prereq that won't be deducted
    public int quantity; // number needed
}

public class Upgrade : MonoBehaviour
{
    public GameObject UIManager;
    public UIManager myUIManager;

	private void Start()
	{
        myUIManager = UIManager.GetComponent<UIManager>();
	}

	// returns true if player has enough resources to afford the upgrade recipe
	private bool CanAffordRecipe(List<RecipeItem> recipe)
    {
        foreach (RecipeItem item in recipe)
        {
            switch(item.type)
            {
                case "unit":
                    var unit = myUIManager.armyManager.transform.Find(item.name).GetComponent<UnitStats>();
                    if (unit.unitNum < item.quantity) { return false; }
                    break;

                case "resource":
                    var mine = myUIManager.mineManager.transform.Find(item.name).GetComponent<MineStats>();
                    if (mine.resourceNum < item.quantity) { return false; }
                    break;

                case "other":
                    // Placeholder for prerequisites
                    break;

                default:
                    print("recipe check failed: tried to check nonexistent recipe item '" + item.name + "'");
                    return false;
            }
        }

        return true;
    }

    // deducts quantities from wallet, returns false if player didn't have enough for some reason (though doesn't undo resources already spent)
    public bool SpendForRecipe(List<RecipeItem> recipe)
    {
        foreach (RecipeItem item in recipe)
        {
            // do nothing if item is a non-expended prerequisite
            if (!item.expended) { break; }

            // otherwise, deduct it from appropriate wallet
            switch(item.type)
            {
                case "unit":
                    var unit = myUIManager.armyManager.transform.Find(item.name).GetComponent<UnitStats>();
                    if (unit.unitNum < item.quantity) 
                    {
                        print("spending failed: tried to deduct too many" + unit.unitNamePlural + " from wallet");
                        return false;
                    }
                    else
                    {
                        unit.unitNum -= item.quantity;
                    }
                    break;

                case "resource":
                    var resource = myUIManager.mineManager.transform.Find(item.name).GetComponent<MineStats>();
                    if (resource.resourceNum < item.quantity)
                    {
                        print("spending failed: tried to deduct too many" + resource.resourceName + " from wallet");
                        return false;
                    }
                    else
                    {
                        resource.resourceNum -= item.quantity;
                    }
                    break;

                case "other":
                    // Placeholder for prerequisites
                    break;

                default:
                    print("spending failed: tried to subtract nonexistent recipe item '" + item.name + "'");
                    return false;
            }
        }

        return true;
    }
}
