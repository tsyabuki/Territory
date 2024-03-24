using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Stores the fields relating to units to be spawned in
[System.Serializable]
public class Unit
{
    public string unitName;
    public tileAffiliation unitAffiliation;
    public GameObject unitPrefab;
    public Vector3 relativeSpawn;
    public bool isUpgrade;
    public string upgradeFrom;
}
