﻿using System;
using ProtoBuf;
using Roma;

public class MsgStartMatch : NetMessage
{
    public MsgStartMatch()
        : base(eNetMessageID.MsgStartMatch)
    {

    }

    public static NetMessage CreateMessage()
    {
        return new MsgStartMatch();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        SetByte<GC_MatchResult>(m_matchResult, ref ls);
    }
    
    public override void OnRecv(ref Conn conn)
    {
        // 接收匹配类型，开始匹配，如果有异常，直接返回
        if (eno == 0)
        {
            int type = GetData<int>(structBytes);
            Console.WriteLine("收到匹配信息：" + conn.player.publicData.userName + " type:" + type);
            if (type == 1)  // 1V1匹配
            {
                // 开始匹配
                LobbyManager.Inst.GetLobby(3).OnJoinMatch(conn.player);
            }
        }
    }

    public GC_MatchResult m_matchResult = new GC_MatchResult();
}

