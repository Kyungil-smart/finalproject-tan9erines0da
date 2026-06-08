using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public static class LocalFeedStorage
{
    /// <summary>
    /// JsonUtility의 리스트 직렬화 한계를 우회하기 위한 포장지 클래스
    /// </summary>
    [System.Serializable]
    public class PostWrapper
    {
        public List<SNSPostDTO> Data = new List<SNSPostDTO>();
    }

    /// <summary>
    /// 메모리의 리스트를 UID가 포함된 JSON 파일로 로컬에 기록합니다.
    /// </summary>
    public static void SavePosts(string uid, string key,
        List<SNSPostDTO> list)
    {
        string path = GetPath(uid, key);
        var wrapper = new PostWrapper { Data = list };

        string json = JsonUtility.ToJson(wrapper, true);

        File.WriteAllText(path, json);
        Debug.Log($"[로컬 저장 완료] {path}");
    }

    /// <summary>
    /// 로컬 JSON 파일을 읽어 리스트로 환원합니다. (파일이 없으면 빈 리스트)
    /// </summary>
    public static List<SNSPostDTO> LoadPosts(string uid, string key)
    {
        string path = GetPath(uid, key);

        // 1. 파일이 아예 없다면 안전하게 빈 리스트 반환
        if (!File.Exists(path))
        {
            string msg = "파일이 없습니다";
            SubscribeManager.instance.Publish<string>(
                SubscribeType.Log_Write, msg);


            return new List<SNSPostDTO>();
        }

        try
        {
            // 2. 텍스트를 읽고 포장지 클래스로 변환 시도
            string json = File.ReadAllText(path);
            var wrapper = JsonUtility.FromJson<PostWrapper>(json);

            // 3. 포장지뿐만 아니라, Data까지이중으로 검증해야만 합니다.
            if (wrapper != null && wrapper.Data != null)
            {
                return wrapper.Data;
            }

            string warnMsg = $"[Storage Warning] {key} 알맹이 누락.\n" +
                         $"JSON 원본: {json}";

            SubscribeManager.instance.Publish<string>(
                SubscribeType.Log_Write, warnMsg);

            return new List<SNSPostDTO>();
        }
        catch (System.Exception ex)
        {
            // 파일 잠김, 권한 없음, JSON 문법 파괴 등 모든 에러 캐치
            string errMsg = $"[Storage Error] {key} 로드 실패.\n" +
                        $"사유: {ex.Message}";

            SubscribeManager.instance.Publish<string>(
                SubscribeType.Log_Write, errMsg);

            return new List<SNSPostDTO>();
        }
    }

    /// <summary>
    /// 계정별로 다른 파일을 가지도록 UID를 조합합니다.
    /// 예: MyPosts_user123uid.json
    /// </summary>
    public static string GetPath(string uid, string key)
    {
        return Path.Combine(Application.persistentDataPath,
            $"{key}_{uid}.json");
    }
}
