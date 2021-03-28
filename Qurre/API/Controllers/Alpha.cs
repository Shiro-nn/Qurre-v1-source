using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.API.Controllers
{
	public class Alpha
	{
		internal Alpha() { }
		private AlphaWarheadController awc;
		private AlphaWarheadNukesitePanel awnp;
		public AlphaWarheadController AlphaWarheadController
		{
			get
			{
				if (awc == null) awc = PlayerManager.localPlayer.GetComponent<AlphaWarheadController>();
				return awc;
			}
		}
		internal AlphaWarheadNukesitePanel AlphaWarheadNukesitePanel
		{
			get
			{
				if (awnp == null) awnp = Object.FindObjectOfType<AlphaWarheadNukesitePanel>();
				return awnp;
			}
		}
		public void Start()
		{
			awc.InstantPrepare();
			awc.StartDetonation();
		}
		public void InstantPrepare() => awc.InstantPrepare();
		public void CancelDetonation() => awc.CancelDetonation();
		public void Stop() => awc.CancelDetonation();
		public void Detonate() => awc.Detonate();
		public void Shake() => awc.CallRpcShake(false);
		public bool Enabled
		{
			get => awnp.Networkenabled;
			set => awnp.Networkenabled = value;
		}
		public bool Detonated => awc.detonated;
		public bool CanDetoante => awc.CanDetonate;
		public bool Active => awc.NetworkinProgress;
		public float TimeToDetonation
		{
			get => awc.NetworktimeToDetonation;
			set => awc.NetworktimeToDetonation = value;
		}
		public bool Locked
		{
			get => awc.Alpha_isLocked();
			set => awc.Alpha_isLocked(value);
		}
		public int Cooldown
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