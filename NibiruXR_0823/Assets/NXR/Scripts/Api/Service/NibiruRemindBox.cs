using UnityEngine;

namespace Nxr.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public class NibiruRemindBox : NibiruRemindBoxBase
    {
        public static NibiruRemindBox Instance { set; get; }

        // [SerializeField]
        private bool isAutoClose = true;
        private Vector2 VOLUMESIZE = new Vector2(60, 1.2f);

        private int VOLUMESTART = -20;

        // private int VOLUMEEND = 36;
        private string typeName = "";

        void Start()
        {
            Instance = this;
            // CloseBox();
            // Calibration();
            NxrViewer.Instance.GazeApi(GazeTag.Hide);
        }

        /// <summary>
        /// The dialog of low battery。
        /// </summary>
        public void Electricity()
        {
            typeName = "electricity";
            if (this.Init(3))
            {
                Create("BGIcon", new Vector3(0, 0, 0), new Vector2(150, 20));
                Create("Context", LocalizationManager.GetInstance.GetValue("remindbox_lower_power"),
                    new Vector3(0, 0, 0), new Vector2(500, 100));
                if (isAutoClose)
                    Close();
            }
        }

        public void CalibrationDelay()
        {
            Loom.QueueOnMainThread((param) => { Calibration(); }, true);
        }

        /// <summary>
        /// The dialog of calibration.
        /// </summary>
        public void Calibration()
        {
            typeName = "calibration";
            if (this.Init(3))
            {
                Create("BGIcon", new Vector3(0, 0, 0), new Vector2(150, 20));
                Create("Context", LocalizationManager.GetInstance.GetValue("remindbox_nkey_tip"), new Vector3(0, 0, 0),
                    new Vector2(600, 100));
                Create("HandleIcon", new Vector3(0, 48, 0), new Vector2(15, 65));
                if (isAutoClose)
                    Close();
            }
        }
        
//         public void CloseBox()
//         {
//             typeName = "close";
//             if (this.Init(4))
//             {
//                 Create("Close", LocalizationManager.GetInstance.GetValue("remindbox_shutdown"), new Vector3(0, 10, 0),
//                     new Vector2(500, 100));
//                 Create("ReStart", LocalizationManager.GetInstance.GetValue("remindbox_reboot"), new Vector3(0, -10, 0),
//                     new Vector2(500, 100));
//                 Create("CloseIcon", new Vector3(-40, 10, 0), new Vector2(10, 10));
//                 Create("ReStartIcon", new Vector3(-40, -10, 0), new Vector2(10, 10));
// #if NIBIRU_VR
//                 Create("dialog_dismiss_normal", new Vector3(50, 16, 0), new Vector2(20, 20));
//                 Create("CloseMeIcon", new Vector3(50, 16, 0), new Vector2(20, 20), OnLiftBack);
// #else
//                 Create("dialog_dismiss_normal", new Vector3(70, 16, 0), new Vector2(20, 20));
//                 Create("CloseMeIcon", new Vector3(70, 16, 0), new Vector2(20, 20), OnLiftBack);
// #endif
//                 Create("CloseBGIcon", new Vector3(0, 9, 0), new Vector2(100, 18), TurnOff);
//                 Create("ReStartBGIcon", new Vector3(0, -9, 0), new Vector2(100, 18), ReStart);
//                 if (isAutoClose)
//                     Close();
//             }
//         }

        /// <summary>
        /// Volume
        /// </summary>
        /// <param name="param"></param>
        public void Volume(string param)
        {
            if (this.typeName.Equals("volume") && remindbox != null)
            {
                Create(new Vector3(VOLUMESTART + (56 * int.Parse(param)) / 15, 0, 0), new Vector2(4.8f, 4.8f));
            }
            else if (this.Init(2))
            {
                this.Create("BGIcon", new Vector3(0, 0, 0), new Vector2(100, 20));
                this.Create("VolumeIcon", new Vector3(-35.8f, 0, 0), new Vector2(7.6f, 7.6f));
                this.Create("VolumeWrite", new Vector3(8, 0, 0), VOLUMESIZE);
                //TODO:更改图标
                this.Create("VolumeTag", new Vector3(VOLUMESTART + (56 * int.Parse(param)) / 15, 0, 0),
                    new Vector2(4.8f, 4.8f));
                if (isAutoClose) Close();
            }

            typeName = "volume";
        }

        public void TurnOff()
        {
            NxrViewer.Instance.TurnOff();
            ReleaseDestory();
        }

        public void ReStart()
        {
            NxrViewer.Instance.Reboot();
            ReleaseDestory();
        }

        public void OnLiftBack()
        {
            if (remindbox != null)
            {
                ReleaseDestory();
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                Volume("1");
                 // CloseBox();
            }
        }
#endif
    }

    public enum RemindBoxParam
    {
        ELECTRICITY,
        CALIBRATION,
        CLOSE,
        VOLUME
    }
}