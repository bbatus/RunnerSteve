using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
//using GameAnalyticsSDK;


public enum LevelState //oyunun statelerini tutuyor, sıra ile numaralandırılma
{
    Stop,
    Playing,
    Win,
    Lose
}

public class UIManager : MonoBehaviour
{
    public LevelState levelState; //oyun durumunu tutan enum değişkeni referansı

    public static UIManager instance = null; //singleton
    // UI managerin diğer scriptlerden erişilebilmesi için statik instance olusturuluyor

    //oyunda feedback icin dizideki stringler
    string[] feedbacks = new string[] { "Wow!", "Amazing!", "Perfect!", "Good!", "Great!", "Incredible!", "Nice!", "Cool!" };

    [SerializeField]
    private GameObject levelCompletePanel;

    [SerializeField]
    private GameObject levelFailedPanel;

    [SerializeField]
    public GameObject tutorialPanel;

    [SerializeField]
    Text currentLevelText, nextLevelText;

    ParticleSystem[] confettiArray;

    GameObject speedLineParticle;

    [SerializeField]
    TextMeshProUGUI feedbackText;

    IEnumerator feedback;

    float mainFOV;

    public Image inkPercentBar;


    public Text inkPercentText;

    [SerializeField]
    private Animation inkAnim;

    bool levelComplete = false;
    [SerializeField]
    Transform cursorTransform;
    Camera Cam;
    float distanceFromCamera = 13;

    [SerializeField]
    GameObject gemRef;

    [SerializeField]
    RectTransform gemEndPoint;

    [SerializeField]
    public Text gemText;
    [HideInInspector]
    public float gemCount;

    [SerializeField]
    Image stageBar;

    GameObject finishline;
    float maxDistance;
    int randomNumber;

    [SerializeField]
    Text rowText;

    [SerializeField]
    Text failPanelRowText;
    [SerializeField]
    Text winPanelRowText;

    Material[] bgClone;
    bool tutorialPanelActive = true;
    [SerializeField]
    public Text releaseShootText;
    [SerializeField]
    GameObject timerPanel;

    [SerializeField]
    public List<Vector3> handPosList = new List<Vector3>();
    [SerializeField] GameObject newFingerPrefab;
    [SerializeField] bool videoFinger = false;
    GameObject finger;
    [SerializeField] GameObject startBtn;
    [SerializeField] GameObject upgradePanel;
    [SerializeField] Image nextCarImg;

    [SerializeField] GameObject gemCreatePrefab;

    [SerializeField] Text YellowAmountText;
    [SerializeField] Text GreenAmountText;
    [SerializeField] Text RedAmountText;
    [SerializeField] Text DropText;

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

        Application.targetFrameRate = 60;

