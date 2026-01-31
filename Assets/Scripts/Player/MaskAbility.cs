using UnityEngine;

public abstract class MaskAbility : MonoBehaviour
{
    public MaskType MaskType { get; protected set; }
    public virtual void OnEquip(PlayerController player) { }
    public virtual void OnUnequip(PlayerController player) { }
}
