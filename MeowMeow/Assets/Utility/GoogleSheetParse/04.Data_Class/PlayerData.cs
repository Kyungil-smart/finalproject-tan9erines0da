using UnityEngine;
[System.Serializable]
public class PlayerData : Basedata
{
    public string m_name;
    public float m_Hp;
    public float m_Attack;

    public override void ApplyRowData(string[] Data)
    {
        
        this.uniqueId = Data[0];
        this.m_name = Data[1];
        this.m_Hp = float.Parse(Data[2]);
        this.m_Attack = float.Parse(Data[3]);
    }
}
