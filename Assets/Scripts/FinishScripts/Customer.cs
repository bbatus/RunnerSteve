using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] Transform customerCharacter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            other.transform.root.GetComponent<PlayerMove>().Distribute(customerCharacter);
            Debug.Log($"XXX");
        }

    }
}

