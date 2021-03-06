﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UI;

namespace Roma
{ 

    public enum eImageEffect
    {
        normal,//正常颜色
        halfAlpha,//rgba乘以0.75
    }

    /// <summary>
    /// 增加UI中经常使用的文字标签，进度，标示等。
    /// </summary>
    public struct UIItemTitle
    {
        // 图片
        public const string imgIcon = "icon";  //小图
        public const string imgIcon_bg = "bg_item";//物品背景等级设置
        public const string imgImage = "image";//大图
        public const string imgLock = "lock";//锁图标
        public const string txtStarLevel = "star";  //星级
        public const string imgRate_bg = "bg_rate"; // 武器阶数
        // 文本
        public const string txtCD = "cd";     // 通用组件的值。
        public const string txtId = "id";     // 通用组件的值。
        public const string txtVal = "val";     // 通用组件的值。
        public const string txtName = "name";   // 玩家名称 
        public const string txtLevel = "level"; // 可表示玩家等级，道具等级等。
        public const string txtCount = "count"; // 可表示道具数量等。
        public const string txtRank = "rank"; // 可表示道具品质。
        public const string txtSex = "sex";
        public const string txtVip = "vip";
        public const string txtSite = "site";   // 好友所在位置等
        // txtTag0和txtTag1用于替换相互排斥的一组图标，如上阵/下阵标记
        public const string markTag0 = "tag_0";   // 用于传入参数true时显示
        public const string markTag1 = "tag_1";   //用于传入参数false时显示
        public const string txtInfo = "info";  //信息描述
        public const string txtDescribe = "describe";   //物品描述

        // 商城
        public const string txt = "txt";
        public const string Add = "add";
        public const string Slider = "Slider";
        public const string txtText = "text";
        public const string txtHp = "hp";
        public const string txtMp = "mp";
        public const string txtExp = "exp";
        public const string txtPre = "pre";   // 可用于原价,以前属性等
        public const string txtCur = "cur";   // 可用于现价，当前属性等
        public const string txtNext = "next"; // 升级后的属性
        public const string txtIntro = "intro";   // 简介
        public const string txtAward = "award";   // 奖励
        public const string txtCaptain = "captain";   // 
        public const string txtAtk = "atk";   // 战力
        public const string txtTitle = "title";   // 职务
        public const string txtDonate = "donate";   // 帮贡
        public const string txtOnline = "online";   // 在线
        public const string txtCondi = "condi";   // 条件
        public const string txtReach = "reach";     // 任务，目标等达成情况
        public const string txtPrice = "price";

        // 标示
        public const string markBg = "bg";  // 带样式的列表，UI层控制item背景隔行显示
        public const string markBig = "big";  // 
        public const string markCaptain = "captain";// 队长标示

        // 滑动条
        public const string sbProgress = "progress";// 一般进度条

        // 开关
        public const string toggle = "toggle";// 一般开关

        // 下拉框
        public const string popup = "popup";    // 弹出菜单

        // 按钮
        public const string btn = "btn";
        public const string btnBuy = "btn_buy";             // 购买
        public const string btnSW = "btn_sw";
        public const string btnTransfer = "btn_transfer";   // 传送
        public const string btnQuicken = "btn_quicken";   // 加速
        public const string btnFindPath = "btn_find_path";   // 自动寻路
        public const string btnJoin = "btn_join";   // 加入
        public const string btnGet = "btn_get";   // 获取
        public const string btnUp = "btn_up";   // 升级
        // 列表
        public const string list = "list";

        public const string goodsItem = "goods";

        // 人物属性
        public const string txtLv = "lv";
        public const string txtFamily = "family";
        public const string txtvEarth = "earth";
        public const string txtvWater = "water";
        public const string txtvFire = "fire";
        public const string txtvWind = "wind";

        public const string txtBlood        = "blood";      //用于加点人物界面血量显示 
        public const string txtBloodAdd     = "bloodadd";
        public const string txtAttack       = "attack";
        public const string txtAttackAdd    = "attackadd";
        public const string txtDefense      = "defense";
        public const string txtDefenseAdd   = "defenseadd";
        public const string txtSpeed        = "speed";
        public const string txtSpeedAdd     = "speedadd";
        public const string txtAttrTotal    = "total";
    }
    public class GoodsItem
    {
        public object parameter;    // 异步列表传递参数时，选择使用的参数

