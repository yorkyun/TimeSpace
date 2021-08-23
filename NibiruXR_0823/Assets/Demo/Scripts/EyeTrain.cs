using EyeTrackingCsUnityPlugins;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;
using LitJson;
using DG.Tweening;
public class EyeTracking
{
    public string upDown;       //上下运动次数
    public string leftRight;    //左右运动次数
    public string squareRight;  //顺时针正方形次数
    public string squareLeft;   //逆时针正方形次数
    public string circleRight;  //顺时针圆形次数
    public string circleLeft;   //逆时针圆形次数
    public string focus;
}
public class EyeTrain : MonoBehaviour
{
    //eyeTrack Init
    Vector2 leftPupilcenter = Vector2.zero;
    Vector2 rightPupilcenter = Vector2.zero;

    float PupilRadiusL;
    float PupilRadiusR;
    long Timestamp;
    //scene Init
    public Text TimestampText;
    public Text TCountText;//训练次数计数
    //public Text GazePointText;
    //public Text cubeText;
    public Text StepText;
    //public Text testtext;
    //public Text Ytext;
//    public GameObject gazePoint;
    public GameObject CtrlObj;
    public GameObject TargetObj;
 //   public GameObject Ftarget;
    public SpriteRenderer DirectionTip;
    public SpriteRenderer FN7;

    //music
    public AudioSource Musicplayer;
    public AudioSource HitSound;
    //   public GameObject gazePoint;
    //Local parameter
    private int Tcount = 0;
    private float CountDownTime = 40;
    private int Step = 1;
    private float GameTime = 8;
    private float timer = 0;

    private bool isDone = false;    //一次性执行锁
    private bool NextStep = true;
    private int minStep = 0;
    private float depth = 10f;
    private Vector3 NewScale = new Vector3(0.1f, 0.1f, 0.1f);
    
    //Focus
    private bool Far = true;
    private int destion = 10;
    private float speed = 5f;
    private bool CW_5 = true;
    private bool CW_6 = true;
    private bool canSound = false;
    private Sprite getSp;
    private float Dis = 0;
    // json data
    public Text  JsonRSP;
    private UnityWebRequest _request;
    private int hour;
    private int minute;
    private int second;
    private int year;
    private int month;
    private int day;
    private EyeTracking EyeTrainData = new EyeTracking();
    


