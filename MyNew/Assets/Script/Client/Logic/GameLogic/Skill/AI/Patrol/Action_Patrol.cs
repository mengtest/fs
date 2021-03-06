﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 巡逻行为
    /// 1.获取移动目标位置
    /// 2.移动到目标位置
    /// </summary>

    public class Action_Patrol : AIAction_
    {

        private int m_curIntervalTime;

        public override BtResult Execute()
        {
            m_curIntervalTime -= FSPParam.clientFrameMsTime;
            if (m_curIntervalTime > 0)
            {
                return BtResult.Running;
            }

            //// 休闲时，继续巡逻
            if (m_creature.m_stateMgr.GetCurState() == StateID.eFspStopMove)
            {
                Debug.Log("设置移动");
                //Vector2 pPos = m_creature.m_bornPoint;
                Vector2 pPos = m_creature.GetPos().ToVector2();
                Vector2 end = CMapMgr.m_map.GetRandomPos(pPos.x, pPos.y, 10, m_creature.m_aiType);
                
                CmdFspAutoMove cmd = new CmdFspAutoMove();
                cmd.m_pos = end.ToVector2d();
                if(m_creature.m_aiType == eAIType.Player)
                {
                    m_creature.SendFspCmd(cmd);
                }
                else
                {
                    m_creature.PushCommand(cmd);
                }
                m_curIntervalTime = 30 * FSPParam.clientFrameMsTime;
            }
            return BtResult.Running;
        }
    }
}
