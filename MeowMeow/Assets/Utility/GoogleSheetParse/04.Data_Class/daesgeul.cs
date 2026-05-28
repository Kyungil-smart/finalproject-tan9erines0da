using UnityEngine;
[System.Serializable]
public class daesgeul : Basedata
{
    public string sentence;

    public string emoji_sprite1;
    public int emoji_count1;

    public string emoji_sprite2;
    public int emoji_count2;

    public int atlas;
    public int dependent_ID;

    public string FormatType;
    public string emojiString;
    public override void ApplyRowData(string[] Data)
    {
        this.uniqueId = Data[0];
        sentence=Data[1];
        emoji_sprite1=Data[2];
        emoji_count1= int.TryParse(Data[3], out int temp) ? temp : 0;

        emoji_sprite2 =Data[4];

        emoji_count2= int.TryParse(Data[5], out int temp2) ? temp2 : 0;

        atlas = int.TryParse(Data[6], out int temp3) ? temp3 : 0;

        dependent_ID = int.TryParse(Data[7], out int temp4) ? temp4 : 0;

        FormatType=Data[9];
        Test(FormatType);
    }

    private void  Test(string _s)
    {
        if (_s == "") return;
       var data= _s.Split("&");
        foreach (var item in data)
        {
            Debug.Log(item);
        }
    }
}
