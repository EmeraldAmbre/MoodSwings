using UnityEngine;

public class PlayerTrapHitBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Trap"))
            return;

        if (PlayerManager.Instance.IsPlayerDead)
            return;

        PlayerManager.Instance.StartDeathSequence();
    }
}
