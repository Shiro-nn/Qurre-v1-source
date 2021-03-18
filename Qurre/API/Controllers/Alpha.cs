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
				if (awnp == null)
					awnp = Object.FindObjectOfType<AlphaWarheadNukesitePanel>();
				return awnp;
			}
		}
		public void Start()
		{
			AlphaWarheadController.InstantPrepare();
			AlphaWarheadController.StartDetonation();
		}
		public void InstantPrepare() => AlphaWarheadController.InstantPrepare();
		public void CancelDetonation() => AlphaWarheadController.CancelDetonation();
		public void Stop() => AlphaWarheadController.CancelDetonation();
		public void Detonate() => AlphaWarheadController.Detonate();
		public void Shake() => AlphaWarheadController.CallRpcShake(false);
		public bool Enabled
		{
			get => AlphaWarheadNukesitePanel.Networkenabled;
			set => AlphaWarheadNukesitePanel.Networkenabled = value;
		}
		public bool Detonated => AlphaWarheadController.detonated;
		public bool CanDetoante => AlphaWarheadController.CanDetonate;
		public bool Active => AlphaWarheadController.NetworkinProgress;
		public float TimeToDetonation
		{
			get => AlphaWarheadController.NetworktimeToDetonation;
			set => AlphaWarheadController.NetworktimeToDetonation = value;
		}
		public bool Locked
		{
			get => AlphaWarheadController.Alpha_isLocked();
			set => AlphaWarheadController.Alpha_isLocked(value);
		}
		public int Cooldown
		{
			get => AlphaWarheadController.cooldown;
			set => AlphaWarheadController.cooldown = value;
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