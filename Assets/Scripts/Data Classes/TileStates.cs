using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Available selection states for the tile
public enum tileSelectionState
{
    unselected,
    selectable,
    selected,
    attackable,
    attacked
}

//Available affiliations for the tile
public enum tileAffiliation
{
    none,
    white,
    black
}

