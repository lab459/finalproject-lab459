using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}
