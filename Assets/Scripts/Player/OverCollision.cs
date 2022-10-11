using UnityEngine;

public class OverCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Enemy":
                if (ComboManager.Instance != null)
                {
                    ComboManager.Instance.Combo++; 
                }
                break;
        }
    }
}
