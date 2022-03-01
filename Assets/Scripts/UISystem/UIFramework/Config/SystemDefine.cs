using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    负责管理系统变量的定义
    1.系统常量
    2.全局性的方法
    3.系统的枚举类型
    4.委托的定义
 */

namespace UI.FrameWork
{
    #region  系统的枚举类型
    /// <summary>
    /// UI窗体（位置）类型
    /// </summary>
    public enum UIWindowType
    {
        //普通窗体
        Normal,
        //固定窗体
        Fixed,
        //弹出窗体
        PopUp,
    }

    /// <summary>
    /// UI窗体显示类型
    /// </summary>
    public enum UIWindowShowType
    {
        //普通窗体
        Normal,
        //反向切换(父子窗体的打开和关闭顺序 --- 堆栈)
        ReverseChange,
        //隐藏其它（子窗体完全覆盖父窗体）
        HideOther,
    }

    /// <summary>
    /// UI窗体透明度类型
    /// </summary>
    public enum UIWindowLucencyType
    {
        //完全透明，不能穿透
        Lucency,
        //半透明，不能穿透
        TransLucency,
        //低透明度，不能穿透
        ImPenetrable,
        //可以穿透
        Penetrable,
    }

    #endregion


    /// <summary>
    /// 定义框架的核心参数
    /// </summary>
    public class SystemDefine : MonoBehaviour
    {

    }
}