using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour
{
    public static MaskManager Instance { get; private set; }

    [SerializeField] private PlayerController _player;

    private List<MaskType> _unlockedMasks = new();

    [SerializeField] private int _currentIndex = -1;

    public event System.Action<MaskType> OnMaskChanged;

    private MaskAbility _currentMask;

    private void Awake()
    {
        Instance = this;
    }

    public void UnlockMask(MaskType type)
    {
        if (_unlockedMasks.Contains(type))
            return;

        _unlockedMasks.Add(type);

        if (_currentIndex == -1)
        {
            _currentIndex = 0;
            EquipCurrent();
        }
    }

    public void SwitchMask(int direction)
    {
        if (_unlockedMasks.Count <= 1)
            return;

        _currentIndex += direction;

        if (_currentIndex < 0)
            _currentIndex = _unlockedMasks.Count - 1;
        else if (_currentIndex >= _unlockedMasks.Count)
            _currentIndex = 0;

        EquipCurrent();
    }

    private void EquipCurrent()
    {
        MaskType type = _unlockedMasks[_currentIndex];

        if (_currentMask != null)
        {
            _currentMask.OnUnequip(_player);
        }

        _currentMask = GetAbility(type);

        if (_currentMask == null)
        {
            Debug.LogError($"MaskAbility manquante pour {type}");
            return;
        }

        _currentMask.OnEquip(_player);
        OnMaskChanged?.Invoke(_currentMask.MaskType);
    }


    private MaskAbility GetAbility(MaskType type)
    {
        foreach (var ability in GetComponents<MaskAbility>())
        {
            if (ability.MaskType == type)
                return ability;
        }

        return null;
    }
}
