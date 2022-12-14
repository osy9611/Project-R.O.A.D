using Sword;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Recovery : Item
{
    private DesignTable.Recovery_itemInfo itemInfo;
    public DesignTable.Recovery_itemInfo ItemInfo
    {
        get => itemInfo;
    }

    private float nowInterval = 0;
   
    public override void Init()
    {
        base.Init();
        itemInfo = DataHelper.Manager.Recovery_itemInfos.Get(itemId);
        if (itemInfo == null)
            return;
    }

    public override void InitDel()
    {
        if(itemInfo.item_Recovery_Time >0)
            Managers.Inven.ItemDel += CalcDurationTime;
        Managers.Inven.ItemDel += CalcCoolTime;
    }

    public override void Execute()
    {

    }

    public override void CalcDurationTime()
    {
        if (nowDurationTime <= itemInfo.item_Recovery_Time)
        {
            nowDurationTime += Time.deltaTime;
            CalcIntervalTime();
        }
        else
        {
            nowDurationTime = 0;
            Managers.Inven.ItemDel -= CalcDurationTime;
        }
    }

    public void CalcIntervalTime()
    {
        if (nowInterval <= 1)
        {
            nowInterval += Time.deltaTime;
        }
        else
        {
            nowInterval = 0;
        }
    }

    public override void CalcCoolTime()
    {
        if (nowCoolTime <= itemInfo.item_Recovery_CoolTime)
        {
            nowCoolTime += Time.deltaTime;
            isCoolTime = true;
        }
        else
        {
            nowCoolTime = 0;
            isCoolTime = false;
            Managers.Inven.ItemDel -= CalcCoolTime;
        }
    }

    public override float GetCoolTime()
    {
        return itemInfo.item_Recovery_CoolTime;
    }
}