        public string name = "";
        public string icon = "";
        public string level = "";   // 强化等级
        public string rank = "";    // 品质
        public string describe = "";    //物品描述
        public int type = 0;        // 扩充的，道具类型，可能用于显示或隐藏特效用
        public int starLevel = -1;        //星级
        public int price = 0;           //价格
        public bool isLock;
        public bool isChoice = false;
        // 服务器传过来的
        //public UIAtlasID iconId = UIAtlasID.icon_item;    // iResID 对应物品的资源ID
        public UInt64 gid;  // 对应 ullGID 物品的GID
        public string count = "";       // 对应 dwNum 物品个数
        public byte bBind; // 绑定：0-未绑定，1-已绑定
        public byte bType; // 物品类别，ITEM_TYPE_
        public int gemHoleNum = 0;
        public long time = 0;      //计时可用
        public int currency = 0;        //货币类型
        public int bg = 0;
        public string rate;

        public GoodsItem()
        {

        }
    }
    /// <summary>
    /// UIItem：编写UI接口的辅助类。
    /// 抽出UI层界面的逻辑通用组件，简化UI层代码。
    /// 1，用于给item子节点中的控件赋值。
    /// 2，局部需要显示大量组件时，只需挂载这些组件的父节点，做为item使用，减少挂载组件的个数。
    /// </summary>
    public class UIItem
    {
        /// <summary>
        /// 一般用于动态设置在特效之上的UI元素的层级
        /// 比如打开宝箱特效之上，还要显示的item
        /// order = 当前UI的order + 特效层级 + 1
        /// </summary>
        public static void SetOrder(GameObject item, int order)
        {
            Canvas canvas = item.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = item.AddComponent<Canvas>();
            }
            GraphicRaycaster gr = item.GetComponent<GraphicRaycaster>();
            if (gr == null)
            {
                gr = item.AddComponent<GraphicRaycaster>();
            }
            if (!canvas.overrideSorting)
                canvas.overrideSorting = true;
            canvas.sortingOrder = order;
        }
        /// <summary>
        /// 设置道具，返回icon对象
        /// </summary>
        public static GameObject SetGoodsItem(Transform item, GoodsItem itemVal)
        {
            if (null == item || null == itemVal)
            {
                return null;
            }
            UIItem.SetText(item, UIItemTitle.txtName, itemVal.name);
            
            //物品描述
            if (itemVal.describe != "")
                UIItem.SetText(item, UIItemTitle.txtDescribe, itemVal.describe);
            // 图标
            //GameObject iconObject = UIItem.SetImage(item, UIItemTitle.imgIcon, itemVal.iconId, itemVal.icon, true);
            GameObject iconObject = UIItem.GetChild(item, UIItemTitle.imgIcon).gameObject;
            if(String.IsNullOrEmpty(itemVal.icon))
            {
                iconObject.SetActiveNew(false);
            }
            else
            {
                UnityEngine.Profiling.Profiler.BeginSample("LoadImage");
                UIItem.SetImage(item, UIItemTitle.imgIcon, int.Parse(itemVal.icon));
                if(!iconObject.ActiveSelfNew())
                    iconObject.SetActiveNew(true);
                UnityEngine.Profiling.Profiler.EndSample();
                //UIButton.Get(iconObject);
            }
            // 强化等级
            //if (itemVal.level != "" && IsInt(itemVal.level) && int.Parse(itemVal.level) > 0)
                UIItem.SetText(item, UIItemTitle.txtLevel, itemVal.level);
                GameObject tmpBgLevel = UIItem.GetBtn(item, "bg_level");
            if(tmpBgLevel!=null)
            {
                if (itemVal.rate == null) tmpBgLevel.SetActiveNew(false);
                else tmpBgLevel.SetActiveNew(true);
            }
            //else
            //    UIItem.SetText(item, UIItemTitle.txtLevel, "");

            // 数量
            if (itemVal.count != "" && IsInt(itemVal.count) && int.Parse(itemVal.count) >= 0)
                UIItem.SetText(item, UIItemTitle.txtCount,itemVal.count);
            else
                UIItem.SetText(item, UIItemTitle.txtCount, "");

            if (itemVal.price >= 0)
                UIItem.SetText(item, UIItemTitle.txtPrice, itemVal.price.ToString());
            // 品质
            if (itemVal.rank != "") UIItem.SetText(item, UIItemTitle.txtRank, itemVal.rank);
            //星级
            if (itemVal.starLevel >= 0)
                UIItem.SetStarLevel(item, UIItemTitle.txtStarLevel, itemVal.starLevel);

            // 宝石孔
            UIItem.SetText(item, "gem_count", itemVal.gemHoleNum + "/6");
    
            if(itemVal.currency>0)
            {

            }
            // 还有一个类型，用于特效预留
            return iconObject;
        }

