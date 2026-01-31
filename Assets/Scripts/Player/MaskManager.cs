using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour
{
    public static MaskManager Instance { get; private set; }

    [SerializeField] private PlayerController _player;
    [SerializeField] private MaskAbility[] _maskAbilities;

    private Dictionary<MaskType, MaskAbility> _maskMap = new();
    private MaskAbility _currentMask;

    private void Awake()
    {
        Instance = this;

        foreach (var mask in _maskAbilities)
        {
            _maskMap.Add(mask.MaskType, mask);
        }
    }

    public void UnlockMask(MaskType type)
    {
        if (!_maskMap.ContainsKey(type))
            return;

        if (_currentMask is null)
        {
            EquipMask(type);
        }
    }

    public void EquipMask(MaskType type)
    {
        if (_currentMask is not null)
            _currentMask.OnUnequip(_player);

        _currentMask = _maskMap[type];
        _currentMask.OnEquip(_player);
    }
}
