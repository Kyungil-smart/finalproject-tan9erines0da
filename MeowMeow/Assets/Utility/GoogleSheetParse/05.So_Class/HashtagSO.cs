using UnityEngine;
[CreateAssetMenu(menuName = "SO/HashtagSO", fileName = "HashtagSO_")]
public class HashtagSO : SheetDataSO<Hashtag>
{
    public string GetNullID()
    {
        string nullId = "";
        foreach (var item in m_Data)
        {
            if(item.TagName == "(Null)")
            {
                nullId = item.uniqueId;
            }
        }
        return nullId;
    }
}
