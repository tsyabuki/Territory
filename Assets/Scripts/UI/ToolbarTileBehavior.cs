using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolbarTileBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Button Variables")]
    [SerializeField] private float _scaleTime = .15f;
    [ReadOnlyInspector] [SerializeField] private Vector3 _defaultScale;
    [SerializeField] private Vector2 _targetScale = new Vector2(2f, 2f);
    [SerializeField] private bool _active = true;

    [Space]
    [Header("Necessary References")]
    [SerializeField] private HeldUnit _heldObjectCursor;
    [SerializeField] private ToolbarManager _tbm;
    [SerializeField] private GridManager _gm;

    private toolButton _passedToolButton;

    public void Awake()
    {
        _defaultScale = transform.localScale;
    }

    public void setActive(bool newActive)
    {
        _active = newActive;

        if (_active == false)
        {
            resetScale();
        }
    }

    //Sets the tool button passed in from the button class
    public void setPassedToolButton(toolButton assignedToolbutton)
    {
        _passedToolButton = assignedToolbutton;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        //Enlarge the object if active
        if(_active)
        {
            LeanTween.scale(gameObject, _targetScale, _scaleTime);
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        resetScale();
    }


    private void resetScale()
    {
        //Stop tweening of currently tweening. Reset size.
        if(LeanTween.isTweening(gameObject))
        {
            LeanTween.cancel(gameObject);
        }

        transform.localScale = _defaultScale;
    }


    //On click
    public void OnPointerDown(PointerEventData eventData)
    {
        //Turn on the held object cursor if it clicks on an active button and disable the rest of the toolbar. Set the selection state for the picked up unit.
        if(_active)
        {
            _heldObjectCursor.setRenderTexure(_passedToolButton.getRenderTexture());
            _heldObjectCursor.grabbedUnitName = _passedToolButton.getUnitName();
            _heldObjectCursor.currentState = holdingCursorState.active;

            //Set the selection state for the unit grabbed.
            switch(_passedToolButton.getUnitName())
            {
                case "White Keep":
                case "Black Keep":
                    _gm.setSelectionStatesKeep();
                    break;
                case "White Territory":
                case "Black Territory":
                    _gm.setSelectionStatesTerritory(_tbm.getTBMAffiliation());
                    break;
                default:
                    _gm.setSelectionStatesUpgrade(_tbm.getTBMAffiliation());
                    break;
            }

            _tbm.changeManagerState(toolbarManagerState.tileGrabbed);
        }
    }
}
