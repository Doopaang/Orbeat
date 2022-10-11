using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectButton : MonoBehaviour
{
    public void Select()
    {
        string songName = GetComponentInChildren<Text>().text;

        StartCoroutine(LoadSong(songName));
    }

    IEnumerator LoadSong(string songName)
    {
        SelectManager.Instance.loading.SetActive(true);
        
        yield return StartCoroutine(AudioManager.Instance.LoadSongCoroutine(songName));
        AudioManager.Instance.source.Play();

        SelectManager.Instance.selectInfo.SetActive(true);
        SelectManager.Instance.SetInfo(songName);

        SelectManager.Instance.loading.SetActive(false);
    }
}
