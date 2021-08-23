using EyeTrackingCsUnityPlugins;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;
using LitJson;
using NibiruTask;

//public class EyeTracking
//{
//    public string upDown;       //上下运动次数
//    public string leftRight;    //左右运动次数
//    public string squareRight;  //顺时针正方形次数
//    public string squareLeft;   //逆时针正方形次数
//    public string circleRight;  //顺时针圆形次数
//    public string circleLeft;   //逆时针圆形次数
//    public string focus;
//}
public class EyeTrain2 : MonoBehaviour
{
    //eyeTrack Init
    Vector2 leftPupilcenter = Vector2.zero;
    Vector2 rightPupilcenter = Vector2.zero;

    float PupilRadiusL;
    float PupilRadiusR;
    long Timestamp;
    //scene Init
    public Text TimestampText;
 //   public Text TCountText;//训练次数计数
    //public Text GazePointText;
    //public Text cubeText;
    public Text StepText;
    public Text DeviceID;
    public Text PupilLoc_text;
    public Text  JsonRSP;
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
    public AudioSource dida;

    //TCP

    public TcpClent TCPFUNC;
    private string TempText;
    //场景模型
    public GameObject Model1,Model2,Model3,Model4;
    //   public GameObject gazePoint;
    //Local parameter

    //重启和退出
    private float pressDurationTime = 2;
    private bool ispress = false;
    private float downtime =0;

    private int Tcount = 0;
    private float CountDownTime = 15;
    private float RestTime = 4;
    public float LastStepTime = 15;
    private int Step = 0;
    public float GameTime = 15;
    private float timer = 0;

    private bool isDone = false;    //一次性执行锁
    private bool NextStep = true;
    private int minStep = 01;
    private const float depth = 10f;
    private Vector3 NewScale = new Vector3(0.1f, 0.1f, 0.1f);
    
    //Focus
    private bool Far = true;
    private int destion = 10;
    private float speed = 8f;
    private float Mspeed = 6f;
    private bool CW_5 = true;
    private bool CW_6 = true;
    public float InitLocal = 2.1f;
    private float Rate  = 0.7f;
//    private bool canSound = false;
    private Sprite getSp;
    private float Dis = 10;
    private int Dial = 0; //九宫格
    private int InputDial=0;
    private bool canCount = false;
    private bool HadRest = false;
    private bool InRest = false;
    private bool CanStart =false;   //确认开始
    private bool HadCalibration =false;
    // json data

    private UnityWebRequest _request;
    private int hour;
    private int minute;
    private int second;
    private int year;
    private int month;
    private int day;
    private EyeTracking EyeTrainData = new EyeTracking();

    //九宫格坐标
    struct PupilDial 
    {
        public Vector3 L1,L2,L3,L4,L5,L6,L7,L8,L9;
    }
    PupilDial  TargetDial;


