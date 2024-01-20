using UnityEngine;
using DG.Tweening;

public class ObstacleTrigger : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.2f;     
    [SerializeField] private Vector3 shakeStrength = new Vector3(0.1f, 0.1f, 0.1f);  

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            ShakeCamera();
            UIManager.instance.ShowMinusText();
            PlayerMove.instance.yellowAmount -=5;
            PlayerMove.instance.greenAmount -=5;
            PlayerMove.instance.redAmount -=5;
        }
    }

    private void ShakeCamera()
    {
        Camera mainCamera = Camera.main;
        mainCamera.transform.DOShakePosition(shakeDuration, shakeStrength)
            .OnComplete(OnShakeComplete);
    }

    private void OnShakeComplete()
    {
        Debug.Log("Kamera shake tamamlandı!");
    }
}
