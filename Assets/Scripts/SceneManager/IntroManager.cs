using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public static IntroManager Instance { get; private set; }

    [SerializeField]
    private GameObject[] dontDestroyObjects;

    void Awake()
    {
        Instance = FindObjectOfType<IntroManager>();
        if (Instance != this)
        {
            Destroy(this);
        }
    }

    void Start()
    {
        foreach (GameObject gameObject in dontDestroyObjects)
        {
            DontDestroyOnLoad(gameObject);
        }

        OptionManager option = GetComponent<OptionManager>();
        option.LoadOption();

        SceneManager.LoadScene("Main");
    }
}
