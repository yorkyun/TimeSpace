using UnityEngine;
using UnityEngine.UI;

public class ErrorReport : MonoBehaviour
{
    public GameObject ErrorTip;
    Text ErrorText;

    private void Awake()
    {
        ErrorText = ErrorTip.transform.GetChild(0).GetComponent<Text>();
        EyeTrackingManager.Instance().OnErrorReport += ErrorRetrun;
    }

    // Use this for initialization
    void Start()
    {
        if(EyeTrackingManager.Instance().ErrorCode !=0)
        {
            ErrorRetrun(EyeTrackingManager.Instance().ErrorCode);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        ErrorTip.SetActive(false);
    }

    void ErrorRetrun(object code)
    {
        if (ErrorTip != null && ErrorText != null)
        {
            if(code is int && (int)code == 0)
            {
                ErrorTip.SetActive(false);
            }
            else
            {
                if (!ErrorTip.activeSelf)
                {
                    ErrorTip.SetActive(true);
                }

                ErrorText.text = code.ToString();
            }
           
        }
    }

    private void OnDestroy()
    {
        EyeTrackingManager.Instance().OnErrorReport -= ErrorRetrun;
    }
}
