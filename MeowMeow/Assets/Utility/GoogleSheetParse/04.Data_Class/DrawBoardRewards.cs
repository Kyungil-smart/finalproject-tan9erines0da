using UnityEngine;
[System.Serializable]
public class DrawBoardRewards : Basedata
{
 [SerializeField] private string m_Name;
 [SerializeField] private string m_Reward_resource_image;
 [SerializeField] private string m_Reward_resource_text_image;
 [SerializeField] private int m_Grade;
 [SerializeField] private int m_Amount;
 [SerializeField] private bool m_Repeat;

   public string ItemName => m_Name;
   public string RewardResourceImage => m_Reward_resource_image;
   public string RewardResourceTextImage => m_Reward_resource_text_image;
   public int Grade => m_Grade;
   public int Amount => m_Amount;
   public bool Repeat => m_Repeat;
    public override void ApplyRowData(string[] Data)
    {
        this.uniqueId = Data[0];
        this.m_Name = Data[1];
        this.m_Grade = int.Parse(Data[2]);
        this.m_Amount = int.Parse(Data[3]);
        this.m_Repeat = int.Parse(Data[4]) == 1? true:false;
        this.m_Reward_resource_image = Data[5];
        this.m_Reward_resource_text_image = Data[6];
       
    }
}
