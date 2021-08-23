#define USEMOUSE
#undef USEMOUSE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetaccuracyPoint : MonoBehaviour
{
    public const string accuracyPointTag = "AccuracyPoint";

    public static Dictionary<string, AccuracyPoint> AllPoints = new Dictionary<string, AccuracyPoint>();

    AccuracyPoint CurrentPoint;

    int Index = 0;
    public int gazePiontCount = 10;
    List<GameObject> gazePointList;

    public GameObject pointPfb;

    //切换点的帧数
    int ChangeFrame = 0;

    //击中当前点的帧数
    int HitFrame = 0;

    //没有击中任何点的帧数
    int LostFrame = 0;

    public static int MaxFrame = 6;

    Vector3 gazeOirgin;
    Vector3 gazeWorldPostion;

    // Use this for initialization
    void Start()
    {
        gazePointList = new List<GameObject>();
        for (int i = 0; i < gazePiontCount; i++)
        {
            GameObject point = GameObject.Instantiate(pointPfb);
            point.transform.parent = transform;
            point.transform.localPosition = Vector3.zero;
            point.transform.localEulerAngles = Vector3.zero;
            point.SetActive(true);
            //point.transform.localScale = Vector3.one;
            gazePointList.Add(point);
        }


    }

    // Update is called once per frame
    void Update()
    {

        if (!EyeTrackingManager.Instance().GetGazeRay(out gazeOirgin, out gazeWorldPostion))
        {
            Debug.Log("GetGazeRay: false");
            return;
        }

        if (Index == gazePiontCount - 1)
        {
            Index = 0;
        }
        else
        {
            Index++;
        }
        gazePointList[Index].transform.position = gazeWorldPostion;

        Ray gazeRay = new Ray(gazeOirgin, gazeWorldPostion - gazeOirgin);

        Debug.DrawRay(gazeRay.origin, gazeRay.direction * 100, Color.red);

        RaycastHit hitInfo;

        if (Physics.Raycast(gazeRay, out hitInfo, 100))
        {
            GameObject hitGO = hitInfo.collider.gameObject;
            if (hitGO.tag == accuracyPointTag)
            {
                if (AllPoints.ContainsKey(hitGO.name))
                {
                    if (CurrentPoint == null)
                    {
                        CurrentPoint = AllPoints[hitGO.name];
                    }
                    if (CurrentPoint != AllPoints[hitGO.name])
                    {
                        if (++ChangeFrame > MaxFrame)
                        {
                            HitFrame = 0;
                            CurrentPoint.GazeLeave();
                            CurrentPoint = AllPoints[hitGO.name];
                        }

                    }
                    else
                    {
                        if (++HitFrame > MaxFrame)
                        {
                            ChangeFrame = 0;
                            CurrentPoint.GazeHit(gazeWorldPostion);
                        }
                    }

                    return;
                }
            }

        }


        if (CurrentPoint != null)
        {
            if (++LostFrame > MaxFrame)
            {
                ChangeFrame = 0;
                HitFrame = 0;
                CurrentPoint.GazeLeave();
            }
        }

    }

    private void OnDestroy()
    {
        AllPoints.Clear();
    }
}