        //PlayerPrefs.SetFloat("Gem", 10000);
    }
    private void Start()
    {
        // TinySauce.OnGameStarted(PlayerPrefs.GetInt("level").ToString());

        // RandomBackGround();
        FindComponents();
        SetFOV();
        /*  if (PlayerPrefs.GetInt("Level1") == 0)
          {
              FindPos();
              PlayerPrefs.SetInt("Level1", 1);
          }*/

        ConfettiSetActive(true);

        UpdateLevelTexts();

        YellowAmountTextChange(0);
        GreenAmountTextChange(0);
        RedAmountTextChange(0);
        // PlayerPrefs.SetFloat("Gem", 0f);
        /*  if (PlayerPrefs.GetInt("FirstLogin") == 1)
          {
              StartCoroutine(TutorialPanelSetActive(true, 0));
          }x
          else
          {
              StartCoroutine(TutorialPanelSetActive(false, 0));
          }
          */
        if (videoFinger)
        {
            finger = Instantiate(newFingerPrefab, newFingerPrefab.transform.position, Quaternion.identity);
        }

    }
    void FindComponents()
    {

        // speedLineParticle= Camera.main.transform.Find("SpeedLineEffect").gameObject;
        // SpeedLineSetActive(false);


        Cam = Camera.main;
        //  confettiArray =FindObjectOfType<Confetti>().transform.GetComponentsInChildren<ParticleSystem>();
        confettiArray = Camera.main.GetComponentsInChildren<ParticleSystem>();



        gemCount = PlayerPrefs.GetFloat("Gem");
        if (gemCount > 99999999) gemText.text = (gemCount / 1000000000).ToString("F1") + "B";
        else if (gemCount > 999999) gemText.text = (gemCount / 1000000).ToString("F1") + "M";
        else if (gemCount > 99999) gemText.text = (gemCount / 1000).ToString() + "K";
        else if (gemCount > 999) gemText.text = (gemCount / 1000).ToString("F1") + "K";
        //else if (gemCount < 1000) gemText.text = (gemCount).ToString("F1");
    }
    public void YellowAmountTextChange(int amount)
    {
        YellowAmountText.text = amount.ToString();
    }
    public void GreenAmountTextChange(int amount)
    {
        GreenAmountText.text = amount.ToString();
    }
    public void RedAmountTextChange(int amount)
    {
        RedAmountText.text = amount.ToString();
    }
    /*  public void FindPos()
      {
          // finger=Resources.Load("finger") as GameObject;
          // newFinger=Instantiate(finger);

          if (PlayerPrefs.GetInt("FirstLogin") == 1)
          {
              for (int i = 0; i < handPosList.Count; i++)
              {
                  handPosList.RemoveAt(i);
              }
              cursorTransform = FindObjectOfType<HandIcon>().transform;
              posEmoji = FindObjectsOfType<EmojiParent>();
              for (int i = 0; i < posEmoji.Length; i++)
              {
                  if (posEmoji[i].emojiIndex == 10)
                  {
                      handPosList.Add(posEmoji[i].transform.position);
                  }
              }
              HandAnim();
          }
          else
          {
              cursorTransform = FindObjectOfType<HandIcon>().transform;
              cursorTransform.transform.gameObject.SetActive(false);
          }
      }*/
    public void HandAnim()
    {
        if (PlayerPrefs.GetInt("FirstLogin") == 1 && tutorialPanelActive)
        {

            cursorTransform.position = handPosList[0];
            cursorTransform.DOMove(handPosList[1], 1.5f).OnComplete(() =>
            {
                HandAnim();
            });
        }
    }

    void SetFOV()
    {

        float cameraFov = 65f;
        Vector3 levelTextPos = currentLevelText.transform.parent.GetComponent<RectTransform>().anchoredPosition;

        if (IsXGenDevice())
        {
            cameraFov = 70;
            levelTextPos.y = -70f;

            //FindObjectOfType<CameraFollow>().offset.y = 1.5f;
        }
        /*   else if (UnityEngine.iOS.Device.generation.ToString().Contains("iPad"))
           {
               cameraFov = 55;
               //FindObjectOfType<CameraFollow>().offset.y = 0;

               // levelTextPos.y = -65f;
           }*/
        else
        {
            cameraFov = 65;
            levelTextPos.y = -10f;

            //FindObjectOfType<CameraFollow>().offset.y = 0;

        }

        // Camera.main.fieldOfView = cameraFov;

        foreach (Camera cam in FindObjectsOfType<Camera>())
        {
            cam.fieldOfView = cameraFov;
        }

        currentLevelText.transform.parent.GetComponent<RectTransform>().anchoredPosition = levelTextPos;

        //ipad 5.6 , level pos = -40
        //ip X 5.5 , level pos = -95
        // ip 5 , level pos = -45


        //  Camera.main.fieldOfView = mainFOV;

        // RectTransform panel = inkPercentBar.transform.parent.GetComponent<RectTransform>();
        // Vector3 panelPos = panel.anchoredPosition;

        // if (IsXGenDevice())
        // {
        //     panelPos.y = -400;
        // }
        // else
        //     panelPos.y = -205;


        // panel.anchoredPosition = panelPos;
    }


    // public void FOVEffect(bool targetBool){

    //     if(targetBool==true){

    //         DOTween.To(()=> Camera.main.fieldOfView, 
    //         x=> Camera.main.fieldOfView = x, mainFOV+4f,.5f).SetEase(Ease.Flash);
    //     }
    //     else {
    //         DOTween.To(()=> Camera.main.fieldOfView, 
    //         x=> Camera.main.fieldOfView = x, mainFOV,.5f).SetEase(Ease.Flash);

    //     }
    // }

    public void StartBtnAction()
    {
        StartCoroutine(TutorialPanelSetActive(false, 0));
        //upgradePanel.SetActive(false);
        levelState = LevelState.Playing;
    }
    void Update()
    {
        /* if (Input.GetMouseButton(0) && tutorialPanelActive)
         {
             StartCoroutine(TutorialPanelSetActive(false, 0));
             // timerPanel.SetActive(true);
             if (cursorTransform != null)
             {
                 cursorTransform.gameObject.SetActive(false);
             }

             tutorialPanelActive = false;
         }
         */
        if (Input.GetKey(KeyCode.R))
        {
            OnTapToRetry();
        }
        if (Input.GetKey(KeyCode.N))
        {
            OnTapToContinue();
        }

        if (finger != null)
        {
            finger.transform.position = Cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceFromCamera));
        }
    }
    void UpdateLevelTexts()
    {

        int levelNum = PlayerPrefs.GetInt("level");
        currentLevelText.text = "Level" + " " + levelNum.ToString();
        nextLevelText.text = (levelNum + 1).ToString();

    }

    public void OnTapToContinue()
    {
        int res = Random.Range(2, SceneManager.sceneCountInBuildSettings);
        // if(SceneManager.GetActiveScene().buildIndex==1)
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // else

        if (PlayerPrefs.GetInt("level") > SceneManager.sceneCountInBuildSettings - 1)
        {

            /* if (res == SceneManager.GetActiveScene().buildIndex)
                 OnTapToContinue();
             else
                 SceneManager.LoadScene(res);*/

            if (res == SceneManager.GetActiveScene().buildIndex)
            {
                OnTapToContinue();
            }

            else
            {
                PlayerPrefs.SetInt("sceneNumber", res);
                SceneManager.LoadScene(PlayerPrefs.GetInt("sceneNumber"));

            }


        }
        else
        {

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);


        }
        PlayerPrefs.SetInt("FirstLogin", 2);
        PlayerPrefs.SetFloat("Gem", gemCount);
    }

    public void OnTapToRetry()
    {
        PlayerPrefs.SetFloat("Gem", gemCount);
        PlayerPrefs.SetInt("FirstLogin", 2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public IEnumerator ActivateLevelCompletePanel(float waitTime)
    {
        levelComplete = true;
        levelState = LevelState.Win;
        winPanelRowText.text = rowText.text;

        //  SendLevelFinishData(true);

        //  GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level " + PlayerPrefs.GetInt("level").ToString());
        //TinySauce.OnGameFinished(PlayerPrefs.GetInt("level").ToString(), levelComplete, 0);
        int levelNum = PlayerPrefs.GetInt("level");
        PlayerPrefs.SetInt("level", levelNum + 1);

        ConfettiSetActive(true);
        Taptic.Success();
        yield return new WaitForSeconds(waitTime);
        levelCompletePanel.SetActive(true);

        yield return null;
    }

    public IEnumerator ActivateLevelFailedPanel(float waitTime)
    {
        levelComplete = false;
        levelState = LevelState.Lose;

        failPanelRowText.text = rowText.text;

        //  SendLevelFinishData(false);

        //   GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "level " + PlayerPrefs.GetInt("level").ToString());
        //TinySauce.OnGameFinished(PlayerPrefs.GetInt("level").ToString(), levelComplete, 0);
        Taptic.Failure();
        yield return new WaitForSeconds(waitTime);
        levelFailedPanel.SetActive(true);

        yield return null;
    }


    public bool IsXGenDevice()
    {
        Resolution res = Screen.currentResolution;
        Debug.Log("Width: " + res.width + " Height: " + res.height);
        Debug.Log("Current Resolution: " + Screen.currentResolution.ToString());
        Debug.Log("Aspect Ratio: " + ((float)Screen.currentResolution.height / Screen.currentResolution.width));

        if ((res.height / res.width) > 1920 / 1080)
        {

            print("x gen= TRUE");
            return true;
        }
        else
        {

            print("x gen= FALSE");
            return false;

        }
    }

    public void GiveFeedback()
    {
        if (feedback == null)
        {
            feedback = StageFeedback();
            StartCoroutine(feedback);
            return;
        }


        if (feedbackText.gameObject.activeInHierarchy == true && feedback != null)
        {
            StopCoroutine(feedback);
            feedbackText.GetComponent<Animation>().Stop();

        }

        feedback = StageFeedback();

        StartCoroutine(feedback);


    }

    public IEnumerator StageFeedback()
    {

        feedbackText.gameObject.SetActive(true);

        string _feedback = feedbacks[Random.Range(0, feedbacks.Length)];
        feedbackText.text = _feedback;
        feedbackText.GetComponent<Animation>().Play();

        yield return new WaitForSeconds(1);
        feedbackText.gameObject.SetActive(false);
    }

    public void ConfettiSetActive(bool targetBool)
    {

        if (confettiArray.Length < 1)
        {
            Debug.LogError("Cannot found any confetti in camera");
            return;
        }

        foreach (ParticleSystem confetti in confettiArray)
        {
            if (confetti.tag != "SpeedLineEffect")
            {
                confetti.gameObject.SetActive(targetBool);
            }


            if (targetBool == true)
                confetti.transform.parent = null;
        }

    }

    public void SpeedLineSetActive(bool targetBool)
    {
        speedLineParticle.SetActive(targetBool);
    }

    public IEnumerator TutorialPanelSetActive(bool targetBool, float delay)
    {

        yield return new WaitForSeconds(delay);
        tutorialPanel.SetActive(targetBool);


    }

    // public void TutorialPanelSetActive()
    // {
    //     tutorialPanel.SetActive(true);
    // }

    /*  public void GemCollectAnim(GameObject gem, int _moneyImageCount, float _moneyValue)
      {


          Vector3 gemUIpos = gemRef.transform.position;

          GameObject _go = Instantiate(gemCreatePrefab, gemUIpos, Quaternion.identity);
          _go.transform.GetComponent<GemControl>().GemMove(_moneyImageCount, gemUIpos, gemRef, gemEndPoint, _moneyValue);

          /*     Vector3 gemUIpos = Camera.main.WorldToScreenPoint(gem.transform.position);

               GameObject _gem = Instantiate(gemRef);
               _gem.gameObject.SetActive(true);

               _gem.transform.parent = gemRef.transform.parent;
               _gem.transform.localScale = gemEndPoint.localScale;
               _gem.transform.position = gemUIpos;

               _gem.GetComponent<RectTransform>().DOAnchorPos(gemEndPoint.anchoredPosition, .75f, false).SetEase(Ease.Flash).OnComplete(() =>
               {
                   Destroy(_gem);
                   //gemCount += 1;
                   //gemText.text = gemCount.ToString();
                   GemTextUpdate(true, _moneyValue);
               });
       */

    //}
    public void GemTextUpdate(bool _value, float _moneyValue)
    {
        if (_value)
        {
            gemCount += _moneyValue;

        }
        else
        {
            gemCount -= _moneyValue;

            if (gemCount < 0)
            {
                gemCount = 0;
            }

        }

        if (gemCount > 99999999) gemText.text = (gemCount / 1000000000).ToString("F1") + "B";
        else if (gemCount > 999999) gemText.text = (gemCount / 1000000).ToString("F1") + "M";
        else if (gemCount > 99999) gemText.text = (gemCount / 1000).ToString() + "K";
        else if (gemCount > 999) gemText.text = (gemCount / 1000).ToString("F1") + "K";
        else if (gemCount < 1000) gemText.text = (gemCount).ToString("F1");
        //gemText.text = gemCount.ToString();
        PlayerPrefs.SetFloat("Gem", gemCount);

        if (!gemEndPoint.transform.GetComponent<Animation>().isPlaying)
        {
            gemEndPoint.transform.GetComponent<Animation>().Play();
        }


    }

    /* public void NextCarBarUpgrade(float _minValue, float _currentValue)
     {
         //float _valuedifference = (_minValue * 2) - _currentValue;

         float _value = ((_currentValue - _minValue) / _minValue);
         // Debug.Log(_value);
         nextCarImg.fillAmount = _value;

         if (nextCarImg.fillAmount >= 1)
         {
             nextCarImg.fillAmount = 0;
         }
     }*/

    public void UpdateRowText(int currRow)
    {
        rowText.text = "#" + currRow.ToString();
    }


    void RandomBackGround()
    {
        bgClone = Resources.LoadAll<Material>("SkyBox/");
        int listSize = bgClone.Length;
        randomNumber = Random.Range(1, listSize);
        Material _mat = bgClone[randomNumber];
        RenderSettings.skybox = _mat;
    }
    /*   void SendLevelStartData()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["level"] = PlayerPrefs.GetInt("level");

        AppMetrica.Instance.ReportEvent("level_started", data);

        AppMetrica.Instance.SendEventsBuffer();

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "level " + PlayerPrefs.GetInt("level").ToString());

        Debug.Log("reported event: level_started");

    }


    void SendLevelFinishData(bool win)
    {

        Dictionary<string, object> data = new Dictionary<string, object>();
        data["level"] = PlayerPrefs.GetInt("level");
        data["result"] = win ? "win" : "lose";

        AppMetrica.Instance.ReportEvent("level_finish", data);
        AppMetrica.Instance.SendEventsBuffer();


        if (win)
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level " + PlayerPrefs.GetInt("level").ToString());

        else
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "level " + PlayerPrefs.GetInt("level").ToString());

    }

    void SendLevelRestartData()
    {

        Dictionary<string, object> data = new Dictionary<string, object>();
        data["level"] = PlayerPrefs.GetInt("level");

        AppMetrica.Instance.ReportEvent("level_restart_button", data);
        AppMetrica.Instance.SendEventsBuffer();


    }

    private void OnApplicationQuit()
    {

        AppMetrica.Instance.SendEventsBuffer();

    }

    private void OnApplicationPause(bool pause)
    {
        AppMetrica.Instance.SendEventsBuffer();

    }*/
    public void ShowMinusText() {
         DropText.gameObject.SetActive(true);
        DropText.text = "-5";
        Invoke("DeactivateDropText", 1f);
    
}
private void DeactivateDropText()
    {
        // Assuming DropText.gameObject is the object you want to deactivate
        DropText.gameObject.SetActive(false);
        
    }
}