using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public enum CommentType
{
    emotion,//감성
    Butler_Life,//집사라이프
    Particle,//조사
    onomatopoeia,//의성어
    Item,//아이템
    Action,//행동
    situation,//상황
}



[System.Serializable]
public class comment : Basedata
{
    public string word;
    public CommentType type;

    public override void ApplyRowData(string[] Data)
    {

        this.uniqueId = Data[0];
        this.word = Data[1];
        SetType(Data[2]);

    }
    public void SetType(string type)
    {
        switch (type)
        {
            case "감성":
                this.type = CommentType.emotion;
                break;
            case "집사생활":
                this.type = CommentType.Butler_Life;
                break;
            case "조사":
                this.type = CommentType.Particle;
                break;
            case "의성어":
                this.type = CommentType.onomatopoeia;
                break;
            case "아이템":
                this.type = CommentType.Item;
                break;
            case "행동":
                this.type = CommentType.Action;
                break;
            case "상황":
                this.type = CommentType.situation;
                break;
        }
    }
}
