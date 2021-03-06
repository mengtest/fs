﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    /// <summary>
    /// 可旋转发射变长矩形
    /// </summary>
    public class CBuffTrigger_Curve : CBuffTrigger
    {
        public CBuffTrigger_Curve(long id)
            : base(id)
        {

        }

        public override bool Create(int csvId, string name, Vector2d pos, Vector2d dir, float scale = 1)
        {
            base.Create(csvId, name, pos, dir, scale);

            if (m_vCreature != null)
            {
                sCurveParam param;
                param.endPos = m_skillPos.ToVector3();
                param.time = m_triggerData.ContinuanceTime;
                param.heigh = m_triggerData.iCurveHeight;
                m_vCreature.PlayCurve(param);
            }

            return true;
        }

        public override void InitPos(ref Vector2d startPos, ref Vector2d startDir)
        {
            // 修正起始位置startPos
            startPos = startPos + startDir.normalized * new FixedPoint(m_triggerData.vBulletDeltaPos.z);

            // 修正结束位置
            Vector2d dir = FPCollide.Rotate(startDir, m_triggerData.dirDelta);
            m_skillPos = m_skillPos + dir.normalized * m_triggerData.disDelta;

            // 一定时间后设置逻辑位置
            CFrameTimeMgr.Inst.RegisterEvent(m_triggerData.ContinuanceTime - 100, () =>
            {
                SetPos(m_skillPos, true);
            });
        }

        public override void ExecuteFrame(int frameId)
        {
            base.ExecuteFrame(frameId);
        }
    }
}

