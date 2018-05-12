using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// based on advice from following sites:
// https://stackoverflow.com/questions/5456463/protection-level-of-a-struct-field-within-a-class
// https://stackoverflow.com/questions/569903/multi-value-dictionary

public delegate void UpgradeEffect(GameObject target);

public class RecipeItem
{
    public string Name { get; set; } // name of gameobject that governs the currency
    public string Type { get; set; } // "unit", "resource", or "other" (other is not in use yet, to be used for prereqs and misc non-currencies)
    public bool Expended { get; set; } // if false, it's a prereq that won't be deducted
    public int Quantity { get; set; } // number needed
}

public class Upgrade
{
    public string Name { get; set; } // name of upgrade
    public List<RecipeItem> Recipe;
    public GameObject Target;
    public UpgradeEffect Effect;

    public void AddItem(string name, string type, bool expended, int quantity)
    {
        var item = new RecipeItem
        {
            Name = name,
            Type = type,
            Expended = expended,
            Quantity = quantity
        };
        Recipe.Add(item);
    }
}

public class UpgradeManager : MonoBehaviour
{
    public GameObject UIManager;
    public UIManager myUIManager;

	private void Start()
	{
        myUIManager = UIManager.GetComponent<UIManager>();
	}

	// returns true if player has enough resources to afford the upgrade recipe
	private bool CanAffordRecipe(Upgrade upg)
    {
        foreach (RecipeItem item in upg.Recipe)
        {
            switch(item.Type)
            {
                case "unit":
                    var unit = myUIManager.armyManager.transform.Find(item.Name).GetComponent<UnitStats>();
                    if (unit.unitNum < item.Quantity) { return false; }
                    break;

                case "resource":
                    var mine = myUIManager.mineManager.transform.Find(item.Name).GetComponent<MineStats>();
                    if (mine.resourceNum < item.Quantity) { return false; }
                    break;

                case "other":
                    // Placeholder for prerequisites
                    break;

                default:
                    print("recipe check failed: tried to check nonexistent recipe item '" + item.Name + "'");
                    return false;
            }
        }

        print("recipe check successful: player can afford upgrade recipe '" + upg.Name + "'");
        return true;
    }

    // deducts quantities from wallet, returns false if player didn't have enough for some reason (though doesn't undo resources already spent)
    public bool SpendForRecipe(Upgrade upg)
    {
        foreach (RecipeItem item in upg.Recipe)
        {
            // do nothing if item is a non-expended prerequisite
            if (!item.Expended) { break; }

            // otherwise, deduct it from appropriate wallet
            switch(item.Type)
            {
                case "unit":
                    var unit = myUIManager.armyManager.transform.Find(item.Name).GetComponent<UnitStats>();
                    if (unit.unitNum < item.Quantity) 
                    {
                        print("spending failed: tried to deduct too many" + unit.unitNamePlural + " from wallet");
                        return false;
                    }
                    else
                    {
                        unit.unitNum -= item.Quantity;
                    }
                    break;

                case "resource":
                    var resource = myUIManager.mineManager.transform.Find(item.Name).GetComponent<MineStats>();
                    if (resource.resourceNum < item.Quantity)
                    {
                        print("spending failed: tried to deduct too many" + resource.resourceName + " from wallet");
                        return false;
                    }
                    else
                    {
                        resource.resourceNum -= item.Quantity;
                    }
                    break;

                case "other":
                    // Placeholder for prerequisites
                    break;

                default:
                    print("spending failed: tried to subtract nonexistent recipe item '" + item.Name + "'");
                    return false;
            }
        }

        print("recipe spend successful: player has paid for upgrade recipe '" + upg.Name + "'");
        return true;
    }

    public void PurchaseUpgrade(Upgrade upg)
    {
        // check that you can afford it
        if (CanAffordRecipe(upg))
        {
            // check that you've successfully paid
            if (SpendForRecipe(upg))
            {
                // deliver the goods
                upg.Effect(upg.Target);
            }
        }
    }
}
