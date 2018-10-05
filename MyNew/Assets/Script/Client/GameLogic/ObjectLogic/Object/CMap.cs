﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 用场景包含一切的方式
    /// </summary>
    public class CMap
    {
        public int m_mapId;
        public List<OBB> m_listBarrier = new List<OBB>();

        public CMap(int mapId)
        {
            m_mapId = mapId;
        }

        public void Create()
        {
            // 地图
            SceneManager.Inst.LoadScene(m_mapId, null);
            // 障碍数据
            SceneBarrierCsv csv = CsvManager.Inst.GetCsv<SceneBarrierCsv>((int)eAllCSV.eAC_SceneBarrier);
            List<SceneBarrierCsvData> list = new List<SceneBarrierCsvData>();
            csv.GetData(ref list, m_mapId);
            Debug.Log("障碍数量：" + list.Count);
            for(int i = 0; i < list.Count; i ++)
            {
                SceneBarrierCsvData data = list[i];
                if(data.shapeType == 1)
                {
                    Vector2 pos = new Vector2(data.vPos.x, data.vPos.z);
                    float dir = data.vDir.y;
                    Vector2 scale = new Vector2(data.vScale.x, data.vScale.z);
                    OBB obb = new OBB(pos, scale, dir);
                    m_listBarrier.Add(obb);
                }
            }
        }

        public void ExecuteFrame()
        {
            //CPlayerMgr.ExecuteFrame();
        }

        public void Enter(CCreature obj, Vector2 pos, float dir)
        {
            obj.Create(pos, dir);
        }

        public void Destroy()
        {

        }
    }
}
