using UnityEngine;

public class PlayerTrapHitBox : MonoBehaviour
{
    private bool _isDying = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDying)
            return;

        if (other.CompareTag("Trap"))
        {
            _isDying = true;
            PlayerManager.Instance.StartDeathSequence();
        }
    }
}
