using UnityEngine;

public enum BuffTier { None, Tier1, Tier2, Tier3 }

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance;

    public BuffTier luckTier = BuffTier.None;
    public BuffTier cashoutTier = BuffTier.None;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    public int GetLuckBonusWeight()
    {
        return luckTier switch
        {
            BuffTier.Tier1 => 1,
            BuffTier.Tier2 => 2,
            BuffTier.Tier3 => 5,
            _              => 0
        };
    }

    public int GetDiceLuckBonus()
    {
        return luckTier switch
        {
            BuffTier.Tier1 => 1,
            BuffTier.Tier2 => 2,
            BuffTier.Tier3 => 3,
            _              => 0
        };
    }

    public float GetWheelLuckBonus()
    {
        return luckTier switch
        {
            BuffTier.Tier1 => 0.05f,
            BuffTier.Tier2 => 0.10f,
            BuffTier.Tier3 => 0.20f,
            _              => 0f
        };
    }


    public float GetCashoutMultiplier()
    {
        return cashoutTier switch
        {
            BuffTier.Tier1 => 2f,
            BuffTier.Tier2 => 3f,
            BuffTier.Tier3 => 4f,
            _              => 1f
        };
    }

    public int ApplyCashout(int delta)
    {
        if (delta <= 0) return delta;
        return Mathf.RoundToInt(delta * GetCashoutMultiplier());
    }
}
