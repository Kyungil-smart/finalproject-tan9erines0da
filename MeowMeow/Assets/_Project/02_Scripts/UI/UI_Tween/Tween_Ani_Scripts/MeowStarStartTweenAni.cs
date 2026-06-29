using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

public class MeowStarStartTweenAni : MonoBehaviour
{
    [Header("NYS_BG_Image → NYS_Merge_Image 참조")]
    [SerializeField] private RectTransform _nysMergeImage;
    [Header("별 연출 전용 필드")]
    [SerializeField] private Sprite _starSprite;
    [SerializeField] private float _radius = 90f;

    private async void Start()
    {
        await PlayAnimation();
    }

    static readonly Color[] Palette = // 별들의 색상을 랜덤으로 출력하기 위한 색상 팔레트
    {
        new Color(1f,  0.27f, 0.27f),
        new Color(1f,  0.85f, 0.1f),
        new Color(0.3f,0.85f, 0.4f),
        new Color(0.3f,0.6f,  1f),
        new Color(0.9f,0.4f,  1f),
        new Color(1f,  0.6f,  0.2f),
        new Color(0.4f,0.95f, 0.95f),
    };

    [ContextMenu("냥스타 진입 애니메이션")]
    public Task PlayAnimation()
    {
        if (_nysMergeImage == null)
        {
            Debug.LogWarning("_nysMergeImage가 비어있습니다.");
            return Task.CompletedTask;
        }

        _nysMergeImage.DOKill();

        _nysMergeImage.gameObject.SetActive(true);
        _nysMergeImage.sizeDelta = new Vector2(700f, 700f);

        Sequence seq = DOTween.Sequence();

        seq.Append(_nysMergeImage.DOSizeDelta(new Vector2(197f, 197f), 0.8f).SetEase(Ease.OutQuad));

        seq.Insert(0.6f, gameObject.transform.DOScale(0.9f, 0.3f).SetEase(Ease.OutQuad));

        seq.InsertCallback(0.6f, Play);

        seq.Append(gameObject.transform.DOScale(1f, 0.2f).SetEase(Ease.OutQuad));

        return seq.AsyncWaitForCompletion();
    }

    public void Play() // 외부(LoadingController)에서 호출하여 5~6개의 별 파티클을 랜덤 방향으로 동시에 발사
    {
        int count = Random.Range(5, 7);
        for (int i = 0; i < count; i++)
            Burst(Random.insideUnitCircle.normalized, Palette[Random.Range(0, Palette.Length)]);
    }

    void Burst(Vector2 dir, Color col) // 별 Image 오브젝트 하나를 생성해 지정 방향으로 날아가며 페이드아웃 후 자동 삭제
    {
        GameObject go = new GameObject("Star", typeof(Image));
        go.transform.SetParent(transform, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50f, 50f);
        rt.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        Image img = go.GetComponent<Image>();
        img.sprite = _starSprite;
        img.color = col;

        DOTween.Sequence() //Append는 앞에서부터 순차적으로 실행시키고, Join은 바로 앞에 것이 실행되면 동시에 실행시킴
            .Append(rt.DOLocalMove(dir * _radius, 0.5f).SetEase(Ease.OutCubic)) // Append앞에 트윈이 없으므로 바로 실행됨, 0.5초 동안 dir 방향으로 radius 거리만큼 이동, Ease.OutCubic은 빠르게 시작해서 천천히 끝나는 움직임을 만들어줌

            .Join(img.DOFade(0f, 1f)); // 이동과 동시에 1초 동안 서서히 투명해짐 // 앞의 Append에서 트윈이 실행되면 바로 이어서 실행됨, 1초 동안 서서히 투명해지도록 만듬, DOFade 는 정해진 시간동안 일정 알파값을 만드는 트윈을 반환하는 메서드

        Destroy(go, 1f); // 1초 후에 go 이미지 오브젝트가 완전히 투명해지기 때문에 1초 뒤에 Destroy로 삭제하여 메모리 관리
    }
}
