using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningTilesManager : MonoBehaviour
{
    [SerializeField] private spin[] _allTiles;
    [SerializeField] private spin[] _whiteTiles;
    [SerializeField] private spin[] _blackTiles;

    public void setSpinStateAll(bool newState)
    {
        for (int i = 0; i < _allTiles.Length; i++)
        {
            _allTiles[i].canSpin = newState;
        }
    }

    public void setSpinStateWhite(bool newState)
    {
        for (int i = 0; i < _whiteTiles.Length; i++)
        {
            _whiteTiles[i].canSpin = newState;
        }
    }

    public void setSpintStateBlack(bool newState)
    {
        for (int i = 0; i < _blackTiles.Length; i++)
        {
            _blackTiles[i].canSpin = newState;
        }
    }
}
