using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Grid Populated Variables")]
    public GameObject grid = null;
    public Vector2 gridPosition;
    //Holds the tiles relative to this one in a 3x3 grid
    public Tile[,] _relativeTiles { get; private set; }
    [Space]
    [Header("Tile Parameters")]
    [SerializeField] private string _tileOccupierName = null; //Unit name of the occupying unit
    [SerializeField] private GameObject _tileOccupier = null; //Object that currently is occupying the tile
    [SerializeField] private tileAffiliation _affiliation = tileAffiliation.none; //Tile affiliation. This holds the tile's current player affiliation
    [SerializeField] private tileSelectionState _selectionState = tileSelectionState.unselected; //Selection state of the object. Changes the visual of the tile itself
    [SerializeField] [ReadOnlyInspector] private bool _isGhost = false; //Determines whether or not a "ghost tile" is placed on the tile. This allows the player to preview a change before it's made. 
    [SerializeField] [ReadOnlyInspector] private bool _isContiguous = false; //Used to keep track of whether a tile is contiguous to the keep tile of its affiliation
    [Space]
    [Header("VFX Parameters")]
    [SerializeField] private GameObject _selectableEffectObject; //Selection effect game object.

    // Start is called before the first frame update
    void Start()
    {
        updateSelectionEffectVisual();
    }

    // Update is called once per frame
    void Update()
    {
        updateSelectionEffectVisual(); //Remove later
    }

    //--------------------------------------
    //-Methods for accessing relative tiles-
    //--------------------------------------

    //Initializes the tile's relative tiles array
    public void initializeRelativeTilesArray()
    {
        _relativeTiles = new Tile[3, 3];
    }

    //Used to reference the relative tiles grid intuitively from -1,-1 to 1,1
    public Tile getRelativeTile(Vector2 index)
    {
        return _relativeTiles[((int)index.x + 1), ((int)index.y + 1)];
    }

    //Used to set the relative tiles grid intuitively from -1,-1 to 1,1
    public void setRelativeTile(Vector2 index, Tile targetTile)
    {
        _relativeTiles[((int)index.x + 1), ((int)index.y + 1)] = targetTile;
    }

    //---------------------------------------------------------
    //-Methods for getting and setting notable tile parameters-
    //---------------------------------------------------------

    //Sets tile's basic paramters
    public void setTileUnitParamters(GameObject newOccupier, string newOccupierName, tileAffiliation newAffiliation)
    {
        _tileOccupier = newOccupier;
        _tileOccupierName = newOccupierName;
        _affiliation = newAffiliation;
    }

    //Sets whether a tile is a ghost or not
    public void setGhostState(bool newGhostState)
    {
        _isGhost = newGhostState;
    }

    //Clears the tile of any occupiers
    public void clearTile()
    {
        Destroy(_tileOccupier);
        _tileOccupier = null;
        _tileOccupierName = null;
        _affiliation = tileAffiliation.none;
        _isGhost = false;
    }

    //Replaces the tile occupier instead of creating a new one
    public void replaceTileOccupier(GameObject newOccupier, string newOccupierName)
    {
        if(_tileOccupier != null)
        {
            Destroy(_tileOccupier);
            _tileOccupier = newOccupier;
            _tileOccupierName = newOccupierName;
        }
        else
        {
            Debug.Log(gameObject.name + " has no occupier to replace!");
        }
    }

    //Checks to see if a tile is already occupied. Returns true if so.
    public bool isOccupied()
    {
        if(_tileOccupier != null)
        {
            return true;
        }
        return false;
    }

    //Checks to see if a tile is occupied by a tile of the type in the string argument. Returns true if so.
    public bool isOccupied(string occupiedByName)
    {
        if(_tileOccupier != null && (_tileOccupierName == occupiedByName))
        {
            return true;
        }
        return false;
    }

    //Gets the tile occupier
    public GameObject getOccupier()
    {
        return _tileOccupier;
    }

    //Gets the tile occupier's name
    public string getOccupierName()
    {
        return _tileOccupierName;
    }

    //Sets the occupier without changing its name
    public void setOccupier(GameObject newOccupier)
    {
        if (_tileOccupier != null)
        {
            Destroy(_tileOccupier);
            _tileOccupier = newOccupier;
        }
        else
        {
            _tileOccupier = newOccupier;
        }
    }

    //Gets the ghost state
    public bool getGhostState()
    {
        return _isGhost;
    }

    //Gets the selection state
    public tileSelectionState getSelectionState()
    {
        return _selectionState;
    }

    //Sets the selection state
    public void setSelectionState(tileSelectionState newSelectionState)
    {
        if(newSelectionState != _selectionState)
        {
            _selectionState = newSelectionState;
        }
    }

    //Gets a tile's affiliation
    public tileAffiliation getAffiliation()
    {
        return _affiliation;
    }

    //Sets the continuity flag of the tile
    public void setContinuity(bool newContinuity)
    {
        _isContiguous = newContinuity;
    }

    //----------------------------------------
    //-Methods for managing selection effects-
    //----------------------------------------

    //Enables the visual for whichever state the tile is currently in
    public void updateSelectionEffectVisual()
    {
        switch(_selectionState)
        {
            case tileSelectionState.unselected:
                _selectableEffectObject.SetActive(false);
                break;
            case tileSelectionState.selectable:
                _selectableEffectObject.SetActive(true);
                break;
        }
    }
}
