using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    private PlayerControl control;
    private SongField songField = null;

    [SerializeField]
    private GameObject menu;

    [HideInInspector]
    public bool toggleMenu = false;

    private void Start()
    {
        control = FindObjectOfType<PlayerControl>();
        songField = FindObjectOfType<SongField>();
    }

    public bool GetActive()
    {
        return menu.activeSelf;
    }

    public void Menu()
    {
        menu.SetActive(!toggleMenu);
        control.canMove = toggleMenu;

        if (toggleMenu == false)
        {
            AudioManager.Instance.source.Pause();
            songField.StopScroll();
        }
        else
        {
            AudioManager.Instance.source.Play();
            songField.StartScroll();
        }

        toggleMenu = !toggleMenu;
    }

    public void Restart()
    {
        MapManager.Instance.ResetPool();

        ComboManager.Instance.ResetScore();
        AudioManager.Instance.source.time = 0.0f;

        GameManager.Instance.GameStart();
    }

    public void Option(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
    }
    
    public void MoveScene(string scene)
    {
        Destroy(MapManager.Instance.gameObject);
        SceneManager.LoadScene(scene);
    }
}
