using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour
{
    [SerializeField] public bool canSpin = true;
    [SerializeField] private Vector3 spinPerSecond;
    [SerializeField] private Space relativeTo = Space.World;

    // Update is called once per frame
    void Update()
    {
        if(canSpin)
        {
            transform.Rotate(spinPerSecond * Time.deltaTime, relativeTo);
        }
    }
}
