using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NoteCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject guide;
    [SerializeField]
    private Image triangleIn;
    [SerializeField]
    private Image triangle;
    [SerializeField]
    private Image line;

    public Image startLine;

    [HideInInspector]
    public int tempo;
    private MapManager.Kind kind;
    //private MapManager.Kind kind = MapManager.Kind.Out;
    
    private GameObject removeObject = null;
    
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() ||
            EditManager.Instance.record)
        {
            guide.SetActive(false);
            return;
        }
        guide.SetActive(true);

        Vector3 position = Input.mousePosition - Vector3.right * Screen.width * 0.5f - Vector3.up * Screen.height * 0.5f;
        Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, position);

        quaternion = MoveGuide(quaternion);

        SetMode(position);

        if (Input.GetMouseButtonDown(0))
        {
            if (removeObject == null)
            {
                MapManager.Instance.CreateTriangle(MapManager.Instance.GetTimeByQuarternion(quaternion), kind);
            }
            else
            {
                MapManager.Instance.RemoveTriangle(removeObject);
            }
        }
    }

    private void OnMouseEnter()
    {
        SetKind(MapManager.Kind.In);
    }

    private void OnMouseExit()
    {
        SetKind(MapManager.Kind.Out);
    }

    private void SetKind(MapManager.Kind kind)
    {
        this.kind = kind;

        MapManager.Instance.SwapObjectKind(triangleIn.transform);
        MapManager.Instance.SwapObjectKind(triangle.transform);
    }

    /*  우클릭 전환 코드  반지름 340
    private void OnMouseOver()
    {
        if(EventSystem.current.IsPointerOverGameObject())
        {
            guide.SetActive(false);
            return;
        }
        guide.SetActive(true);

        Vector3 position = Input.mousePosition - Vector3.right * Screen.width * 0.5f - Vector3.up * Screen.height * 0.5f;
        Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, position);

        quaternion = MoveGuide(quaternion);

        SetMode(position);

        if (Input.GetMouseButtonDown(0))
        {
            if (removeObject == null)
            {
                MapManager.Instance.CreateTriangle(quaternion, kind);
            }
            else
            {
                MapManager.Instance.RemoveTriangle(removeObject);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            kind++;

            MapManager.Instance.SwapObjectKind(triangleIn.transform);
            MapManager.Instance.SwapObjectKind(triangle.transform);
        }
    }

    private void OnMouseExit()
    {
        guide.SetActive(false);
    }
    */

    private Quaternion MoveGuide(Quaternion quaternion)
    {
        if (tempo > 0)
        {
            guide.transform.rotation = Quaternion.Euler(Vector3.back * Mathf.Round((360.0f - quaternion.eulerAngles.z) / (90.0f / (tempo * 2.0f))) * (90.0f / (tempo * 2.0f)));

            //Debug.Log(Mathf.Round((360.0f - quaternion.eulerAngles.z) / (90.0f / (tempo * 2.0f))) * (90.0f / (tempo * 2.0f)));
        }
        else
        {
            guide.transform.rotation = quaternion;

            //Debug.Log(360.0f - quaternion.eulerAngles.z);
        }
        return guide.transform.rotation;
    }

    private void SetMode(Vector3 position)
    {
        RaycastHit2D[] raycasts = 
            Physics2D.BoxCastAll(triangle.transform.position, Vector2.right * 17.0f + Vector2.up * 34.0f, 0.0f, triangle.transform.localRotation.eulerAngles, 0.0f, 1 << LayerMask.NameToLayer("Enemy"));
        if (raycasts.Length > 0)
        {
            removeObject = CalcRemoveObject(position, raycasts);

            line.color = Color.red;
            triangle.color = Color.red;
        }
        else
        {
            removeObject = null;

            line.color = Color.black;
            triangle.color = Color.black;
        }
    }

    private GameObject CalcRemoveObject(Vector3 position, RaycastHit2D[] raycasts)
    {
        Transform removeTransform = null;
        float removeDistance = -1.0f, distance;
        foreach (RaycastHit2D raycast in raycasts)
        {
            distance = Vector3.Distance(position, raycast.transform.position);

            if (removeDistance < 0.0f ||
                distance < removeDistance)
            {
                removeTransform = raycast.transform;
                removeDistance = distance;
            }
        }

        return removeTransform.gameObject;
    }
}
