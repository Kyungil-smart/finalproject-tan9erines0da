
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;


 
    [System.Serializable]
    public class SheetLoader<T> where T : ISheetParsable, IIdentifiable, new()
    {
        private string m_url;
        private bool _isLoaded = false;
        private Dictionary<int, T> Datas = new Dictionary<int, T>();
        private TaskCompletionSource<bool> _loadTaskSource = new TaskCompletionSource<bool>();

        private int m_gid;
        public SheetLoader(string url, int gid = 0)
        {
            m_url = url;
            m_gid = gid;
            Start();
        }
        public void NotifyLoadComplete()
        {
            _isLoaded = true;
            _loadTaskSource.TrySetResult(true);
        }
        private async void Start()
        {
            UnityWebRequest request = UnityWebRequest.Get(ConvertToDownloadUrl(m_url, m_gid.ToString()));
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield(); // 다음 프레임까지 양보
            }


            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string csv = request.downloadHandler.text;
                ParseCSV(csv);
                NotifyLoadComplete();
            }

        }
        void ParseCSV(string csv)
        {

            // 줄 분리
            string[] lines = csv.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < lines.Length; i++) // 0번은 헤더
            {
                T temp = new T();
                string[] values = lines[i].Split(',');
                temp.ApplyRowData(values);
                Datas.Add(i, temp);

            }
        }
        public string ConvertToDownloadUrl(string url, string gid = "0")
        {
            // 1. "/edit" 및 그 뒤에 오는 모든 내용을 제거
            // 2. "/export?format=csv"를 붙임
            // 3. 특정 시트(gid)를 지정하고 싶다면 "&gid=xxx" 추가

            string pattern = @"/edit.*";
            string replacement = $"/export?format=csv&gid={gid}";

            if (Regex.IsMatch(url, pattern))
            {
                return Regex.Replace(url, pattern, replacement);
            }

            return url; // 패턴이 없으면 그대로 반환
        }
        public async Task<List<T>> GetDataAsync()
        {
            // 데이터가 아직 로드되지 않았다면 완료될 때까지 await
            if (!_isLoaded)
            {
                Debug.Log("데이터 로딩 중... 완료될 때까지 대기합니다.");
                await _loadTaskSource.Task;
            }

            // 로드 완료 후 리스트 생성 및 반환
            List<T> save = new List<T>();
            foreach (var item in Datas)
            {
                save.Add(item.Value);
            }
            return save;
        }

        public async Task<T> GetDataByString(string DataName)
        {
            if (!_isLoaded)
            {
                Debug.Log("데이터 로딩 중... 완료될 때까지 대기합니다.");
                await _loadTaskSource.Task;
            }

            foreach (var item in Datas)
            {

                if (item.Value.uniqueId == DataName)
                {
                    return item.Value;
                }

            }
            return default(T);
        }
    }
 

