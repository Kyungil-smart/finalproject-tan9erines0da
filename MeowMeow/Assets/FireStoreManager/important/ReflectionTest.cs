using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

public class ReflectionGCTest : MonoBehaviour
{
    // 테스트용 임시 정적 캐시 딕셔너리
    private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _testCache
        = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

    [Header("테스트 반복 횟수")]
    [SerializeField] private int m_RepeatCount = 1000;

    // 1️⃣ 인스펙터 창에서 마우스 우클릭 -> '캐싱 없이 리플렉션 난사 테스트' 클릭
    [ContextMenu("1. 캐싱 없이 리플렉션 난사 테스트")]
    public void TestRawReflection()
    {
        // 정확한 측정을 위해 기존 가비지를 깨끗하게 청소
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // 로직 시작 전 힙 메모리 상태 기록
        long startMemory = GC.GetTotalMemory(false);

        Type targetType = typeof(SNSPostDTO);

        // m_RepeatCount만큼 리플렉션을 반복 수행해서 가비지를 고의로 누적시킵니다.
        for (int i = 0; i < m_RepeatCount; i++)
        {
            // ❌ 호출할 때마다 매번 Type객체에서 프로퍼티 배열을 생성하고(GC Alloc),
            // 그걸 또 ToDictionary로 복사하면서 엄청난 양의 가비지가 힙에 쌓입니다.
            var propertyDict = targetType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(p => p.Name);
        }

        // 로직 종료 후 힙 메모리 상태 기록
        long endMemory = GC.GetTotalMemory(false);
        long allocatedMemory = endMemory - startMemory;

        Debug.Log($"<color=red><b>[캐싱 없음]</b> {m_RepeatCount}회 실행 동안 발생한 가비지: <b>{allocatedMemory:N0} Bytes</b></color>");
    }

    // 2️⃣ 인스펙터 창에서 마우스 우클릭 -> '캐싱 적용 후 리플렉션 테스트' 클릭
    [ContextMenu("2. 캐싱 적용 후 리플렉션 테스트")]
    public void TestCachedReflection()
    {
        // 똑같이 가비지 청소 후 시작
        GC.Collect();
        GC.WaitForPendingFinalizers();
        _testCache.Clear(); // 테스트를 위해 캐시를 비우고 시작합니다.

        long startMemory = GC.GetTotalMemory(false);

        Type targetType = typeof(SNSPostDTO);

        for (int i = 0; i < m_RepeatCount; i++)
        {
            //  질문자님이 구현하신 메모이제이션(캐싱) 메커니즘 작동
            if (!_testCache.TryGetValue(targetType, out var propertyDict))
            {
                propertyDict = targetType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .ToDictionary(p => p.Name);
                _testCache[targetType] = propertyDict; // 최초 1회만 저장
            }
            else
            {
                // 두 번째 루프부터는 무거운 연산 없이 기존 캐시 데이터를 뺴다 쓰기만 함
            }
        }

        long endMemory = GC.GetTotalMemory(false);
        long allocatedMemory = endMemory - startMemory;

        Debug.Log($"<color=lime><b>[캐싱 적용]</b> {m_RepeatCount}회 실행 동안 발생한 가비지: <b>{allocatedMemory:N0} Bytes</b> (최초 1회 캐싱 비용만 포함됨)</color>");
    }
}