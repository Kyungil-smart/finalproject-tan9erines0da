using UnityEngine;
[System.Serializable]
public class QuestData :Basedata
{
    public int id;
    public string Descritction;
     
    public override void ApplyRowData(string[] Data)
    {
        this.id = int.Parse(Data[0]);
        this.uniqueId = Data[2];
        this.Descritction = Data[1];
    }
}
