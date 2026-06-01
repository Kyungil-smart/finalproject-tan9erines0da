using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(UIImageShaderController))]
public class SNSPostPreviewRenderer : MonoBehaviour
{
    [Header("이미지 데이터 베이스")]
    [SerializeField] private CGImageDatabase _cgDatabase;
    [SerializeField] private StickerImageDatabase _stickerDB;

    [Header("스티커 프리팹")]
    [SerializeField] private Image _rawStickerPrefab;

    private UIImageShaderController _shaderController;

    private Image _bgImage;
    private List<GameObject> _spawnedStickers = new List<GameObject>();

    private void Awake()
    {
        _bgImage = GetComponent<Image>();
        _shaderController = GetComponent<UIImageShaderController>();
    }

    /// <summary>
    /// DTO만 던져주면 이미지를 복원합니다.
    /// </summary>
    public void RenderPreview(SNSPostDTO snapshot)
    {
        // 1. 기본 이미지 복원
        if (_cgDatabase != null)
        {
            _bgImage.sprite = _cgDatabase.GetSprite(snapshot.ImageIndex);
        }

        // 2. 셰이더 프로퍼티 복원
        if (_shaderController != null)
        {
            UIShaderProperty prop = snapshot.ShaderProperty;
            _shaderController.UpdateShaderProperties(
                prop.Brightness, prop.Contrast,
                prop.Saturation, prop.Temperature);
        }

        // 3. 스티커 레이어 청소 및 복원
        ClearStickers();

        if (snapshot.Stickers == null) return;

        RectTransform bgRect = _bgImage.rectTransform;

        foreach (var data in snapshot.Stickers)
        {
            GameObject obj = Instantiate(
                _rawStickerPrefab.gameObject, transform);

            Image img = obj.GetComponent<Image>();
            img.sprite = _stickerDB.GetSprite(data.StickerId);

            RectTransform rect = obj.GetComponent<RectTransform>();

            Vector2 savedPos = new Vector2(data.RelativeX, data.RelativeY);
            rect.RestorePos(savedPos, bgRect);
            rect.RestoreScale(data.RelativeScale, bgRect);
            rect.localEulerAngles = new Vector3(0f, 0f, data.Rotation);

            _spawnedStickers.Add(obj);
        }
    }

    private void ClearStickers()
    {
        foreach (var obj in _spawnedStickers)
        {
            if (obj != null) Destroy(obj);
        }
        _spawnedStickers.Clear();
    }
}
