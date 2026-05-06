public class WargSteakItem : Item
{
    public override void Pickup()
    {
        SoundEffectManager.Play("victory");
        base.Pickup();
    }
}
