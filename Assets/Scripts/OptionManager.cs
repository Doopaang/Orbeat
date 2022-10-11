using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class OptionManager : MonoBehaviour
{
    public static OptionManager Instance { get; private set; }

    #region __볼륨 변수__

    [SerializeField]
    private Slider volumeSlider;
    [SerializeField]
    private InputField volumeField;

    #endregion

    #region __키 변수__

    [SerializeField]
    private Text playerOut;
    [SerializeField]
    private Text playerIn;
    [SerializeField]
    private Text playerShift;

    [SerializeField]
    private Text editPlay;
    [SerializeField]
    private Text editRewind;
    [SerializeField]
    private Text editRecord;
    [SerializeField]
    private Text editBackward;
    [SerializeField]
    private Text editForward;

    [SerializeField]
    private GameObject keyScreen;

    #endregion
    
    void Awake()
    {
        Instance = FindObjectOfType<OptionManager>();
        if (Instance != this)
        {
            Destroy(this);
        }
    }

    void Start()
    {
        SetOptionUI();
    }

    #region __볼륨 함수__

    public void SetVolumeSlider()
    {
        volumeField.text = volumeSlider.value.ToString();
        AudioManager.Instance.source.volume = volumeSlider.value * 0.01f;
    }

    public void SetVolumeField()
    {
        volumeSlider.value = int.Parse(volumeField.text);
        AudioManager.Instance.source.volume = int.Parse(volumeField.text) * 0.01f;
    }

    #endregion

    #region __키 함수__
    
    public void SetKey(string keyKind)
    {
        keyScreen.SetActive(true);

        StartCoroutine(GetInput(keyKind));
    }

    private IEnumerator GetInput(string keyVar)
    {
        while (true)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode input in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(input))
                    {
                        SetInput(input, keyVar);
                        break;
                    }
                }
                break;
            }
            else
            {
                yield return null;
            }
        }
    }

    private void SetInput(KeyCode key, string keyVar)
    {
        keyScreen.SetActive(false);

        if (key == KeyCode.Escape)
        {
            return;
        }
        
        switch (keyVar)
        {
            case "PlayerOut":
                playerOut.text = key.ToString();
                Key.Instance.playerOut = key;
                break;

            case "PlayerIn":
                playerIn.text = key.ToString();
                Key.Instance.playerIn = key;
                break;

            case "PlayerShift":
                playerShift.text = key.ToString();
                Key.Instance.playerShift = key;
                break;

            case "EditPlay":
                editPlay.text = key.ToString();
                Key.Instance.editPlayPause = key;
                break;

            case "EditRewind":
                editRewind.text = key.ToString();
                Key.Instance.editRewind = key;
                break;

            case "EditRecord":
                editRecord.text = key.ToString();
                Key.Instance.editRecord = key;
                break;

            case "EditBackward":
                editBackward.text = key.ToString();
                Key.Instance.editBackward = key;
                break;

            case "EditForward":
                editForward.text = key.ToString();
                Key.Instance.editForward = key;
                break;
        }
    }

    #endregion

    #region __저장 함수__

    public void LoadOption()
    {
        FileStream f = new FileStream(Application.dataPath + "/../Option", FileMode.OpenOrCreate, FileAccess.Read);
        StreamReader reader = new StreamReader(f, System.Text.Encoding.Default);

        string str = reader.ReadToEnd();

        if (str.Length > 0)
        {
            string[] option = str.Split('\n');

            AudioManager.Instance.source.volume = int.Parse(option[0]);

            Key.Instance.playerOut = (KeyCode)System.Enum.Parse(typeof(KeyCode), option[1]);
            Key.Instance.playerIn = (KeyCode)System.Enum.Parse(typeof(KeyCode), option[2]);
            Key.Instance.playerShift = (KeyCode)System.Enum.Parse(typeof(KeyCode), option[3]);

            Key.Instance.editPlayPause = (KeyCode)System.Enum.Parse(typeof(KeyCode), option[4]);
            Key.Instance.editRewind = (KeyCode)System.Enum.Parse(typeof(KeyCode), option[5]);
            Key.Instance.editRecord = (KeyCode)System.Enum.Parse(typeof(KeyCode), option[6]);
            Key.Instance.editBackward = (KeyCode)System.Enum.Parse(typeof(KeyCode), option[7]);
            Key.Instance.editForward = (KeyCode)System.Enum.Parse(typeof(KeyCode), option[8]);
        }
        else
        {
            ResetOption();
        }

        reader.Close();
    }

    public void SaveOption()
    {
        FileStream f = new FileStream(Application.dataPath + "/../Option", FileMode.Create, FileAccess.Write);
        StreamWriter writer = new StreamWriter(f, System.Text.Encoding.Default);

        writer.WriteLine(volumeSlider.value * 100);

        writer.WriteLine(Key.Instance.playerOut);
        writer.WriteLine(Key.Instance.playerIn);
        writer.WriteLine(Key.Instance.playerShift);

        writer.WriteLine(Key.Instance.editPlayPause);
        writer.WriteLine(Key.Instance.editRewind);
        writer.WriteLine(Key.Instance.editRecord);
        writer.WriteLine(Key.Instance.editBackward);
        writer.WriteLine(Key.Instance.editForward);

        writer.Close();
    }

    public void ResetOption()
    {
        AudioManager.Instance.source.volume = 100;

        Key.Instance.playerOut = KeyCode.UpArrow;
        Key.Instance.playerIn = KeyCode.DownArrow;
        Key.Instance.playerShift = KeyCode.Space;

        Key.Instance.editPlayPause = KeyCode.P;
        Key.Instance.editRewind = KeyCode.Backspace;
        Key.Instance.editRecord = KeyCode.R;
        Key.Instance.editBackward = KeyCode.LeftArrow;
        Key.Instance.editForward = KeyCode.RightArrow;
    }

    #endregion

    public void SetOptionUI()
    {
        if(!volumeSlider)
        {
            return;
        }

        volumeSlider.value = AudioManager.Instance.source.volume * 100;

        SetInput(Key.Instance.playerOut, "PlayerOut");
        SetInput(Key.Instance.playerIn, "PlayerIn");
        SetInput(Key.Instance.playerShift, "PlayerShift");

        SetInput(Key.Instance.editPlayPause, "EditPlay");
        SetInput(Key.Instance.editRewind, "EditRewind");
        SetInput(Key.Instance.editRecord, "EditRecord");
        SetInput(Key.Instance.editBackward, "EditBackward");
        SetInput(Key.Instance.editForward, "EditForward");
    }

    public void Exit(string scene)
    {
        SceneManager.UnloadSceneAsync(scene);
    }
}
