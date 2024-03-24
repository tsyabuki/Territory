using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileUnitSpawner : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private GameObject _ghostDisplayOccupier = null; //Stores the previous tile occupier when a ghost tile needs to be displayed
    [Space]
    [Header("Neccessary References")]
    [SerializeField] private Tile _hostTile;
    [SerializeField] private UnitManager _um = null;
    [SerializeField] private HeldUnit _heldUnitCursor = null;
    [SerializeField] private GameStateManager _gsm = null;

    private void Awake()
    {
        _hostTile = gameObject.GetComponent<Tile>();
        _heldUnitCursor = GameObject.FindGameObjectWithTag("heldUnitCursor").GetComponent<HeldUnit>();
        _um = GameObject.FindGameObjectWithTag("UnitManager").GetComponent<UnitManager>();
        _gsm = GameObject.FindGameObjectWithTag("GameStateManager").GetComponent<GameStateManager>();
    }

    //Subscribe to the click event of the held unit
    private void OnEnable()
    {
        HeldUnit.onMouseReleaseEvent += OnClickRelease;
    }

    private void OnDisable()
    {
        HeldUnit.onMouseReleaseEvent -= OnClickRelease;
    }

    private void OnMouseEnter()
    {
        //Only activate this block if a held unit cursor is active above the tile and the tile is selectable
        if(_heldUnitCursor.currentState == holdingCursorState.active && (_hostTile.getSelectionState() == tileSelectionState.selectable))
        {
            if(_hostTile.isOccupied()) //Stores the old occupier to make place for the ghost block
            {
                _ghostDisplayOccupier = Instantiate(_hostTile.getOccupier(), gameObject.transform);
                _ghostDisplayOccupier.SetActive(false);
            }

            //Set the occupier to the ghost state version of the tile in the cursor
            _um.placeUnitNoRestrictions(_heldUnitCursor.grabbedUnitName, _hostTile);

            UnitGhostManager _spawnedUnitGhostManager = _hostTile.getOccupier().GetComponent<UnitGhostManager>();
            _spawnedUnitGhostManager.setGhostVisual(true);

            //Set the ghost state for both the tile and the held unit cursor
            _hostTile.setGhostState(true);
            _heldUnitCursor.currentState = holdingCursorState.ghost;
        }
    }

    //Called when the held unit cursor calls the click release event
    private void OnClickRelease()
    {
        //Only activate this block if a held unit cursor is active and the tile is a ghost
        if (_heldUnitCursor.currentState == holdingCursorState.ghost && _hostTile.getGhostState())
        {
            //Set cursor state to active so it can reset naturally (deactivate code runs after the on click release event)
            _heldUnitCursor.currentState = holdingCursorState.active;

            //Place the unit on the tile
            //First determine if this is an upgrade. If so, replace the occupier with the old piece and then run the upgrade unit command
            //If it's not an upgrade, simply clear the tile and spawn a unit
            bool placementIsAttemptedUpgrade = false;
            if(_ghostDisplayOccupier != null)
            {
                placementIsAttemptedUpgrade = true;
            }
            if(placementIsAttemptedUpgrade)
            {
                _ghostDisplayOccupier.SetActive(true);
                _hostTile.setOccupier(_ghostDisplayOccupier);
                _ghostDisplayOccupier = null;
                _um.upgradeUnit(_hostTile.getOccupierName(), _heldUnitCursor.grabbedUnitName, _hostTile);
            }
            else
            {
                _hostTile.clearTile();
                //Only allow the tile to be placed if it isn't an upgraded tile
                if(!_um.isUnitUpgrade(_heldUnitCursor.grabbedUnitName))
                {
                    _um.spawnUnit(_heldUnitCursor.grabbedUnitName, _hostTile);
                }
                else
                {
                    Debug.Log("Cannot place upgrade tiles directly. Abborting spawn.");
                }
            }

            //Uses action points for the affiliation that just placed the tile.
            if(_gsm.getCurrentTurnAffiliation() == tileAffiliation.white)
            {
                _gsm.useActionPointWhite();
            }
            else if(_gsm.getCurrentTurnAffiliation() == tileAffiliation.black)
            {
                _gsm.useActionPointBlack();
            }

            //Resets the tile's ghost state
            _hostTile.setGhostState(false);
        }
    }

    private void OnMouseExit()
    {
        //Only reset the ghost state if a tile wasn't placed
        if(_hostTile.getGhostState() == true)
        {
            resetGhostState();
        }
    }

    //Resets ghost states on both the tile and cursor
    private void resetGhostState()
    {
        _hostTile.setGhostState(false);
        if(_heldUnitCursor.currentState == holdingCursorState.ghost)
        {
            _heldUnitCursor.currentState = holdingCursorState.active;
        }

        //If there was an occupier before the ghost operation, replace it with the previous occupier
        if(_ghostDisplayOccupier != null)
        {
            _ghostDisplayOccupier.SetActive(true);
            _hostTile.setOccupier(_ghostDisplayOccupier);
            _ghostDisplayOccupier = null;
        }
        else
        {
            _hostTile.setOccupier(null);
        }
    }
}
