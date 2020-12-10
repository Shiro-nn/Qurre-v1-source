using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.API
{
	public static class Alpha
	{
		private static AlphaWarheadController awc;
		private static AlphaWarheadNukesitePanel awnp;
		public static AlphaWarheadController AlphaWarheadController
		{
			get
			{
				if (awc == null)
					awc = PlayerManager.localPlayer.GetComponent<AlphaWarheadController>();
				return awc;
			}
		}
		internal static AlphaWarheadNukesitePanel AlphaWarheadNukesitePanel
		{
			get
			{
				if (awnp == null)
					awnp = Object.FindObjectOfType<AlphaWarheadNukesitePanel>();
				return awnp;
			}
		}
		public static void Start()
		{
			AlphaWarheadController.InstantPrepare();
			AlphaWarheadController.StartDetonation();
		}
		public static void Stop() => AlphaWarheadController.CancelDetonation();
		public static void Detonate() => AlphaWarheadController.Detonate();
		public static bool IsEnabled
		{
			get => AlphaWarheadNukesitePanel.Networkenabled;
			set => AlphaWarheadNukesitePanel.Networkenabled = value;
		}
		public static bool IsDetonated => AlphaWarheadController.detonated;
		public static bool IsInProgress => AlphaWarheadController.inProgress;
		public static float TimeToDetonation
		{
			get => AlphaWarheadController.NetworktimeToDetonation;
			set => AlphaWarheadController.NetworktimeToDetonation = value;
		}
		public static bool IsLocked
		{
			get => AlphaWarheadController.Alpha_isLocked();
			set => AlphaWarheadController.Alpha_isLocked(value);
		}
	}
}