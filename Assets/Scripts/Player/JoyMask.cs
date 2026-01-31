using UnityEngine;

public class JoyMask : MaskAbility
{
    [SerializeField] private float _baseJumpForce = 12f;
    [SerializeField] private float _boostedJumpForce = 13.5f;

    private void Awake()
    {
        MaskType = MaskType.Joy;
    }

    public override void OnEquip(PlayerController player)
    {
        player.EnableDoubleJump(true);
        player.ModifyJumpForce(_boostedJumpForce);
    }

    public override void OnUnequip(PlayerController player)
    {
        player.EnableDoubleJump(false);
        player.ModifyJumpForce(_baseJumpForce);
    }
}
