using UnityEngine;

public class JoyMask : MaskAbility
{
    private void Awake()
    {
        MaskType = MaskType.Joy;
    }

    public override void OnEquip(PlayerController player)
    {
        player.EnableDoubleJump(true);
    }

    public override void OnUnequip(PlayerController player)
    {
        player.EnableDoubleJump(false);
    }
}
