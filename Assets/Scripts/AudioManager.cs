using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [HideInInspector]
    public AudioSource source;

    private WWW file;

    void Awake()
    {
        Instance = FindObjectOfType<AudioManager>();
        if (Instance != this)
        {
            Destroy(this);
        }
    }

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public IEnumerator LoadSongCoroutine(string songName)
    {
        file = new WWW("file://" + Application.dataPath + "/../Songs/" + songName + "/" + songName + ".mp3");

        yield return new WaitUntil(() => file.isDone);
        
        source.clip = NAudioPlayer.FromMp3Data(file.bytes);
        
        yield return new WaitForSeconds(0.1f);
    }
}
