using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum toolButtonState
{
    disabled,
    inactive,
    active,
    selected
}

public enum toolbarManagerState
{
    normal,
    off,
    tileGrabbed
}

[System.Serializable]
public class toolButton
{
    [SerializeField] private string _unitName;
    [SerializeField] private GameObject _buttonUI;
    [SerializeField] private Texture _renderTexture;
    [SerializeField] private toolButtonState _state;
    [SerializeField] private Color _inactiveColor;


    public string getUnitName()
    {
        return _unitName;
    }

    public void setUnitName(string newName)
    {
        _unitName = newName;
    }

    public GameObject getButtonUIObject()
    {
        return _buttonUI;
    }

    //Sets the UIObject's passed reference to this class
    public void setButtonUIObject(GameObject newButtonUI)
    {
        _buttonUI = newButtonUI;
        _buttonUI.transform.GetChild(0).GetComponent<ToolbarTileBehavior>().setPassedToolButton(this);
    }

    public Texture getRenderTexture()
    {
        return _renderTexture;
    }

    public void setRenderTexture(Texture newRenderTexture)
    {
        _renderTexture = newRenderTexture;

        //Set the button object render texture
        RawImage buttonImage = _buttonUI.transform.GetChild(0).GetComponent<RawImage>();
        buttonImage.texture = _renderTexture;
    }

    public Color getInactiveColor()
    {
        return _inactiveColor;
    }

    public void setInactiveColor(Color newInactiveColor)
    {
        _inactiveColor = newInactiveColor;
    }

    public toolButtonState getState()
    {
        return _state;
    }

    //Executes various functions depending on the button state
    public void setState(toolButtonState newState)
    {
        _state = newState;
        RawImage buttonImage = _buttonUI.transform.GetChild(0).GetComponent<RawImage>();

        switch (_state)
        {
            case toolButtonState.disabled:
                _buttonUI.SetActive(false);
                break;

            case toolButtonState.inactive:
                _buttonUI.SetActive(true);
                setIsDark(buttonImage, true);

                break;

            case toolButtonState.active:
                _buttonUI.SetActive(true);
                setIsDark(buttonImage, false);
                break;

            case toolButtonState.selected:
                _buttonUI.SetActive(false);
                setIsDark(buttonImage, false);
                break;
         }
    }

    //Darkens the render texture if true. Returns it to default brightness if false.
    private void setIsDark(RawImage image, bool darkState)
    {
        //Darken the render texture if true. Brighten it if false.
        if (image != null)
        {
            if(darkState)
            {
                image.color = _inactiveColor;
            }
            else
            {
                image.color = Color.white;
            }
        }
    }
}