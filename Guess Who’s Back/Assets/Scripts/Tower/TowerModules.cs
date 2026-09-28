using System;
using UnityEngine;

[Serializable]
public abstract class TowerModule
{
    // Called for every enemy that gets hit (the main target AND anything caught in splash).
    // multiplier is 1 for a direct hit and lower for splash hits further from the centre.
    public virtual void OnHit(Tower tower, Enemy target, Vector3 hitFrom, float multiplier) { }

    // Called every frame an enemy stands on a trap tile (only used when the tower is a trap).
    public virtual void OnTrapStay(Tower tower, Enemy target) { }
    public abstract string GetDescription();
}

[Serializable]
public class KnockbackModule : TowerModule
{
    public float distance = 1f;

    public override void OnHit(Tower tower, Enemy target, Vector3 hitFrom, float multiplier)
    {
        if (distance <= 0) return;
        Vector3 dir = (target.transform.position - hitFrom).normalized;
        target.ApplyKnockback(distance * multiplier, dir);
    }

    public override string GetDescription() => $"Knockback: {distance}";
}

[Serializable]
public class FreezeModule : TowerModule
{
    [Range(0, 100)] public float chance = 25f;
    public float duration = 1f;

    public override void OnHit(Tower tower, Enemy target, Vector3 hitFrom, float multiplier)
    {
        if (UnityEngine.Random.Range(0f, 100f) < chance)
            target.ApplyFreeze(duration);
    }

    public override string GetDescription() => $"Freeze: {chance}% ({duration}s)";
}

[Serializable]
public class SlowModule : TowerModule
{
    [Range(0, 100)] public float amount = 30f;
    public float duration = 2f;

    public override void OnHit(Tower tower, Enemy target, Vector3 hitFrom, float multiplier)
    {
        target.ApplySlow(amount * multiplier, duration);
    }

    public override void OnTrapStay(Tower tower, Enemy target)
    {
        target.ApplySlow(amount, 0.1f);
    }

    public override string GetDescription() => $"Slow: {amount}% ({duration}s)";
}

[Serializable]
public class DotModule : TowerModule
{
    public float damage = 5f;
    public float duration = 3f;
    public float tickRate = 0.5f;

    public override void OnHit(Tower tower, Enemy target, Vector3 hitFrom, float multiplier)
    {
        string sourceId = tower.gameObject.GetInstanceID().ToString();
        target.ApplyDot(damage * multiplier, duration, tickRate, sourceId);
    }

    public override string GetDescription() => $"DOT: {damage}/tick ({duration}s)";
}

[Serializable]
public class AoeModule : TowerModule
{
    public float radius = 1f;
    [Range(0, 100)] public float damageFalloff = 100f;
    public GameObject circlePrefab;

    public float GetMultiplier(float distance)
    {
        if (distance >= radius) return 0f;
        float falloff = damageFalloff / 100f;
        return 1f - (distance / radius) * (1f - falloff);
    }

    public override string GetDescription() => $"AOE: {radius} radius";
}