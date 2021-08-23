using UnityEngine;

/// <summary>
/// 根据设备配置相机
/// </summary>
public class DeployCameraRig : MonoBehaviour
{
    /// <summary>
    /// Pico相机预制体
    /// </summary>
    public GameObject PicoVRCameraPfb;

    void Awake()
    {
        if (PicoVRCameraPfb != null)
        {
            GameObject CameraRig = Instantiate(PicoVRCameraPfb);
            CameraRig.transform.position = transform.position;
            CameraRig.transform.rotation = transform.rotation;
            CameraRig.transform.localScale = transform.localScale;

            Transform Head = CameraRig.transform.Find("Head");

            if (Head == null)
            {
                Debug.LogError("Pico's Head not found");
                return;
            }

            Transform[] allChilds = new Transform[transform.childCount];
            for (int i = 0; i < allChilds.Length; i++)
            {
                allChilds[i] = transform.GetChild(i);
            }

            foreach (Transform v in allChilds)
            {
                v.parent = Head;
            }

            Camera[] renderCameras = Head.GetComponentsInChildren<Camera>();

            Camera LeftCamera = null;
            Camera RightCamera = null;

            Camera OrgCamera = GetComponent<Camera>();

            if (OrgCamera == null)
            {
                Debug.LogError("OrgCamera not found");
                return;
            }

            foreach (Camera cam in renderCameras)
            {
                cam.backgroundColor = OrgCamera.backgroundColor;
                if (cam.name.Contains("Left") || cam.name.Contains("left"))
                {
                    LeftCamera = cam;
                }
                if (cam.name.Contains("Right") || cam.name.Contains("right"))
                {
                    RightCamera = cam;
                }
            }

            //设置EyeCamera
            if (LeftCamera != null && RightCamera != null)
            {
                EyeTrackingManager.Instance().SetEyeCamera(LeftCamera,RightCamera);
            }
            else
            {
                Debug.LogError("LeftCamera or RightCamera not found");
            }

            DestroyImmediate(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

}
