using UnityEngine;
[System.Serializable]

public class VirtualUserProfile : Basedata
{
     public string UserName;
     public int Profile_Icon;

    public override void ApplyRowData(string[] Data)
    {

        this.uniqueId = Data[0];
        this.UserName=Data[1];
        this.Profile_Icon=int.Parse(Data[2]);


    }
}
