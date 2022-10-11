using UnityEngine;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance { get; private set; }

    private Text comboText;
    private int combo = 0;
    public int Combo
    {
        get { return combo; }
        set
        {
            if (combo == value)
            {
                return;
            }

            combo = value;
            comboText.text = combo.ToString() + " Combo";
            comboText.GetComponent<Animator>().SetTrigger("ComboUp");

            if(combo > maxCombo)
            {
                maxCombo = combo;
            }
        }
    }
    [HideInInspector]
    public int maxCombo = 0;
    
    public int death { get; set; }

    void Awake()
    {
        Instance = FindObjectOfType<ComboManager>();
        if (Instance != this)
        {
            Destroy(this);
        }
    }

    void Start()
    {
        comboText = GetComponent<Text>();

        death = 0;
    }

    public void ResetScore()
    {
        Combo = 0;
        maxCombo = 0;
        death = 0;
    }
}