        /// <summary>
        /// 设置按钮变灰 及可点击
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isGray">true变灰</param>
        /// <param name="isActive">true 可点击</param>
        public static void SetGray(GameObject obj, bool isGray, bool isActive=false)
        {
            if (isActive)
            {
                UIGray.Get(obj).SetGrayButActive(isGray);
            }
            else
            {
                UIGray.Get(obj).SetGray(isGray,isActive);
            }
        }

        /// <summary>
        /// 设置动态下载的图片，直接通过IconCsv加载
        /// </summary>
        public static GameObject SetImage(Transform item, string iconTitle, int iconId)
        {
            GameObject iconObject = item.FindChild(iconTitle).gameObject;
            if (iconId == 0)
            {
                iconObject.SetActiveNew(false);
                return iconObject;
            }
            else
            {
                iconObject.SetActiveNew(true);
            }
            return iconObject;
        }

        public static GameObject SetImage(GameObject item, int iconId)
        {
            Image image = item.GetComponent<Image>();
            if(image != null)
            {
                SetImage(image, iconId);
            }
            return item;
        }

        public static GameObject SetImage(Transform item, string iconTitle, string iconName)
        {
            GameObject iconObject = item.FindChild(iconTitle).gameObject;
            int iconId;
            if (int.TryParse(iconName, out iconId))
            {
                if (iconId == 0)
                {
                    iconObject.SetActiveNew(false);
                    return iconObject;
                }
                else iconObject.SetActiveNew(true);
            }
            //UILoadImage comIcon = UILoadImage.Get(iconObject);
            //comIcon.Load(iconId);
            return iconObject;
        }

        /// <summary>
        /// 设置动态下载的图片，直接通过IconCsv加载
        /// </summary>
        public static void SetImage(Image image, string iconName)
        {
            int iconId;
            if(int.TryParse(iconName, out iconId))
            {
                if (iconId == 0)
                {
                    image.gameObject.SetActiveNew(false);
                    return;
                }
                else image.gameObject.SetActiveNew(true);
                //UILoadImage comIcon = UILoadImage.Get(image.gameObject);
                //comIcon.Load(iconId);
            }
        }

        /// <summary>
        /// 设置动态下载的图片，直接通过IconCsv加载
        /// </summary>
        public static void SetImage(Image image, int iconId)
        {
            if (iconId == 0)
            {
                image.gameObject.SetActiveNew(false);
                return;
            }
            else image.gameObject.SetActiveNew(true);
            //UILoadImage comIcon = UILoadImage.Get(image.gameObject);
            //comIcon.Load(iconId);
        }

        /// <summary>
        /// 设置动态下载的图片,图集通过资源配置UIIconID加载
        /// </summary>
        //public static GameObject SetImage(Transform item, string iconTitle, UIAtlasID iconID, string val, bool isHandle)
        //{
        //    if (val.Equals(""))
        //    {
        //        item.FindChild(iconTitle).gameObject.SetActiveNew(false);
        //        return null;
        //    }
        //    GameObject iconObject = item.FindChild(iconTitle).gameObject;
        //    iconObject.SetActiveNew(true);
        //    UILoadImage comIcon = UILoadImage.Get(iconObject);
        //    comIcon.Load((uint)iconID, val);
        //    return iconObject;
        //}

        /// <summary>
        /// 指定图集的，设置动态下载的图片
        /// </summary>
        //public static GameObject SetImage(Transform item, string iconTitle, UIAtlasID iconID, string val, bool nativeSize = false)
        //{
        //    GameObject iconObject = item.FindChild(iconTitle).gameObject;
        //    iconObject.SetActiveNew(true);
        //    UILoadImage comIcon = UILoadImage.Get(iconObject);
        //    comIcon.m_nativeSize = nativeSize;
        //    comIcon.Load((uint)iconID, val);
        //    return iconObject;
        //}
        /// <summary>
        /// 设置动态下载的图片
        /// </summary>
        //public static GameObject SetImage(Transform item, string iconTitle, UIAtlasID iconID, string val)
        //{
        //    GameObject iconObject = item.FindChild(iconTitle).gameObject;
        //    UILoadImage comIcon = UILoadImage.Get(iconObject);
        //    comIcon.Load((uint)iconID, val);
        //    return iconObject;
        //}

