using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStateManager : MonoBehaviour
{
    //Starting variables
    [Header("Starting Variables")]
    [SerializeField] private tileAffiliation _currentTurnAffiliation = tileAffiliation.white;
    [SerializeField] private int _turnNumber = 1;
    [SerializeField] private int _whiteAP = 0;
    [SerializeField] private int _blackAP = 1;
    [SerializeField] private bool _whiteHasFreeAction = true;
    [SerializeField] private bool _blackHasFreeAction = true;
    [Space]
    [Header("Necessary References")]
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private ToolbarManager _toolbarManager;
    [SerializeField] private TextMeshProUGUI _currentTurnText;
    [SerializeField] private TextMeshProUGUI _currentPlayerAPText;
    [SerializeField] private GameObject _currentPlayerFreeActionRing;
    [SerializeField] private TextMeshProUGUI _enemyPlayerAPText;
    [SerializeField] private GameObject _enemyPlayerFreeActionRing;

    private void Start()
    {
        UpdateTurnUI();
    }

    //--------------------------
    //-Public gamestate methods-
    //--------------------------
    
    public void useActionPointWhite()
    {
        //Remove the free action if available. Otherwise, remove 3 from white's AP
        if(_whiteHasFreeAction)
        {
            _whiteHasFreeAction = false;

            //If a free action is used, update the free action ring UI.
            if (_currentTurnAffiliation == tileAffiliation.white)
            {
                _currentPlayerFreeActionRing.SetActive(false);
            }
            else
            {
                _enemyPlayerFreeActionRing.SetActive(false);
            }
        }
        else
        {
            _whiteAP -= 3;
        }

        //Check whether or not the turn should end after the action has been taken
        checkEndTurn();
    }

    public void useActionPointBlack()
    {
        //Remove the free action if available. Otherwise, remove 3 from black's AP
        if (_blackHasFreeAction)
        {
            _blackHasFreeAction = false;

            //If a free action is used, update the free action ring UI.
            if(_currentTurnAffiliation == tileAffiliation.black)
            {
                _currentPlayerFreeActionRing.SetActive(false);
            }
            else
            {
                _enemyPlayerFreeActionRing.SetActive(false);
            }
        }
        else
        {
            _blackAP -= 3;
        }

        //Check whether or not the turn should end after the action has been taken
        checkEndTurn();
    }

    //Handles all code having to do with ending the turn.
    public void endTurn()
    {
        //Switch the current turn affiliation and reset the incoming player's free action
        //Also increases the AP count by the number of farms in the player who's turn is ending's posession
        //Only increase the turn number if black (player 2) has made a move
        if (_currentTurnAffiliation == tileAffiliation.white)
        {
            _whiteAP += _gridManager.checkFarmsNumber(tileAffiliation.white);
            _currentTurnAffiliation = tileAffiliation.black;
            _blackHasFreeAction = true;
            _currentPlayerFreeActionRing.SetActive(true);
        }
        else if (_currentTurnAffiliation == tileAffiliation.black)
        {
            _blackAP += _gridManager.checkFarmsNumber(tileAffiliation.black);
            _currentTurnAffiliation = tileAffiliation.white;
            _whiteHasFreeAction = true;
            _currentPlayerFreeActionRing.SetActive(true);
            _turnNumber++;
        }

        //Update the UI after changing the turn
        UpdateTurnUI();
    }

    //Checks whether or not the turn should end automatically after an action
    private void checkEndTurn()
    {
        if (_currentTurnAffiliation == tileAffiliation.white)
        {
            if(_whiteAP < 3 && _whiteHasFreeAction == false)
            {
                endTurn();
            }
        }
        else if (_currentTurnAffiliation == tileAffiliation.black)
        {
            if (_blackAP < 3 && _blackHasFreeAction == false)
            {
                endTurn();
            }
        }
    }

    //-----------------------
    //-UI Management methods-
    //-----------------------

    //Updates the text relating to the turn, including the current UI element
    private void UpdateTurnUI()
    {
        setToolboxUI();

        //Set the UI based on the color
        if (_currentTurnAffiliation == tileAffiliation.white)
        {
            _currentTurnText.text = "White Turn";
            _currentPlayerAPText.text = _whiteAP.ToString();
            _enemyPlayerAPText.text = _blackAP.ToString();
            _toolbarManager.setTBMAffiliation(tileAffiliation.white);
        }

        if(_currentTurnAffiliation == tileAffiliation.black)
        {
            _currentTurnText.text = "Black Turn";
            _currentPlayerAPText.text = _blackAP.ToString();
            _enemyPlayerAPText.text = _whiteAP.ToString();
            _toolbarManager.setTBMAffiliation(tileAffiliation.black);
        }
    }

    private void setToolboxUI()
    {
        //Set the toolbox UI based on the turn. Must be called before the affiliation is set.
        switch (_turnNumber)
        {
            //On turn 1, only enable the keep.
            case 1:
                for(int i = 0; i < _toolbarManager.whiteToolButtons.Length;  i++)
                {
                    if(i == 4)
                    {
                        _toolbarManager.whiteToolButtons[i].setState(toolButtonState.active);
                    }
                    else
                    {
                        _toolbarManager.whiteToolButtons[i].setState(toolButtonState.disabled);
                    }
                }

                for (int i = 0; i < _toolbarManager.blackToolButtons.Length; i++)
                {
                    if (i == 4)
                    {
                        _toolbarManager.blackToolButtons[i].setState(toolButtonState.active);
                    }
                    else
                    {
                        _toolbarManager.blackToolButtons[i].setState(toolButtonState.disabled);
                    }
                }

                break;
            //On turn 2, only enable territory tiles.
            case 2:
                for (int i = 0; i < _toolbarManager.whiteToolButtons.Length; i++)
                {
                    if (i == 0)
                    {
                        _toolbarManager.whiteToolButtons[i].setState(toolButtonState.active);
                    }
                    else
                    {
                        _toolbarManager.whiteToolButtons[i].setState(toolButtonState.disabled);
                    }
                }

                for (int i = 0; i < _toolbarManager.blackToolButtons.Length; i++)
                {
                    if (i == 0)
                    {
                        _toolbarManager.blackToolButtons[i].setState(toolButtonState.active);
                    }
                    else
                    {
                        _toolbarManager.blackToolButtons[i].setState(toolButtonState.disabled);
                    }
                }
                break;
            //On turn 3 and onwards, enable all tiles except for the keep
            default:
                for (int i = 0; i < _toolbarManager.whiteToolButtons.Length; i++)
                {
                    if (i == 4)
                    {
                        _toolbarManager.whiteToolButtons[i].setState(toolButtonState.disabled);
                    }
                    else
                    {
                        _toolbarManager.whiteToolButtons[i].setState(toolButtonState.active);
                    }
                }

                for (int i = 0; i < _toolbarManager.blackToolButtons.Length; i++)
                {
                    if (i == 4)
                    {
                        _toolbarManager.blackToolButtons[i].setState(toolButtonState.disabled);
                    }
                    else
                    {
                        _toolbarManager.blackToolButtons[i].setState(toolButtonState.active);
                    }
                }
                break;
        }
    }

    //---------------------------
    //-Getter and setter methods-
    //---------------------------

    public tileAffiliation getCurrentTurnAffiliation()
    {
        return _currentTurnAffiliation;
    }
}
