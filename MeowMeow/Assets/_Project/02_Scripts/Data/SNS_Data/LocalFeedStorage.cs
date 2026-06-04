using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class LocalFeedStorage
{
    /// <summary>
    /// JsonUtility의 리스트 직렬화 한계를 우회하기 위한 포장지 클래스
    /// </summary>
    [System.Serializable]
    private class PostWrapper
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

        if (!File.Exists(path))
            return new List<SNSPostDTO>();

        string json = File.ReadAllText(path);
        var wrapper = JsonUtility.FromJson<PostWrapper>(json);

        return wrapper != null ? wrapper.Data : new List<SNSPostDTO>();
    }

    /// <summary>
    /// 계정별로 다른 파일을 가지도록 UID를 조합합니다.
    /// 예: MyPosts_user123uid.json
    /// </summary>
    private static string GetPath(string uid, string key)
    {
        return Path.Combine(Application.persistentDataPath,
            $"{key}_{uid}.json");
    }
}
