using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;

public class TcpSocket
{
    private Socket m_Socket;
    private byte[] data;
    private bool m_IsServer;

    private Action<string> m_receiveCallBack;
    public TcpSocket(Socket socket, int dataLength,bool isServer,Action<string> receivemsg)
    {
        this.m_Socket = socket;
        data = new byte[dataLength];
        this.m_IsServer = isServer;
        m_receiveCallBack = receivemsg;
    }
    #region 接受
    public void ClientReceive()
    {
        
         //data 数据缓存 0:指的是 接受位的偏移量   data.length指的是数据的长度   SocketFlags.None固定格式 
        //new AsyncCallback(ClientEndReceive)需要有返回值的回调函数,返回值是下面的方法
        //public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, 
        //AsyncCallback callback, object state);
        m_Socket.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ClientEnReceive), null);
    }
    public void ClientEnReceive(IAsyncResult ar)
    {
        //数据处理
        int receiveLength = m_Socket.EndReceive(ar);
        //吧结束完毕的字节数组转化为string类型
        string dataStr = System.Text.Encoding.UTF8.GetString(data, 0, receiveLength);
        m_receiveCallBack?.Invoke(dataStr);
        if (m_IsServer)
        {
            for(int i=0;i<ServerLoop.Instance.List_clients.Count;i++)
            {
                TcpSocket tmpsocket = ServerLoop.Instance.List_clients[i];
                if(tmpsocket.ClientConnected())
                {
                    GameMain.Instance.Log("  dataStr "+ dataStr);
                    tmpsocket.ClientSeed(System.Text.Encoding.UTF8.GetBytes("服务器回复:我收到了" + dataStr));
                }

            }
        }
        
    }
    #endregion
    #region 发送
    public void ClientSeed(byte[] data)
    {
        m_Socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ClientSeedEnd), null);
    }
    private void ClientSeedEnd(IAsyncResult ar)
    {
        m_Socket.EndSend(ar);
    }
    #endregion
    #region 链接
    public void ClientConnect(string ip,int port)
    {
        m_Socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), new AsyncCallback(ClientEndConnect), null);
    }
    private void ClientEndConnect(IAsyncResult ar)
    {
        if(ar.IsCompleted)
        {
            m_receiveCallBack?.Invoke("连接成功");
        }
        m_Socket.EndConnect(ar);
    }
    #endregion
    #region 是否在链接状态
    public bool ClientConnected()
    {
        return m_Socket.Connected;
    }
    #endregion
}
