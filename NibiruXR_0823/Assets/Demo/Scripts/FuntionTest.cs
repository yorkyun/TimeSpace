using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class FuntionTest : MonoBehaviour
{
    public GameObject Target;
    // Start is called before the first frame update
    
    void Start()
    {
        Target.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.5f);
  //      Target.transform.DORotate(new Vector3(0, 0, 180), 0.3f).Loops();
        //StartCoroutine(DelayToInvokeDo(() => 
        //{
        //    Target.transform.localPosition = new Vector3(0, 5, 10);
        //}, 3f));
 //       Target.transform.DOLocalMove(new Vector3(0, 5, 10), 1).SetDelay(1);
 //       Target.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetDelay(2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static IEnumerator DelayToInvokeDo(Action action, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        action();
    }
}
