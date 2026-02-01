using UnityEngine;

public class AngerMask : MaskAbility
{
    private void Awake()
    {
        MaskType = MaskType.Anger;
    }

    public override void OnEquip(PlayerController player)
    {
        player.EnablePush(true);
    }

    public override void OnUnequip(PlayerController player)
    {
        player.EnablePush(false);
    }
}
