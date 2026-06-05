using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(UIImageShaderController))]
public class SNSPostImageRenderer : MonoBehaviour
{
    [Header("이미지 데이터 베이스")]
    [SerializeField] private CGImageDatabase _cgDatabase;
    [SerializeField] private StickerImageDatabase _stickerDB;

    [Header("스티커 프리팹")]
    [SerializeField] private Image _rawStickerPrefab;

    [Header("생명주기 문제로 인한 Null 해결을 위한 직접 참조")]
    [SerializeField] private Image _bgImage;
    [SerializeField] private UIImageShaderController _shaderController;
    private List<GameObject> _spawnedStickers = new List<GameObject>();

    private void Awake()
    {
        if(_bgImage == null) _bgImage = GetComponent<Image>();
        if(_shaderController == null) _shaderController = GetComponent<UIImageShaderController>();
    }

    /// <summary>
    /// DTO만 던져주면 이미지를 복원합니다.
    /// </summary>
    public void RenderPreview(SNSPostDTO snapshot)
    {
        if (_bgImage == null) _bgImage = GetComponent<Image>();
        if (_shaderController == null) _shaderController = GetComponent<UIImageShaderController>();

        // 1. 기본 이미지 복원
        if (_cgDatabase != null)
        {
            _bgImage.sprite = _cgDatabase.GetSprite(snapshot.ImageIndex);
        }

        // 2. 스티커 레이어 청소 및 복원
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

            Vector2 savedPos = new Vector2((float)data.RelativeX, (float)data.RelativeY);
            rect.RestorePos(savedPos, bgRect);
            rect.RestoreScale((float)data.RelativeScale, bgRect);
            rect.localEulerAngles = new Vector3(0f, 0f, (float)data.Rotation);

            _spawnedStickers.Add(obj);
        }

        // 3. 셰이더 복원
        if (_shaderController != null)
        {
            UIShaderProperty prop = snapshot.ShaderProperty;
            _shaderController.UpdateShaderProperties(
                prop.Brightness, prop.Contrast,
                prop.Saturation, prop.Temperature);
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
