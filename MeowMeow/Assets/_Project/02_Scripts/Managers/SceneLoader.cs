using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string _sceneName;
    [SerializeField] private int _sceneIndex;

    public void LoadScene()
    {
        SceneManager.LoadScene(_sceneName);
    }
}
