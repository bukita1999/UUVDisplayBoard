using System.Net;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;


public class UDPServer : MonoBehaviour
{
    private Socket ServerSocket;

    private IPEndPoint Clients;

    private IPEndPoint Server;

    private EndPoint epSender;

    //发送数据的字符数组
    private byte[] SendData = new byte[1024];

    //接受数据的字符数组
    private byte[] ReceiveData = new byte[1024];
    // Start is called before the first frame update
    void Start()
    {
        //服务器Socket对象实例化
        ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //服务器的IP和端口
        Server = new IPEndPoint(IPAddress.Any, 8866);

        //Socket对象跟服务器端的IP和端口绑定
        ServerSocket.Bind(Server);

        //客户端的IP和端口，端口 0 表示任意端口
        Clients = new IPEndPoint(IPAddress.Any, 0);

        //实例化客户端 终点
        epSender = (EndPoint)Clients;

        ServerSocket.BeginReceiveFrom(ReceiveData, 0, ReceiveData.Length, SocketFlags.None, ref epSender, new AsyncCallback(ReceiveFromClients), epSender);
    }
    /// <summary>
    /// 异步接受，处理数据
    /// </summary>
    /// <param name="iar"></param>
    private void ReceiveFromClients(IAsyncResult iar)
    {
        int reve = ServerSocket.EndReceiveFrom(iar, ref epSender);

        //数据处理
        string str = System.Text.Encoding.UTF8.GetString(ReceiveData, 0, reve);

        //每次接受到数据就传给ServerInput做处理。
        ServerInput.GetInstance().AnalyzeJsonData(str);

        ServerSocket.BeginReceiveFrom(ReceiveData, 0, ReceiveData.Length, SocketFlags.None, ref epSender, new AsyncCallback(ReceiveFromClients), epSender);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
class ServerInput{
    public event Action<ResPiEntity> ServerInputEvent;

    private static ServerInput instance = null;
    public static ServerInput GetInstance() { return instance ?? new ServerInput(); }

    ServerInput() { instance = this; }

    /// <summary>
    /// 处理服务器传来的json信息,传给事件
    /// </summary>
    /// <param name="jsonData"></param>
    public void AnalyzeJsonData(string jsonData)
    {

        try
        {
            ResPiEntity myentity = new ResPiEntity();
            /*List<Vector2> posList = new List<Vector2>();
            JArray jsonObj = JArray.Parse(jsonData);

            foreach (JObject jObject in jsonObj)
            {
                //Debug.Log(jObject["x"]);
                //Debug.Log(jObject["y"]);

                Vector2 pos = new Vector2((int)jObject["x"], Screen.height - (int)jObject["y"]);
                posList.Add(pos);

            }
            if (ServerInputEvent != null)
                ServerInputEvent(posList);*/
            myentity = JsonUtility.FromJson<ResPiEntity>(jsonData);
            if (ServerInputEvent != null)
                ServerInputEvent(myentity);
            Debug.Log(myentity.humidity);
            Debug.Log(myentity.temperature);
        }
        catch (Exception)
        {

            throw;
        }
    }
};
[Serializable]
class ResPiEntity{
    public string humidity = "";
    public string temperature = "";
    public string x_rotation = "";
    public string y_rotation = "";
    public string angle = "";
};
