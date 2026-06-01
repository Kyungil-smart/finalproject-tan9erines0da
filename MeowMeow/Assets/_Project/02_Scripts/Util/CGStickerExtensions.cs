using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스티커 위치 저장 및 복원을 위한 확장 메서드입니다.
/// 배경 이미지에대한 스티커의 상대적인 recttransform을 계산합니다
/// </summary>
public static class CGStickerExtensions
{
    /// <summary>
    /// 배경 대비 상대 좌표(0~1)를 반환합니다.
    /// 좌하단(0,0) 우상단(1,1)
    /// 사용법 : sticker의 rect.ToRelPos(배경의 Rect)
    /// </summary>
    public static Vector2 ToRelPos(this RectTransform self, RectTransform bg)
    {
        float bgWidth = bg.rect.width;
        float bgHeight = bg.rect.height;

        float relX = (self.anchoredPosition.x / bgWidth) + 0.5f;
        float relY = (self.anchoredPosition.y / bgHeight) + 0.5f;

        return new Vector2(relX, relY);
    }

    /// <summary>
    /// 배경 대비 상대적인 크기를 구합니다
    /// 사용법 : sticker의 rect.ToRelScale(배경의 rect)
    /// </summary>
    public static float ToRelScale(this RectTransform self, RectTransform bg)
    {
        float pixelWidth = self.rect.width * self.localScale.x;
        return pixelWidth / bg.rect.width;
    }

    /// <summary>
    /// 스티커의 RectPosition을 복원합니다.
    /// 사용법 : sticker의 rect.RestorePos(상대좌표(vector2), 배경의 Rect)
    /// </summary>
    public static void RestorePos(this RectTransform self, Vector2 relPos, RectTransform bg)
    {
        float pixelX = (relPos.x - 0.5f) * bg.rect.width;
        float pixelY = (relPos.y - 0.5f) * bg.rect.height;
        self.anchoredPosition = new Vector2(pixelX, pixelY);
    }

    /// <summary>
    /// 스티커의 크기를 복원합니다.
    /// 사용법 : sticker의 rect.RestoreScale(RelativeScale, 배경의 Rect)
    /// </summary>
    public static void RestoreScale(this RectTransform self, float relScale, RectTransform bg)
    {
        float targetPixelWidth = relScale * bg.rect.width;
        float originWidth = self.rect.width;

        if (originWidth <= 0f) return;

        float scale = targetPixelWidth / originWidth;
        self.localScale = new Vector3(scale, scale, 1f);
    }
}
