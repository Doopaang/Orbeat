using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SongField : MonoBehaviour
{
    private Text songText;

    private float fieldSize;
    [SerializeField]
    private float textEmpty;

    [SerializeField]
    private float scrollSpeed;
    [SerializeField]
    private float scrollWait;

    private IEnumerator scrollName = null;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        songText = GetComponentInChildren<Text>();
        fieldSize = GetComponent<RectTransform>().rect.width;
    }

    public void SetSongName(string songName)
    {
        songText.text = " " + songName;
        songText.rectTransform.sizeDelta = Vector2.right * (songText.preferredWidth + textEmpty) + Vector2.up * (songText.preferredHeight + 10.0f);
    }

    public void StartScroll()
    {
        if (songText.rectTransform.sizeDelta.x > fieldSize)
        {
            scrollName = ScrollName();
            StartCoroutine(scrollName);
        }
    }

    public void StopScroll()
    {
        if (scrollName != null)
        {
            StopCoroutine(scrollName);
        }
    }

    private IEnumerator ScrollName()
    {
        while (true)
        {
            Vector3 position = songText.rectTransform.localPosition;

            if (position.x < -songText.rectTransform.sizeDelta.x + fieldSize)
            {
                position.x = 0.0f;
                songText.rectTransform.localPosition = position;

                yield return new WaitForSeconds(scrollWait);
            }

            position.x -= scrollSpeed;
            songText.rectTransform.localPosition = position;

            if (position.x < -songText.rectTransform.sizeDelta.x + fieldSize)
            {
                yield return new WaitForSeconds(scrollWait);
            }

            yield return null;
        }
    }
}
