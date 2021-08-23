using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableXRSetting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Awake()
    {
        UnityEngine.XR.XRSettings.enabled = false;
    }
}
