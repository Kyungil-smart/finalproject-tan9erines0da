using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    //AI 사용 텍스트 출력 씬에서 사용하는 클래스 입니다.
    public void LoadScene() => SceneManager.LoadScene(1);
}
