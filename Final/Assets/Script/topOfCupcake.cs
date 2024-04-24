using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class topOfCupcake : MonoBehaviour
{
    [SerializeField] playerCupcake playerCupcake;

    private void OnTriggerEnter(Collider other)
    {
        playerCupcake.hit = true;
        //ouch bird script can go here
    }

}