        /// <summary>
        /// 设置子对象CD
        /// </summary>
        public static GameObject SetCD(Transform item, string cdTitle, float cur, float max)
        {
            Transform iconTrans = item.FindChild(cdTitle);
            if (null == iconTrans)
            {
                //Debug.Log(item.name + " 下不存在:" + textTitle);
                return null;
            }
            Image image = iconTrans.GetComponent<Image>();
            if (null == image)
            {
                //Debug.LogError(iconTrans.name + " 不存在UILabel组件");
                return null;
            }
            // 比较道具的显示数量 大于1才显示的逻辑在这里处理
            image.fillAmount = cur/max;
            return image.gameObject;
        }

        public static GameObject SetText(GameObject item, string textTitle, string val)
        {
            return SetText(item.transform, textTitle, val);
        }

        public static void SetRawImage(Transform item, string imageTitle, int resId, UIBase refUI = null)
        {
            if (item == null)
                return;
            Transform image = item.FindChild(imageTitle);
            SetRawImage(image, resId, refUI);
        }

        public static void SetRawImage(Transform item, int resId, UIBase refUI = null)
        {
            if (item == null)
                return;
            RawImage label = item.GetComponent<RawImage>();
            if (label == null)
                Debug.LogError("rawImage is null : " + item);

            if (resId == -1)
            {
                label.enabled = false;
                return;
            }

            TextureManager.SetImage(resId, label);
            
        }

        /// <summary>
        /// 设置图标为动态
        /// </summary>
        /// <param name="item"></param>
        /// <param name="resId"></param>
        public static void SetUIMatAnimation(Transform item, int resId, bool enableAnim = true)
        {
            // UIMatAnimation matAnim = UIMatAnimation.Get(item);
            // if (null == matAnim) return;

            // ResInfo resInfo = ResInfosResource.GetResInfo(resId);
            // if (null != resInfo)
            //     matAnim.SetAnimTexture(resInfo.strName);

            // matAnim.EnableAnim(enableAnim);
        }

        public static InputField SetInput(Transform item, string textTitle, string val)
        {
            if (item == null)
                return null;
            Transform iconTrans = item.FindChild(textTitle);
            if (null == iconTrans)
            {
                //Debug.Log(item.name + " 下不存在:" + textTitle);
                return null;
            }
            iconTrans.gameObject.SetActiveNew(true);
            InputField label = iconTrans.GetComponent<InputField>();
            label.text = val;
            return label;
        }

        /// <summary>
        /// 设置子对象文本
        /// </summary>
        public static GameObject SetText(Transform item, string textTitle, string val)
        {
            if (item == null)
                return null;
            Transform iconTrans = item.FindChild(textTitle);
            if (null == iconTrans)
            {
                return null;
            }
            iconTrans.gameObject.SetActiveNew(true);
            Text label = iconTrans.GetComponent<Text>();
            if (null == label)
            {
                return null;
            }
            label.text = val;
            return label.gameObject;
        }

        public static GameObject SetRichText(Transform item, string textTitle, string val)
        {
            if (item == null)
                return null;
            Transform iconTrans = item.FindChild(textTitle);
            if (null == iconTrans)
            {
                return null;
            }
            iconTrans.gameObject.SetActiveNew(true);
            // UIRichText label = iconTrans.GetComponent<UIRichText>();
            // if (null == label)
            // {
            //     return null;
            // }
            // label.text = val;
            //return label.gameObject;
            return null;
        }

        public static void SetTxtLength(Transform item, string title,string txt,float width_margin)
        {
            Transform txt_trans = item.FindChild(title);
            if (null == txt_trans)
            {
                return ;
            }
            Text text = txt_trans.GetComponent<Text>();
            if (null == text)
            {
                return ;
            }
            text.gameObject.SetActiveNew(true);
            text.text = txt;
            RectTransform rt_item_bg = GetChild(item, "bg") as RectTransform;
            if (null != rt_item_bg)
            {
                rt_item_bg.sizeDelta = new Vector2(text.preferredWidth + width_margin, rt_item_bg.sizeDelta.y);
            }
        }

