using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DebugButtonEventSystem : MonoBehaviour
{
    public UnityEvent interactFunction;

    public void onButton()
    {
        interactFunction.Invoke();
    }
}
