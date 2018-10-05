using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    public class SkillSingleFly : SkillBase
    {

        private bool m_bFly;
        private Vector2 m_startPos;

        public SkillSingleFly(long id, VSkillBase vSkill)
            : base(id, vSkill)
        {
        
        }

        public override void ExecuteFrame(int frameId)
        {
            base.ExecuteFrame(frameId);
            if(m_vSkill != null && m_bFly)
            {
                m_curPos = m_curPos + m_curSkillCmd.m_dir * FSPParam.clientFrameScTime * m_skillInfo.flySpeed * 0.001f;
                Vector3 pos = new Vector3(m_curPos.x, 1, m_curPos.y);
                m_vSkill.SetPos(pos);
                m_vSkill.SetDir(m_curSkillCmd.m_dir);

                // 动态检测
                foreach(KeyValuePair<long, CPlayer> item in CPlayerMgr.m_dicPlayer)
                {
                    CCreature creature = item.Value;
                    if(creature.GetUid() ==  GetCaster().GetUid())
                        continue;
                    Sphere flyS = new Sphere();
                    flyS.c = m_curPos;
                    flyS.r = m_skillInfo.length;

                    Sphere playerS = new Sphere();
                    playerS.c = creature.GetPos();
                    playerS.r = creature.GetR();

                    if(Collide.bSphereSphere(flyS, playerS))
                    {
                        Debug.Log("检测:" + creature.GetUid());
                        OnHit(creature);
                        m_bFly = false;
                        Destory();
                        return;
                    }
                }
                // 静态碰撞检测
                foreach(OBB item in CMapMgr.m_map.m_listBarrier)
                {
                    Sphere s = new Sphere();
                    s.c = m_curPos;
                    s.r = 1;
                    if(Collide.bSphereOBB(s, item))
                    {
                        OnHit(m_curPos);
                        m_bFly = false;
                        Destory();
                        return;
                    }
                }
                // 自爆
                if(Vector2.Distance(m_startPos, m_curPos) >= m_skillInfo.distance)
                {
                    OnHit(m_curPos);
                    m_bFly = false;
                    Destory();
                    return;
                }
            }
        }

        public override void Launch()
        {
            Debug.Log("发射技能");
            m_curPos = CPlayerMgr.Get(m_curSkillCmd.m_casterUid).GetPos() + m_curSkillCmd.m_dir.normalized;
            m_startPos = m_curPos;

            CmdSkillCreate cmd = new CmdSkillCreate();
            m_vSkill.PushCommand(cmd);

            m_bFly = true;
        }




    }
}