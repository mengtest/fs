﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace Roma
{
    public class AutoMoveState : FSMState
    {
        public CmdFspAutoMove m_cmdFspAutoMove;

        private List<Vector2> m_movePath;
        private List<float> m_pathTime;
        private int m_curMoveIndex;
        private float m_curMoveTime;
        public bool m_bMovePathing;
        // 为了兼容碰撞的推开，这里改成根据方向移动（碰撞会修改curPos），但是必须保证障碍数据比碰撞体大一个角色的半径
        private List<Vector2> m_dirList;
        private Action m_moveEnd;

        private const int UI_UNIT_DIS = 10;

        public AutoMoveState(CCreature go)
            : base(go)
        {
            m_stateId = StateID.eFspAutoMove;

            if (m_movePath == null)
                m_movePath = new List<Vector2>();
            if (m_pathTime == null)
                m_pathTime = new List<float>();
            if (m_dirList == null)
                m_dirList = new List<Vector2>();
            if (m_cmdFspAutoMove == null)
                m_cmdFspAutoMove = new CmdFspAutoMove();

        }
        public override void Enter(IFspCmdType cmd)
        {
            m_cmdFspAutoMove = cmd as CmdFspAutoMove;

            List<Vector2> m_movePath = new List<Vector2>();
            CMapMgr.m_map.GetPath(m_creature, m_creature.GetPos().ToVector2(), m_cmdFspAutoMove.m_pos.ToVector2(), ref m_movePath);
            //m_moveEnd = moveEnd;

            float divSpeed = 1 / m_creature.GetSpeed().value;
            StartAutoMove(ref m_movePath, divSpeed);
        }


        public void StartAutoMove(ref List<Vector2> pathList, float divSpeed)
        {
            m_movePath.Clear();
            m_pathTime.Clear();
            m_curMoveIndex = 0;
            m_curMoveTime = 0.0f;
            m_dirList.Clear();
            m_bMovePathing = true;
            // 给路径赋值
            for (int i = 0; i < pathList.Count; i++)
            {
                m_movePath.Add(pathList[i]);
            }
            // 插入玩家当前起点位置
            m_movePath.Insert(0, m_creature.GetPos().ToVector2());

            // 开始获取移动时间
            float runTime = 0.0f;
            for (int i = 0; i < m_movePath.Count - 1; i++)
            {
                float dis = Vector2.Distance(m_movePath[i], m_movePath[i + 1]);
                runTime += dis * divSpeed;
                m_pathTime.Add(runTime);
                // 方向
                m_dirList.Add(m_movePath[i + 1] - m_movePath[i]);
            }
            m_pathTime.Insert(0, 0);    // 在第一个位置时间当然是0了
        }

        public override void ExecuteFrame(int frameId)
        {
            if (m_bMovePathing)
            {
                _UpdateMove(0.0f, FSPParam.clientFrameScTime);

                if (Debug.logger.logEnabled)
                {
                    for (int i = 0; i < m_movePath.Count - 1; i++)
                    {
                        float x1 = m_movePath[i].x;
                        float y1 = m_movePath[i].y;
                        float x2 = m_movePath[i + 1].x;
                        float y2 = m_movePath[i + 1].y;
                        Debug.DrawLine(
                            new Vector3(x1, 4, y1),
                            new Vector3(x2, 4, y2),
                            Color.green);
                    }
                }
            }
        }

        private void _UpdateMove(float fTime, float fDTime)
        {
            //float t = 0.0f;
            m_curMoveTime += fDTime;
            int begin = m_curMoveIndex + 1;
            for (int i = begin; i < m_pathTime.Count; i++)    // 索引0,1,2,3,4 取 1-4 作为对比的目标时间
            {
                if (m_curMoveTime > m_pathTime[i])
                {
                    m_curMoveIndex = i;
                    break;
                }
            }

            if (m_curMoveIndex >= m_pathTime.Count - 1)
            {
                // 最后一个地点
                StopAutoMove();
                return;
            }
            else
            {
                float delta = FSPParam.clientFrameScTime * m_creature.GetSpeed().value;
                Vector2 nextPos = m_creature.m_curPos.ToVector2() + m_dirList[m_curMoveIndex].normalized * delta;

                m_creature.SetPos(nextPos.ToVector2d());
                m_creature.SetDir(m_dirList[m_curMoveIndex].normalized.ToVector2d());
            }
        }

        public void StopAutoMove()
        {
            if (m_bMovePathing)
            {
                m_bMovePathing = false;
                m_creature.PushCommand(CmdFspStopMove.Inst);
                if (m_moveEnd != null)
                {
                    m_moveEnd();
                    m_moveEnd = null;
                }
            }
        }



        public override void Exit()
        {

        }


    }
}
