using Respawning;
using Respawning.NamingRules;
public class Loader
{
    public static void LoadModSystem()
    {
        LoadRules();
        LoadDefault();
        static void LoadRules()
        {
            UnitNamingRules.AllNamingRules[SpawnableTeamType.NineTailedFox] = new NineTailedFoxNamingRule();
            UnitNamingRules.AllNamingRules[SpawnableTeamType.None] = new NineTailedFoxNamingRule();
            UnitNamingRules.AllNamingRules[SpawnableTeamType.NineTailedFox] = new NineTailedFoxNamingRule();
            UnitNamingRules.AllNamingRules[SpawnableTeamType.ChaosInsurgency] = new NineTailedFoxNamingRule();
            UnitNamingRules.AllNamingRules[SpawnableTeamType.ClassD] = new NineTailedFoxNamingRule();
            UnitNamingRules.AllNamingRules[SpawnableTeamType.Scientist] = new NineTailedFoxNamingRule();
            UnitNamingRules.AllNamingRules[SpawnableTeamType.SCP] = new NineTailedFoxNamingRule();
            UnitNamingRules.AllNamingRules[SpawnableTeamType.Tutorial] = new NineTailedFoxNamingRule();
        }
        static void LoadDefault()
        {
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.FacilityGuard] = SpawnableTeamType.NineTailedFox;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.NtfPrivate] = SpawnableTeamType.NineTailedFox;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.NtfCaptain] = SpawnableTeamType.NineTailedFox;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.NtfSergeant] = SpawnableTeamType.NineTailedFox;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.NtfSpecialist] = SpawnableTeamType.NineTailedFox;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.ChaosConscript] = SpawnableTeamType.ChaosInsurgency;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.ChaosMarauder] = SpawnableTeamType.ChaosInsurgency;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.ChaosRepressor] = SpawnableTeamType.ChaosInsurgency;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.ChaosRifleman] = SpawnableTeamType.ChaosInsurgency;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.ClassD] = SpawnableTeamType.ClassD;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.Scientist] = SpawnableTeamType.Scientist;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.Scp173] = SpawnableTeamType.SCP;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.Scp096] = SpawnableTeamType.SCP;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.Scp106] = SpawnableTeamType.SCP;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.Scp049] = SpawnableTeamType.SCP;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.Scp079] = SpawnableTeamType.SCP;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.Scp93953] = SpawnableTeamType.SCP;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.Scp93989] = SpawnableTeamType.SCP;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.Scp0492] = SpawnableTeamType.SCP;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.Tutorial] = SpawnableTeamType.Tutorial;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.Spectator] = SpawnableTeamType.None;
            UnitNamingManager.RolesWithEnforcedDefaultName[RoleType.None] = SpawnableTeamType.None;
        }
    }
}