using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbarManager : MonoBehaviour
{
    [Header("Tool Button Settings")]
    [SerializeField] private toolButton[] currentToolButtons;
    [SerializeField] public toolButton[] whiteToolButtons;
    [SerializeField] public toolButton[] blackToolButtons;
    [SerializeField] private tileAffiliation currentAffiliation;
    [Space]
    [Header("Current Bar State")]
    [SerializeField] [ReadOnlyInspector] private toolbarManagerState toolbarState;
    [Space]
    [Header("Necessary References")]
    [SerializeField] private SpinningTilesManager _stm;
    [SerializeField] private ToolbarTileBehavior[] _ttb;

    private void Awake()
    {
        initializeToolButtonsList();
        matchButtonsToAffiliation();
    }

    public void DEBUGSETNORMALSTATE()
    {
        changeManagerState(toolbarManagerState.normal);
    }

    public void DEBUGSETOFFSTATE()
    {
        changeManagerState(toolbarManagerState.off);
    }

    public void DEBUGTILEGRABBEDSTATE()
    {
        changeManagerState(toolbarManagerState.tileGrabbed);
    }

    private void initializeToolButtonsList()
    {
        currentToolButtons = new toolButton[5];
    }

    //Set the current tool button to the tools of the correct color. Must use the set functions to properly set the state, object, and render texture
    private void matchButtonsToAffiliation()
    {
        if(currentAffiliation == tileAffiliation.white)
        {
            currentToolButtons = whiteToolButtons;
        }
        else if (currentAffiliation == tileAffiliation.black)
        {
            currentToolButtons = blackToolButtons;
        }

        for (int i = 0; i < currentToolButtons.Length; i++)
        {
            currentToolButtons[i].setRenderTexture(currentToolButtons[i].getRenderTexture());
            currentToolButtons[i].setState(currentToolButtons[i].getState());
            currentToolButtons[i].setButtonUIObject(currentToolButtons[i].getButtonUIObject());
        }
    }

    public void changeManagerState(toolbarManagerState managerState)
    {
        toolbarState = managerState;

        switch(toolbarState)
        {
            case toolbarManagerState.normal:
                for (int i = 0; i < currentToolButtons.Length; i++)
                {
                    //Reactivate any inactive or selected tiles.
                    if(currentToolButtons[i].getState() == toolButtonState.inactive || currentToolButtons[i].getState() == toolButtonState.selected)
                    {
                        currentToolButtons[i].setState(toolButtonState.active);
                    }

                    //Enable the hovering effect
                    for (int j = 0; j < _ttb.Length; j++)
                    {
                        _ttb[j].setActive(true);
                    }
                }

                //Turn spin back on
                _stm.setSpinStateAll(true);

                break;
            case toolbarManagerState.off:
                for (int i = 0; i < currentToolButtons.Length; i++)
                {
                    //Set all non-disabled tiles to inactive
                    if (currentToolButtons[i].getState() != toolButtonState.disabled)
                    {
                        currentToolButtons[i].setState(toolButtonState.inactive);
                    }

                    //Disable the hovering effect
                    for(int j = 0; j < _ttb.Length; j++)
                    {
                        _ttb[j].setActive(false);
                    }
                }

                //Turn spin off
                _stm.setSpinStateAll(false);

                break;
            case toolbarManagerState.tileGrabbed:
                for (int i = 0; i < currentToolButtons.Length; i++)
                {
                    //Set all non-disabled tiles to inactive
                    if (currentToolButtons[i].getState() != toolButtonState.disabled)
                    {
                        currentToolButtons[i].setState(toolButtonState.inactive);
                    }

                    //Disable the hovering effect
                    for (int j = 0; j < _ttb.Length; j++)
                    {
                        _ttb[j].setActive(false);
                    }
                }

                //Turn spin off
                _stm.setSpinStateAll(false);

                break;
        }
    }

    public void setTBMAffiliation(tileAffiliation newAffiliation)
    {
        currentAffiliation = newAffiliation;
        matchButtonsToAffiliation();
    }

    public tileAffiliation getTBMAffiliation()
    {
        return currentAffiliation;
    }
}
