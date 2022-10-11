using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class EditMenu : MonoBehaviour
{
    [SerializeField]
    private Dropdown songSelect;
    [SerializeField]
    private Button backButton;
    [SerializeField]
    private Button saveButton;
    [SerializeField]
    private Button resetButton;
    [SerializeField]
    private GameObject loading;

    private WWW file;

    void Start()
    {
        songSelect.AddOptions(ListManager.Instance.songList);
        
        backButton.enabled = false;
        saveButton.enabled = false;
        resetButton.enabled = false;
    }
    
    public void SelectSong()
    {
        if (songSelect.value <= 0)
        {
            AudioManager.Instance.source.Stop();
            saveButton.enabled = false;
            resetButton.enabled = false;
            backButton.enabled = false;

            return;
        }

        MapManager.Instance.songName = songSelect.options[songSelect.value].text;

        // mp3 불러오기
        StartCoroutine(LoadSongCoroutine(MapManager.Instance.songName));

        saveButton.enabled = true;
        resetButton.enabled = true;
        backButton.enabled = true;
        SongManager.Instance.toggle.enabled = true;

        SongField songField = FindObjectOfType<SongField>();
        songField.SetSongName(MapManager.Instance.songName);
        songField.StartScroll();
    }

    IEnumerator LoadSongCoroutine(string songName)
    {
        loading.SetActive(true);

        yield return StartCoroutine(AudioManager.Instance.LoadSongCoroutine(songName));

        ReadInfo();

        SongManager.Instance.SetSong();
        SongManager.Instance.toggle.isOn = !SongManager.Instance.toggle.isOn;

        MapManager.Instance.ResetPool();
        MapManager.Instance.LoadMap();
        MapManager.Instance.SetTriangle(AudioManager.Instance.source.time);

        loading.SetActive(false);
    }
    
    public void Save()
    {
        MapManager.Instance.SaveMap();
        SaveInfo();
        SongManager.Instance.Rewind();
    }

    public void ResetMap()
    {
        MapManager.Instance.ResetPool();
        MapManager.Instance.ResetMap();
        SongManager.Instance.Rewind();
    }

    public void Option(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
    }

    public void MainMenu(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void Back()
    {
        if (!backButton.enabled)
        {
            return;
        }
        EditManager.Instance.record = false;
        SongManager.Instance.Rewind();
        CircleCollider2D collider = EditManager.Instance.creator.GetComponent<CircleCollider2D>();
        collider.enabled = !collider.enabled;
        gameObject.SetActive(!gameObject.activeSelf);
    }

    private void ReadInfo()
    {
        FileStream f = new FileStream(Application.dataPath + "/../Songs/" + MapManager.Instance.songName + "/Info", FileMode.OpenOrCreate, FileAccess.Read);
        StreamReader reader = new StreamReader(f, System.Text.Encoding.Default);

        string str = reader.ReadToEnd();

        if (str.Length > 0)
        {
            string[] info = str.Split('\n');

            EditManager.Instance.sideTab.speedSlider.value = int.Parse(info[1]);
        }
        else
        {
            EditManager.Instance.sideTab.speedSlider.value = 2;
        }

        reader.Close();
    }

    private void SaveInfo()
    {
        FileStream f = new FileStream(Application.dataPath + "/../Songs/" + MapManager.Instance.songName + "/Info", FileMode.Create, FileAccess.Write);
        StreamWriter writer = new StreamWriter(f, System.Text.Encoding.Default);

        writer.WriteLine(AudioManager.Instance.source.clip.length);
        writer.WriteLine(EditManager.Instance.control.moveSpeed);
        writer.WriteLine(MapManager.Instance.map.Count);

        writer.Close();
    }
}
