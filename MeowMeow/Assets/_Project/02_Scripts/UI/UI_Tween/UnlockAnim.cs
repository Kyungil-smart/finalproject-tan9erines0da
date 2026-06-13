using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UnlockAnim : MonoBehaviour
{
    [SerializeField] private RectTransform container; // 부모 (Axis)
    [SerializeField] private RectTransform lockImage;  // 자식 (Sliced 이미지)
    [SerializeField] private float duration = 0.4f;

    private Image uiImage;
    private float originalWidth;
    private float minWidth;

    void Awake()
    {
        uiImage = lockImage.GetComponent<Image>();
        originalWidth = lockImage.sizeDelta.x;

        
        minWidth = 17f;
        
    }

    [ContextMenu("Rotate Lock Clean Version")]
    public void PlayRotation()
    {
        container.DOKill();
        lockImage.DOKill();

        // 초기화 (이제 자식의 위치는 무조건 (0,0) 고정입니다!)
        lockImage.sizeDelta = new Vector2(originalWidth, lockImage.sizeDelta.y);
        lockImage.localPosition = Vector3.zero;
        container.localScale = Vector3.one;

        Sequence seq = DOTween.Sequence();

        // 1단계: 크기 줄이기 (자식 피벗이 우측 기둥이므로 자동으로 우측 고정)
        seq.Append(lockImage.DOSizeDelta(new Vector2(minWidth, lockImage.sizeDelta.y), duration * 0.5f)
            .SetEase(Ease.InQuad));

        // 2단계: 순간 플립 (부모 피벗도 우측 기둥이므로 제자리에서 회전)
        seq.AppendCallback(() =>
        {
            container.localScale = new Vector3(-1, 1, 1);
        });

        // 3단계: 다시 크기 늘리기
        seq.Append(lockImage.DOSizeDelta(new Vector2(originalWidth, lockImage.sizeDelta.y), duration * 0.5f)
            .SetEase(Ease.OutQuad));
    }
}
