using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    #region __오브젝트 풀 변수__

    private List<GameObject> pool;
    public int defaultSize;
    public GameObject prefab;
    public GameObject prefabLast;

    #endregion

    #region __맵 스택 변수__

    [HideInInspector]
    public List<Triangle> map = new List<Triangle>();
    public struct Triangle
    {
        public float time;
        public Kind kind;
        public GameObject poolObject;
    }
    public enum Kind
    {
        Out, In
    }

    #endregion

    [HideInInspector]
    public string songName;

    private PlayerControl control;

    private float pastTime = 0.0f;

    void Awake()
    {
        Instance = FindObjectOfType<MapManager>();
        if (Instance != this)
        {
            Destroy(this);
        }

        pool = new List<GameObject>(defaultSize);
    }

    void Start()
    {
        InitPool();
    }

    void Update()
    {
        UpdateTriangle();
    }

    public void Init()
    {
        control = FindObjectOfType<PlayerControl>();
        control.canMove = true;
    }

    #region __오브젝트 풀 함수__

    private void InitPool()
    {
        for (int count = 0; count < defaultSize; count++)
        {
            AddPool();
        }
    }

    private GameObject AddPool()
    {
        GameObject temp = Instantiate(prefab, gameObject.transform);
        temp.SetActive(false);
        pool.Add(temp);
        return temp;
    }

    private void PushPool(int mapCount)
    {
        foreach (GameObject poolObject in pool)
        {
            if (!poolObject.activeSelf)
            {
                SetPoolObject(poolObject, mapCount);
                return;
            }
        }

        SetPoolObject(AddPool(), mapCount);
    }

    private int PopPool(GameObject poolObject)
    {
        int mapCount = SearchMap(poolObject);
        if (mapCount < 0)
        {
            return -1;
        }

        Triangle temp = map[mapCount];
        temp.poolObject = null;
        map[mapCount] = temp;

        poolObject.SetActive(false);

        return mapCount;
    }

    public void ResetPool()
    {
        foreach (GameObject poolObject in pool)
        {
            PopPool(poolObject);
        }
    }

    private void SetPoolObject(GameObject poolObject, int mapCount)
    {
        SetObjectPosition(poolObject.transform, map[mapCount].time);
        SwapObjectKind(poolObject.transform, map[mapCount].kind);

        poolObject.name = mapCount.ToString();

        Triangle temp = map[mapCount];
        temp.poolObject = poolObject;
        map[mapCount] = temp;

        poolObject.SetActive(true);

        if (GameManager.Instance)
        {
            poolObject.GetComponent<Animator>().SetBool("isLast", mapCount == map.Count - 1);
        }
    }

    #endregion

    #region __맵 스택 함수__

    public void LoadMap()
    {
        ResetMap();

        FileStream f = new FileStream(Application.dataPath + "/../Songs/" + songName + "/Map", FileMode.OpenOrCreate, FileAccess.Read);
        StreamReader reader = new StreamReader(f, System.Text.Encoding.Default);
        string[] value;
        for (string str = reader.ReadLine(); str != null; str = reader.ReadLine())
        {
            value = str.Split('|');

            Triangle triangle = new Triangle
            {
                time = float.Parse(value[0]),
                kind = (Kind)System.Enum.Parse(typeof(Kind), value[1]),
                poolObject = null
            };

            map.Add(triangle);
        }
        reader.Close();
    }

    public void SaveMap()
    {
        FileStream f = new FileStream(Application.dataPath + "/../Songs/" + songName + "/Map", FileMode.Create, FileAccess.Write);
        StreamWriter writer = new StreamWriter(f, System.Text.Encoding.Default);
        foreach (Triangle triangle in map)
        {
            writer.WriteLine("{0}|{1}", triangle.time, triangle.kind);
        }
        writer.Close();
    }

    public void ResetMap()
    {
        map.Clear();
    }

    private int SearchMap(float time)
    {
        if (map.Count == 0)
        {
            return -1;
        }

        for (int count = 0; count < map.Count; count++)
        {
            if (map[count].time >= time)
            {
                return count;
            }
        }
        return map.Count;
    }

    private int SearchMap(GameObject poolObject)
    {
        for (int count = 0; count < map.Count; count++)
        {
            if (map[count].poolObject == poolObject)
            {
                return count;
            }
        }
        return -1;
    }

    #endregion

    #region __오브젝트 함수__

    public void ResetObjectPosition(Transform transform)
    {
        transform.SetPositionAndRotation(Vector3.up * 297.0f, Quaternion.identity);
    }

    public void SetObjectPosition(Transform transform, float time)
    {
        ResetObjectPosition(transform);
		if (control != null)
		{
			transform.RotateAround (Vector3.zero, Vector3.back, time / (control.moveSpeed * 2.0f) * 360.0f);
		}
    }

    public void SwapObjectKind(Transform transform, Kind kind)
    {
        Vector3 scale = transform.localScale;
        scale.y = (int)kind % 2 == 0 ? 1 : -1;
        transform.localScale = scale;
    }

    public void SwapObjectKind(Transform transform, float value)
    {
        Vector3 scale = transform.localScale;
        scale.y = value;
        transform.localScale = scale;
    }

    public void SwapObjectKind(Transform transform)
    {
        Vector3 scale = transform.localScale;
        scale.y *= -1;
        transform.localScale = scale;
    }

    #endregion

    #region __진행 함수__

    public float GetCurrentChapter()
    {
        return (int)(AudioManager.Instance.source.time / (control.moveSpeed * 0.5f)) * control.moveSpeed * 0.5f;
    }

    public void SetTriangle(float time)
    {
        ResetPool();

        AudioManager.Instance.source.time = time;
        pastTime = time;

        float startTime = Mathf.Clamp(GetCurrentChapter() - control.moveSpeed * 0.5f, 0.0f, AudioManager.Instance.source.clip.length - control.moveSpeed * 2.0f);

        if (EditManager.Instance)
        {
            SetObjectPosition(EditManager.Instance.creator.startLine.transform, startTime);
        }

        //int setCount = 0;

        float endTime = (GetCurrentChapter() - control.moveSpeed * 0.5f) < 0.0f ? 1.5f : 2.0f;
        for (int count = SearchMap(startTime), endCount = SearchMap(startTime + control.moveSpeed * endTime); count >= 0 && count < endCount; count++)
        {
            PushPool(count);

            //setCount++;
        }

        //Debug.Log("Set : " + startTime + " ~ " + (startTime + control.moveSpeed * 2.0f) + "(" + setCount + ")");
    }

    private void UpdateTriangle()
    {
        if (AudioManager.Instance.source.clip &&
            AudioManager.Instance.source.time % (control.moveSpeed * 0.5f) < pastTime % (control.moveSpeed * 0.5f))
        {
            float currentTime = GetCurrentChapter();
            if (currentTime < control.moveSpeed * 0.5f ||
                currentTime > AudioManager.Instance.source.clip.length - control.moveSpeed)
            {
                return;
            }

            if (EditManager.Instance)
            {
                SetObjectPosition(EditManager.Instance.creator.startLine.transform, currentTime - control.moveSpeed * 0.5f);
            }

            //int deleteCount = 0, createCount = 0;

            for (int count = SearchMap(currentTime - control.moveSpeed), endCount = SearchMap(currentTime - control.moveSpeed * 0.5f);
                count >= 0 && count < endCount; count++)
            {
                PopPool(map[count].poolObject);

                //deleteCount++;
            }
            for (int count = SearchMap(currentTime + control.moveSpeed), endCount = SearchMap(currentTime + control.moveSpeed * 1.5f);
                count >= 0 && count < endCount; count++)
            {
                PushPool(count);

                //createCount++;
            }

            //Debug.Log("Time : " + AudioManager.Instance.source.time + "    " +
            //    "Delete : " + (currentTime - control.moveSpeed) + " ~ " + (currentTime - control.moveSpeed * 0.5f) + "(" + deleteCount + ")    " +
            //    "Create : " + (currentTime + control.moveSpeed) + " ~ " + (currentTime + control.moveSpeed * 1.5f + "(" + createCount + ")"));
        }
        pastTime = AudioManager.Instance.source.time;
    }

    #endregion

    #region __에디터 함수__

    public float GetTimeByQuarternion(Quaternion quaternion)
    {
        float startTime = Mathf.Clamp(GetCurrentChapter() - control.moveSpeed * 0.5f, 0.0f, AudioManager.Instance.source.clip.length - control.moveSpeed * 2.0f);
        float startAngle = (startTime / (control.moveSpeed * 0.5f) % 4.0f) * 90.0f;

        quaternion = quaternion * Quaternion.AngleAxis(startAngle, Vector3.forward);

        return startTime + (360.0f - quaternion.eulerAngles.z) * control.moveSpeed / 180.0f; // control.moveSpeed * 0.5f / 90.0f
    }

    public GameObject CreateTriangle(float time, Kind kind)
    {
        int mapCount = SearchMap(time);
        mapCount = mapCount < 0 ? 0 : mapCount;
        Triangle triangle = new Triangle
        {
            time = time,
            kind = kind,
            poolObject = null
        };

        map.Insert(mapCount, triangle);
        PushPool(mapCount);

        return map[mapCount].poolObject;

        //Debug.Log("Add : " + time + "[" + mapCount + "]");
    }

    public void RemoveTriangle(GameObject removeObject)
    {
        int remove = PopPool(removeObject);
        map.RemoveAt(remove);
    }

    #endregion
}