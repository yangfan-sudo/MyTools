using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private InputField m_InputField;
    [SerializeField] private Button m_HostButton;
    [SerializeField] private Button m_LoginButton;
    [SerializeField] private Button m_SentButton;
    private string m_logStr;
    private static MainUI m_MainUI;
    public static MainUI Instance => m_MainUI;
    
    private void Awake()
    {
        m_MainUI = this;
        m_HostButton.onClick.AddListener(OnHostClick);
        m_LoginButton.onClick.AddListener(OnLogin);
        m_SentButton.onClick.AddListener(OnSendMsg);
        m_SentButton.gameObject.SetActive(false);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnHostClick()
    {
        GameMain.Instance.OnHostClick();
        m_HostButton.gameObject.SetActive(false);
        m_LoginButton.gameObject.SetActive(false);
        m_SentButton.gameObject.SetActive(false);
        m_InputField.gameObject.SetActive(false);
    }
    public void OnLogin()
    {
        GameMain.Instance.OnLogin();
        GameMain.Instance.OnConnect(m_InputField.text);
        m_SentButton.gameObject.SetActive(true);
        m_HostButton.gameObject.SetActive(false);
        m_LoginButton.gameObject.SetActive(false);
        m_InputField.text = "";
    }
    public void OnSendMsg()
    {
        GameMain.Instance.OnSendMsg(m_InputField.text);
        //Log(m_InputField.text);
        m_InputField.text = "";
    }
    public void Log(string log)
    {
        m_logStr += (log + "\n");
    }
    private void OnGUI()
    {
        GUILayout.BeginHorizontal("Box");
        GUILayout.Label(m_logStr);
        GUILayout.EndHorizontal();
    }

}
