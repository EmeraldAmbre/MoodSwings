using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour
{
    public static MaskManager Instance { get; private set; }

    [SerializeField] private PlayerController _player;

    private Dictionary<MaskType, MaskAbility> _unlockedMasks = new();
    private MaskAbility _currentMask;

    private void Awake()
    {
        Instance = this;
    }

    public void UnlockMask(MaskAbility mask)
    {
        if (_unlockedMasks.ContainsKey(mask.MaskType))
            return;

        _unlockedMasks.Add(mask.MaskType, mask);

        if (_currentMask == null)
        {
            EquipMask(mask.MaskType);
        }
    }

    public void EquipMask(MaskType type)
    {
        if (!_unlockedMasks.ContainsKey(type))
            return;

        if (_currentMask != null)
        {
            _currentMask.OnUnequip(_player);
        }

        _currentMask = _unlockedMasks[type];
        _currentMask.OnEquip(_player);
    }
}
