using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance { get; private set; }

    void Awake()
    {
        Instance = FindObjectOfType<MainManager>();
        if (Instance != this)
        {
            Destroy(this);
        }
    }
    
    public void LoadSceneSingle(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void LoadSceneAdd(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
