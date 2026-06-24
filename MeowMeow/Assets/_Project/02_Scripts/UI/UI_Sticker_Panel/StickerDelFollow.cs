using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerDelFollow : MonoBehaviour
{
    private RectTransform _targetRect;
    private RectTransform _myRect;

    // 스티커 삭제버튼 생성위치(이 위치에서 스티커와 거리를 두며 따라갑니다.)
    private Vector2 _offset = new Vector2(110f, 450f);

    private void LateUpdate()
    {
        if (_targetRect == null) return;

        _myRect.anchoredPosition = GetDeleteButtonPosition();
    }

    private Vector2 GetDeleteButtonPosition()
    {
        float width = _targetRect.rect.width * (_targetRect.localScale.x - 1f);
        float height = _targetRect.rect.height * (_targetRect.localScale.y - 1f);

        return _targetRect.anchoredPosition
               + _offset
               + new Vector2(width * 0.5f, height * 0.5f);
    }

    /// <summary>
    /// 외부에서 StickerDelFollow 스크립트를 초기화 하기위해 만든 함수입니다.
    /// </summary>
    /// <param name="target">따라갈 스티커의 RectTransform을 인자로 받습니다.</param>
    public void InitStickerDelFollow(RectTransform targetRect)
    {
        _targetRect = targetRect;
        _myRect = GetComponent<RectTransform>();
    }
}