        public static void SetTxtColor(Transform item, string title, string txt, Color color)
        {
            Transform txt_trans = item.FindChild(title);
            if (null == txt_trans)
            {
                return;
            }
            Text text = txt_trans.GetComponent<Text>();
            if (null == text)
            {
                return;
            }
            text.gameObject.SetActiveNew(true);
            text.text = txt;
            text.color = color;
        }
        public static void SetTxtColor(Transform item,string txt, Color color)
        {
            
            Text text = item.FindChild(txt).GetComponent<Text>();
            if (null == text)
            {
                return;
            }
            text.gameObject.SetActiveNew(true);
            text.color = color;
        }
        
        /// <summary>
        /// 设置星级
        /// </summary>
        public static void SetStarLevel(Transform item, string textTitle, int level)
        {
            Transform starTrans = item.FindChild(textTitle);
            if (starTrans == null)
                return;
            Text label = starTrans.GetComponent<Text>();
            if (null != label)
            {
                label.text = level.ToString();
            }
        }
        /// <summary>
        /// 设置是否被选中
        /// </summary>
        public static void SetChoice(Transform item, bool isChoice)
        {
            Toggle choice = item.GetComponent<Toggle>();
            if (choice == null)
                return;
            choice.isOn = isChoice;
        }

        public static bool GetChoice(Transform item)
        {
            Toggle choice = item.GetComponent<Toggle>();
            if (choice == null)
                return false;
            return choice.isOn;
        }
        public static GameObject m_choice;
        public static void SetItemChoice(Transform parent)
        {
            if (m_choice != null) m_choice.SetActiveNew(false);
            m_choice = UIItem.GetBtn(parent, "choice");
            m_choice.SetActiveNew(true);
        }

        public static void SetItemChoice(Transform parent,bool bShow)
        {
            if (m_choice != null) m_choice.SetActiveNew(bShow);
            m_choice = UIItem.GetBtn(parent, "choice");
            m_choice.SetActiveNew(bShow);
        }

        /// <summary>
        /// 获取子对象文本
        /// 
        public static string GetText(Transform item, string textTitle)
        {
            Transform iconTrans = GetBtn(item, textTitle).transform;
            if (null == iconTrans)
            {
                //Debug.Log(item.name + " 下不存在:" + textTitle);
                return null;
            }
            Text label = iconTrans.GetComponent<Text>();
            if (null == label)
            {
                //Debug.LogError(iconTrans.name + " 不存在UILabel组件");
                return null;
            }
            return label.text;
        }
        /// <summary>
        /// 设置子对象标示：显示|隐藏
        /// </summary>
        public static void SetMark(Transform item, string markTitle, bool isShow)
        {
            Transform iconTrans = item.FindChild(markTitle);
            if (null == iconTrans)
            {
                //Debug.Log(item.name + " 下不存在:" + markTitle);
                return;
            }
            iconTrans.gameObject.SetActiveNew(isShow);
        }
        public static GameObject GetBtn(Transform item, int index)
        {
            return GetBtn(item, index.ToString());
        }
        public static GameObject GetBtn(Transform item, string btnTitle)
        {
            Transform btnTrans = item.FindChild(btnTitle);
            if(null == btnTrans)
            {
                return null;
            }
            //btnTrans.gameObject.SetActiveNew(true);
            return btnTrans.gameObject;
        }
        public static GameObject GetList(Transform item, string listTitle)
        {
            Transform trans = item.FindChild(listTitle);
            if (trans == null)
            {
                return null;
            }
            else
            {
                GameObject btn = item.FindChild(listTitle).gameObject;
                return btn;
            }
        }

        /// <summary>
        /// 设置变灰
        /// </summary>
        /// <param name="bTrue"></param>
        /// <param name="obj"></param>
        public static void SetItemIsGray(bool bTrue, GameObject obj)
        {
            UIGray gray3 = obj.GetComponent<UIGray>();
            if (gray3 == null)
                gray3 = obj.AddComponent<UIGray>();
            gray3.gameObject.SetActiveNew(bTrue);
        }

        public static void SetItemIsGray(Transform item, string name, bool isGray)
        {
            Transform trans = item.FindChild(name);
            if(trans != null)
            {
                SetItemIsGray(isGray, trans.gameObject);
            }
        }

        /// <summary>
        /// 设置子对象进度条
        /// </summary>
        public static void SetProgress(Transform item, string proTitle, float cur, float max)
        {
            Slider proTrans = item.FindChild(proTitle).GetComponent<Slider>();
            if (null == proTrans)
            {
                //Debug.Log(item.name + " 下不存在:" + proTitle);
                return;
            }
            SetSelfProgress(proTrans, cur, max);
        }

