using UnityEngine;

public class MaskPickup : MonoBehaviour
{
    [SerializeField] private MaskType _maskType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        MaskManager.Instance.UnlockMask(_maskType);
        Destroy(gameObject);
    }
}
