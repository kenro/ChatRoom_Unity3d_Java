using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/// <summary>
/// 封装Socket
/// </summary>
public class NetClient
{
    public static NetClient instance = new NetClient();
    private TcpClient client;
    private bool isHead;
    private int len;

    /// <summary>
    /// 初始化网络连接
    /// </summary>
    public void Init()
    {
        client = new TcpClient();
        client.Connect("127.0.0.1", 8888);
        isHead = true;
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public void SendMsg(byte[] msg)
    {
        //消息体结构：消息体长度+消息体
        byte[] data = new byte[4 + msg.Length];
        IntToBytes(msg.Length).CopyTo(data, 0);
        msg.CopyTo(data, 4);
        client.GetStream().Write(data, 0, data.Length);
    }

    /// <summary>
    /// 接收消息
    /// </summary>
    public void ReceiveMsg()
    {
        NetworkStream stream = client.GetStream();
        if (!stream.CanRead)
        {
            return;
        }
        //读取消息体的长度
        if (isHead)
        {
            if (client.Available < 4)
            {
                return;
            }
            byte[] lenByte = new byte[4];
            stream.Read(lenByte, 0, 4);
            len = BytesToInt(lenByte, 0);
            isHead = false;
        }
        //读取消息体内容
        if (!isHead)
        {
            if (client.Available < len)
            {
                return;
            }
            byte[] msgByte = new byte[len];
            stream.Read(msgByte, 0, len);
            isHead = true;
            len = 0;
            if (onRecMsg != null)
            {
                //处理消息
                onRecMsg(msgByte);
            }
        }

    }

    /// <summary>
    /// bytes转int
    /// </summary>
    /// <param name="data"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static int BytesToInt(byte[] data, int offset)
    {
        int num = 0;
        for (int i = offset; i < offset + 4; i++)
        {
            num <<= 8;
            num |= (data[i] & 0xff);
        }
        return num;
    }

    /// <summary>
    /// int 转 bytes
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static byte[] IntToBytes(int num)
    {
        byte[] bytes = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            bytes[i] = (byte)(num >> (24 - i * 8));
        }
        return bytes;
    }

    public delegate void OnRevMsg(byte[] msg);

    public OnRevMsg onRecMsg;
}

