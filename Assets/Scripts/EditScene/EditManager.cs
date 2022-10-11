using UnityEngine;
using UnityEngine.UI;

public class EditManager : MonoBehaviour
{
    public static EditManager Instance { get; private set; }

    [HideInInspector]
    public PlayerControl control;
    [HideInInspector]
    public SideTab sideTab;
    [HideInInspector]
    public NoteCreator creator;
    [HideInInspector]
    public EditMenu editMenu;

    [SerializeField]
    private Toggle toggle;
    [HideInInspector]
    public bool record = false;

    void Awake()
    {
        Instance = FindObjectOfType<EditManager>();
        if (Instance != this)
        {
            Destroy(this);
        }

        editMenu.gameObject.SetActive(true);
    }

    void Start()
    {
        control = FindObjectOfType<PlayerControl>();
        sideTab = FindObjectOfType<SideTab>();
        creator = FindObjectOfType<NoteCreator>();
        editMenu = FindObjectOfType<EditMenu>();

        control.canMove = false;

        MapManager.Instance.Init();
    }

    void Update()
    {
        if (Input.GetKeyDown(Key.Instance.menu))
        {
            editMenu.Back();
        }
        if (Input.GetKeyDown(Key.Instance.editRecord))
        {
            toggle.isOn = !toggle.isOn;
        }

        if (record)
        {
            AddTriangle();
        }
    }

    public void Record()
    {
        record = !record;
    }

    private void AddTriangle()
    {
        bool swap;
        MapManager.Kind kind = control.transform.localScale.y > 0 ? MapManager.Kind.Out : MapManager.Kind.In;
        if (Input.GetKeyDown(Key.Instance.playerOut))
        {
            if (kind == MapManager.Kind.Out)
            {
                swap = false;
            }
            else
            {
                swap = true;
            }
        }
        else if (Input.GetKeyDown(Key.Instance.playerIn))
        {
            if (kind == MapManager.Kind.In)
            {
                swap = false;
            }
            else
            {
                swap = true;
            }
        }
        else if (Input.GetKeyDown(Key.Instance.playerShift))
        {
            swap = true;
        }
        else
        {
            return;
        }

        GameObject poolObject;
        if (swap)
        {
            poolObject = MapManager.Instance.CreateTriangle(AudioManager.Instance.source.time + 0.2f / control.moveSpeed, kind);
        }
        else
        {
            poolObject = MapManager.Instance.CreateTriangle(AudioManager.Instance.source.time, kind == MapManager.Kind.Out ? MapManager.Kind.In : MapManager.Kind.Out);
        }

        RaycastHit2D[] raycasts =
            Physics2D.BoxCastAll(poolObject.transform.position, Vector2.right * 17.0f + Vector2.up * 34.0f, 0.0f, poolObject.transform.localRotation.eulerAngles, 0.0f, 1 << LayerMask.NameToLayer("Enemy"));
        foreach(RaycastHit2D cast in raycasts)
        {
            if(cast.transform != poolObject.transform)
            {
                MapManager.Instance.RemoveTriangle(cast.transform.gameObject);
            }
        }
    }
}