        public static void SetSelfProgress(Slider self, float cur, float max)
        {
            if (max <= 0)
            {
                self.GetComponent<Slider>().value = 0f;
            }
            else
            {
                self.GetComponent<Slider>().value = cur / max;
            }
            // 滑动条子对象包含val
            SetText(self.transform, UIItemTitle.txtVal, ((int)cur).ToString() + "/" + ((int)max).ToString());
        }

        public static void SetSelfProgress(Slider self, float cur, float max, string text)
        {
            if (max <= 0)
            {
                self.GetComponent<Slider>().value = 0f;
            }
            else
            {
                self.GetComponent<Slider>().value = cur / max;
            }
            // 滑动条子对象包含val
            SetText(self.transform, UIItemTitle.txtVal, text);
        }

        /// <summary>
        /// 设置自身进度条
        /// </summary>
        public static void SetSelfProgressPct(Slider self, float cur)
        {
            self.GetComponent<Slider>().value = cur / 100;
            // 滑动条子对象包含val
            SetText(self.transform, UIItemTitle.txtVal, ((int)cur).ToString() + "%");
        }

        public static Transform GetGoodsItem(Transform item)
        {
            Transform goods = item.FindChild(UIItemTitle.goodsItem);
            return goods;
        }

        public static Transform GetChild(Transform parent, string name)
        {
            return parent.FindChild(name);
        }

        /// <summary>
        /// 创建item，用于动态列表,但是每个列表item高度不一样，这里就不控制高度了
        /// </summary>
        public static Transform CreateItem(Transform parent, GameObject itemCopy, int index)
        {
            Transform item = GetItem(parent, index);
            if (null == item)
            {
                item = ((GameObject)GameObject.Instantiate(itemCopy)).transform;
                item.name = index.ToString();
                item.SetParent(parent);
            }
            item.localScale = Vector2.one;
        
            item.gameObject.SetActiveNew(true);
            return item;
        }

        public static Transform CreateItem(Transform parent, GameObject itemCopy, string name)
        {
            Transform item = GetChild(parent, name);
            if (null == item)
            {
                item = ((GameObject)GameObject.Instantiate(itemCopy)).transform;
                item.name = name;
                item.SetParent(parent);
            }
            //item.localPosition = Vector2.zero;
            item.localScale = Vector2.one;
            item.gameObject.SetActiveNew(true);
            return item;
        }