    void Awake()
    {
        UnityEngine.XR.XRSettings.enabled = false;  //禁用默认开机动画
    //    EyeTrackingManager.Instance().OnGetEyesData += GetEyeData;
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

        Maindata["id"] = NibiruTaskApi.GetDeviceId().Substring(NibiruTaskApi.GetDeviceId().Length-7,6);
        Maindata["time"] = GetTime();
        Maindata["data"] = data;
        return Maindata;
    }
    // Start is called before the first frame update
    void Start()
    {
        Hidebackground();
        Init();
        //取id最后六位
        DeviceID.text="ID: "+ NibiruTaskApi.GetDeviceId().Substring(NibiruTaskApi.GetDeviceId().Length-7,6);
        
    }
    void Init()
    {
        FN7.enabled = false;
        EyeTrainData.upDown = "1";
        EyeTrainData.leftRight = "2"; 
        EyeTrainData.squareLeft = "3";
        EyeTrainData.squareRight = "4";
        EyeTrainData.circleLeft = "5";
        EyeTrainData.circleRight = "6";
        EyeTrainData.focus = "7";
        CtrlObj.GetComponent<Renderer>().enabled = false;
        dida.volume = 0.2f;
        HitSound.volume = 0.5f;
        Musicplayer.volume = 0.5f;
        bool  isStart =TCPFUNC.StartClient();
        
        if(isStart)
        {
            PupilLoc_text.text="连接成功";
            TempText="连接成功";
        } 
        else
        {
            PupilLoc_text.text="连接失败";
            TempText="连接失败";
        }            
        
        
        //九宫格坐标
        TargetDial.L1 = new Vector3(-InitLocal*Rate, InitLocal*Rate, depth);
        TargetDial.L2 = new Vector3(0f, InitLocal, depth);
        TargetDial.L3 = new Vector3(InitLocal*Rate, InitLocal*Rate, depth);
        TargetDial.L4 = new Vector3(-InitLocal, 0f, depth);
        TargetDial.L5 = new Vector3(0f,0f,depth);
        TargetDial.L6 = new Vector3(InitLocal, 0f, depth);
        TargetDial.L7 = new Vector3(-InitLocal*Rate, -InitLocal*Rate, depth);
        TargetDial.L8 = new Vector3(0f, -InitLocal, depth);
        TargetDial.L9 = new Vector3(InitLocal*Rate, -InitLocal*Rate, depth);

        TargetObj.transform.localPosition = TargetDial.L5;
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
 //   Vector3 ori, destin;
    // Update is called once per frame
    void LocSwitch()
    {
        if(TargetObj.transform.localPosition == TargetDial.L1)
        {
            Dial=1;
        }
        if(TargetObj.transform.localPosition == TargetDial.L2)
        {
            Dial=2;
        }
        if(TargetObj.transform.localPosition == TargetDial.L3)
        {
            Dial=3;
        }
        if(TargetObj.transform.localPosition == TargetDial.L4)
        {
            Dial=4;
        }
        if(TargetObj.transform.localPosition == TargetDial.L5)
        {
            Dial=5;
        }
        if(TargetObj.transform.localPosition == TargetDial.L6)
        {
            Dial=6;
        }
        if(TargetObj.transform.localPosition == TargetDial.L7)
        {
            Dial=7;
        }
        if(TargetObj.transform.localPosition == TargetDial.L8)
        {
            Dial=8;
        }
        if(TargetObj.transform.localPosition == TargetDial.L9)
        {
            Dial=9;
        }
    }
    void Update()
    {
        EyeTrackingInit();
        // Dis = Vector2.Distance(new Vector2(TargetObj.transform.localPosition.x, TargetObj.transform.localPosition.y),
        //                    new Vector2(CtrlObj.transform.localPosition.x, CtrlObj.transform.localPosition.y));

        //PupilLoc_text.text=TempText+":" +TCPFUNC.PupilLoc;
        PupilLoc_text.text=TempText+":" +TCPFUNC.PupilLoc;
        InputDial= System.Convert.ToInt32(TCPFUNC.PupilLoc);

        if (NextStep&&!HadRest)
        {
            StepCtl();
            NextStep = false;
        }
        LocSwitch();
        if(Dial!=0&&InputDial==Dial)//
        {
            if (canCount)
            {
                canCount = false;
                HitSound.Play();               
                Tcount++;
 //               TCountText.text = Tcount.ToString();
            }
        }

        if(Step==7)
        {          
            
            Focus();            
        }        
        if (Step<=8&&CanStart)//&&CanStart
        {
            TimeDown();
        }  

        /* if(Tcount==0)
        {
            Debug.Log("step:"+Step+ "ministep"+minStep);
        } */
    }
    //每一秒做的事情
    public void TargetLocation()
    {

        switch (Step)
        {
            case 0:
                if(GameTime<10)
                {
                    switch(minStep)
                    {
                        case 01:
                            TCPFUNC.SendStartCheck();
                            minStep = 02;
                            CubeLocation(01);
                            break;
                        case 02:
                            minStep = 03;
                            CubeLocation(02);
                            break;
                        case 03:
                            minStep = 04;
                            CubeLocation(03);                           
                            break;
                        case 04:
                            minStep = 05;
                            CubeLocation(04);                                                   
                            break;
                        case 05:
                            minStep = 06;
                            CubeLocation(01);
                            break;
                        case 06:
                            minStep = 07;
                            CubeLocation(02);
                            break;
                        case 07:
                            minStep = 08;
                            CubeLocation(03);
                            break;
                        case 08:
                            minStep = 31;
                            CubeLocation(04); 
                              
                            break;                          
                    }
                }
                if(GameTime<=1)
                {
                    StepText.text="校准完成";
                    TCPFUNC.SendStopCheck();  
                    Showbackground();   
                }
                
                break;
            case 3://上下
                switch (minStep)
                {
                    case 31:
                        minStep = 32;
                        CubeLocation(32);
                        break;
                    case 32:
                        minStep = 31;
                        CubeLocation(31);                       
                        break;
                }
                break;
            case 4://左右
                switch (minStep)
                {
                    case 41:
                        minStep = 42;
                        CubeLocation(42);
                        break;
                    case 42:
                        minStep = 41;
                        CubeLocation(41);
                       
                        break;
                }
                break;
            case 5://正方形
                if (CW_5)
                {
                    switch (minStep)
                    {
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
                            break;
                    }
                }
                else// 逆时针
                {
                    switch (minStep)
                    {
                        case 51:
                            minStep = 54;
                            CubeLocation(54);                           
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
                if (CW_6)
                {
                    switch (minStep)
                    {
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
                            
                            break;
                    }
                }
                else
                {
                    switch (minStep)
                    {
                        case 61:
                            minStep = 68;
                            CubeLocation(68);                           
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
        
    }
    private void OnDestroy()
    {
  //      EyeTrackingManager.Instance().OnGetEyesData -= GetEyeData;
    }
    /// <summary>
    /// 倒计时控制
    /// </summary>
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
                    if(!InRest)//非休息时间
                    {
                        TargetLocation();
                    }
                }
                else//计时时间到
                {
                    if (InRest)
                        InRest = false;
                    switch (Step)
                    {
                        case 0:
                            GameTime =8;
                            Step++;
                            break;
                        case 1:
                            GameTime = 8;
                            Step++;
                            break;
                        case 2:
                            GameTime = CountDownTime;
                            Step++;
                            break;
                        case 3:
                            if(!HadRest)
                            {
                                Fun_Rest();
                            }
                            else
                            {
                                Step++;
                                HadRest = false;
                                TargetObj.SetActive(true);
                                CtrlObj.SetActive(true);
                                DirectionTip.enabled = true;
                                GameTime = CountDownTime;
                                JsonRSP.text = "用眼睛注视" + "\n" + "击中靶心";
                            }
                            break;
                        case 4:
                            if (!HadRest)
                            {
                                Fun_Rest();
                            }
                            else
                            {
                                Step++;
                                HadRest = false;
                                TargetObj.SetActive(true);
                                CtrlObj.SetActive(true);
                                DirectionTip.enabled = true;
                                GameTime = CountDownTime;
                                JsonRSP.text = "用眼睛注视" + "\n" + "击中靶心";
                            }
                            break;
                        case 5:
                            
                            if (CW_5)
                            {
                                if (!HadRest)
                                {
                                    Fun_Rest();
                                }
                                else
                                {
                                    CW_5 = false;
                                    HadRest = false;
                                    TargetObj.SetActive(true);
                                    CtrlObj.SetActive(true);
                                    DirectionTip.enabled = true;
                                    GameTime = CountDownTime;
                                    JsonRSP.text = "用眼睛注视" + "\n" + "击中靶心";
                                }
                                
                            }
                            else
                            {
                                if(!HadRest)
                                {
                                    Fun_Rest();
                                }
                                else
                                {
                                    Step++;
                                    HadRest = false;
                                    TargetObj.SetActive(true);
                                    CtrlObj.SetActive(true);
                                    DirectionTip.enabled = true;
                                    GameTime = CountDownTime;
                                    JsonRSP.text = "用眼睛注视" + "\n" + "击中靶心";
                                }                               
                            }
                            break;
                        case 6:
                            if (CW_6)
                            {
                                if (!HadRest)
                                {
                                    Fun_Rest();
                                }
                                else
                                {
                                    CW_6 = false;
                                    HadRest = false;
                                    TargetObj.SetActive(true);
                                    CtrlObj.SetActive(true);
                                    DirectionTip.enabled = true;
                                    GameTime = CountDownTime;
                                    JsonRSP.text = "用眼睛注视" + "\n" + "击中靶心";
                                }                               
                            }
                            else
                            {
                                if (!HadRest)
                                {
                                    Fun_Rest();
                                }
                                else
                                {
                                    
                                    Step++;
                                    HadRest = false;
                                    TargetObj.SetActive(true);
                                    CtrlObj.SetActive(true);
                                    DirectionTip.enabled = true;
                                    GameTime = LastStepTime;   //下一次步骤时间
                                    JsonRSP.text = "用眼睛注视" + "\n" + "击中靶心";
                                }                                
                            }
                            break;                               
                        default:
                            Step++;
                            break;
                    }
                    NextStep = true;                                        
                }
                TimestampText.text = GameTime.ToString();
  //              TCountText.text = Tcount.ToString();
            }
        }
        else
        {
            TimestampText.text = "0";
 //           TCountText.text = "0";
        }        
    }
    private void Fun_Rest()
    {
        HadRest = true;
        InRest = true;
        TargetObj.SetActive(false);
        CtrlObj.SetActive(false);
        DirectionTip.enabled = false;
        GameTime = RestTime;
        switch (Step)
        {
            case 3:
                JsonRSP.text = "内外直肌" + "\n" + "调节准备";//  RL
                break;
            case 4:
                JsonRSP.text = "眼外肌"+"\n" + "正调节准备";
                break;
            case 5:
                if(CW_5)
                JsonRSP.text = "眼外肌" + "\n" + "逆调节准备";
                else
                JsonRSP.text = "眼外肌正转" + "\n" + "调节准备";
                break;
            case 6:
                if (CW_6)
                    JsonRSP.text = "眼外肌逆转" + "\n" + "调节准备";
                else
                    JsonRSP.text = "睫状肌收缩" + "\n" + "调节准备";
                break;
        }        
    }
    private void StepCtl()
    {
        switch (Step)
        {
            //校准
            case 0: 
                TargetObj.SetActive(true);
                CtrlObj.SetActive(false);
                DirectionTip.enabled = false;
                Musicplayer.Play();
                Musicplayer.loop = true;
                StepText.text = "按下确认5秒后"+"\n"+"开始眼球校准";              
                Debug.Log("Step:"+Step);
                break;
            case 1:
                TargetObj.SetActive(false);
                CtrlObj.SetActive(false);
                DirectionTip.enabled = false;
                JsonRSP.enabled=false;
                StepText.text = "第一节 放松准备 " + "\n" + "眨眼睛";                
                break; 
            case 2:
                StepText.text = "第二节 眯眼睛 睫状肌准备";               
                break;
            case 3:
                if(!InRest)
                {
                    TargetObj.SetActive(true);
                    CtrlObj.SetActive(true);
                    TargetObj.transform.localPosition = TargetDial.L2;
                    dida.Play();
                    DirectionTip.enabled = true;
                   // Tcount = 0;
                    getSp = Resources.Load<Sprite>("Image/UD");
                    DirectionTip.sprite = getSp;
                    JsonRSP.enabled=true;
                    StepText.text = "第三节 上下看 唤醒睫状肌";
                }               
                break;
            case 4:
                TargetObj.transform.localPosition = TargetDial.L4;
                minStep = 41;
                dida.Play();
                StepText.text = "第四节 左右看 唤醒睫状肌";
                EyeTrainData.upDown = Tcount.ToString();
                 getSp = Resources.Load<Sprite>("Image/RL");
                DirectionTip.sprite = getSp;
                //Tcount = 0;
                break;
            case 5:
                TargetObj.transform.localPosition = TargetDial.L1;
                minStep = 51;
                dida.Play();
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
                //Tcount = 0;
                break;
            case 6:
                TargetObj.transform.localPosition = TargetDial.L2;
                minStep = 61;
                dida.Play();
                StepText.text = "第六节 画圈 睫状肌加油";
                if(CW_6)
                {
                    EyeTrainData.squareLeft = Tcount.ToString();
                    getSp = Resources.Load<Sprite>("Image/RCW");
                    DirectionTip.sprite = getSp;
                }
                else
                {   
                    Debug.Log("Right:"+EyeTrainData.circleRight+"tcount:"+Tcount.ToString());
                    EyeTrainData.circleRight = Tcount.ToString();
                    getSp = Resources.Load<Sprite>("Image/RCCW");
                    DirectionTip.sprite = getSp;                    
                }               
 //               Tcount = 0;
                break;
            case 7:
                //Tcount=99;
                Debug.Log("Tcount:"+Tcount.ToString()+" left:" +EyeTrainData.circleLeft);
                EyeTrainData.circleLeft =Tcount.ToString();    
                Debug.Log("Tcount:"+Tcount.ToString()+" left:" +EyeTrainData.circleLeft);
                DirectionTip.enabled = false;
                FN7.enabled = true;               
                StepText.text = "第七节 远近焦点恢复" + "\n" + "眼睛盯着小球";
                TargetObj.transform.localPosition = new Vector3(0, 0, depth + 10);
                TargetObj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                CtrlObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                //Tcount = 0;        
                break;
            case 8:
                TargetObj.SetActive(false);
                CtrlObj.SetActive(false);
                FN7.enabled = false;
                EyeTrainData.focus = Tcount.ToString();
                //Tcount = 0;               
                JsonData DataUpload = Embedjson(EyeTrainData);
                StartCoroutine(Upload(DataUpload));
                isDone = true;
                Musicplayer.Stop();
                DirectionTip.enabled = false;
                StepText.text = "完成运动，双手戳热" + "\n" + "放在眼睛上";               
                break;

        }
        Tcount=0;
    }

    private void EyeTrackingInit()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ispress=true;    
            if(!CanStart)
            {
                CanStart = true;
                Debug.Log("确认开始校准");
            }
            
           // JsonRSP.enabled = !JsonRSP.enabled;
        }
        if(Input.GetKeyUp(KeyCode.Return))
        {
            ispress = false;
        }
        if(ispress)
        {
            downtime += Time.deltaTime;
            if(downtime>=pressDurationTime)  //长按重启
            {
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            }         
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();  
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!CanStart)
            {
                CanStart = true;
                Debug.Log("确认开始校准");
            }
            Debug.Log("按下确认键");
 //            JsonRSP.enabled = !JsonRSP.enabled;
        }

    }
     
    private void CubeLocation(int Loc)
    {
        switch (Loc)
        {
            //校准正方形
            case 01://"S1"
                TargetObj.transform.localPosition = TargetDial.L2;
                break;
            case 02://"S2"
                TargetObj.transform.localPosition = TargetDial.L6;
                break;
            case 03://"S3"
                TargetObj.transform.localPosition = TargetDial.L8;
                break;
            case 04://"S4"
                TargetObj.transform.localPosition = TargetDial.L4;
                
                break;

            case 31://"Up"
                TargetObj.transform.localPosition = TargetDial.L2;
                break;
            case 32://"Down"
                TargetObj.transform.localPosition = TargetDial.L8;
                break;
            case 41://"Left"
                TargetObj.transform.localPosition = TargetDial.L4;
                break;
            case 42://"Right"
                TargetObj.transform.localPosition = TargetDial.L6;
                break;

            case 51://"S1"
                TargetObj.transform.localPosition = TargetDial.L1;
                break;
            case 52://"S2"
                TargetObj.transform.localPosition = TargetDial.L3;
                break;
            case 53://"S3"
                TargetObj.transform.localPosition = TargetDial.L9;
                break;
            case 54://"S4"
                TargetObj.transform.localPosition = TargetDial.L7;
                break;

            case 61://"C1"
                TargetObj.transform.localPosition =TargetDial.L2;
                break;
            case 62://"C2"
                TargetObj.transform.localPosition = TargetDial.L3;
                break;
            case 63://"C3"
                TargetObj.transform.localPosition = TargetDial.L6;
                break;
            case 64://"C4"
                TargetObj.transform.localPosition = TargetDial.L9;
                break;
            case 65://"C5"
                TargetObj.transform.localPosition = TargetDial.L8;
                break;
            case 66://"C6"
                TargetObj.transform.localPosition = TargetDial.L7;
                break;
            case 67://"C7"
                TargetObj.transform.localPosition = TargetDial.L4;
                break;
            case 68://"C8"
                TargetObj.transform.localPosition = TargetDial.L1;
                break;
        }
        canCount = true;
        dida.Play();
    }
    private void Focus()
    {
 //       TCube.transform.localPosition = new Vector3(0, 0, depth+5);  //后移
        if ((TargetObj.transform.localPosition.z -(depth+10)) <=-destion&&!Far)
        {
            Far = true;
            if(InputDial==5)
            {
                Tcount++;
            }
//            TCountText.text = Tcount.ToString();
        }
        else if((TargetObj.transform.localPosition.z - (depth+10)) >= destion && Far)
        {
            Far = false;             
        }
        TargetObj.transform.localPosition += (Far ? Vector3.forward : Vector3.back)
                                    * speed * Time.deltaTime;
        TargetObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f) -
                new Vector3(0.03f, 0.03f, 0.03f) * (TargetObj.transform.localPosition.z - (depth + 10));
    }
    private void Movie()
    {
        TargetObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f) * Mspeed * Time.deltaTime;
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

    private void Showbackground()
    {
        Model1.SetActive(true);
        Model2.SetActive(true);
        Model3.SetActive(true);
        Model4.SetActive(true);
    }
    private void Hidebackground()
    {
        Model1.SetActive(false);
        Model2.SetActive(false);
        Model3.SetActive(false);
        Model4.SetActive(false);
    }
}
