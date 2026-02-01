using UnityEngine;
using UnityEngine.UI;

public class MaskSlotUI : MonoBehaviour
{
    [SerializeField] private MaskType _maskType;
    [SerializeField] private Image _background;

    [Header("Colors")]
    [SerializeField] private Color _inactiveColor = Color.gray;
    [SerializeField] private Color _activeColor = Color.white;

    private void Awake()
    {
        SetActive(false);
    }

    private void Start()
    {
        if (MaskManager.Instance != null)
            MaskManager.Instance.OnMaskChanged += OnMaskChanged;
    }

    private void OnDestroy()
    {
        if (MaskManager.Instance != null)
            MaskManager.Instance.OnMaskChanged -= OnMaskChanged;
    }

    private void OnMaskChanged(MaskType activeMask)
    {
        SetActive(activeMask == _maskType);
    }

    private void SetActive(bool value)
    {
        _background.color = value ? _activeColor : _inactiveColor;
    }
}
