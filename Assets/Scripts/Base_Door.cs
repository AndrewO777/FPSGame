using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Door : MonoBehaviour
{
    public SphereCollider sphereCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sphereCollider.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sphereCollider.enabled = true;
        }
    }
}
