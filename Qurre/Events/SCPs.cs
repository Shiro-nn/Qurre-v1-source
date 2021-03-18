using Qurre.Events.modules;
using Qurre.API.Events;
using static Qurre.Events.modules.main;
namespace Qurre.Events
{
    public static class SCPs
    {
        #region SCP914
        public static class SCP914
        {
            public static event AllEvents<ActivatingEvent> Activating;
            public static event AllEvents<ChangeKnobEvent> ChangeKnob;
            public static event AllEvents<UpgradeEvent> Upgrade;
            public static void activating(ActivatingEvent ev) => Activating.invoke(ev);
            public static void changeknob(ChangeKnobEvent ev) => ChangeKnob.invoke(ev);
            public static void upgrade(UpgradeEvent ev) => Upgrade.invoke(ev);
        }
        #endregion
        #region SCP173
        public static class SCP173
        {
            public static event AllEvents<BlinkEvent> Blink;
            public static void blink(BlinkEvent ev) => Blink.invoke(ev);
        }
        #endregion
        #region SCP106
        public static class SCP106
        {
            public static event AllEvents<PortalUsingEvent> PortalUsing;
            public static event AllEvents<PortalCreateEvent> PortalCreate;
            public static event AllEvents<ContainEvent> Contain;
            public static event AllEvents<FemurBreakerEnterEvent> FemurBreakerEnter;
            public static event AllEvents<PocketDimensionEnterEvent> PocketDimensionEnter;
            public static event AllEvents<PocketDimensionEscapeEvent> PocketDimensionEscape;
            public static event AllEvents<PocketDimensionFailEscapeEvent> PocketDimensionFailEscape;
            public static void portalusing(PortalUsingEvent ev) => PortalUsing.invoke(ev);
            public static void portalcreate(PortalCreateEvent ev) => PortalCreate.invoke(ev);
            public static void contain(ContainEvent ev) => Contain.invoke(ev);
            public static void femurbreakerenter(FemurBreakerEnterEvent ev) => FemurBreakerEnter.invoke(ev);
            public static void pocketdimensionenter(PocketDimensionEnterEvent ev) => PocketDimensionEnter.invoke(ev);
            public static void pocketdimensionescape(PocketDimensionEscapeEvent ev) => PocketDimensionEscape.invoke(ev);
            public static void pocketdimensionfailescape(PocketDimensionFailEscapeEvent ev) => PocketDimensionFailEscape.invoke(ev);
        }
        #endregion
        #region SCP096
        public static class SCP096
        {
            public static event AllEvents<EnrageEvent> Enrage;
            public static event AllEvents<CalmDownEvent> CalmDown;
            public static event AllEvents<AddTargetEvent> AddTarget;
            public static void enrage(EnrageEvent ev) => Enrage.invoke(ev);
            public static void calmdown(CalmDownEvent ev) => CalmDown.invoke(ev);
            public static void addtarget(AddTargetEvent ev) => AddTarget.invoke(ev);
        }
        #endregion
        #region SCP079
        public static class SCP079
        {
            public static event AllEvents<GeneratorActivateEvent> GeneratorActivate;
            public static event AllEvents<GetEXPEvent> GetEXP;
            public static event AllEvents<GetLVLEvent> GetLVL;
            public static void generatoractivate(GeneratorActivateEvent ev) => GeneratorActivate.invoke(ev);
            public static void getEXP(GetEXPEvent ev) => GetEXP.invoke(ev);
            public static void getLVL(GetLVLEvent ev) => GetLVL.invoke(ev);
        }
        #endregion
        #region SCP049
        public static class SCP049
        {
            public static event AllEvents<StartRecallEvent> StartRecall;
            public static event AllEvents<FinishRecallEvent> FinishRecall;
            public static void startrecall(StartRecallEvent ev) => StartRecall.invoke(ev);
            public static void finishrecall(FinishRecallEvent ev) => FinishRecall.invoke(ev);
        }
        #endregion
    }
}