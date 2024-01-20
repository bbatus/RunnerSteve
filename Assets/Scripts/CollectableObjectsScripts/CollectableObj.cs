using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    public enum ObjType
{
    Yellow,
    Green,
    Red,
    Black
}
public class CollectableObj : MonoBehaviour
{
  public ObjType objType;
    [SerializeField]
    int objID;

    [SerializeField]
    Collider collider;


 private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            collider.enabled = false;
            other.transform.root.GetComponent<PlayerMove>().ObjCollectMove(this.gameObject, objType);
        }
    }

}
