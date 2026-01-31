using UnityEngine;

public class FearMask : MaskAbility
{
    private void Awake()
    {
        MaskType = MaskType.Fear;
    }

    public override void OnEquip(PlayerController player)
    {
        player.EnableDash(true);
    }

    public override void OnUnequip(PlayerController player)
    {
        player.EnableDash(false);
    }
}
