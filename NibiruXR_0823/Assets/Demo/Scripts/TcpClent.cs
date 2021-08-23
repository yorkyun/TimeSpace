using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TcpClent : MonoBehaviour
{
/// <summary>
/// 常用参数
/// </summary>

    private string staInfo = "NULL";             //状态信息
    private int _port = 3000;             //获取端口号
    string _ip = "127.0.0.1";
    public string Revstr="";
    private Socket socketSend;                   //客户端套接字，用来链接远端服务器
 //   public InputField input;
 //   public Text TCPinfo;
    // Start is called before the first frame update
    private Thread c_thread;
    private string Payload= "STARTCHECK";
    private string Packagelen;
    public string ClientMsg = "UPO"+"0123456789AB"+"0123456789AB"+"0"+"01"+"0000"
                                +"1111"+"000A"+""+"00";
    public const string StartCheck = "UPO00000000000000000000000010100010000000ASTARTCHECK00";
    //服务器响应
    public const string CheckStart = "ODW00000000000000000000000000300010001000ACHECKSTART00";
    public const string StopCheck  = "UPO000000000000000000000000101000100000009STOPCHECK00";
    public const string CheckStop  = "ODW000000000000000000000000003000100010009CHECKSTOP00";
    public bool isStartCheck = false;
    public bool isStopCheck = false;
    public string PupilLoc="0";


    void Start()
    {
 //       StartClient();
    }

    // Update is called once per frame 
    void Update()
    {
        
    }
    public bool StartClient()
    {
        bool islink =bt_connect_Click();
        //SendMsg( );
        return islink;
    }
    public void SendMsg()
    {
        //bt_send_Click(input.text);
        bt_send_Click(StartCheck);
    }
    private bool bt_connect_Click()
    {
        try
        {
            //创建客户端Socket，获得远程ip和端口号
            socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(_ip);
            IPEndPoint point = new IPEndPoint(ip, _port);

            socketSend.Connect(point);
            
            Debug.Log("tlog"+"连接成功!");
           // showinfo("isLink");
            //开启新的线程，不停的接收服务器发来的消息
            c_thread = new Thread(Received);
            c_thread.IsBackground = true;
            c_thread.Start();
            return true;
        }
        catch (System.Exception )
        {
            Debug.Log("IP或者端口号错误...");
            //showinfo("IP或者端口号错误...");
            return false;
        }
    }

    void Received()
    {
        while (true)
        {
            try
            {
                byte[] buffer = new byte[1024 * 1024 * 3];
                //实际接收到的有效字节数
                int len = socketSend.Receive(buffer);
                if (len == 0)
                {
                    break; 
                }
                Revstr = Encoding.UTF8.GetString(buffer, 0, len);
                Revdata();
                if(Revstr.StartsWith("ODW")&&len>44)
                {
                    Debug.Log("tlog"+"接收1：" +len+":" +socketSend.RemoteEndPoint + ":" + Revstr);
/*                     string v = "收到" + ":" + str;
                    TCPinfo.text = v; */
                   // showinfo(str);
                    switch(Revstr)
                    {
                        //开始校准
                        case CheckStart :
                            isStartCheck= true;
                        
                        Debug.Log("send:"+StopCheck);
                        break;
                        case CheckStop :
                            isStopCheck = true;
                        break;
                        default:
                        break;                        
                    }  
                 //  Debug.Log("包长度"+str.Substring(41,1)) ;   
                    if("3"==Revstr.Substring(41,1)) 
                    {
                        PupilLoc= Revstr.Substring(43,2);
                        Debug.Log("tlog"+"瞳孔位置:"+PupilLoc);     
                     //   TCPinfo.text="瞳孔位置:"+PupilLoc;               
                    }    
                }
                else
                {
                    Debug.Log("客户端打印：" + "非服务器数据:"+socketSend.RemoteEndPoint + ":" + Revstr);
                //    TCPinfo.text= "非服务器数据:"+socketSend.RemoteEndPoint + ":" + str;
                }                              
            }
            catch { }
        }
    }

// <summary>
    /// 向服务器发送消息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void bt_send_Click(string str)
    {
        try
        {                        
/*             string msg = str.Insert(42,Payload);
            Debug.Log("插入后数据："+msg); */
            string msg =str;
            byte[] buffer = new byte[1024 * 1024 * 3];
            buffer = Encoding.UTF8.GetBytes(msg);
            socketSend.Send(buffer);
        }
        catch { }
    }
    public void SendStopCheck()
    {
        bt_send_Click(StopCheck);
    }
    public void SendStartCheck()
    {
        bt_send_Click(StartCheck);
    }
    public void SocketQuit()
    {
        //关闭线程
        if (c_thread != null)
        {
            c_thread.Interrupt();
            c_thread.Abort();
        }
        //最后关闭服务器
        if (socketSend != null)
            socketSend.Close();
        print("关闭diconnect");
    }
    void OnApplicationQuit()
    {
        //退出时关闭连接
        SocketQuit();
    }
/*     public void showinfo(string info)
    {
        TCPinfo.text=info;
    } */
    public string Revdata()
    {
        return Revstr;
    }
}