    void Awake()
    {
        EyeTrackingManager.Instance().OnGetEyesData += GetEyeData;
    }
    private JsonData Embedjson(EyeTracking Trackingdata)
    {
        JsonData data = new JsonData();
        data["upDown"] = Trackingdata.upDown;
        data["leftRight"] = Trackingdata.leftRight;
        data["squareRight"] = Trackingdata.squareRight;
        data["squareLeft"] = Trackingdata.squareLeft;
        data["circleRight"] = Trackingdata.circleRight;
        data["circleLeft"] = Trackingdata.circleLeft;
        data["focus"] = Trackingdata.focus;
        JsonData Maindata = new JsonData();

        Maindata["id"] = "654321";
        Maindata["time"] = GetTime();
        Maindata["data"] = data;
        return Maindata;
    }
    // Start is called before the first frame update
    void Start()
    {
 //       Musicplayer.Play();
 //       GameTime = 8;
        FN7.enabled = false;
        EyeTrainData.upDown = "1";
        EyeTrainData.leftRight = "2";
        EyeTrainData.squareLeft = "3";
        EyeTrainData.squareRight = "4";
        EyeTrainData.circleLeft = "5";
        EyeTrainData.circleRight = "6";
        EyeTrainData.focus = "7";
 //       CtrlObj.GetComponent<Renderer>().enabled = false;
        //CtrlObj = new GameObject();
        //CtrlObj.transform.localPosition = new Vector3(0, 0, 0);
       // CtrlObj.GetComponent<Renderer>().enabled = false;
        //JsonData DataTest= Embedjson(EyeTrainData);
        //StartCoroutine(Upload(DataTest));
    }
    void GetEyeData(EYE_DATA eYE)
    {
        if (eYE.rightPupilInfo.GetValidity(PupilInfoValidity.PUPIL_INFO_CENTER))
            rightPupilcenter = eYE.rightPupilInfo.pupilCenter;
        if (eYE.leftPupilInfo.GetValidity(PupilInfoValidity.PUPIL_INFO_CENTER))
            leftPupilcenter = eYE.leftPupilInfo.pupilCenter;

        if (eYE.rightPupilInfo.GetValidity(PupilInfoValidity.PUPIL_INFO_DIAMETER))
            PupilRadiusR = eYE.rightPupilInfo.pupilDiameter / 2;
        if (eYE.leftPupilInfo.GetValidity(PupilInfoValidity.PUPIL_INFO_DIAMETER))
            PupilRadiusL = eYE.leftPupilInfo.pupilDiameter / 2;

        Timestamp = eYE.timestamp;
    }
    Vector3 ori, destin;
    // Update is called once per frame
    void Update()
    {
        EyeTrackingInit();
        //GazePointText.text = TCube.transform.position.ToString("F3");
        //cubeText.text = cubeObj.transform.position.ToString("F3");
        //testtext.text = Ftarget.transform.position.ToString("F1") + "\n" +
        //                Ftarget.transform.rotation.ToString("F1");
        //Ytext.text = TCube.transform.position.ToString()+"\n" +
        //              TCube.transform.rotation.ToString();
        if (NextStep)
        {
            StepCtl();
            NextStep = false;
        }
        Dis = Vector2.Distance(new Vector2(TargetObj.transform.localPosition.x, TargetObj.transform.localPosition.y),
                            new Vector2(CtrlObj.transform.localPosition.x, CtrlObj.transform.localPosition.y));
        if (Dis < 1.1f)
        {
            // Focus();
            switch (Step)
            {
                case 3://上下
                    switch (minStep)
                    {
                        case 0:
                            minStep = 31;                           
                            CubeLocation(31);
                            break;
                        case 31:
                            minStep = 32;                            
                            CubeLocation(32);
                            break;
                        case 32:
                            minStep = 31;                            
                            CubeLocation(31);
                            Tcount++;
                            break;
                    }
                    break;
                case 4://左右
                    switch (minStep)
                    {
                        case 0:
                            minStep = 41;
                            CubeLocation(41);
                            break;
                        case 41:
                            minStep = 42;
                            CubeLocation(42);
                            break;
                        case 42:
                            minStep = 41;
                            CubeLocation(41);
                            Tcount++;
                            break;
                    }
                    break;
                case 5://正方形
                    if(CW_5)
                    {
                        switch (minStep)
                        {
                            case 0:
                                minStep = 51;
                                CubeLocation(51);
                                break;
                            case 51:
                                minStep = 52;
                                CubeLocation(52);
                                break;
                            case 52:
                                minStep = 53;
                                CubeLocation(53);
                                break;
                            case 53:
                                minStep = 54;
                                CubeLocation(54);
                                break;
                            case 54:
                                minStep = 51;
                                CubeLocation(51);
                                Tcount++;
                                break;
                        }
                    }
                    else// 逆时针
                    {
                        switch (minStep)
                        {
                            case 0:
                                minStep = 54;
                                CubeLocation(54);                               
                                break;
                            case 51:
                                minStep = 54;
                                CubeLocation(54);
                                Tcount++;
                                break;
                            case 52:
                                minStep = 51;
                                CubeLocation(51);
                                break;
                            case 53:
                                minStep = 52;
                                CubeLocation(52);
                                break;
                            case 54:
                                minStep = 53;
                                CubeLocation(53);

                                break;
                        }
                    }

                    break;
                case 6:
                    if(CW_6)
                    {
                        switch (minStep)
                        {
                            case 0:
                                minStep = 61;
                                CubeLocation(61);

                                break;
                            case 61:
                                minStep = 62;
                                CubeLocation(62);
                                break;
                            case 62:
                                minStep = 63;
                                CubeLocation(63);
                                break;
                            case 63:
                                minStep = 64;
                                CubeLocation(64);
                                break;
                            case 64:
                                minStep = 65;
                                CubeLocation(65);
                                break;
                            case 65:
                                minStep = 66;
                                CubeLocation(66);
                                break;
                            case 66:
                                minStep = 67;
                                CubeLocation(67);
                                break;
                            case 67:
                                minStep = 68;
                                CubeLocation(68);
                                break;
                            case 68:
                                minStep = 61;
                                CubeLocation(61);
                                Tcount++;
                                break;
                        }
                    }
                    else
                    {
                        switch (minStep)
                        {
                            case 0:
                                
                                minStep = 68;
                                CubeLocation(68);
                                break;
                            case 61:
                                minStep = 68;
                                CubeLocation(68);
                                Tcount++;
                                break;
                            case 62:
                                minStep = 61;
                                CubeLocation(61);
                                break;
                            case 63:
                                minStep = 62;
                                CubeLocation(62);
                                break;
                            case 64:
                                minStep = 63;
                                CubeLocation(63);
                                break;
                            case 65:
                                minStep = 64;
                                CubeLocation(64);
                                break;
                            case 66:
                                minStep = 65;
                                CubeLocation(65);
                                break;
                            case 67:
                                minStep = 66;
                                CubeLocation(66);
                                break;
                            case 68:
                                minStep = 67;
                                CubeLocation(67);

                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
            TCountText.text = Tcount.ToString();
            if(canSound)
            {
                HitSound.Play();
                canSound = false;
            }            
        }
        if(Step==7)
        {
            if (Dis < 1)
            {
                Focus();
            }
        }
        
 //       JsonRSP.text = Dis.ToString("F1") + "  " + new Vector2(TCube.transform.position.x, TCube.transform.position.y).ToString() +
 //                       "\n" + new Vector2(cubeObj.transform.position.x, cubeObj.transform.position.y).ToString();
        if (Step<=8)
        {
            TimeDown();
        }       
    }

    private void OnDestroy()
    {
        EyeTrackingManager.Instance().OnGetEyesData -= GetEyeData;
    }
    private void TimeDown()
    {
        float Sec = GameTime % 60;
        timer += Time.deltaTime;
        if(!isDone)
        {
            if (timer >= 1f)
            {
                timer = 0;
                if (GameTime > 0)
                {
                    GameTime--;
                }
                else//计时时间到
                {
                    if (Step < 2)
                    {
                        GameTime = 8;
                    }
                    else
                    {
                        GameTime = CountDownTime;
                    }
                    switch(Step)
                    {
                        case 5:
                            if (CW_5)
                            {                                
                                CW_5 = false;
                            }
                            else
                            {                                
                                Step++;
                            }
                            break;
                        case 6:
                            if(CW_6)
                            {                                                               
                                CW_6 = false;
                            }
                            else
                            {                               
                                Step++;
                            }
                            break;
                        default:                            
                            Step++;
                            break;
                    }
                    minStep = 0;
                    NextStep = true;
                }
                TimestampText.text = GameTime.ToString();
            }
        }
        else
        {
            TimestampText.text = "0";
        }
        
    }
    private void StepCtl()
    {
        switch (Step)
        {
            case 1:
                TargetObj.SetActive(false);
                CtrlObj.SetActive(false);
                DirectionTip.enabled = false;
                StepText.text = "第一节 放松准备 " + "\n" + "眨眼睛";
                Musicplayer.Play();
                break; 
            case 2:
                StepText.text = "第二节 眯眼睛 睫状肌准备";               
                break;
            case 3:
                TargetObj.SetActive(true);
                CtrlObj.SetActive(true);
                TargetObj.transform.localPosition = new Vector3(0f, 1.5f, depth);
                DirectionTip.enabled = true;
                Tcount = 0;
                getSp = Resources.Load<Sprite>("Image/UD");
                DirectionTip.sprite = getSp;
                StepText.text = "第三节 上下看 唤醒睫状肌";
                break;
            case 4:
                TargetObj.transform.localPosition = new Vector3(-2f, 0f, depth);
                StepText.text = "第四节 左右看 唤醒睫状肌";
                EyeTrainData.upDown = Tcount.ToString();
                 getSp = Resources.Load<Sprite>("Image/RL");
                DirectionTip.sprite = getSp;
                Tcount = 0;
                break;
            case 5:
                TargetObj.transform.localPosition = new Vector3(-1.5f, 1.5f, depth);
                StepText.text = "第五节 画正方形 恢复睫状肌";
                if(CW_5)
                {
                    EyeTrainData.leftRight = Tcount.ToString();
                    getSp = Resources.Load<Sprite>("Image/SCW");
                    DirectionTip.sprite = getSp;
                }
                else
                {
                    EyeTrainData.squareRight = Tcount.ToString(); //方形顺时针数据
                    getSp = Resources.Load<Sprite>("Image/SCCW");
                    DirectionTip.sprite = getSp;
                }                
                Tcount = 0;
                break;
            case 6:
                TargetObj.transform.localPosition = new Vector3(0f, 1.5f, depth);
                StepText.text = "第六节 画圈 睫状肌加油";
                if(CW_6)
                {
                    EyeTrainData.squareLeft = Tcount.ToString();
                    getSp = Resources.Load<Sprite>("Image/RCW");
                    DirectionTip.sprite = getSp;
                }
                else
                {
                    EyeTrainData.circleRight = Tcount.ToString();
                    getSp = Resources.Load<Sprite>("Image/RCCW");
                    DirectionTip.sprite = getSp;
                }               
                Tcount = 0;
                break;
            case 7:
                DirectionTip.enabled = false;
                FN7.enabled = true;
                EyeTrainData.circleLeft = Tcount.ToString();
                StepText.text = "第七节 远近焦点恢复" + "\n" + "眼睛盯着小球";
                TargetObj.transform.localPosition = new Vector3(0, 0, depth + 5);
                TargetObj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                CtrlObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                Tcount = 0;        
                break;
            case 8:
                TargetObj.SetActive(false);
                CtrlObj.SetActive(false);
                FN7.enabled = false;
                EyeTrainData.focus = Tcount.ToString();
                Tcount = 0;               
                JsonData DataUpload = Embedjson(EyeTrainData);
                StartCoroutine(Upload(DataUpload));
                isDone = true;
                Musicplayer.Stop();
                DirectionTip.enabled = false;
                StepText.text = "完成运动，双手戳热" + "\n" + "放在眼睛上";               
                break;
        }
    }

    private void EyeTrackingInit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        if(Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
        if (!EyeTrackingManager.Instance().isStartTracking)
        { return; }
        if (EyeTrackingManager.Instance().GetGazeRay(out ori, out destin))
        {
           // gazePoint.transform.position = destin;
            CtrlObj.transform.position = destin;           
        }
 //       depth = destin.z;
    }

    private void CubeLocation(int Loc)
    {
        switch (Loc)
        {
            case 31://"Up"
                TargetObj.transform.localPosition = new Vector3(0f, 1.8f, depth);
                break;
            case 32://"Down"
                TargetObj.transform.localPosition = new Vector3(0f, -1.8f, depth);
                break;
            case 41://"Left"
                TargetObj.transform.localPosition = new Vector3(-2f, 0f, depth);
                break;
            case 42://"Right"
                TargetObj.transform.localPosition = new Vector3(2f, 0f, depth);
                break;
            case 51://"S1"
                TargetObj.transform.localPosition = new Vector3(-1.5f, 1.5f, depth);
                break;
            case 52://"S2"
                TargetObj.transform.localPosition = new Vector3(1.5f, 1.5f, depth);
                break;
            case 53://"S3"
                TargetObj.transform.localPosition = new Vector3(1.5f, -1.5f, depth);
                break;
            case 54://"S4"
                TargetObj.transform.localPosition = new Vector3(-1.5f, -1.5f, depth);
                break;
            case 61://"C1"
                TargetObj.transform.localPosition = new Vector3(0f, 1.5f, depth);
                break;
            case 62://"C2"
                TargetObj.transform.localPosition = new Vector3(1f, 1f, depth);
                break;
            case 63://"C3"
                TargetObj.transform.localPosition = new Vector3(1.5f, 0f, depth);
                break;
            case 64://"C4"
                TargetObj.transform.localPosition = new Vector3(1f, -1f, depth);
                break;
            case 65://"C5"
                TargetObj.transform.localPosition = new Vector3(0f, -1.5f, depth);
                break;
            case 66://"C6"
                TargetObj.transform.localPosition = new Vector3(-1f, -1f, depth);
                break;
            case 67://"C7"
                TargetObj.transform.localPosition = new Vector3(-1.5f, 0f, depth);
                break;
            case 68://"C8"
                TargetObj.transform.localPosition = new Vector3(-1f, 1f, depth);
                break;

        }
        canSound = true;
    }

    private void Focus()
    {
 //       TCube.transform.localPosition = new Vector3(0, 0, depth+5);  //后移
        if ((TargetObj.transform.localPosition.z -(depth+5)) <=-destion&&!Far)
        {
            Far = true;
            Tcount++;
        }
        else if((TargetObj.transform.localPosition.z - (depth+5)) >= destion && Far)
        {
            Far = false;             
        }
        TargetObj.transform.localPosition += (Far ? Vector3.forward : Vector3.back)
                                    * speed * Time.deltaTime;
    }
    public string GetTime()
    {
        hour = DateTime.Now.Hour;
        minute = DateTime.Now.Minute;
        second = DateTime.Now.Second;
        year = DateTime.Now.Year;
        month = DateTime.Now.Month;
        day = DateTime.Now.Day;
        return string.Format("{0:D4}-{1:D2}-{2:D2} " + "{3:D2}:{4:D2}:{5:D2}", year, month, day, hour, minute, second);
    }
    public IEnumerator Upload(JsonData jsondata)
    {

        var url = "http://14.18.90.78:3006/eye/postEyeChacingFromEyeProtector";
        //    byte[] postBytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson( jsondata)); 
        byte[] postBytes = Encoding.UTF8.GetBytes(jsondata.ToJson());
        //       byte[] databyte = Encoding.UTF8.GetBytes(jsondata);
        _request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        _request.uploadHandler = new UploadHandlerRaw(postBytes);
        _request.downloadHandler = new DownloadHandlerBuffer();
        _request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        yield return _request.SendWebRequest();

        Debug.Log(_request.responseCode);
        JsonRSP.text = "训练完成";
 //       JsonRSP.text = _request.responseCode.ToString();

        if (_request.isHttpError || _request.isNetworkError)
        {
            Debug.LogError(_request.error);
            JsonRSP.text = _request.error;
            JsonRSP.text = "训练完成" + "\n" +"数据上传失败，请检查网络";
        }
        else
        {
            Debug.Log(_request.downloadHandler.text);
            JsonRSP.text = "训练完成"+"" + '\n' + "数据上传成功";

        }
    }
}
