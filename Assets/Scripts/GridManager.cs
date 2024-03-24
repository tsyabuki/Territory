using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Setup Variables")]
    [SerializeField] private Vector2 _gridSize;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private float _tileDistance;
    [Space]
    [Header("Continuity Calculation Variables")]
    [SerializeField] private List<Tile> bodyTiles; //All tiles in the currently checked tile body
    [SerializeField] private List<Tile> newTiles; //Tiles that have just been added to the currently checked tile body

    public GameObject[,] gridArray;

    public Vector2 DEBUG_TILE_SELECTOR;
    public tileAffiliation DEBUG_PLAYER_AFFILIATION;

    private void Awake()
    {
        //Set proper grid size
        gridArray = new GameObject[(int)_gridSize.x, (int)_gridSize.y];

        //Create all of the grid tiles
        initializeGrid();
    }



    //------------------------
    //-Initialization methods-
    //------------------------

    //Generates all tiles in the grid
    private void initializeGrid()
    {
        //Find the offset to center the grid's tiles
        Vector3 gridOffset = new Vector3(_tileDistance * (_gridSize.x - 1) * -0.5f, 0f, _tileDistance * (_gridSize.y - 1) * -0.5f);

        //Create all grid tiles
        Vector3 newTilePosition = Vector3.zero;
        for (int i = 0; i < _gridSize.y; i++)
        {
            for (int j = 0; j < _gridSize.x; j++)
            {
                newTilePosition = (_tileDistance * new Vector3(j, 0f, i)) + gridOffset;
                gridArray[j, i] = Instantiate(_tilePrefab, newTilePosition, Quaternion.identity, transform);
                initializeTile(gridArray[j, i], new Vector2(j, i));
            }
        }

        //Populate the "relative tiles" field of each tile after all tiles have been initialized
        for (int k = 0; k < _gridSize.y; k++)
        {
            for (int l = 0; l < _gridSize.x; l++)
            {
                populateRelativeTiles(gridArray[l, k]);
            }
        }
    }

    //Initializes a tile's internal grid position and parents it to the grid (does not populate adjacent tiles)
    private void initializeTile(GameObject tileObject, Vector2 position)
    {
        tileObject.name = "Tile (" + position.x + ", " + position.y + ")";

        Tile targetTile = tileObject.GetComponent<Tile>();

        targetTile.grid = gameObject;
        targetTile.gridPosition = position;
    }

    //Populates the tile's relative tiles. All tiles must be initialized already
    private void populateRelativeTiles(GameObject tileObject)
    {
        Tile targetTile = tileObject.GetComponent<Tile>();
        Vector2 currentPosition = targetTile.gridPosition;
        //Initializes the relative tiles 2D array as a 3x3 grid
        targetTile.initializeRelativeTilesArray();

        //Debug.Log("Current Tile: " + targetTile.name);

        //Ensures that the tile being checked for is inside of the main grid, then populates it
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                //Debug.Log("Attempting to filling relative tile (" + j + ", " + i + ")");

                if ((currentPosition.x + j < _gridSize.x) && (currentPosition.y + i < _gridSize.y) && (currentPosition.x + j >= 0) && (currentPosition.y + i >= 0))
                {
                    //Debug.Log("Success. Filled grid tile (" + ((int)targetTile.gridPosition.x + j) + ", " + ((int)targetTile.gridPosition.y + i) + ")");
                    targetTile.setRelativeTile(new Vector2(j, i), gridArray[(int)targetTile.gridPosition.x + j, (int)targetTile.gridPosition.y + i].GetComponent<Tile>());
                }
            }
        }
    }

    //-----------------------------------------------
    //-Methods for checking the number of farm tiles-
    //-----------------------------------------------

    //Find the number of farms
    public int checkFarmsNumber(tileAffiliation farmAffiliation)
    {
        int totalFarms = 0;
        Tile currentTile = null;

        //Loop through all the tiles. If it's a black or white farm, check if it matches the farm affiliation and then add to the total.
        for (int i = 0; i < _gridSize.y; i++)
        {
            for (int j = 0; j < _gridSize.x; j++)
            {
                currentTile = gridArray[j, i].GetComponent<Tile>();

                if(farmAffiliation == tileAffiliation.white && currentTile.getOccupierName() == "White Farm")
                {
                    totalFarms++;
                }

                if (farmAffiliation == tileAffiliation.black && currentTile.getOccupierName() == "Black Farm")
                {
                    totalFarms++;
                }
            }

        }

        return totalFarms;
    }

    //----------------------------------------------------------
    //-Methods for finding and setting the continuity of a tile-
    //----------------------------------------------------------

    //Sets the tile continuity of every tile on the grid.
    public void updateBoardTileContinuity()
    {
        Tile currentTile = null;
        tileAffiliation currentTileAffiliation = tileAffiliation.none;
        bool currentTileCheckResult = false;

        for(int i = 0; i < _gridSize.y; i++)
        {
            for(int j = 0; j < _gridSize.x; j++)
            {
                currentTile = gridArray[j, i].GetComponent<Tile>();
                currentTileAffiliation = currentTile.getAffiliation();

                currentTileCheckResult = checkTileContinuity(currentTile, currentTileAffiliation);

                currentTile.setContinuity(currentTileCheckResult);
            }
        }
    }

    //Checks the continuity of a target tile given an affiliation
    public bool checkTileContinuity(Tile targetTile, tileAffiliation targetAffiliation)
    {
        //Used for storing tiles in the iteration
        Tile currentTile = null;
        Tile currentRelativeTile = null;

        //Clear the body and new tile arrays from previous operations
        bodyTiles.Clear();
        newTiles.Clear();

        //If the target tile is not occupied, immediately return false. A tile cannot be continuous if it has not tile.
        if(targetTile.isOccupied() == false)
        {
            return false;
        }

        //Add the target tile as the first tile in the body of tiles we are checking
        bodyTiles.Add(targetTile);
        newTiles.Add(targetTile);

        //Debug.Log("Starting continuity loop");

        //Loops through all "new tiles" until there are none left present on the list
        do
        {
            //Sets the first item in new tiles to the current tile being checked
            currentTile = newTiles[0];

            //Debug.Log("Current tile: " + currentTile.gameObject.name);

            //Check each adjacent tile. If not null and the correct affiliation, add them to the new tiles list.
            if (currentTile.getRelativeTile(new Vector2(-1, 0)) != null) //Left
            {
                currentRelativeTile = currentTile.getRelativeTile(new Vector2(-1, 0));

                //Debug.Log("Checking tile Left: " + currentRelativeTile.name);

                if (
                (currentRelativeTile.isOccupied() == true)
                && (currentRelativeTile.getAffiliation() == targetAffiliation)
                && (bodyTiles.Contains(currentRelativeTile) == false)
                )
                {
                    bodyTiles.Add(currentRelativeTile);
                    newTiles.Add(currentRelativeTile);
                }
            }

            if (currentTile.getRelativeTile(new Vector2(1, 0)) != null) //Right
            {
                currentRelativeTile = currentTile.getRelativeTile(new Vector2(1, 0));

                //Debug.Log("Checking tile Right: " + currentRelativeTile.name);

                if (
                (currentRelativeTile.isOccupied() == true)
                && (currentRelativeTile.getAffiliation() == targetAffiliation)
                && (bodyTiles.Contains(currentRelativeTile) == false)
                )
                {
                    bodyTiles.Add(currentRelativeTile);
                    newTiles.Add(currentRelativeTile);
                }
            }

            if (currentTile.getRelativeTile(new Vector2(0, 1)) != null) //Up
            {
                currentRelativeTile = currentTile.getRelativeTile(new Vector2(0, 1));

                //Debug.Log("Checking tile Up: " + currentRelativeTile.name);

                if (
                (currentRelativeTile.isOccupied() == true)
                && (currentRelativeTile.getAffiliation() == targetAffiliation)
                && (bodyTiles.Contains(currentRelativeTile) == false)
                )
                {
                    bodyTiles.Add(currentRelativeTile);
                    newTiles.Add(currentRelativeTile);
                }
            }

            if (currentTile.getRelativeTile(new Vector2(0, -1)) != null) //Down
            {
                currentRelativeTile = currentTile.getRelativeTile(new Vector2(0, -1));

                //Debug.Log("Checking tile Down: " + currentRelativeTile.name);

                if (
                (currentRelativeTile.isOccupied() == true)
                && (currentRelativeTile.getAffiliation() == targetAffiliation)
                && (bodyTiles.Contains(currentRelativeTile) == false)
                )
                {
                    bodyTiles.Add(currentRelativeTile);
                    newTiles.Add(currentRelativeTile);
                }
            }

            //Remove the just-checked tile from the new tiles list
            newTiles.Remove(currentTile);
        } while (newTiles.Count != 0);

        //Run through the body tiles array and return true if the keep for the correct affiliation is found
        for(int i = 0; i < bodyTiles.Count; i++)
        {
            if(targetAffiliation == tileAffiliation.white && bodyTiles[i].getOccupierName() == "White Keep")
            {
                return true;
            }

            if (targetAffiliation == tileAffiliation.black && bodyTiles[i].getOccupierName() == "Black Keep")
            {
                return true;
            }
        }

        return false;
    }

    //---------------------------------------
    //-Methods for handling selection states-
    //---------------------------------------

    //Resets all selection state to unselected for all tiles
    public void resetSelectionStates()
    {
        Tile currentTile = null;

        for (int i = 0; i < _gridSize.y; i++)
        {
            for (int j = 0; j < _gridSize.x; j++)
            {
                currentTile = gridArray[j, i].GetComponent<Tile>();
                currentTile.setSelectionState(tileSelectionState.unselected);
            }
        }
    }

    //Sets the selection state for all tiles except for currently occupied tiles to selectable. Used for placing keeps.
    public void setSelectionStatesKeep()
    {
        resetSelectionStates();

        Tile currentTile = null;

        for (int i = 0; i < _gridSize.y; i++)
        {
            for (int j = 0; j < _gridSize.x; j++)
            {
                currentTile = gridArray[j, i].GetComponent<Tile>();
                if (currentTile.isOccupied() == false)
                {
                    currentTile.setSelectionState(tileSelectionState.selectable);
                }
            }
        }
    }

    //Sets the selection state for all tiles adjactent to other tiles of the same color to selectable. Used for placing territory tiles.
    public void setSelectionStatesTerritory(tileAffiliation playerAffiliation)
    {
        resetSelectionStates();

        Tile currentTile = null;
        Tile currentRelativeTile = null;

        for (int i = 0; i < _gridSize.y; i++)
        {
            for (int j = 0; j < _gridSize.x; j++)
            {
                currentTile = gridArray[j, i].GetComponent<Tile>();
                //Check if a tile is occupied by a tile of the player affiliation
                if ((currentTile.isOccupied() == true) && (currentTile.getAffiliation() == playerAffiliation))
                {
                    //Check the surrounding tiles. If unoccupied, set the selection state to selectable
                    if(currentTile.getRelativeTile(new Vector2(-1, 0)) != null) //Left
                    {
                        currentRelativeTile = currentTile.getRelativeTile(new Vector2(-1, 0));
                        if((currentRelativeTile.isOccupied() == false))
                        {
                            currentRelativeTile.setSelectionState(tileSelectionState.selectable);
                        }
                    }

                    //Check the surrounding tiles. If unoccupied, set the selection state to selectable
                    if (currentTile.getRelativeTile(new Vector2(1, 0)) != null) //Right
                    {
                        currentRelativeTile = currentTile.getRelativeTile(new Vector2(1, 0));
                        if ((currentRelativeTile.isOccupied() == false))
                        {
                            currentRelativeTile.setSelectionState(tileSelectionState.selectable);
                        }
                    }

                    //Check the surrounding tiles. If unoccupied, set the selection state to selectable
                    if (currentTile.getRelativeTile(new Vector2(0, 1)) != null) //Up
                    {
                        currentRelativeTile = currentTile.getRelativeTile(new Vector2(0, 1));
                        if ((currentRelativeTile.isOccupied() == false))
                        {
                            currentRelativeTile.setSelectionState(tileSelectionState.selectable);
                        }
                    }

                    //Check the surrounding tiles. If unoccupied, set the selection state to selectable
                    if (currentTile.getRelativeTile(new Vector2(0, -1)) != null) //Down
                    {
                        currentRelativeTile = currentTile.getRelativeTile(new Vector2(0, -1));
                        if ((currentRelativeTile.isOccupied() == false))
                        {
                            currentRelativeTile.setSelectionState(tileSelectionState.selectable);
                        }
                    }
                }
            }
        }
    }

    //Sets the selection state for all tiles territory tiles of the correct affiliation to selectable. Used for upgrading tiles.
    public void setSelectionStatesUpgrade(tileAffiliation playerAffiliation)
    {
        resetSelectionStates();

        Tile currentTile = null;

        for (int i = 0; i < _gridSize.y; i++)
        {
            for (int j = 0; j < _gridSize.x; j++)
            {
                currentTile = gridArray[j, i].GetComponent<Tile>();
                //Checks to see if the tile is both occupied and of the correct affiliation
                if (currentTile.isOccupied() == true && (currentTile.getAffiliation() == playerAffiliation))
                {
                    //White affiliation selection
                    if((playerAffiliation == tileAffiliation.white) && (currentTile.getOccupierName() == "White Territory"))
                    {
                        currentTile.setSelectionState(tileSelectionState.selectable);
                    }

                    //Black affiliation selection
                    if ((playerAffiliation == tileAffiliation.black) && (currentTile.getOccupierName() == "Black Territory"))
                    {
                        currentTile.setSelectionState(tileSelectionState.selectable);
                    }
                }
            }
        }
    }

    public void DEBUGSETSELECTIONSTATETERRITORY()
    {
        setSelectionStatesTerritory(DEBUG_PLAYER_AFFILIATION);
    }

    public void DEBUGSETSELECTIONSTATEUPGRADE()
    {
        setSelectionStatesUpgrade(DEBUG_PLAYER_AFFILIATION);
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // DEBUG UPDATE
    void Update()
    {
        //Cheat key to allow the player to place a tile anywhere for debugging purposes
        /*if(Input.GetKeyDown("p"))
        {
            setSelectionStatesKeep();
        }*/
    }
}
