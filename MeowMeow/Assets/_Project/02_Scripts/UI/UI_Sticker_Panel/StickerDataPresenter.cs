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
    }

    public void RequestContext()
    {
        SubscribeManager.instance.Publish<Action<SNSPostDTO>>(SubscribeType.Request_CurrentPostContext, ReceiveSnapshot);
    }

    public void SubmitContext()
    {
        if(_stickerState == null || _bgRect == null) return;

        // 스냅샷에 담을 스티커 변수 리스트
        List<StickerTransformData> currentStickers = new List<StickerTransformData>();

        var runtimeStickers = _stickerState.stickers;

        for(int i = 0; i < runtimeStickers.Count; i++)
        {
            if(runtimeStickers[i] == null) continue;

            RectTransform stickerRect = runtimeStickers[i].sticker.GetComponent<RectTransform>();

            // 상대적인 포지션과 스케일 계산
            Vector2 relPos = stickerRect.ToRelPos(_bgRect);
            float relScale = stickerRect.ToRelScale(_bgRect);

            // 데이터 구조체 생성 및 리스트에 추가
            StickerTransformData data = new StickerTransformData
            {
                StickerId = runtimeStickers[i].stickerIndex,
                RelativeX = relPos.x,
                RelativeY = relPos.y,
                RelativeScale = relScale,
                Rotation = stickerRect.localEulerAngles.z
            };

            currentStickers.Add(data);
        }

        //스냅샷 수정 및 저장
        _snapshot.Stickers = currentStickers;
        SubscribeManager.instance.Publish<SNSPostDTO>(SubscribeType.Update_PostModelData, _snapshot);

    }
}
