using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum holdingCursorState
{
    disabled,
    active,
    ghost //Used when the unit is over a tile it can be placed on
}

public class HeldUnit : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] public holdingCursorState currentState = holdingCursorState.disabled;
    [SerializeField] public string grabbedUnitName;

    [Header("Necessary References")]
    [SerializeField] private GameObject _art;
    [SerializeField] private RawImage _artRawImage;
    [SerializeField] private ToolbarManager _tbm;
    [SerializeField] private GridManager _gm;

    //Click release event variables
    public delegate void ReleaseAction();
    public static event ReleaseAction onMouseReleaseEvent;



    private bool _clickLastFrame;

    private void Update()
    {
        parseClickMethods();
        updateVisibility();
    }


    private void onClickHeld()
    {
        transform.position = Input.mousePosition;
    }

    private void onClickRelease()
    {
        //Call the mouse release event for other classes to manage
        onMouseReleaseEvent();

        //If currently active on release, execute this
        if (currentState == holdingCursorState.active)
        {
            currentState = holdingCursorState.disabled;
            updateVisibility();

            _tbm.changeManagerState(toolbarManagerState.normal);
        }

        //Reset the selection state for the grid
        _gm.resetSelectionStates();
    }


    public void setRenderTexure(Texture newTexture)
    {
        _artRawImage.texture = newTexture;
    }

    //Fires the onClickHeld or onClickRelease methods based on this method's logic
    private void parseClickMethods()
    {
        //If holding click
        if (Input.GetMouseButton(0))
        {
            onClickHeld();
            _clickLastFrame = true;
        }
        else if (_clickLastFrame == true)
        {
            onClickRelease();
            _clickLastFrame = false;
        }
        else
        {
            _clickLastFrame = false;
        }
    }

    //Checks to see whether the art should be visible based on the cursor state
    private void updateVisibility()
    {
        if(currentState == holdingCursorState.disabled || currentState == holdingCursorState.ghost)
        {
            _art.SetActive(false);
        }
        else
        {
            _art.SetActive(true);
        }
    }
}
