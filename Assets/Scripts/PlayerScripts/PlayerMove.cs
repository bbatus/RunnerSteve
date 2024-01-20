using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;
public enum PlayerType
{
    YellowPlayer,
    GreenPlayer,
    RedPlayer,
    BlackPlayer
}
public class PlayerMove : MonoBehaviour
{

    public PlayerType pType;
    [SerializeField]
    int objID;
    [SerializeField] private float playerSpeed = 4f;
    [SerializeField] private float dragSpeed = 0.0075f * 3.34f;
    [HideInInspector] public float playerStartSpeed = 0;

    [SerializeField] private float maxPosX;
    [SerializeField] private float minPosX;
    public float _mouseX;
    Vector3 myEuler;
    [SerializeField]
    List<float> mouseXList;
    [SerializeField] public bool isDrag = true;
    Sequence collectObjSequence;

    [SerializeField] public List<GameObject> colorlist = new List<GameObject>();
    [SerializeField] int colorCount = 0;
    public GameObject character;
    public int yellowAmount;
    public int greenAmount;
    public int redAmount;
    [SerializeField] GameObject distrubatedObject;
    public static PlayerMove instance = null;

    private void Awake()
    {
        // levelManager = FindObjectOfType<LevelManager>();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
    private void FixedUpdate()
    {
        if (UIManager.instance.levelState == LevelState.Playing)
        {
            Move();
            Drag();
        }
    }
    private void Move()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed);
    }
    public void SpeedEdit(float _time)
    {
        DOTween.To(() => playerSpeed, x => playerSpeed = x, playerStartSpeed, _time);
    }
    void Drag()
    {
        if (!isDrag)
            return;

#if UNITY_EDITOR
        _mouseX = Mathf.Clamp((_mouseX + Input.GetAxis("Mouse X") * dragSpeed * 12.5f), minPosX, maxPosX);
#else
        if (Input.touchCount > 0)
        {

            _mouseX = Mathf.Clamp((_mouseX + Mathf.Clamp(Input.touches[0].deltaPosition.x, -35, 35) * dragSpeed * 2), minPosX, maxPosX);//, DragIncrementSpeed * Time.fixedDeltaTime);
        }
#endif

        mouseXList.Insert(0, _mouseX);

        if (mouseXList.Count > 20)
            mouseXList.RemoveAt(mouseXList.Count - 1);

        transform.position = new Vector3(Mathf.Clamp(_mouseX, minPosX, maxPosX), transform.position.y, transform.position.z);

    }



    public void ColorChange(bool isUpdate)
    {
        if (isUpdate)
        {
            if (colorCount + 1 > colorlist.Count - 1)
                return;

            colorlist[colorCount].SetActive(false);

            colorlist[colorCount + 1].SetActive(true);
            colorCount++;
            PlayerRotateChange();
        }
        else
        {
            if (colorCount - 1 < 0)
                return;

            colorlist[colorCount].SetActive(false);

            colorlist[colorCount - 1].SetActive(true);
            colorCount--;
            PlayerRotateChange();
        }
    }

    void PlayerRotateChange()
    {
        float _rotY = colorlist[colorCount].transform.localEulerAngles.y;
        float _startRotY = colorlist[colorCount].transform.localEulerAngles.y;
        _rotY += 360f;
        Debug.Log(_rotY);
        DOTween.To(() => _startRotY, x => _startRotY = x, _rotY, .5f).OnUpdate(() =>
        {
            colorlist[colorCount].transform.eulerAngles = (new Vector3(colorlist[colorCount].transform.eulerAngles.x, _startRotY, colorlist[colorCount].transform.eulerAngles.z));
        });


        Transform targetTransform = colorlist[colorCount].transform;

        // Set up rotation tween
        float targetRotation = targetTransform.localEulerAngles.y + 360f;
        DOTween.To(() => targetTransform.localEulerAngles.y, x => targetTransform.localEulerAngles = new Vector3(targetTransform.localEulerAngles.x, x, targetTransform.localEulerAngles.z), targetRotation, 0.5f);
        // Set up jump tween
        float jumpHeight = 1f; // Adjust as needed
        DOTween.To(() => targetTransform.position.y, x => targetTransform.position = new Vector3(targetTransform.position.x, x, targetTransform.position.z), targetTransform.position.y + jumpHeight, 0.5f)
            .OnComplete(() =>
            {
                // Optionally add a smooth landing:
                DOTween.To(() => targetTransform.position.y, x => targetTransform.position = new Vector3(targetTransform.position.x, x, targetTransform.position.z), targetTransform.position.y, 0.15f);
            });
        //carlist[carCount].transform.DORotate(new Vector3(carlist[carCount].transform.eulerAngles.x, _rotY, carlist[carCount].transform.eulerAngles.z), 1F, RotateMode.Fast).SetLoops(-1).SetEase(Ease.Flash);
        // carlist[carCount].transform.eulerAngles = (new Vector3(carlist[carCount].transform.eulerAngles.x, _rotY, carlist[carCount].transform.eulerAngles.z));
        // carlist[carCount].transform.DORotate(new Vector3(carlist[carCount].transform.eulerAngles.x, _rotY, carlist[carCount].transform.eulerAngles.z), .5f);
        //carlist[carCount].transform.eulerAngles = Quaternion.Euler(new Vector3(carlist[carCount].transform.eulerAngles.x, 575f, carlist[carCount].transform.eulerAngles.z));

    }
    public void ObjDropMove(GameObject gate, GateType gateType)
    {
        if (gateType == GateType.BlackGate)
        {
            UIManager.instance.ShowMinusText();
            yellowAmount -= 5;
            UIManager.instance.YellowAmountTextChange(yellowAmount);
            greenAmount -= 5;
            UIManager.instance.GreenAmountTextChange(greenAmount);
            redAmount -= 5;
            UIManager.instance.RedAmountTextChange(redAmount);
        }
        if (gateType == GateType.YellowGate)
        {
            yellowAmount += 5;
            UIManager.instance.YellowAmountTextChange(yellowAmount);
        }
        if (gateType == GateType.GreenGate)
        {
            greenAmount += 5;
            UIManager.instance.GreenAmountTextChange(greenAmount);
        }
        if (gateType == GateType.RedGate)
        {
            redAmount += 5;
            UIManager.instance.RedAmountTextChange(redAmount);
        }
    }
    public void ObjCollectMove(GameObject obj, ObjType objType)
    {
        obj.transform.parent = colorlist[colorCount].transform;
        if (objType == ObjType.Yellow)
        {
            yellowAmount++;
            UIManager.instance.YellowAmountTextChange(yellowAmount);
        }
        else if (objType == ObjType.Green)
        {
            greenAmount++;
            UIManager.instance.GreenAmountTextChange(greenAmount);
        }
        else if (objType == ObjType.Red)
        {
            redAmount++;
            UIManager.instance.RedAmountTextChange(redAmount);
        }
        collectObjSequence = DOTween.Sequence().SetAutoKill(false);
        collectObjSequence.Append(obj.transform.DOLocalJump(colorlist[colorCount].transform.localPosition, 2f, 1, .5f).OnComplete(() =>
         {

             Debug.Log($"Eklendi");
             Taptic.Light();
         }));
        collectObjSequence.Join(obj.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), .5f));

    }

    public void FinishLine()
    {
        isDrag = false;
        transform.DOMove(new Vector3(0, transform.position.y, transform.position.z), .25f);
        CameraFollow.instance.CameraFinish();
        playerSpeed /= 2;
    }

    public void Distribute(Transform customer)
    {
        if (yellowAmount == 0 || greenAmount == 0 || redAmount == 0)
            return;

        yellowAmount--;
        greenAmount--;
        redAmount--;
        GameObject _go = Instantiate(distrubatedObject, transform.position, Quaternion.identity);
        _go.transform.parent = customer;

        _go.transform.DOLocalJump(new Vector3(0, 0, -0.75f), 2f, 1, .5f).OnComplete(() =>
        {
            int distributedGemAmount = 100;
            UIManager.instance.GemTextUpdate(true, distributedGemAmount);
            Debug.Log($"Dagitildi");
            Taptic.Light();
        });
    }
}
