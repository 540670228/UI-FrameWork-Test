using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    �������ϵͳ�����Ķ���
    1.ϵͳ����
    2.ȫ���Եķ���
    3.ϵͳ��ö������
    4.ί�еĶ���
 */

namespace UI.FrameWork
{
    #region  ϵͳ��ö������
    /// <summary>
    /// UI���壨λ�ã�����
    /// </summary>
    public enum UIWindowType
    {
        //��ͨ����
        Normal,
        //�̶�����
        Fixed,
        //��������
        PopUp,
    }

    /// <summary>
    /// UI������ʾ����
    /// </summary>
    public enum UIWindowShowType
    {
        //��ͨ����
        Normal,
        //�����л�(���Ӵ���Ĵ򿪺͹ر�˳�� --- ��ջ)
        ReverseChange,
        //�����������Ӵ�����ȫ���Ǹ����壩
        HideOther,
    }

    /// <summary>
    /// UI����͸��������
    /// </summary>
    public enum UIWindowLucencyType
    {
        //��ȫ͸�������ܴ�͸
        Lucency,
        //��͸�������ܴ�͸
        TransLucency,
        //��͸���ȣ����ܴ�͸
        ImPenetrable,
        //���Դ�͸
        Penetrable,
    }

    #endregion


    /// <summary>
    /// �����ܵĺ��Ĳ���
    /// </summary>
    public class SystemDefine : MonoBehaviour
    {

    }
}