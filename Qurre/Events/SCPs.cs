using Qurre.Events.modules;
using Qurre.API.Events;
using static Qurre.Events.modules.Main;
namespace Qurre.Events
{
    #region Scp049
    public static class Scp049
    {
        public static event AllEvents<StartRecallEvent> StartRecall;
        public static event AllEvents<FinishRecallEvent> FinishRecall;
        internal static void Invokes(StartRecallEvent ev) => StartRecall.invoke(ev);
        internal static void Invokes(FinishRecallEvent ev) => FinishRecall.invoke(ev);
    }
    #endregion
    #region Scp079
    public static class Scp079
    {
        public static event AllEvents<GeneratorActivateEvent> GeneratorActivate;
        public static event AllEvents<GetEXPEvent> GetEXP;
        public static event AllEvents<GetLVLEvent> GetLVL;
        internal static void Invokes(GeneratorActivateEvent ev) => GeneratorActivate.invoke(ev);
        internal static void Invokes(GetEXPEvent ev) => GetEXP.invoke(ev);
        internal static void Invokes(GetLVLEvent ev) => GetLVL.invoke(ev);
    }
    #endregion
    #region Scp096
    public static class Scp096
    {
        public static event AllEvents<EnrageEvent> Enrage;
        public static event AllEvents<WindupEvent> Windup;
        public static event AllEvents<CalmDownEvent> CalmDown;
        public static event AllEvents<AddTargetEvent> AddTarget;
        internal static void Invokes(EnrageEvent ev) => Enrage.invoke(ev);
        internal static void Invokes(WindupEvent ev) => Windup.invoke(ev);
        internal static void Invokes(CalmDownEvent ev) => CalmDown.invoke(ev);
        internal static void Invokes(AddTargetEvent ev) => AddTarget.invoke(ev);
    }
    #endregion
    #region Scp106
    public static class Scp106
    {
        public static event AllEvents<PortalUsingEvent> PortalUsing;
        public static event AllEvents<PortalCreateEvent> PortalCreate;
        public static event AllEvents<ContainEvent> Contain;
        public static event AllEvents<FemurBreakerEnterEvent> FemurBreakerEnter;
        public static event AllEvents<PocketDimensionEnterEvent> PocketDimensionEnter;
        public static event AllEvents<PocketDimensionEscapeEvent> PocketDimensionEscape;
        public static event AllEvents<PocketDimensionFailEscapeEvent> PocketDimensionFailEscape;
        internal static void Invokes(PortalUsingEvent ev) => PortalUsing.invoke(ev);
        internal static void Invokes(PortalCreateEvent ev) => PortalCreate.invoke(ev);
        internal static void Invokes(ContainEvent ev) => Contain.invoke(ev);
        internal static void Invokes(FemurBreakerEnterEvent ev) => FemurBreakerEnter.invoke(ev);
        internal static void Invokes(PocketDimensionEnterEvent ev) => PocketDimensionEnter.invoke(ev);
        internal static void Invokes(PocketDimensionEscapeEvent ev) => PocketDimensionEscape.invoke(ev);
        internal static void Invokes(PocketDimensionFailEscapeEvent ev) => PocketDimensionFailEscape.invoke(ev);
    }
    #endregion
    #region Scp173
    public static class Scp173
    {
        public static event AllEvents<BlinkEvent> Blink;
        public static event AllEvents<TantrumPlaceEvent> TantrumPlace;
        internal static void Invokes(BlinkEvent ev) => Blink.invoke(ev);
        internal static void Invokes(TantrumPlaceEvent ev) => TantrumPlace.invoke(ev);
    }
    #endregion
    #region Scp914
    public static class Scp914
    {
        public static event AllEvents<ActivatingEvent> Activating;
        public static event AllEvents<KnobChangeEvent> KnobChange;
        public static event AllEvents<UpgradeEvent> Upgrade;
        public static event AllEvents<UpgradePlayerEvent> UpgradePlayer;
        public static event AllEvents<UpgradePickupEvent> UpgradePickup;
        internal static void Invokes(ActivatingEvent ev) => Activating.invoke(ev);
        internal static void Invokes(KnobChangeEvent ev) => KnobChange.invoke(ev);
        internal static void Invokes(UpgradeEvent ev) => Upgrade?.invoke(ev);
        internal static void Invokes(UpgradePlayerEvent ev) => UpgradePlayer?.invoke(ev);
        internal static void Invokes(UpgradePickupEvent ev) => UpgradePickup?.invoke(ev);
    }
    #endregion
}