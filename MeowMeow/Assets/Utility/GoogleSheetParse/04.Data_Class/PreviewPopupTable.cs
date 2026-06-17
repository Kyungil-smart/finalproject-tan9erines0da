using UnityEngine;

[System.Serializable]
public class PreviewPopupTable : Basedata
{
    [SerializeField] private string m_Name;
    [SerializeField] private string m_Resource;
    [SerializeField] private string m_Description;

    public string Name => m_Name;
    public string Resource => m_Resource;
    public string Description => m_Description;

    public override void ApplyRowData(string[] Data)
    {
        this.uniqueId = Data[0];
        this.m_Name = Data[1];
        this.m_Resource = Data[2];
        this.m_Description = Data[3].Replace("\\n", "\n");
    }
}
