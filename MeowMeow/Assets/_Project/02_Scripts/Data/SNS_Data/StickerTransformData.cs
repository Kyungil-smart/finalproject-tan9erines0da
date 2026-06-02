using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스티커 한개의 위치 크기 회전 정보를 담기 위한 구조체
/// 배치가 완료된 후 다음 판넬로 넘어갈때 기록하는 용도입니다.
/// 배치를 재현할때 구조체의 값을 바탕으로 복원합니다.
/// </summary>
public struct StickerTransformData
{
    public int StickerId;       // 스티커 이미지를 파악하기 위한 ID
    public float RelativeX;     // 상대적인x좌표 (0.0 ~1.0)
    public float RelativeY;     // 상대적인y좌표 (0.0 ~1.0)
    public float RelativeScale; // 상대적인 크기 비율
    public float Rotation;      // z축 회전 값

    public Vector2 RelativePos()
    {
        return new Vector2(RelativeX, RelativeY);
    }
}

public struct UIShaderProperty
{
    public float Brightness;  // 밝기 (-1.0 ~1.0)
    public float Contrast;    // 대비 (0.0 ~2.0)
    public float Saturation;  // 채도 (0.0 ~2.0)
    public float Temperature; // 온도 (0.0 ~1.0)
}
