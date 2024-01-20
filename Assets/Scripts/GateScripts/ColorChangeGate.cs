using System.Collections;
using System.Collections.Generic;
using UnityEngine;
   public enum GateType
{
    BlackGate,
    GreenGate,
    YellowGate,
    RedGate
}
public class ColorChangeGate : MonoBehaviour
{
    public GateType gateType;
    [SerializeField]
    int gateID;
    [SerializeField] bool isUpgrade = false;
    [SerializeField] Collider collider;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            collider.enabled = false;
            other.transform.root.GetComponent<PlayerMove>().ColorChange(isUpgrade);
            other.transform.root.GetComponent<PlayerMove>().ObjDropMove(this.gameObject, gateType);

            Debug.Log($"Kapıdan geçti");
        }


    }

}
