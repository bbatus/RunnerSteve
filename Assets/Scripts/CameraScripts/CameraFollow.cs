using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;
    public Transform Target;
    public Transform camTransform;
    public Vector3 Offset;
    public float SmoothTime;
    [SerializeField] private bool isFinishPos = false;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    private void Start()
    {
        Offset = camTransform.position - Target.position;
    }
    private void FixedUpdate() {
        Vector3 targetPosition = Target.position + Offset;
        camTransform.position = Vector3.SmoothDamp(transform.position, 
        new Vector3(transform.position.x, transform.position.y, targetPosition.z), ref velocity, SmoothTime);

    }

    public void CameraFinish()
    {
        //isFinishPos = true;
        camTransform.DOMoveX(3f, .15f); // move 3f to right in .15 seconds 
        camTransform.DORotate(new Vector3(camTransform.eulerAngles.x, -20, 0), .15f);
    }
}
