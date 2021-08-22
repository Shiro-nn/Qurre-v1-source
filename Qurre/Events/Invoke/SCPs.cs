using Qurre.API.Events;
using static Qurre.Events.Scp049;
using static Qurre.Events.Scp079;
using static Qurre.Events.Scp096;
using static Qurre.Events.Scp106;
using static Qurre.Events.Scp173;
using static Qurre.Events.Scp914;
namespace Qurre.Events.Invoke
{
    #region Scp049
    public static class Scp049
    {
        public static void StartRecall(StartRecallEvent ev) => Invokes(ev);
        public static void FinishRecall(FinishRecallEvent ev) => Invokes(ev);
    }
    #endregion
    #region Scp079
    public static class Scp079
    {
        public static void GeneratorActivate(GeneratorActivateEvent ev) => Invokes(ev);
        public static void GetEXP(GetEXPEvent ev) => Invokes(ev);
        public static void GetLVL(GetLVLEvent ev) => Invokes(ev);
    }
    #endregion
    #region Scp096
    public static class Scp096
    {
        public static void Enrage(EnrageEvent ev) => Invokes(ev);
        public static void Windup(WindupEvent ev) => Invokes(ev);
        public static void CalmDown(CalmDownEvent ev) => Invokes(ev);
        public static void AddTarget(AddTargetEvent ev) => Invokes(ev);
    }
    #endregion
    #region Scp106
    public static class Scp106
    {
        public static void PortalUsing(PortalUsingEvent ev) => Invokes(ev);
        public static void PortalCreate(PortalCreateEvent ev) => Invokes(ev);
        public static void Contain(ContainEvent ev) => Invokes(ev);
        public static void FemurBreakerEnter(FemurBreakerEnterEvent ev) => Invokes(ev);
        public static void PocketDimensionEnter(PocketDimensionEnterEvent ev) => Invokes(ev);
        public static void PocketDimensionEscape(PocketDimensionEscapeEvent ev) => Invokes(ev);
        public static void PocketDimensionFailEscape(PocketDimensionFailEscapeEvent ev) => Invokes(ev);
    }
    #endregion
    #region Scp173
    public static class Scp173
    {
        public static void Blink(BlinkEvent ev) => Invokes(ev);
    }
    #endregion
    #region Scp914
    public static class Scp914
    {
        public static void Activating(ActivatingEvent ev) => Invokes(ev);
        public static void Upgrade(UpgradeEvent ev) => Invokes(ev);
        public static void UpgradePlayer(UpgradePlayerEvent ev) => Invokes(ev);
        public static void UpgradePickup(UpgradePickupEvent ev) => Invokes(ev);
    }
    #endregion
}