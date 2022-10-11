using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class EndManager : MonoBehaviour
{
    public static EndManager Instance { get; private set; }

    [SerializeField]
    private SongField songField;
    [SerializeField]
    private Text comboText;
    [SerializeField]
    private Text deathText;

    void Awake()
    {
        Instance = FindObjectOfType<EndManager>();
        if (Instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        songField.Init();
        songField.SetSongName(MapManager.Instance.songName);
        songField.StartScroll();

        int[] score = new int[2];
        LoadScore(score);
        SetText(score);
        SaveScore(score);
    }

    void Update()
    {
        if (Input.GetKeyDown(Key.Instance.menu))
        {
            MoveScene("Select");
        }
    }

    public void Restart(string scene)
    {
        GameManager.Instance.menu.Restart();
        SceneManager.UnloadSceneAsync(scene);
    }

    public void MoveScene(string scene)
    {
        Destroy(MapManager.Instance.gameObject);
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    private void LoadScore(int[] score)
    {
        FileStream f = new FileStream(Application.dataPath + "/../Songs/" + MapManager.Instance.songName + "/Score", FileMode.OpenOrCreate, FileAccess.Read);
        StreamReader reader = new StreamReader(f, System.Text.Encoding.Default);

        string str = reader.ReadToEnd();

        if (str.Length > 0)
        {
            string[] split = str.Split('\n');

            if (ComboManager.Instance.maxCombo > int.Parse(split[0]))
            {
                score[0] = int.Parse(split[0]);
                score[1] = int.Parse(split[1]);
            }
            else
            {
                score[0] = ComboManager.Instance.maxCombo;
                score[1] = ComboManager.Instance.death;
            }
        }
        else
        {
            score[0] = ComboManager.Instance.maxCombo;
            score[1] = ComboManager.Instance.death;
        }

        reader.Close();
    }

    private void SaveScore(int[] score)
    {
        FileStream f = new FileStream(Application.dataPath + "/../Songs/" + MapManager.Instance.songName + "/Score", FileMode.Create, FileAccess.Write);
        StreamWriter writer = new StreamWriter(f, System.Text.Encoding.Default);

        writer.WriteLine(score[0]);
        writer.WriteLine(score[1]);

        writer.Close();
    }

    private void SetText(int[] score)
    {
        comboText.text = score[0].ToString();
        deathText.text = score[1].ToString();
    }
}
