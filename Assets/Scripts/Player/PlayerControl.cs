using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [HideInInspector]
    public bool canMove = false;

    [HideInInspector]
    public float moveSpeed = 2.0f;
    // 반바퀴 돌때 걸리는 시간

    void Update()
    {
        if (!canMove)
        {
            return;
        }

        InputControll();

        MapManager.Instance.SetObjectPosition(transform, AudioManager.Instance.source.time);
    }

    private void InputControll()
    {
        if (Input.GetKey(Key.Instance.playerIn))
        {
            MapManager.Instance.SwapObjectKind(transform, -1);
        }
        if (Input.GetKey(Key.Instance.playerOut))
        {
            MapManager.Instance.SwapObjectKind(transform, 1);
        }

        if (Input.GetKeyDown(Key.Instance.playerShift))
        {
            MapManager.Instance.SwapObjectKind(transform);
        }
    }
}
