﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Roma
{
    public enum EThingType
    {
        None = 0,           // 0 = 未明确
        Master,             // 主角
        Player,             // 玩家
        NPC,                // NPC
        Monster,            // 怪物
        Skill,

        BuffTrigger,
    }
    public class CThing
    {
        protected long m_uId;
        protected EThingType m_type;
        // 所有逻辑对象的销毁标识，用于下一帧销毁
        public bool m_destroy;
        public CThing(long id)
        {
            m_uId = id;
        }

        public virtual void Destory()
        {

        }

        public long GetUid()
        {
            return m_uId;
        }

        public void SetType(EThingType type)
        {
            m_type = type;
        }

        public EThingType GetThingType()
        {
            return m_type;
        }

        public bool IsMaster()
        {
            //return m_type == EThingType.Master;
            return m_uId == EGame.m_uid;
        }

        public bool IsPlayer()
        {
            return m_type == EThingType.Player;
        }

        public bool IsMonster()
        {
            return m_type == EThingType.Monster;
        }

        public bool IsNpc()
        {
            return m_type == EThingType.NPC;
        }
    }
}