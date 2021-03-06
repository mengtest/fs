﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 帧同步游戏主类，负责最上层接口调用
    /// 1.连接帧服务器
    /// 2.创建房间
    /// 3.加入房间
    /// 4.选角色
    /// 5.点击准备，服务器都OK
    /// 6.同步玩家信息，开始游戏 > 加载场景
    /// 7.同步场景进度
    /// 8.场景加载完毕，进入游戏
    /// </summary>
    public class GameManager : Singleton
    {
        public static GameManager Inst;
        public GameManager() : base(true) { }

        public FspManager m_fspMgr;      // FSP管理器

        private bool m_bRunning;
        private float m_sendProgressTime;  // 发送进度间隔
        private bool m_bSendLoaded;        // 是否发送加载结束

        public override void Init()
        {
            FspNetRunTime.Inst = new FspNetRunTime();
            CFrameTimeMgr.Inst = new CFrameTimeMgr();

            PhysicsManager.Inst = new PhysicsManager();
            PhysicsManager.Inst.SetRolePush(true);

            CreatureProp.Init();
        }
        /// <summary>
        /// 服务器收到所有人准备后，开始游戏
        /// </summary>
        public void Start(SC_BattleInfo battleInfo)
        {
            m_fspMgr = new FspManager();
            m_fspMgr.Init();

            Debug.Log("开始加载场景，开始汇报场景进度");

            LoginModule loginHero = (LoginModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelLogin);
            loginHero.SetVisible(false);

            SelectHeroModule selectHero = (SelectHeroModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelSelectHero);
            selectHero.SetVisible(false);

            MainModule mainModule = (MainModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelMain);
            mainModule.SetVisible(false);

            JoyStickModule js = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
            js.SetVisible(true);
            HeadModule head = (HeadModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelHead);
            head.SetVisible(true);


            SetRandSeed(10);
            CMap map = CMapMgr.Create(1);
            map.Create();


            for(int i = 0; i < battleInfo.playerInfo.Count; i ++)
            {
                PlayerInfo playerInfo = battleInfo.playerInfo[i];
                int uid = (int)playerInfo.uid;
                if (EGame.m_openid.Equals(uid.ToString()))
                {
                    EGame.m_uid = uid;
                }
                CCreature master = CCreatureMgr.Create(EThingType.Player, uid);
                master.Create(playerInfo.heroIndex, uid.ToString(), new Vector2d(60, 60 + i * 4), FPCollide.GetVector(60));

                master.m_ai = new CCreatureAI(master, eAILevel.HARD);
                master.m_aiType = eAIType.Player;
                master.StartAi(false);
            }

           for (int i = 0; i < 1; i++)
           {
               CCreature test1 = CCreatureMgr.Create(EThingType.Player, 3000 + i);
               test1.Create(3, "测试" + i, new Vector2d(50 + i * 2, 60), FPCollide.GetVector(-220));
               test1.m_ai = new CCreatureAI(test1, eAILevel.EASY);
               test1.StartAi(true);
           }

           for (int i = 0; i < 1; i++)
           {
               CCreature test1 = CCreatureMgr.Create(EThingType.Player, 2000 + i);
               test1.Create(2, "测试" + i, new Vector2d(50 + i * 2, 60), FPCollide.GetVector(-220));
               test1.m_ai = new CCreatureAI(test1, eAILevel.EASY);
               test1.StartAi(true);
           }

           for (int i = 0; i < 1; i++)
           {
               CCreature test1 = CCreatureMgr.Create(EThingType.Player, 1000 + i);
               test1.Create(1, "测试" + i, new Vector2d(50 + i * 2, 60), FPCollide.GetVector(-220));
               test1.m_ai = new CCreatureAI(test1, eAILevel.EASY);
               test1.StartAi(true);
           }

            m_bRunning = true;
        }


        public void HandleLoadingProcess()
        {
            if (!m_bRunning)
                return;
            if (m_bSendLoaded)
                return;
            // 进度完成，直接再发送一次
            if (SceneManager.Inst.GetMapLoadProcess().fPercent >= 1.0f)
            {
                m_bSendLoaded = true;
                m_sendProgressTime = 1;
                // 进度加载完成，发送可以控制
                FspMsgStartControl msg = (FspMsgStartControl)NetManager.Inst.GetMessage(eNetMessageID.FspMsgStartControl);
                FspNetRunTime.Inst.SendMessage(msg);
                return;
            }
            // 场景加载进度不用帧指令,间隔发送
            m_sendProgressTime += Time.deltaTime;
            if(m_sendProgressTime >= 1)
            {
                m_sendProgressTime = 0;
                FspMsgLoadProgress msg = (FspMsgLoadProgress)NetManager.Inst.GetMessage(eNetMessageID.FspMsgLoadProgress);
                msg.m_progress = SceneManager.Inst.GetMapLoadProcess().fPercent;
                FspNetRunTime.Inst.SendMessage(msg);
            }
        }

        public void FixedUpdate()
        {
            FspNetRunTime.Inst.Update(0,0);
            if(m_fspMgr != null)
                m_fspMgr.FixedUpdate();

            HandleLoadingProcess();
        }

        public void AddFrameMsg(NetMessage msg)
        {
            if (m_fspMgr != null)
                m_fspMgr.AddServerFrameMsg(msg);
        }

        public override void Destroy()
        {
            if(FspNetRunTime.Inst != null)
            {
                FspNetRunTime.Inst.Destroy();
            }
        }

        public FspManager GetFspManager()
        {
            return m_fspMgr;
        }

        private System.Random m_rand = null;
        public System.Random m_clientRand = new System.Random();
        public void SetRandSeed(int nRandSeed)
        {
            //Debuger.LogWarning(string.Format("[{0}]GameManager::SetRandSeed::nRandSeed:{1}", GameManager.Instance.Context.currentFrameIndex, nRandSeed));
            m_rand = new System.Random(nRandSeed);
        }

        /// <summary>
        /// 获取随机数,只适用于帧同步的逻辑；本地表现层，模拟玩家的AI则不能使用
        /// </summary>
        public int GetRand(int nMin, int nMax, int from = 500)
        {
            int val = m_rand.Next(nMin, nMax);
            Debug.LogWarning("CGameMangerGetRand" + "当前帧：" + m_fspMgr.GetCurFrameIndex() + " 来自：from:" + from);
            return val;
        }

        public int GetClientRand(int nMin, int nMax, int from = 500)
        {
            int val = m_clientRand.Next(nMin, nMax);
            //Debuger.LogWarning("CGameManger", "GetRand", "当前帧：" + curFrame + " 来自：from:" + from);
            return val;
        }
    }
}
