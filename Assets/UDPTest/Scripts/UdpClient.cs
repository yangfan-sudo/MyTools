﻿using UnityEngine;
using System.Collections;
//引入库
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UdpClient : MonoBehaviour
{
    string editString = "hello wolrd"; //编辑框文字

    //以下默认都是私有的成员
    Socket socket; //目标socket
    EndPoint serverEnd; //服务端
    IPEndPoint ipEnd; //服务端端口
    string recvStr; //接收的字符串
    string sendStr; //发送的字符串
    byte[] recvData = new byte[1024]; //接收的数据，必须为字节
    byte[] sendData = new byte[1024]; //发送的数据，必须为字节
    int recvLen; //接收的数据长度
    Thread connectThread; //连接线程

    //初始化
    void InitSocket()
    {
        //定义连接的服务器ip和端口，可以是本机ip，局域网，互联网
        ipEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8001);
        //定义套接字类型,在主线程中定义
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //定义服务端
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        serverEnd = (EndPoint)sender;
        print("waiting for sending UDP dgram");

        //建立初始连接，这句非常重要，第一次连接初始化了serverEnd后面才能收到消息
        SocketSend("hello");

        //开启一个线程连接，必须的，否则主线程卡死
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    void SocketSend(string sendStr)
    {
        //清空发送缓存
        sendData = new byte[1024];
        //数据类型转换
        sendData = Encoding.ASCII.GetBytes(sendStr);
        //发送给指定服务端
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
    }

    //服务器接收
    void SocketReceive()
    {
        //进入接收循环
        while (true)
        {
            //对data清零
            recvData = new byte[1024];
            //获取客户端，获取服务端端数据，用引用给服务端赋值，实际上服务端已经定义好并不需要赋值
            recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
            print("message from: " + serverEnd.ToString()); //打印服务端信息
            //输出接收到的数据
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);
        }
    }

    //连接关闭
    void SocketQuit()
    {
        //关闭线程
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最后关闭socket
        if (socket != null)
            socket.Close();
    }

    // Use this for initialization
    void Start()
    {
        InitSocket(); //在这里初始化
    }

    void OnGUI()
    {
        editString = GUI.TextField(new Rect(10, 10, 100, 20), editString);
        if (GUI.Button(new Rect(10, 30, 60, 20), "send"))
            SocketSend(editString);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnApplicationQuit()
    {
        SocketQuit();
    }
}