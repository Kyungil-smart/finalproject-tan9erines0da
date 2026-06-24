using UnityEngine;
[System.Serializable]
public class Hashtag : Basedata
{
    public string TagName;
    public override void ApplyRowData(string[] Data)
    {

        this.uniqueId = Data[0];
        this.TagName = Data[1].Replace("\\n", "\n");
    }
}
