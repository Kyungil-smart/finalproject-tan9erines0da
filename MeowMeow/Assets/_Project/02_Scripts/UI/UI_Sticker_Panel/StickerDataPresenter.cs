using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StickerDataPresenter : MonoBehaviour, ISNSPanelPresenter
{
    [Header("스티커 싱글톤")]
    [SerializeField] private StickerStateSingleton _stickerState;

    [Header("프리뷰 이미지")]
    [SerializeField] private RectTransform _bgRect;

    [Header("CG DB")]
    [SerializeField] private CGImageDatabase _cgDatabase;

    SNSPostDTO _snapshot;
    Image _bgImage;
    UIImageShaderController _shaderController;

    void Awake()
    {
        _bgImage = _bgRect.GetComponent<Image>();
        _shaderController = _bgRect.GetComponent<UIImageShaderController>();
    }
    void OnDisable()
    {
        // 기존 스티커 초기화
        _stickerState.AllClearSticker();
    }

    public void ReceiveSnapshot(SNSPostDTO snapshot)
    {
        if (_bgImage == null || _shaderController == null)
        {
            _bgImage = _bgRect.GetComponent<Image>();
            _shaderController = _bgRect.GetComponent<UIImageShaderController>();
        }

        _snapshot = snapshot;

        // 스냅샷 기준 이미지 복원
        if(_cgDatabase != null && _bgImage != null)
        {
            _bgImage.sprite = _cgDatabase.GetSprite(_snapshot.ImageIndex);
        }
        if (_shaderController != null)
        {
            UIShaderProperty savedProperty = _snapshot.ShaderProperty;
            _shaderController.UpdateShaderProperties(
                savedProperty.Brightness,
                savedProperty.Contrast,
                savedProperty.Saturation,
                savedProperty.Temperature);
        }

        RestoreRuntimeStickers();
    }

    public void RequestContext()
    {
        SubscribeManager.instance.Publish<Action<SNSPostDTO>>(SubscribeType.Request_CurrentPostContext, ReceiveSnapshot);
    }

    public void SubmitContext()
    {
        if (_stickerState == null || _bgRect == null) return;

        List<StickerTransformData> currentStickers =
            new List<StickerTransformData>();

        // _stickerIndexes 딕셔너리 순회
        var activeStickers = _stickerState.StickerIndexes;

        foreach (var kvp in activeStickers)
        {
            GameObject stickerObj = kvp.Key;
            int stickerIndex = kvp.Value;

            if (stickerObj == null) continue;

            RectTransform stickerRect = stickerObj.GetComponent<RectTransform>();

            // 부모 Rect 비례 상대 좌표 연산
            Vector2 relPos = stickerRect.ToRelPos(_bgRect);
            float relScale = stickerRect.ToRelScale(_bgRect);

            // 타입 변환 오차 제어를 염두에 둔 값 주입 (double 디바이스 호환)
            StickerTransformData data = new StickerTransformData
            {
                StickerId = stickerIndex,
                RelativeX = System.Math.Round((double)relPos.x, 2),
                RelativeY = System.Math.Round((double)relPos.y, 2),
                RelativeScale = System.Math.Round((double)relScale, 2),
                Rotation = System.Math.Round((double)stickerRect.localEulerAngles.z, 2)
            };

            currentStickers.Add(data);
        }

        _snapshot.Stickers = currentStickers;
        SubscribeManager.instance.Publish<SNSPostDTO>(
            SubscribeType.Update_PostModelData, _snapshot);

        _stickerState.AllClearSticker();
    }

    private void RestoreRuntimeStickers()
    {
        if (_snapshot.Stickers == null || _snapshot.Stickers.Count == 0) return;
        if (_stickerState == null) return;

        // 기존 스티커 초기화
        _stickerState.AllClearSticker();

        foreach (var data in _snapshot.Stickers)
        {
            // 1. 싱글톤의 생성 파이프라인 그대로 컴포넌트들을 스폰합니다.
            _stickerState.SetSticker(data.StickerId);

            // 2. 방금 생성되어 리스트 끝자락에 등록된 스티커 객체를 확보합니다.
            int lastIndex = _stickerState.ToggleList.Count - 1;
            Toggle lastToggle = _stickerState.ToggleList[lastIndex];
            GameObject spawnedSticker = _stickerState.ToggleToSticker[lastToggle];

            // 3. 생성된 스티커의 트랜스폼을 스냅샷 저장 수치로 재배치합니다.
            RectTransform rect = spawnedSticker.GetComponent<RectTransform>();
            Vector2 savedPos = new Vector2((float)data.RelativeX, (float)data.RelativeY);

            rect.RestorePos(savedPos, _bgRect);
            rect.RestoreScale((float)data.RelativeScale, _bgRect);
            rect.localEulerAngles = new Vector3(0f, 0f, (float)data.Rotation);
        }
    }
}
