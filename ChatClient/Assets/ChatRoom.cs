using ProtoBuf;
using System.IO;
using System.Text;
using UnityEngine;

public class ChatRoom : MonoBehaviour
{
    public UIInput nameInput;
    public UITextList textList;
    public UIInput talk;
    private string name = "guest";

    void Start()
    {
        NetClient.instance.Init();
        NetClient.instance.onRecMsg = OnRecMsg;
        textList.Add("Hi,Welcome guest! You can chang your nick name.");
    }

    void Update()
    {
        //接受消息
        NetClient.instance.ReceiveMsg();
    }

    //提交消息
    void OnSubmit()
    {
        string text = NGUITools.StripSymbols(talk.text);

        if (!string.IsNullOrEmpty(text))
        {
            talk.text = "";
            NetClient.instance.SendMsg(serial(name,text));
        }
    }

    //更改昵称
    void OnSubmitName()
    {
        if (!string.IsNullOrEmpty(nameInput.text))
        {
            name = nameInput.text;
        }
    }

    //将二进制流解码成Protobuf对象，显示出来
    void OnRecMsg(byte[] msg)
    {
		using(MemoryStream ms = new MemoryStream()){
			ms.Write(msg,0,msg.Length);
			ms.Position = 0;
		    ChatMsg chatMsg = Serializer.Deserialize<ChatMsg>(ms);
			textList.Add(chatMsg.sender+":"+chatMsg.msg);
		}
    }

    //将消息体编码成二进制流
    private byte[] serial(string sender, string msg)
    {
        ChatMsg chatMsg = new ChatMsg();
        chatMsg.sender = sender;
        chatMsg.msg = msg;

        using (MemoryStream ms = new MemoryStream())
        {
            Serializer.Serialize<ChatMsg>(ms, chatMsg);
			byte[] data = new byte[ms.Length];
			ms.Position= 0;
            ms.Read(data, 0, data.Length);
            return data;
        }
    }

}