        public static void InitList(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                parent.GetChild(i).gameObject.SetActiveNew(false);
            }
        }
        public static void ClearList(Transform parent)
        {
            if (parent == null)
                return;
        
            for (int i = 0; i < parent.childCount; i++)
            {
                parent.GetChild(i).gameObject.SetActiveNew(false);
            }
        }

        public static void DestoryList(Transform parent)
        {
            if (parent == null)
                return;

            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject.Destroy(parent.GetChild(i).gameObject);
            }
        }

        ///// <summary>
        ///// 获取item，用于静态列表
        ///// </summary>
        public static Transform GetItem(Transform parent, int index)
        {
            Transform item = parent.FindChild(index.ToString());
            if (null == item)
            {
                return null;
            }
            //item.gameObject.SetActiveNew(true);
            return item;
        }

        /// <summary>
        /// 通过对象名称获取索引
        /// </summary>
        /// <returns></returns>
        public static int GetIndex(GameObject uiObject)
        {
            if (null == uiObject)
            {
                //Debug.LogError("ui object is null in GetIndex");
                return 0;
            }
            string index = UIEventListener.Get(uiObject).parameter.ToString();
            if (!IsInt(index))
            {
                //Debug.LogError("index not is int");
                return 0;
            }
            return int.Parse(index);
        }
        private static bool IsInt(string inString)
        {
            int a = 0;
            return int.TryParse(inString, out a);
        }
        public static void SetStarNum(Transform parent, int num)
        {
            Transform star = GetChild(parent, "star");
            bool isShow;
            for (int i = 0; i < 7; i++)
            {
                if (i < num)
                    isShow = true;
                else
                    isShow = false;
                GameObject item = GetBtn(star, i);
                if(item != null)
                    item.SetActiveNew(isShow);
            }
        }
        public static void SetItemUnlocking(Transform parent, bool isShow)
        {
            GetBtn(parent, "unlocking").SetActiveNew(isShow);
        }
        public static void SetItemUnlocked(Transform parent, bool isShow)
        {
            GetBtn(parent, "unlocked").SetActiveNew(isShow);
        }

        public static void SetItemLock(Transform parent, bool isShow)
        {
            GetBtn(parent, "lock").SetActiveNew(isShow);
        }
        public static void SetRedPoint(Transform parent, bool isRed)
        {
            UIItem.GetBtn(parent, "red").SetActiveNew(isRed);
        }
        public static void SetSiblingIndex(Transform parent, string name, int index)
        {
            Transform child = parent.FindChild(name);
            if (child != null)
            {
                child.GetComponent<RectTransform>().SetSiblingIndex(index);
            }
        }
        public static void SetActiveNew(Transform parent, string name, bool active)
        {
            Transform child = parent.FindChild(name);
            if (child != null)
                child.gameObject.SetActiveNew(active);
        }

        public static void SetSliderValue(Transform parent,string name,  float slider_value)
        {
            Image slider = UIItem.GetChild(parent, name).GetComponent<Image>();
            if (slider != null)
            {
                slider.fillAmount = slider_value;
            }
        }
        
        public static void ShowArrow(GameObject parent, bool show)
        {
            //ArrowEffect.Get(parent).Show(show);

            //改为弱引导
            //if(show)
                //LayoutMgr.Inst.GetLogicModule<WeakGuideModule>(LayoutName.S_WeakGuide).AddGuide((int)eWeakGuideId.AddPoiont);
        }

        public static void SetItemMask(Transform parent, string txt)
        {
            Transform item = UIItem.GetChild(parent, "fg_mask");
            if (txt == null)
                item.gameObject.SetActiveNew(false);
            else
            {
                item.gameObject.SetActiveNew(true);
                UIItem.SetText(item, "txt", txt);
            }
        }
        //设置图片
        public static void SetImageEffect(GameObject obj, eImageEffect effect)
        {
            Color m_color = Vector4.one;
            switch (effect)
            {
                case eImageEffect.halfAlpha:
                    m_color=Vector4.one*0.75f;
                    break;
            }
            Image image = obj.GetComponent<Image>();
            image.color=m_color;
        }
        public static string InterceptString(string str, int num)
        {
            if (str.Length > num)
            {
                return str.Substring(0, num)+"...";
            }
            return str;
        }
        //备用，如果策划加了零时需求，可以拿来用
        public static void SetItemLevel(Transform parent,string level)
        {
            UIItem.SetText(parent, UIItemTitle.txtLevel, level);
        }
        public static void SetItemCondition(Transform parent,int icon_condition)
        {
            UIItem.SetImage(parent, "icon_condition", icon_condition);
        }
    
        public static void SetItemIsOpen(Transform parent,bool open)
        {
            UIItem.GetBtn(parent, SSwitchBtn.s_open).SetActiveNew(open);
            UIItem.GetBtn(parent, SSwitchBtn.s_close).SetActiveNew(!open);
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// 获取子节点GameObject
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childPath"></param>
        /// <returns></returns>
        public static GameObject GetChildObj(Transform parent, string childPath)
        {
            if(null == parent)
            {
                Debug.LogWarning("GetChildObj parent is null !");
                return null;
            }
            Transform childTrans = parent.FindChild(childPath);
            if (null == childTrans)
                return null;
            else
                return childTrans.gameObject;
        }
        /// <summary>
        /// 获取子节点上的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="childPath"></param>
        /// <returns></returns>
        public static T GetChildComp<T>(Transform parent, string childPath) where T : Component
        {
            if (null == parent)
            {
                Debug.LogWarning("GetChildComp parent is null !");
                return null;
            }
            Transform childTrans = parent.FindChild(childPath);
            if (null == childTrans)
                return null;
            else
                return childTrans.GetComponent<T>();
        }
        /// <summary>
        /// 从父级节点上寻找组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static T GetParentComp<T>(Transform trans) where T : Component
        {
            if (null == trans || null == trans.parent)
                return null;

            Transform parent = trans.parent;
            T comp = null;

            while (null != parent)
            {
                comp = parent.GetComponent<T>();
                if (null != comp)
                    return comp;
                else
                    parent = parent.parent;
            }
            return null;
        }

        /// <summary>
        /// 获得该节点所在的UI
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static UIBase GetParentUI(Transform item)
        {
            if (null == item || null == item.parent)
                return null;

            Transform parent = item.parent;
            UIBase uiComp = null;

            while(null != parent)
            {
                uiComp = parent.GetComponent<UIBase>();
                if (null != uiComp)
                    return uiComp;
                else
                    parent = item.parent;
            }
            return null;
        }

        public enum eItemAlignType
        {
            Left,
            Center,
            Top,
            VCenter
        }

        /// <summary>
        /// 因为不再使用active, 用于代替系统的GridLayoutGroup组建居中,注意itemParent的X坐标为0
        /// </summary>
        public static void SetItemAlign(eItemAlignType type, Transform itemParent)
        {
            float width = 0;
            float height = 0;
            int index = 0;
            if (type == eItemAlignType.VCenter || type == eItemAlignType.Top)
            {
                for (int i = 0; i < itemParent.childCount; i++)
                {
                    RectTransform item = (RectTransform)itemParent.FindChild(i.ToString());
                    if (item == null)
                        continue;
                    if (item.localScale == Vector3.zero)
                        continue;
                    item.localPosition = new Vector3(item.localPosition.x, -index * item.sizeDelta.y, 0);

                    height = item.sizeDelta.y * 0.5f * index;
                    index++;
                    //itemParent.localPosition = new Vector3(itemParent.localPosition.x, height, 0);
                }
                if (type == eItemAlignType.VCenter)
                {
                    itemParent.localPosition = new Vector3(itemParent.localPosition.x,  height, 0);
                }
            }
            else
            {
                for (int i = 0; i < itemParent.childCount; i++)
                {
                    RectTransform item = (RectTransform)itemParent.FindChild(i.ToString());
                    if (item == null)
                        continue;
                    if (item.localScale == Vector3.zero)
                        continue;
                    item.localPosition = new Vector3(index * item.sizeDelta.x, 0, 0);

                    width = item.sizeDelta.x * 0.5f * index;
                    index++;
                }
                if (type == eItemAlignType.Center)
                {
                    itemParent.localPosition = new Vector3(-1 * width, itemParent.localPosition.y, 0);
                }
            }
           
        }

        /// <summary>
        /// 获取或创建Component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static T GetOrCreateComp<T>(Transform trans) where T : Component
        {
            if(null == trans)
            {
                Debug.LogWarning("GetOrCreate trans is null !");
                return null;
            }

            T comp = trans.GetComponent<T>();
            if (null == comp)
                comp = trans.gameObject.AddComponent<T>();

            return comp;
        }
        public static T GetOrCreateComp<T>(Transform trans, string childPath) where T : Component
        {
            if (null == trans)
            {
                Debug.LogWarning("GetOrCreate trans is null !");
                return null;
            }
            return GetOrCreateComp<T>(trans.FindChild(childPath));
        }

        /// <summary>
        /// 获取Component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static T GetComponent<T>(Transform trans) where T : Component
        {
            if (null == trans)
            {
                Debug.LogWarning("GetComponent trans is null !");
                return null;
            }
            return trans.GetComponent<T>();
        }
        public static T GetComponent<T>(Transform trans, string childPath) where T : Component
        {
            if (null == trans)
            {
                Debug.LogWarning("GetComponent trans is null !");
                return null;
            }
            return GetComponent<T>(trans.FindChild(childPath));
        }

        /// <summary>
        /// 获取或实例化子节点
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childPath"></param>
        /// <param name="sampleObj"></param>
        /// <returns></returns>
        public static Transform GetOrInstantChild(Transform parent, string childPath, GameObject sampleObj)
        {
            if (null == parent)
                return null;

            Transform childTrans = parent.FindChild(childPath);
            if(null == childTrans)
            {
                if (null == sampleObj)
                    return null;
                else
                {
                    GameObject go = GameObject.Instantiate(sampleObj);
                    childTrans = go.transform;
                    childTrans.SetParent(parent);
                    childTrans.localRotation = Quaternion.identity;
                    childTrans.localScale = Vector3.one;
                    go.SetActiveNew(true);
                }
            }
            return childTrans;
        }

        /// <summary>
        /// 获得节点路径
        /// </summary>
        /// <param name="item"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static string GetHierarchyPath(Transform item, int depth = 0)
        {
            if (null == item)
                return "../";

            if (depth <= 0)
                depth = int.MaxValue;

            string path = item.name;
            Transform parent = item.parent;
            while(null != parent && depth-- > 0)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }
    }
    public struct SSwitchBtn
    {
        public const string s_open = "open";
        public const string s_close = "close";
        public const bool opening = true;//表示正打开着
        public const bool closing = false;//表示正关闭着
    }
}