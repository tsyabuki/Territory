using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGhostManager : MonoBehaviour
{
    [SerializeField] private bool _isGhostVisual = false;
    [SerializeField] private GameObject _normalVisualArt;
    [SerializeField] private GameObject _ghostVisualArt;

    private void Start()
    {
        setGhostVisual(_isGhostVisual);
    }

    public void setGhostVisual(bool newGhostVisual)
    {
        _isGhostVisual = newGhostVisual;

        if(_isGhostVisual)
        {
            _ghostVisualArt.SetActive(true);
            _normalVisualArt.SetActive(false);
        }
        else
        {
            _ghostVisualArt.SetActive(false);
            _normalVisualArt.SetActive(true);
        }
    }
}
