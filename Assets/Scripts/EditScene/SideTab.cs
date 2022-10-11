using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SideTab : MonoBehaviour
{
    public Slider speedSlider;
    [SerializeField]
    private Text speedText;

    [SerializeField]
    private Slider tempoSlider;
    [SerializeField]
    private Text tempoText;

    [SerializeField]
    private float toggleSpeed;
    private bool isOpen = false;
    Coroutine openning;

    void Start()
    {
        speedText.text = EditManager.Instance.control.moveSpeed.ToString();
        tempoText.text = "None";
    }

    public void Toggle()
    {
        if (openning != null)
        {
            StopCoroutine(openning);
        }
        openning = StartCoroutine(ToggleSideTab(isOpen));
        isOpen = !isOpen;
    }

    public void SetPlayerSpeed()
    {
        EditManager.Instance.control.moveSpeed = speedSlider.value;
        speedText.text = speedSlider.value.ToString();

        MapManager.Instance.SetObjectPosition(EditManager.Instance.control.transform, AudioManager.Instance.source.time);
        MapManager.Instance.SetTriangle(AudioManager.Instance.source.time);
    }

    public void SetTempo()
    {
        EditManager.Instance.creator.tempo = (int)tempoSlider.value;
        if (tempoSlider.value == 0)
        {
            tempoText.text = "None";
        }
        else
        {
            tempoText.text = "1/" + (int)tempoSlider.value * 2;
        }
    }

    IEnumerator ToggleSideTab(bool isOpen)
    {
        RectTransform rect = GetComponent<RectTransform>();
        float delta = rect.rect.width * rect.lossyScale.x, move = toggleSpeed * Time.smoothDeltaTime;

        while (delta > 0.0f)
        {
            transform.Translate(Vector3.right * (isOpen ? -1 : 1) * move);
            delta -= move;

            yield return new WaitForFixedUpdate();
        }
        transform.Translate(Vector3.right * (isOpen ? 1 : -1) * -delta);
    }
}
