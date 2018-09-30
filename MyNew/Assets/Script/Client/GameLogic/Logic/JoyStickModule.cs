﻿using UnityEngine;

namespace Roma
{
    public partial class JoyStickModule : Widget
    {
        public JoyStickModule()
            : base(LogicModuleIndex.eLM_PanelJoyStick)
        {
        }

        public static Widget CreateLogicModule()
        {
            return new JoyStickModule();
        }

        public override void Init()
        {
            m_ui = GetUI<UIPanelJoyStick>();
        
            // 注册移动
            MoveJoyStick move = m_ui.m_move;
            move.OnJoySticjEvent = OnMoveEvent;
            // 注册技能事件，升级按钮
            SkillJoyStick[] skillList = m_ui.m_SkillBtn;
            for (int i = 0; i < skillList.Length; i++)
            {
                skillList[i].SetLv(1);
                skillList[i].OnJoyStickEvent = OnSkillEvent;
                //skillList[i].OnLvUpEvent = OnClickLvUpBtn;
            }

            
            // 注册取消按钮
            UIDragListener.Get(m_ui.m_cancelBtn).OnDragEvent = (ev, delta) =>
            {
                if (ev == eDragEvent.Enter)       // 红色,取消技能
                {
                    SetSkillCancel(true);
                }
                else if (ev == eDragEvent.Exit)    // 不取消技能
                {
                    SetSkillCancel(false);
                }
            };
        }

        public override void InitData()
        {
            OnLoadSelect();
        }

        private void OnLoadSelect()
        {
            m_master = CPlayerMgr.GetMaster();
            EntityBaseInfo info = new EntityBaseInfo();
            info.m_resID = 3;
            info.m_ilayer = (int)LusuoLayer.eEL_Dynamic;
            m_skillChoseHid = EntityManager.Inst.CreateEntity(eEntityType.eNone, info, (ent) =>
            {
                ent.SetDirection(Vector3.zero);
                //ent.SetColor(SKLL_BLUE);
                ent.SetRenderQueue(3001);
                GameObject skillChose = ent.GetObject();
                m_skillChose = skillChose.transform;
                m_skillDistance = m_skillChose.FindChild("distance");
                m_skillCenter = m_skillChose.FindChild("center");
                m_skillDir = m_skillCenter.FindChild("dir");
            });
        }

        public override void UpdateUI(float time, float fdTime)
        {
            if(m_master != null && m_skillChose != null)
            {
                m_skillChose.position = m_master.m_vCreature.GetEnt().GetPos();
            }
        }

        private void SetSkillCancel(bool bCancel)
        {
             if (bCancel)       // 红色,取消技能
            {
                m_bSkillCancel = true;
                Entity ent = EntityManager.Inst.GetEnity(m_skillChoseHid);
                ent.SetColor(SKLL_RED);
                m_ui.SetColor(m_curSkillIndex, SKLL_RED);
                //SetSelectColor(SKLL_RED);
            }
            else   // 不取消技能
            {
                m_bSkillCancel = false;
                Entity ent = EntityManager.Inst.GetEnity(m_skillChoseHid);
                ent.SetColor(SKLL_BLUE);
                m_ui.SetColor(m_curSkillIndex, SKLL_BLUE);
                //SetSelectColor(SKLL_BLUE);
            } 
        }

        #region 移动相关
        private void OnMoveEvent(eJoyStickEvent jsEvent, MoveJoyStick move)
        {
            if (jsEvent == eJoyStickEvent.Up)
            {
                OnMove(true, move.m_delta);
            }
            else
            {
                OnMove(false, move.m_delta);
                //Debug.Log("发送移动。。。。。。。。。。。。。。。。。。。。。" + move.m_delta);
            }
        }

        public void OnMove(bool bUp, Vector2 delta)
        {
            if (bUp)
            {
                //停止事件
                m_moveDir = Vector3.zero;
                m_curVelocity = Vector3.zero;
                m_isFirstJoyStick = true;
                CMasterPlayer master = CPlayerMgr.GetMaster();
                if (master != null)
                {
                    //发送停止消息
                    master.SendFspCmd(new CmdFspStopMove());
                    //Debug.Log("发送停止。。。。。。。。。。。。。。。。。。。。。");
                }
                return;
            }
            else
            {
                m_moveDir = delta;
                m_moveDir.Normalize();
                MasterMove();
            }
        }


