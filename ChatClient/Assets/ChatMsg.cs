using System.Collections;
using ProtoBuf;

/// <summary>
/// 消息对象
/// </summary>
[ProtoContract]
public class ChatMsg {

    [ProtoMember(1)]
    public string sender;//发送者
    [ProtoMember(2)]
    public string msg;//消息

}
