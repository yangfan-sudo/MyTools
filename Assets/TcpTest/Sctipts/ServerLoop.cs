using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;

public class ServerLoop : IGameLoop
{
    public List<TcpSocket> List_clients;
    private Socket server;
    private bool isLoopAccept = true;
    private static ServerLoop m_serverLoop;
    public static ServerLoop Instance => m_serverLoop;
    public void Start()
    {
        m_serverLoop = this;
        //服务器socket  协议族
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //绑定端口号
        server.Bind(new IPEndPoint(IPAddress.Any, 10086));
        //可以监听的客户端数目
        server.Listen(100);

        //开辟新的线程  处理客户端请求
        Thread listenThread = new Thread(ReceiveClient);
        //开启线程
        listenThread.Start();
        //后台运行
        listenThread.IsBackground = true;
        List_clients = new List<TcpSocket>();
    }
    
    
    /// <summary>
    /// 接收客户端连接请求
    /// </summary>
    private void ReceiveClient()
    {
        while(isLoopAccept)
        {
            server.BeginAccept(AcceptClient, null);
            Debug.Log("检测客户端连接中.....");

            //每隔1秒检测 有没有链接
            Thread.Sleep(1000);
        }
    }
    private void AcceptClient(IAsyncResult ar)
    {
        Socket client = server.EndAccept(ar);
        TcpSocket clientSocket = new TcpSocket(client, 1024, true, RectiveStr);
        List_clients.Add(clientSocket);
        Debug.Log("连接成功");
    }
    public void RectiveStr(string receiveStr)
    {
        Debug.Log(receiveStr);
    }
    public void OnApplicationQuit()
    {
        isLoopAccept = false;
        if(server !=null && server.Connected)
        {
            server.Close();
        }
    }
    public void Update()
    {
        if(List_clients!=null&&List_clients.Count>0)
        {
            for(int i=0;i<List_clients.Count;i++)
            {
                List_clients[i].ClientReceive();
            }
        }
    }
}
