using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class GetTcp : MonoBehaviour
{
    public TcpClent Tcp;
    public Text  ShowText;
    // Start is called before the first frame update
    void Start()
    {
       bool isStart= Tcp.StartClient();
        Debug.Log(isStart);
    }

    // Update is called once per frame
    void Update()
    {
        ShowText.text = Tcp.PupilLoc;
    }
}
