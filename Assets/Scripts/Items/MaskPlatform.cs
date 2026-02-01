using UnityEngine;

public class MaskPlatform : MonoBehaviour
{
    [SerializeField] private MaskType _requiredMask;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Collider2D _collider;

    private void Awake()
    {
        SetActive(false);
    }

    private void OnEnable()
    {
        MaskManager.Instance.OnMaskChanged += OnMaskChanged;
    }

    private void OnDisable()
    {
        if (MaskManager.Instance != null)
            MaskManager.Instance.OnMaskChanged -= OnMaskChanged;
    }

    private void OnMaskChanged(MaskType activeMask)
    {
        SetActive(activeMask == _requiredMask);
    }

    private void SetActive(bool value)
    {
        _spriteRenderer.enabled = value;
        _collider.enabled = value;
    }
}
