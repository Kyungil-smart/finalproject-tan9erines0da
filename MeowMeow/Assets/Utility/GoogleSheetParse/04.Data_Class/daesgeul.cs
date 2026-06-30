using UnityEngine;
[System.Serializable]
public class daesgeul : Basedata
{
    public string sentence;
    public int atlas;
    public int dependent_ID;
  
    
    public override void ApplyRowData(string[] Data)
    {
        if(Data.Length==2)
        {
            Debug.Log("sds");
        }
        this.uniqueId = Data[0];
        string rawSentence=Data[1].Replace("\\n", "\n");
        sentence = rawSentence.Replace("<sprite", "\u200B<sprite");
        atlas = int.TryParse(Data[2], out int temp) ? temp : 0;
        dependent_ID=int.TryParse(Data[3], out int temp2) ? temp2 : 0;
    }

     
}
