// Copyright 2016 Nibiru. All rights reserved.
using UnityEngine;
using UnityEngine.UI;
using NibiruTask;
namespace NXR.Samples
{
    public class SystemApiScript : MonoBehaviour
    {

        Text pathText;
        Text InfoText;

        string path = "";
        string info = "";
        // Use this for initialization
        void Start()
        {

            GameObject pathObj = GameObject.Find("FilePath");
            if (pathObj != null)
            {
                pathText = pathObj.GetComponent<Text>();
            }

            GameObject infoObj = GameObject.Find("SystemInfo");
            if (infoObj != null)
            {
                InfoText = infoObj.GetComponent<Text>();
            }

            NibiruTaskApi.setSelectionCallback(onSelectionResult);
        }

        public void onSelectionResult(AndroidJavaObject task)
        {
            path = NibiruTaskApi.GetResultPathFromSelectionTask(task);

        }

        // Update is called once per frame
        void Update()
        {
            if (pathText != null)
            {
                pathText.text = "GethFilePath: " + path;
            }
            if (InfoText != null)
            {
                InfoText.text = "SystenInfo: " + info;
            }
        }

        public void PointerEnter()
        {
            Debug.Log("pointer enter");
        }

        public void PointerExit()
        {
            Debug.Log("pointer exit");
        }

        public void OpenVideoPlayer()
        {
            //NvrViewer.Instance.OpenVideoPlayer(NvrViewer.Instance.GetStoragePath() + "/nibiru.mp4", 0, 2, 1);
            NibiruTaskApi.OpenVideoPlayer("sdcard/nibiru.mp4");
        }

        public void GetSystemInfo()
        {
            info = "GetVRVersion:" + NibiruTaskApi.GetVRVersion() + "\n"
                + "GetOSVersion:" + NibiruTaskApi.GetOSVersion() + "\n"
                + "GetSysSleepTime:" + NibiruTaskApi.GetSysSleepTime() + "\n"
                + "GetCurrentLanguage:" + NibiruTaskApi.GetCurrentLanguage() + "\n"
                + "GetCurrentTimezone:" + NibiruTaskApi.GetCurrentTimezone() + "\n"
                + "GetDeviceName:" + NibiruTaskApi.GetDeviceName() + "\n";

        }

        public void OpenExplorer()
        {
            NibiruTaskApi.OpenBrowerExplorer("http://www.inibiru.com");
        }

        public void OpenSettings()
        {
            NibiruTaskApi.OpenSettingsMain();
        }

        public void OpenImage()
        {
            NibiruTaskApi.OpenImageGallery("sdcard/nibiru.png");
        }

        public void GetFilePath()
        {
            NibiruTaskApi.GetFilePath("sdcard");
        }

    }
}