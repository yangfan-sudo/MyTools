using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameLoop
{
    void Start();
    void Update();
}
public class GameMain : MonoBehaviour
{
    private IGameLoop m_GameLoop;
    [SerializeField] MainUI m_MainUI;
    private static GameMain m_gameMain;
    public static GameMain Instance => m_gameMain;

    private void Awake()
    {
        m_gameMain = this;
    }
    public void Log(string receiveStr)
    {

        m_MainUI.Log(receiveStr);
    }
    public void OnHostClick()
    {
        if(m_GameLoop==null)
        {
            m_GameLoop = new ServerLoop();
        }
        m_GameLoop.Start();
    }
    public void OnLogin()
    {
        if (m_GameLoop == null)
        {
            m_GameLoop = new ClientLoop();
        }
        m_GameLoop.Start();
    }
    public void OnConnect(string ipStr)
    {
        if(m_GameLoop is ClientLoop)
        {
            (m_GameLoop as ClientLoop).OnClickConnectBtn(ipStr);
        }
    }
    public void OnSendMsg(string sendStr)
    {
        if (m_GameLoop is ClientLoop)
        {
            (m_GameLoop as ClientLoop).OnClickSendToServer(sendStr);
        }
    }
    private void OnApplicationQuit()
    {
        if (m_GameLoop is ServerLoop)
        {
            (m_GameLoop as ServerLoop).OnApplicationQuit();
        }
    }
    // Update is called once per frame
    void Update()
    {
        m_GameLoop?.Update();
    }
}
