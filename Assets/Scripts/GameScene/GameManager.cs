using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameMenu menu;

    private bool isPlaying = true;

    void Awake()
    {
        Instance = FindObjectOfType<GameManager>();
        if (Instance != this)
        {
            Destroy(this);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(Key.Instance.menu))
        {
            menu.Menu();
        }

        if (!AudioManager.Instance.source.isPlaying &&
            (AudioManager.Instance.source.time > 0.0f) &&
            isPlaying &&
            !menu.GetActive())
        {
            GameEnd();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        isPlaying = !pause;
    }

    public void GameStart()
    {
        AudioManager.Instance.source.time = 0;
        MapManager.Instance.SetTriangle(AudioManager.Instance.source.time);
        AudioManager.Instance.source.Play();

        ComboManager.Instance.Combo = 0;
        ComboManager.Instance.death = 0;

        MapManager.Instance.enabled = true;
    }

    private void GameEnd()
    {
        SceneManager.LoadScene("End", LoadSceneMode.Additive);
    }
}
