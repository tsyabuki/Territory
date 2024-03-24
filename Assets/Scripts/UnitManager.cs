using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UnitManager : MonoBehaviour
{
    [SerializeField] private Unit[] spawnableUnits;
    [ReadOnlyInspector] [SerializeField] private GridManager _gm = null;
    [Space]
    [SerializeField] private String TESTSPAWNUNIT;
    [SerializeField] private String TESTUPGRADEUNIT;
    [SerializeField] private Vector2 TESTSPAWNLOC;

    private void Awake()
    {
        findGridManager();
    }


    //------------------------
    //-Initialization methods-
    //------------------------

    //Finds the grid manager
    private void findGridManager()
    {
        _gm = GameObject.FindGameObjectWithTag("GridManager").GetComponent<GridManager>();
        if (_gm == null)
        {
            Debug.LogWarning("WARNING: Unit Manager could not find a Grid Manager in the scene. Check to ensure it is present and tagged.");
        }
    }

    //-----------------------
    //-Unit spawning methods-
    //-----------------------

    //Finds the referenced unit in the unit manager. Returns null if not found.
    private Unit findUnitReference(string name)
    {
        Unit refUnit = null;
        refUnit = Array.Find(spawnableUnits, Unit => Unit.unitName == name);
        return refUnit;
    }

    //Checks to see whether or not a unit is an upgrade or not
    public bool isUnitUpgrade(string name)
    {
        Unit checkedUnit = findUnitReference(name);
        return checkedUnit.isUpgrade;
    }

    //Spawns a unit on the tile specified in the arguments. Returns false if it was unable to spawn the unit.
    public bool spawnUnit(string name, Tile spawnTile)
    {
        Unit spawnedUnit = findUnitReference(name);
        if(spawnedUnit == null)
        {
            Debug.Log("Unit " + name + " not found. Aborting unit spawn.");
            return false;
        }

        if(spawnedUnit.unitPrefab == null)
        {
            Debug.Log("Unit " + name + " has no prefab. Aborting unit spawn.");
            return false;
        }

        if(spawnTile.isOccupied())
        {
            Debug.Log(spawnTile.gameObject.name + " is already occupied. Aborting unit spawn.");
            return false;
        }

        GameObject newUnitObject = Instantiate(spawnedUnit.unitPrefab, spawnTile.gameObject.transform.position + spawnedUnit.relativeSpawn, Quaternion.identity, spawnTile.gameObject.transform);
        spawnTile.setTileUnitParamters(newUnitObject, spawnedUnit.unitName, spawnedUnit.unitAffiliation);

        return true;
    }

    //Spawns a unit on the tile specified in the arguments. Returns false if it was unable to spawn the unit.
    public bool placeUnitNoRestrictions(string name, Tile spawnTile)
    {
        Unit spawnedUnit = findUnitReference(name);
        if (spawnedUnit == null)
        {
            Debug.Log("Unit " + name + " not found. Aborting unit spawn.");
            return false;
        }

        if (spawnedUnit.unitPrefab == null)
        {
            Debug.Log("Unit " + name + " has no prefab. Aborting unit spawn.");
            return false;
        }

        GameObject newUnitObject = Instantiate(spawnedUnit.unitPrefab, spawnTile.gameObject.transform.position + spawnedUnit.relativeSpawn, Quaternion.identity, spawnTile.gameObject.transform);
        spawnTile.setOccupier(newUnitObject);

        return true;
    }

    //Upgrades a unit on the tile specified in the arguments. Returns false if it was unable to upgrade the unit.
    public bool upgradeUnit(string upgradeFromName, string upgradeToName, Tile upgradeTile)
    {
        Unit spawnedUnit = findUnitReference(upgradeToName);
        if (spawnedUnit == null)
        {
            Debug.Log("Unit " + upgradeToName + " not found. Aborting unit upgrade.");
            return false;
        }

        if (spawnedUnit.unitPrefab == null)
        {
            Debug.Log("Unit " + upgradeToName + " has no prefab. Aborting unit upgrade.");
            return false;
        }

        if (!upgradeTile.isOccupied())
        {
            Debug.Log(upgradeTile.gameObject.name + " is not occupied. Aborting unit upgrade.");
            return false;
        }

        if (!upgradeTile.isOccupied(upgradeFromName))
        {
            Debug.Log(upgradeTile.gameObject.name + " is not occupied by " + upgradeFromName + ". Aborting unit upgrade.");
            return false;
        }

        if(!spawnedUnit.isUpgrade ||  upgradeFromName != spawnedUnit.upgradeFrom)
        {
            Debug.Log(upgradeTile.gameObject.name + " name does not match upgrade path of " + upgradeToName + ". Aborting unit upgrade.");
            return false;
        }

        GameObject newUnitObject = Instantiate(spawnedUnit.unitPrefab, upgradeTile.gameObject.transform.position + spawnedUnit.relativeSpawn, Quaternion.identity, upgradeTile.gameObject.transform);
        upgradeTile.replaceTileOccupier(newUnitObject, spawnedUnit.unitName);

        return true;
    }

    public void SPAWNTESTEVENT()
    {
        spawnUnit(TESTSPAWNUNIT, _gm.gridArray[(int)TESTSPAWNLOC.x, (int)TESTSPAWNLOC.y].GetComponent<Tile>());
    }

    public void UPGRADETESTEVENT()
    {
        upgradeUnit(TESTSPAWNUNIT, TESTUPGRADEUNIT, _gm.gridArray[(int)TESTSPAWNLOC.x, (int)TESTSPAWNLOC.y].GetComponent<Tile>());
    }
}
