using UnityEngine;

public class Key : MonoBehaviour
{
    public static Key Instance { get; private set; }

    #region __키 변수__

    public KeyCode playerOut = KeyCode.UpArrow;
    public KeyCode playerIn = KeyCode.DownArrow;
    public KeyCode playerShift = KeyCode.Space;

    public KeyCode editPlayPause = KeyCode.P;
    public KeyCode editRewind = KeyCode.Backspace;
    public KeyCode editRecord = KeyCode.R;
    public KeyCode editBackward = KeyCode.LeftArrow;
    public KeyCode editForward = KeyCode.RightArrow;

    public KeyCode menu = KeyCode.Escape;
    
    #endregion

    void Awake()
    {
        Instance = FindObjectOfType<Key>();
        if (Instance != this)
        {
            Destroy(this);
        }
    }
}
