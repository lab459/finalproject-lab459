using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineStats : MonoBehaviour {

    public string mineName;
    public bool locked;
    public string resourceName;
    public float resourceNum;
    public string[] workersAllowed;
    public Dictionary<string, int> workerList = new Dictionary<string, int>();
    public Sprite mineDoor;

}