        //主角移动, 如果是update心跳。避免频繁发送消息，加入15度和时间的概念。如果是fixupdate心跳，可以取消这个设置。（暂时放入update心跳中）
        public void MasterMove()
        {
            if (m_isFirstJoyStick)
            {
                //第一次直接发送移动消息
                PushMoveCommand();
            }
            else
            {
                m_time += Time.deltaTime;
                if(m_time > 0.1f)
                {
                    m_time = 0;
                    //之后大于15度发送消息
                    float angle = Vector3.Angle(m_curVelocity, m_moveDir);
                    if (angle > 15)
                    {
                        PushMoveCommand();
                    }
                }

            }
        }

        private float m_time =0;
        //直接发送移动消息
        private void PushMoveCommand()
        {
            if (m_moveDir == Vector2.zero)
            {
                return;
            }

            CMasterPlayer master = CPlayerMgr.GetMaster();

            if (master != null)
            {
                m_isFirstJoyStick = false;
                CmdFspMove cmd = new CmdFspMove(ref m_moveDir);
                master.SendFspCmd(cmd);
            }
        }
        #endregion
 
        /// <summary>
        /// 摇杆的场景指示器也只能主角自己有，并且是操作完成才发送指令
        /// </summary>
        private void OnSkillEvent(int index, eJoyStickEvent jsEvent, SkillJoyStick joyStick, bool bCancel)
        {
            m_curSkillIndex = index;
            m_curSkillJoyStick = jsEvent;
            CMasterPlayer master = CPlayerMgr.GetMaster();

            Vector3 dir = new Vector3(joyStick.m_delta.x, 0 , joyStick.m_delta.y);
         
            dir.Normalize();
            float distance = 0;
            float length = 0;
            float width = 0;
            if(jsEvent == eJoyStickEvent.Down)
            {
                m_bSkillCancel = false;
                m_ui.m_cancelBtn.SetActiveNew(true);
                m_ui.SetColor(index, SKLL_BLUE);

                int skillId = master.m_csv.skill0;
                skillInfo = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill).GetData(skillId);

                m_skillChose.gameObject.SetActiveNew(true);
                m_skillCenter.gameObject.SetActiveNew(true);
                m_skillDistance.gameObject.SetActiveNew(true);
                m_skillDir.gameObject.SetActiveNew(true);

                distance = skillInfo.distance;
                length = skillInfo.length;
                width = skillInfo.width;

                m_skillDistance.localScale = new Vector3(distance, 0.01f, distance);
                m_skillDir.localScale = new Vector3(distance, 0.01f, distance);
            }
            else if(jsEvent == eJoyStickEvent.Drag)
            {
                
            }
            else if(jsEvent == eJoyStickEvent.Up)
            {
                // 发送技能
                if (!m_bSkillCancel)
                {
                    m_bSkillCancel = true;
                    Debug.Log("发送技能：" + skillInfo.id);
                    CmdFspSendSkill cmd = new CmdFspSendSkill();
                    cmd.m_skillId = skillInfo.id;
                    cmd.m_dir = joyStick.m_delta;
                    master.SendFspCmd(cmd);
                }
                // 还原指示器
                CancelSkill();

                m_skillChose.gameObject.SetActiveNew(false);

 
            }
            m_skillCenter.rotation = Quaternion.LookRotation(dir);   // 只用控制中心点的方向
        }

        /// <summary>
        /// 本地还原指示器，取消技能
        /// </summary>
        public void CancelSkill()
        {
            if (m_skillChose == null)
                return;
            m_bSkillCancel = true;
            m_ui.m_cancelBtn.SetActiveNew(false);
            m_ui.CloseSkillFocus();
            m_skillChose.gameObject.SetActiveNew(false);
            //CameraMgr.Inst.OnFov(0, 30f, 0);
            Entity ent = EntityManager.Inst.GetEnity(m_skillChoseHid);
            ent.SetColor(SKLL_BLUE);
        }

        private CMasterPlayer m_master;
        private int m_skillChoseHid;
        private Transform m_skillChose;
        private Transform m_skillCenter;
        private Transform m_skillDistance;
        private Transform m_skillDir;

        private SkillCsvData skillInfo;
        private Color SKLL_BLUE = new Color(0f, 0.145f, 0.807f, 0.5f);
        private Color SKLL_RED = new Color(0.345f, 0f, 0f, 0.5f);

        /// <summary>
        /// 是否取消施法
        /// </summary>
        private bool m_bSkillCancel;
        private int m_curSkillIndex;
        private eJoyStickEvent m_curSkillJoyStick = eJoyStickEvent.Up;

        //摇杆控制参数
        private Vector3 m_curVelocity;
        private Vector2 m_moveDir;
        private float m_sendMoveTime = 0;
        private float m_sendMoveInterval = 0;
        public bool m_isFirstJoyStick = true;

        public UIPanelJoyStick m_ui;
    }
}

