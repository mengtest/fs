using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 技能的逻辑层
    /// </summary>
    public partial class SkillBase : CCreature
    {
        public CmdFspSendSkill m_curSkillCmd;
        public SkillCsvData m_skillInfo;
        
        private bool m_bLaunch;
        private int m_curLaunchTime;

        public VSkillBase m_vSkill;

        public SkillBase(long id, VSkillBase vSkill)
            : base(id)
        {
            m_type = EThingType.Skill;
            m_vSkill = vSkill;
        }

        public override void PushCommand(IFspCmdType cmd)
        {
            switch (cmd.GetCmdType())
            {
                case CmdFspEnum.eFspSendSkill:
                    m_curSkillCmd = cmd as CmdFspSendSkill;
                    m_skillInfo = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill).GetData(m_curSkillCmd.m_skillId);
                    m_bLaunch = true;
                    break;
            }
        }        

        public override void ExecuteFrame(int frameId)
        {
            if(m_bLaunch)
            {
                m_curLaunchTime += FSPParam.clientFrameMsTime;
                if(m_curLaunchTime > m_skillInfo.launchTime)
                {
                    m_bLaunch = false;
                    Launch();
                }
            }
        }

        /// <summary>
        /// 弹道起飞,近战，AOE受击
        /// </summary>
        public virtual void Launch()
        {

        }

        public void OnHit(CCreature creature)
        {
            CmdSkillHit cmd = new CmdSkillHit();
            cmd.bPlayer = true;
            cmd.uid = (int)creature.GetUid();
            m_vSkill.PushCommand(cmd);
        }

        public void OnHit(Vector2 pos)
        {
            CmdSkillHit cmd = new CmdSkillHit();
            cmd.bPlayer = false;
            cmd.pos = new Vector3(pos.x, 1, pos.y);
            m_vSkill.PushCommand(cmd);
        }

        public CCreature GetCaster()
        {
            return CPlayerMgr.Get(m_curSkillCmd.m_casterUid);
        }

        public override void Destory()
        {
            if(m_vSkill != null)
            {
                m_vSkill.Destory();
                m_vSkill = null;
            }
 
        }

    }
}