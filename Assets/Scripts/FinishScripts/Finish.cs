using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            other.transform.root.GetComponent<PlayerMove>().FinishLine();

            Debug.Log($"Finish");
        }

    }
}

