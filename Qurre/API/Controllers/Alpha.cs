using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.API.Controllers
{
	public static class Alpha
	{
		private static AlphaWarheadController awc;
		private static AlphaWarheadNukesitePanel awnp;
		public static AlphaWarheadController AlphaWarheadController
		{
			get
			{
				if (awc == null) awc = PlayerManager.localPlayer.GetComponent<AlphaWarheadController>();
				return awc;
			}
		}
		internal static AlphaWarheadNukesitePanel AlphaWarheadNukesitePanel
		{
			get
			{
				if (awnp == null) awnp = Object.FindObjectOfType<AlphaWarheadNukesitePanel>();
				return awnp;
			}
		}
		public static void Start()
		{
			awc.InstantPrepare();
			awc.StartDetonation();
		}
		public static void InstantPrepare() => awc.InstantPrepare();
		public static void CancelDetonation() => awc.CancelDetonation();
		public static void Stop() => awc.CancelDetonation();
		public static void Detonate() => awc.Detonate();
		public static void Shake() => awc.CallRpcShake(false);
		public static bool Enabled
		{
			get => awnp.Networkenabled;
			set => awnp.Networkenabled = value;
		}
		public static bool Detonated => awc.detonated;
		public static bool CanDetoante => awc.CanDetonate;
		public static bool Active => awc.NetworkinProgress;
		public static float TimeToDetonation
		{
			get => awc.NetworktimeToDetonation;
			set => awc.NetworktimeToDetonation = value;
		}
		public static bool Locked
		{
			get => awc.Alpha_isLocked();
			set => awc.Alpha_isLocked(value);
		}
		public static int Cooldown
		{
			get => awc.cooldown;
			set => awc.cooldown = value;
		}
		public static class InsidePanel
		{
			private static AlphaWarheadNukesitePanel Panel => AlphaWarheadOutsitePanel.nukeside;
			public static bool Enabled
			{
				get => Panel.Networkenabled;
				set => Panel.Networkenabled = value;
			}
			public static float LeverStatus
			{
				get => Panel.Alpha_leverStatus();
				set => Panel.Alpha_leverStatus(value);
			}
			public static bool Locked { get; set; }
			public static Transform Lever => Panel.lever;
		}
		public static class OutsidePanel
		{
			private static AlphaWarheadOutsitePanel Panel => PlayerManager.localPlayer.GetComponent<AlphaWarheadOutsitePanel>();
			public static bool KeyCardEntered
			{
				get => Panel.NetworkkeycardEntered;
				set => Panel.NetworkkeycardEntered = value;
			}
		}
	}
}