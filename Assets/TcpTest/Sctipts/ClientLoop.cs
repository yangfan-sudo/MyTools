using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;

public class ClientLoop : IGameLoop
{
    private TcpSocket m_tcpClient;
    private Socket m_Socket;
    public void Start()
    {
        m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_tcpClient = new TcpSocket(m_Socket,1024, false, RectiveStr);
    }

    public void Update()
    {
        if (m_tcpClient != null && m_tcpClient.ClientConnected())
        {
            m_tcpClient?.ClientReceive();
        }
    }
    //固定使用10086端口，ip格式:127.0.0.1
    public void OnClickConnectBtn(string connectipstr)
    {
        if(m_tcpClient != null && !m_tcpClient.ClientConnected())
        {
            m_tcpClient.ClientConnect(connectipstr, 10086);
        }
    }
    public void OnClickSendToServer(string sendtoServer)
    {
        Debug.Log(" sendtoServer  " + sendtoServer);
        if (m_tcpClient != null && m_tcpClient.ClientConnected())
        {
            m_tcpClient.ClientSeed(System.Text.Encoding.UTF8.GetBytes(sendtoServer));
        }
    }
    public void RectiveStr(string receiveStr)
    {
        GameMain.Instance.Log(receiveStr);
    }
}
