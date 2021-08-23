using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize : MonoBehaviour
{
    private bool IsDeviceAuth = false;
    private string mAccessKey = "494df681a3e94df6b83cd936c559c76f";

    // Use this for initialization
    void Awake()
    {
        Consts.IsDeviceAuth = IsDeviceAuth;
        Consts.MaxCalibrationRange = 10;
        Consts.MinCalibrationRange = -10;

        //获取硬件指纹
        EyeTrackingManager.Instance().SetupAuthorization(mAccessKey);
        
    }

}