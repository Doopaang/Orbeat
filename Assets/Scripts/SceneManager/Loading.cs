using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class Loading : MonoBehaviour
{
    [SerializeField]
    private string loadScene;

    private AsyncOperation loadAsync;

    void Start()
    {
        loadAsync = SceneManager.LoadSceneAsync(loadScene, LoadSceneMode.Additive);
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return new WaitUntil(() => loadAsync.isDone);

        LoadSpeed();

        MapManager.Instance.Init();
        MapManager.Instance.LoadMap();

        SongField songField = FindObjectOfType<SongField>();
        songField.SetSongName(MapManager.Instance.songName);
        songField.StartScroll();

        GameManager.Instance.GameStart();

        SceneManager.UnloadSceneAsync("Loading");
    }

    private void LoadSpeed()
    {
        FileStream f = new FileStream(Application.dataPath + "/../Songs/" + MapManager.Instance.songName + "/Info", FileMode.OpenOrCreate, FileAccess.Read);
        StreamReader reader = new StreamReader(f, System.Text.Encoding.Default);
        
        string str = reader.ReadToEnd();

        if (str.Length > 0)
        {
            string[] info = str.Split('\n');

            FindObjectOfType<PlayerControl>().moveSpeed = int.Parse(info[1]);
        }

        reader.Close();
    }
}
