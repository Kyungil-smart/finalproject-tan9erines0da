using System.Threading.Tasks;
using UnityEngine;
public  abstract class BaseDataSO : ScriptableObject
{
    [Header("구글 시트 URL 그냥 다 넣으면 된다.(구글 시트 공유 설정 확인 바람)")]
    public string url;
    [Header("사이트 URL 끝에 gid번호 있는데 시트마다 확인해서 해야함")]//기본 첫 시트는 0 인거 같음 그래도 확인 필수
    public int gid;
    public string genericType;
    public abstract Task InitAsync();
}
