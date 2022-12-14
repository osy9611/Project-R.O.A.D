using DesignTable;
using System;

public class StatusType<T> where T : class
{
    protected long[] statusData;
    public long[] StatusData
    {
        get => statusData;
        set => statusData = value;
    }

    public virtual void Init() { statusData = new long[Enum.GetValues(typeof(DesignEnum.Status_Id)).Length]; }
    
    //일반 또는 아이템 관련
    public virtual void IncreaseStatus(T entity, DesignEnum.Status_Id statusInfo, long value) { }
    public virtual void DecreaseStatus(T entity, DesignEnum.Status_Id statusInfo,long value) { }
    public virtual void ResetStatus(T entity, DesignEnum.Status_Id statusInfo) { }
    

    //Buff 관련
    public virtual void IncreaseStatus(T entity, DesignEnum.Status_Id statusInfo, DesignEnum.Buff_Type buffType, skill_effect_buffInfo info) { }
    public virtual void DecreaseStatus(T entity, DesignEnum.Status_Id statusInfo, DesignEnum.Buff_Type buffType, skill_effect_buffInfo info) { }
    public virtual void ResetStatus(T entity, DesignEnum.Status_Id statusInfo, DesignEnum.Buff_Type buffType) { }


    //카운터나 기타 Buff 관련
    public virtual void IncreaseStatus(T entity, DesignEnum.Status_Id statusInfo, DesignEnum.Buff_Type buffType, long value) { }
    public virtual void DecreaseStatus(T entity, DesignEnum.Status_Id statusInfo, DesignEnum.Buff_Type buffType, long value) { }

}
