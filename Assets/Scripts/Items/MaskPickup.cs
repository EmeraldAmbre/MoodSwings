using UnityEngine;

public class MaskPickup : MonoBehaviour
{
    [SerializeField] private MaskAbility _maskAbility;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        MaskManager.Instance.UnlockMask(_maskAbility);
        Destroy(gameObject);
    }
}
