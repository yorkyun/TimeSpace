using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyPoint : MonoBehaviour {

    float target0Value = 0.025f;
    float target1Value = 0.045f;
    float target2Value = 0.1f;
    float target3Value = 0.165f;

    public Sprite target0;
    public Sprite target1;
    public Sprite target2;
    public Sprite target3;

    SpriteRenderer SR;

    int AccuracyLevel;

    private void Awake()
    {
        if(!TargetaccuracyPoint.AllPoints.ContainsValue(this))
        {
            TargetaccuracyPoint.AllPoints.Add(name, this);
        }
        SR = GetComponent<SpriteRenderer>();

        float ratio = transform.localPosition.z / 4;

        target0Value *= ratio;
        target1Value *= ratio;
        target2Value *= ratio;
        target3Value *= ratio;

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GazeHit(Vector3 hitPostion)
    {
        AccuracyLevel = GetAccuracyLevel(Vector3.Distance(transform.position, hitPostion));
        if(AccuracyLevel == 0)
        {
            SR.sprite = target0;
        }
        else if(AccuracyLevel==1||AccuracyLevel==2)
        {
            SR.sprite = target1;
        }
        else if(AccuracyLevel==3)
        {
            SR.sprite = target2;
        }
        else
        {
            SR.sprite = target3;
        }

    }

    public void GazeLeave()
    {
        SR.sprite = target3;
    }

    int GetAccuracyLevel(float distance)
    {
        if(distance<target0Value)
        {
            return 1;
        }
        else if(distance>target0Value&& distance <= target1Value)
        {
            return 1;
        }
        else if(distance> target1Value&& distance<= target2Value)
        {
            return 2;
        }
        else if(distance> target2Value&& distance < target3Value)
        {
            return 3;
        }
        else
        {
            return 4;
        }
    }

}
