<<<<<<< HEAD
﻿using HarmonyLib;
using PlayerStatsSystem;
using Qurre.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.Patches.Fixes
{
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.staminaController.ServesideUpdate))]
    internal static class Stamina
    {
        internal static bool Prefix(FirstPersonController _instance, ref float _result)
        {
            try
            {
                var ply = Player.Get(_instance.staminaController._hub);
                if (ply != null) return true;
                ply.Stamina.RemainingStamina = _result;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Fixes [Stamina]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}
=======
﻿using HarmonyLib;
using PlayerStatsSystem;
using Qurre.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.Patches.Fixes
{
    [HarmonyPatch(typeof(FirstPersonController), "Fix", MethodType.Getter)]
    internal static class Stamina
    {
        internal static bool Prefix(FirstPersonController _instance, ref float _result)
        {
            try
            {
                var ply = Player.Get(_instance.staminaController._hub);
                if (ply != null) return true;
                ply.Stamina.RemainingStamina = _result;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Fixes [Stamina]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}
>>>>>>> 7130dedba77981c88f7ce506ac525cc201ea643a
