using UnityEngine;
using UnityEngine.UI;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance { get; private set; }

    [SerializeField]
    private Slider songGauge;
    public Toggle toggle;
    [SerializeField]
    private Text timeText;

    private bool duplication = false;

    void Awake()
    {
        Instance = FindObjectOfType<SongManager>();
        if (Instance != this)
        {
            Destroy(this);
        }
    }

    void Start()
    {
        toggle.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(Key.Instance.editPlayPause) &&
            toggle.enabled)
        {
            toggle.isOn = !toggle.isOn;
        }
        if (Input.GetKeyDown(Key.Instance.editRewind))
        {
            Rewind();
        }
        if (Input.GetKeyDown(Key.Instance.editBackward))
        {
            Backward();
        }
        if (Input.GetKeyDown(Key.Instance.editForward))
        {
            Forward();
        }

        if (AudioManager.Instance.source.clip)
        {
            duplication = true;
            songGauge.value = AudioManager.Instance.source.time;

            int time = (int)AudioManager.Instance.source.time, length = (int)AudioManager.Instance.source.clip.length;
            timeText.text = string.Format("{0:00}:{1:00} / {2:00}:{3:00}", time / 60, time % 60, length / 60, length % 60);

            if (toggle.isOn &&
                !AudioManager.Instance.source.isPlaying)
            {
                Rewind();
            }
        }
    }

    public void SetSong()
    {
        songGauge.maxValue = AudioManager.Instance.source.clip.length;
    }

    public void ChangeGauge()
    {
        if (duplication)
        {
            duplication = false;
            return;
        }
        AudioManager.Instance.source.time = songGauge.value;
        MapManager.Instance.SetObjectPosition(EditManager.Instance.control.transform, AudioManager.Instance.source.time);
        MapManager.Instance.SetTriangle(AudioManager.Instance.source.time);
        duplication = true;
    }

    public void PlayOrPause()
    {
        if (AudioManager.Instance.source.isPlaying)
        {
            AudioManager.Instance.source.Pause();
        }
        else
        {
            AudioManager.Instance.source.Play();
        }
    }

    public void CanMove()
    {
        if (AudioManager.Instance.source.isPlaying)
        {
            EditManager.Instance.control.canMove = true;
            return;
        }
        EditManager.Instance.control.canMove = false;
    }

    public void Rewind()
    {
        toggle.isOn = false;
        EditManager.Instance.control.canMove = false;
        AudioManager.Instance.source.Pause();
        AudioManager.Instance.source.time = 0.0f;
        MapManager.Instance.ResetObjectPosition(EditManager.Instance.control.transform);
        MapManager.Instance.SetTriangle(AudioManager.Instance.source.time);
    }

    public void Backward()
    {
        float speed = EditManager.Instance.control.moveSpeed;
        float time = ((int)(AudioManager.Instance.source.time / speed) - 1) * speed;
        time = time < 0 ? 0 : time;

        AudioManager.Instance.source.time = time;
        MapManager.Instance.SetObjectPosition(EditManager.Instance.control.transform, AudioManager.Instance.source.time);
        MapManager.Instance.SetTriangle(AudioManager.Instance.source.time);
    }

    public void Forward()
    {
        float speed = EditManager.Instance.control.moveSpeed;
        float time = ((int)(AudioManager.Instance.source.time / speed) + 1) * speed;
        time = time > AudioManager.Instance.source.clip.length ? AudioManager.Instance.source.clip.length : time;

        AudioManager.Instance.source.time = time;
        MapManager.Instance.SetObjectPosition(EditManager.Instance.control.transform, AudioManager.Instance.source.time);
        MapManager.Instance.SetTriangle(AudioManager.Instance.source.time);
    }
}