using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ListManager : MonoBehaviour
{
    public static ListManager Instance { get; private set; }

    [HideInInspector]
    public List<string> songList = new List<string>();

    void Awake()
    {
        Instance = FindObjectOfType<ListManager>();
        if (Instance != this)
        {
            Destroy(this);
        }

        SetSongList();
    }

    public void SetSongList()
    {
        string path = Application.dataPath + "/../Songs/";

        // 폴더 검색 후 mp3 파일 검색
        string[] folders = Directory.GetDirectories(path);
        for (int i = 0; i < folders.Length; i++)
        {
            string songName = folders[i].Remove(0, path.Length);

            if (File.Exists(folders[i] + "/" + songName + ".mp3"))
            {
                songList.Add(songName);
            }
        }
        // mp3 파일 검색
        string[] files = Directory.GetFiles(path);
        for (int i = 0; i < files.Length; i++)
        {
            string songName = files[i].Remove(0, path.Length);
            if (songName.EndsWith(".mp3"))
            {
                // 이름만 남기기
                songName = songName.Remove(songName.Length - 4, 4);

                // 파일 이동
                Directory.CreateDirectory(path + songName);
                File.Move(path + songName + ".mp3", path + songName + "/" + songName + ".mp3");

                songList.Add(songName);
            }
        }

        songList.Sort();
    }
}
