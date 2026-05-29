using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerButtonCollector : MonoBehaviour
{
    [Header("스티커 생성 버튼들을 모두 참조")]
    [SerializeField] private List<StickerEditor> _stickerEditors = new List<StickerEditor>();

    private void Awake()
    {
        for(int i = 0; i < _stickerEditors.Count; i++)
        {
            _stickerEditors[i].MyIndex = i;
        }
    }
}
