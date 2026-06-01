using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StickerDB", menuName = "SO/StickerDatabaseSO")]

public class StickerImageDatabase : ScriptableObject
{
    // 임시로 리스트의 인덱스(0, 1, 2...)를 이미지 ID로 겸해서 사용합니다.
    [SerializeField] private List<Sprite> _stickerSprites = new List<Sprite>();

    // 외부 패널들이 리스트의 크기만큼 루프를 돌 수 있도록 프로퍼티 노출
    public int Count => _stickerSprites.Count;

    // 인덱스를 주면 해당 스프라이트를 안전하게 반환하는 인덱서 기능
    public Sprite GetSprite(int index)
    {
        if (index < 0 || index >= _stickerSprites.Count)
        {
            Debug.LogError($"[SO] 인덱스 범위 초과: {index}");
            return null;
        }
        return _stickerSprites[index];
    }
}
