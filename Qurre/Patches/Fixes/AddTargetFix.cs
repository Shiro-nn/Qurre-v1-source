using HarmonyLib;
using Qurre.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Qurre.Patches.Fixes
{
    [HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.AddTarget))]
    public class AddTargetFix
    {
        public static bool Postfix(PlayableScps.Scp096 _instance, GameObject _gameObject)
        {
            try
            {
                List<Player> _targets = new List<Player>();
                Player _target = Player.Get(_gameObject);
                _targets.Add(_target);
                if (_instance._targets.Contains(_target.ReferenceHub) && _targets.Contains(_target))
                {
                    return true;
                }
                else if (_instance._targets.Contains(_target.ReferenceHub) && !_targets.Contains(_target))
                {
                    _instance._targets.Remove(_target.ReferenceHub);
                    return true;
                }
                return true;
            }
            catch(Exception e)
            {
                Log.Error($"umm, error in patching Fixes [AddTargetFix]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}
