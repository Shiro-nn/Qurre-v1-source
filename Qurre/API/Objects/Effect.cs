using CustomPlayerEffects;
using Qurre.API.Objects;
using System;
namespace Qurre.API
{
    public static class Effect
    {
        public static Type Type(this EffectType effect)
        {
            return effect switch
            {
                EffectType.Amnesia => typeof(Amnesia),
                EffectType.Asphyxiated => typeof(Asphyxiated),
                EffectType.Bleeding => typeof(Bleeding),
                EffectType.Blinded => typeof(Blinded),
                EffectType.Burned => typeof(Burned),
                EffectType.Concussed => typeof(Concussed),
                EffectType.Corroding => typeof(Corroding),
                EffectType.Deafened => typeof(Deafened),
                EffectType.Decontaminating => typeof(Decontaminating),
                EffectType.Disabled => typeof(Disabled),
                EffectType.Ensnared => typeof(Ensnared),
                EffectType.Exhausted => typeof(Exhausted),
                EffectType.Flashed => typeof(Flashed),
                EffectType.Hemorrhage => typeof(Hemorrhage),
                EffectType.Invigorated => typeof(Invigorated),
                EffectType.BodyshotReduction => typeof(BodyshotReduction),
                EffectType.Poisoned => typeof(Poisoned),
                EffectType.Scp207 => typeof(Scp207),
                EffectType.Invisible => typeof(Invisible),
                EffectType.SinkHole => typeof(SinkHole),
                EffectType.Visuals939 => typeof(Visuals939),
                EffectType.DamageReduction => typeof(DamageReduction),
                EffectType.MovementBoost => typeof(MovementBoost),
                EffectType.RainbowTaste => typeof(RainbowTaste),
                EffectType.SeveredHands => typeof(SeveredHands),
                EffectType.Stained => typeof(Stained),
                EffectType.Visual173Blink => typeof(Visuals173Blink),
                EffectType.Vitality => typeof(Vitality),
                _ => throw new InvalidOperationException("Invalid effect enum provided"),
            };
        }
    }
}