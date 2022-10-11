using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Enemy":
                if (ComboManager.Instance != null)
                {
                    ComboManager.Instance.Combo = 0;
                    ComboManager.Instance.death++;
                }
                break;
        }
    }
}
