using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class SelectManager : MonoBehaviour
{
    public static SelectManager Instance { get; private set; }

    [SerializeField]
    private GameObject buttonPrefab;
    [SerializeField]
    private Transform content;

    public GameObject selectInfo;
    public SongField songField;
    public Text length;
    public Text speed;
    public Text triangle;
    public Text maxCombo;
    public Text death;

    public GameObject loading;
    [SerializeField]
    private Button start;

    [SerializeField]
    private GameObject trianglePrefab;
    private string songName;

    void Awake()
    {
        Instance = FindObjectOfType<SelectManager>();
        if (Instance != this)
        {
            Destroy(this);
        }
    }

    void Start()
    {
        songField.Init();

        CreateSongList();
    }

    void Update()
    {
        if(Input.GetKeyDown(Key.Instance.menu))
        {
            Back("Main");
        }
    }

    private void CreateSongList()
    {
        float height = buttonPrefab.GetComponent<RectTransform>().rect.height;

        GameObject button;
        for (int count = 0; count < ListManager.Instance.songList.Count; count++)
        {
            button = Instantiate(buttonPrefab, content);
            button.GetComponent<RectTransform>().localPosition = Vector3.down * count * height;
            button.GetComponentInChildren<Text>().text = ListManager.Instance.songList[count];
        }
    }

    public void GameStart(string scene)
    {
        GameObject gameObject = new GameObject("MapManager", typeof(MapManager));
        MapManager.Instance.enabled = false;
        MapManager.Instance.prefab = trianglePrefab;
        MapManager.Instance.defaultSize = 10;
        MapManager.Instance.songName = songName;
        DontDestroyOnLoad(gameObject);

        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void Back(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
    
    public void SetInfo(string songName)
    {
        start.enabled = false;

        this.songName = songName;
        songField.SetSongName(songName);
        songField.StartScroll();

        FileStream f = new FileStream(Application.dataPath + "/../Songs/" + songName + "/Info", FileMode.OpenOrCreate, FileAccess.Read);
        StreamReader reader = new StreamReader(f, System.Text.Encoding.Default);

        string str = reader.ReadToEnd();

        if (str.Length > 0)
        {
            string[] info = str.Split('\n');

            float time = float.Parse(info[0]);
            length.text = string.Format("{0:00}:{1:00} ", time / 60, time % 60);
            speed.text = info[1] + " ";
            triangle.text = info[2] + " ";

            start.enabled = true;
        }
        else
        {
            length.text = "??:?? ";
            speed.text = "? ";
            triangle.text = "? ";
        }

        reader.Close();
        
        f = new FileStream(Application.dataPath + "/../Songs/" + songName + "/Score", FileMode.OpenOrCreate, FileAccess.Read);
        reader = new StreamReader(f, System.Text.Encoding.Default);

        str = reader.ReadToEnd();

        if (str.Length > 0)
        {
            string[] info = str.Split('\n');

            maxCombo.text = info[0] + " ";
            death.text = info[1] + " ";
        }
        else
        {
            maxCombo.text = "? ";
            death.text = "? ";
        }

        reader.Close();
    }
}
