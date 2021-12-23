﻿using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Firearms.Attachments;
using MapGeneration;
using MapGeneration.Distributors;
using PlayerStatsSystem;
using Qurre.API.Addons;
using Qurre.API.Controllers;
using Qurre.API.Modules;
using Qurre.API.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
using _lift = Qurre.API.Controllers.Lift;
using _locker = Qurre.API.Controllers.Locker;
using _workStation = Qurre.API.Controllers.WorkStation;
namespace Qurre.API
{
	public static class Extensions
	{
		public static DoorVariant DoorPrefabLCZ { get; internal set; }
		public static DoorVariant DoorPrefabHCZ { get; internal set; }
		public static DoorVariant DoorPrefabEZ { get; internal set; }
		public static DoorVariant GetDoorPrefab(this DoorPrefabs prefab)
		{
			if (prefab == DoorPrefabs.DoorLCZ) return DoorPrefabLCZ;
			else if (prefab == DoorPrefabs.DoorHCZ) return DoorPrefabHCZ;
			else return DoorPrefabEZ;
		}
		public static Room GetRoom(this RoomName type) => Map.Rooms.FirstOrDefault(x => x.RoomName == type);
		public static Room GetRoom(this RoomType type) => Map.Rooms.FirstOrDefault(x => x.Type == type);
		public static Room GetRoom(this RoomIdentifier identifier) => Map.Rooms.FirstOrDefault(x => x.Identifier == identifier);
		public static Door GetDoor(this DoorType type) => Map.Doors.FirstOrDefault(x => x.Type == type);
		public static Door GetDoor(this GameObject gameObject) => Map.Doors.FirstOrDefault(x => x.GameObject == gameObject);
		public static _lift GetLift(this LiftType type) => Map.Lifts.FirstOrDefault(x => x.Type == type);
		public static Door GetDoor(this DoorVariant door) => Map.Doors.FirstOrDefault(x => x.GameObject == door.gameObject);
		public static Generator GetGenerator(this GameObject gameObject) => Map.Generators.FirstOrDefault(x => x.GameObject == gameObject);
		public static Generator GetGenerator(this Scp079Generator generator079) => Map.Generators.FirstOrDefault(x => x.GameObject == generator079.gameObject);
		public static Tesla GetTesla(this TeslaGate teslaGate) => Map.Teslas.FirstOrDefault(x => x.GameObject == teslaGate.gameObject);
		public static Tesla GetTesla(this GameObject gameObject) => Map.Teslas.FirstOrDefault(x => x.GameObject == gameObject);
		public static Primitive GetPrimitive(this GameObject gameObject) => Map.Primitives.FirstOrDefault(x => x.Base.gameObject == gameObject);
		public static Controllers.Light GetLight(this GameObject gameObject) => Map.Lights.FirstOrDefault(x => x.Base.gameObject == gameObject);
		public static _lift GetLift(this Lift lift) => Map.Lifts.FirstOrDefault(x => x.GameObject == lift.gameObject);
		public static _locker GetLocker(this MapGeneration.Distributors.Locker locker) => Map.Lockers.FirstOrDefault(x => x.Transform == locker.gameObject);
		public static _workStation GetWorkStation(this WorkstationController station) => Map.WorkStations.FirstOrDefault(x => x.GameObject == station.gameObject);
		public static Window GetWindow(this BreakableWindow station) => Map.Windows.FirstOrDefault(x => x.Breakable == station);
		public static Window GetWindow(this GameObject go) => Map.Windows.FirstOrDefault(x => x.GameObject == go);
		public static Player GetAttacker(this DamageHandlerBase handler)
		{
			var plz = GetAttackerPLZ(handler);
			if (plz == null) return null;
			return Player.Get(plz.Attacker.Hub);
		}
		public static AttackerDamageHandler GetAttackerPLZ(DamageHandlerBase handler)
		{
			return handler switch
			{
				AttackerDamageHandler adh2 => adh2,
				_ => null,
			};
		}
		public static DamageTypesPrimitive GetDamageTypesPrimitive(this DamageHandlerBase handler) => _getDamageTypesPrimitive(handler);
		private static DamageTypesPrimitive _getDamageTypesPrimitive(DamageHandlerBase handler)
		{
			return handler switch
			{
				CustomReasonDamageHandler _ => DamageTypesPrimitive.Custom,
				ExplosionDamageHandler _ => DamageTypesPrimitive.Explosion,
				FirearmDamageHandler _ => DamageTypesPrimitive.Firearm,
				MicroHidDamageHandler _ => DamageTypesPrimitive.MicroHid,
				RecontainmentDamageHandler _ => DamageTypesPrimitive.Recontainment,
				Scp018DamageHandler _ => DamageTypesPrimitive.Scp018,
				Scp096DamageHandler _ => DamageTypesPrimitive.Scp096,
				ScpDamageHandler _ => DamageTypesPrimitive.ScpDamage,
				UniversalDamageHandler _ => DamageTypesPrimitive.Universal,
				WarheadDamageHandler _ => DamageTypesPrimitive.Warhead,
				_ => DamageTypesPrimitive.Unknow,
			};
		}
		public static DamageTypes GetDamageType(this DamageHandlerBase handler) => _getDamageType(handler);
		private static DamageTypes _getDamageType(DamageHandlerBase handler)
		{
			switch (handler)
			{
				case UniversalDamageHandler tr:
					{
						if (tr.TranslationId == 0) return DamageTypes.Recontainment;
						if (tr.TranslationId == 1) return DamageTypes.Nuke;
						if (tr.TranslationId == 2) return DamageTypes.Scp049;
						if (tr.TranslationId == 4) return DamageTypes.Asphyxiation;
						if (tr.TranslationId == 5) return DamageTypes.Bleeding;
						if (tr.TranslationId == 6) return DamageTypes.Falldown;
						if (tr.TranslationId == 7) return DamageTypes.Pocket;
						if (tr.TranslationId == 8) return DamageTypes.Decont;
						if (tr.TranslationId == 9) return DamageTypes.Poison;
						if (tr.TranslationId == 10) return DamageTypes.Scp207;
						if (tr.TranslationId == 11) return DamageTypes.SeveredHands;
						if (tr.TranslationId == 12) return DamageTypes.MicroHid;
						if (tr.TranslationId == 13) return DamageTypes.Tesla;
						if (tr.TranslationId == 14) return DamageTypes.Explosion;
						if (tr.TranslationId == 15) return DamageTypes.Scp096;
						if (tr.TranslationId == 16) return DamageTypes.Scp173;
						if (tr.TranslationId == 17) return DamageTypes.Scp939;
						if (tr.TranslationId == 18) return DamageTypes.Scp0492;
						if (tr.TranslationId == 20) return DamageTypes.Wall;
						if (tr.TranslationId == 21) return DamageTypes.Contain;
						if (tr.TranslationId == 22) return DamageTypes.FriendlyFireDetector;
					}
					break;
				case FirearmDamageHandler fr:
					{
						if (fr.WeaponType == ItemType.GunAK) return DamageTypes.AK;
						if (fr.WeaponType == ItemType.GunCOM15) return DamageTypes.Com15;
						if (fr.WeaponType == ItemType.GunCOM18) return DamageTypes.Com18;
						if (fr.WeaponType == ItemType.GunCrossvec) return DamageTypes.CrossVec;
						if (fr.WeaponType == ItemType.GunE11SR) return DamageTypes.E11SR;
						if (fr.WeaponType == ItemType.GunFSP9) return DamageTypes.FSP9;
						if (fr.WeaponType == ItemType.GunLogicer) return DamageTypes.Logicer;
						if (fr.WeaponType == ItemType.GunRevolver) return DamageTypes.Revolver;
						if (fr.WeaponType == ItemType.GunShotgun) return DamageTypes.Shotgun;
					}
					break;
				case ScpDamageHandler sr:
					{
						if (sr._translationId == 0) return DamageTypes.Recontainment;
						if (sr._translationId == 1) return DamageTypes.Nuke;
						if (sr._translationId == 2) return DamageTypes.Scp049;
						if (sr._translationId == 4) return DamageTypes.Asphyxiation;
						if (sr._translationId == 5) return DamageTypes.Bleeding;
						if (sr._translationId == 6) return DamageTypes.Falldown;
						if (sr._translationId == 7) return DamageTypes.Pocket;
						if (sr._translationId == 8) return DamageTypes.Decont;
						if (sr._translationId == 9) return DamageTypes.Poison;
						if (sr._translationId == 10) return DamageTypes.Scp207;
						if (sr._translationId == 11) return DamageTypes.SeveredHands;
						if (sr._translationId == 12) return DamageTypes.MicroHid;
						if (sr._translationId == 13) return DamageTypes.Tesla;
						if (sr._translationId == 14) return DamageTypes.Explosion;
						if (sr._translationId == 15) return DamageTypes.Scp096;
						if (sr._translationId == 16) return DamageTypes.Scp173;
						if (sr._translationId == 17) return DamageTypes.Scp939;
						if (sr._translationId == 18) return DamageTypes.Scp0492;
						if (sr._translationId == 20) return DamageTypes.Wall;
						if (sr._translationId == 21) return DamageTypes.Contain;
						if (sr._translationId == 22) return DamageTypes.FriendlyFireDetector;
					}
					break;
				case WarheadDamageHandler _: return DamageTypes.Nuke;
				case Scp096DamageHandler _: return DamageTypes.Scp096;
				case Scp018DamageHandler _: return DamageTypes.Scp018;
				case RecontainmentDamageHandler _: return DamageTypes.Recontainment;
				case MicroHidDamageHandler _: return DamageTypes.MicroHid;
				case ExplosionDamageHandler _: return DamageTypes.Explosion;
				default: return DamageTypes.None;
			}
			return DamageTypes.None;
		}
		public static ItemType GetItemType(this AmmoType type)
		{
			return type switch
			{
				AmmoType.Ammo556 => ItemType.Ammo556x45,
				AmmoType.Ammo762 => ItemType.Ammo762x39,
				AmmoType.Ammo9 => ItemType.Ammo9x19,
				AmmoType.Ammo12Gauge => ItemType.Ammo12gauge,
				AmmoType.Ammo44Cal => ItemType.Ammo44cal,
				_ => ItemType.None,
			};
		}
		public static AmmoType GetAmmoType(this ItemType type)
		{
			return type switch
			{
				ItemType.Ammo9x19 => AmmoType.Ammo9,
				ItemType.Ammo556x45 => AmmoType.Ammo556,
				ItemType.Ammo762x39 => AmmoType.Ammo762,
				ItemType.Ammo12gauge => AmmoType.Ammo12Gauge,
				ItemType.Ammo44cal => AmmoType.Ammo44Cal,
				_ => AmmoType.None,
			};
		}
		public static System.Random Random { get; } = new System.Random();
		public static void Shuffle<T>(this IList<T> list)
		{
			RNGCryptoServiceProvider provider = new();
			int n = list.Count;
			while (n > 1)
			{
				byte[] box = new byte[1];
				do provider.GetBytes(box);
				while (!(box[0] < n * (byte.MaxValue / n)));
				int k = (box[0] % n);
				n--;
				(list[n], list[k]) = (list[k], list[n]);
			}
		}
		public static void CopyProperties(this object target, object source)
		{
			Type type = target.GetType();
			if (type != source.GetType()) return;
			foreach (PropertyInfo sourceProperty in type.GetProperties())
				type.GetProperty(sourceProperty.Name)?.SetValue(target, sourceProperty.GetValue(source, null), null);
		}
		public static void Reload(this IConfig cfg) => CustomConfigsManager.Load(cfg);
	}
}