using System.Linq;
namespace Qurre.API.Controllers
{
    public class Heavy
    {
        internal Heavy() { }
        public bool ForcedOvercharge => Generator079.mainGenerator.forcedOvercharge;
        public byte ActiveGenerators { get => ForcedOvercharge ? (byte)5 : Generator079.mainGenerator.totalVoltage; internal set => Generator079.mainGenerator.totalVoltage = value; }
        public bool Recontained079 { get; internal set; } = false;
        public void Recontain079(bool forced = true) => Recontainer079.BeginContainment(forced);
        public void Overcharge(bool forced = true)
        {
            if (forced)
            {
                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase("ALLSECURED . SCP 0 7 9 RECONTAINMENT SEQUENCE COMMENCING . FORCEOVERCHARGE", 0.1f, 0.07f);
                Generator079.mainGenerator.forcedOvercharge = true;
                Recontain079(forced);
            }
            else foreach (var gen in Map.Generators.Where(x => !x.Overcharged)) gen.Overcharge();
        }
        public void LightsOff(float duration) => Generator079.mainGenerator.ServerOvercharge(duration, true);
            /// <summary> 
            /// <param name="bool"></param> 
            /// <returns>Check the lights close in the site</returns> 
            /// </summary> 
            public bool IsLightsOff => Generator079.mainGenerator.enabled;
    }
}