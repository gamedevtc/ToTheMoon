using System;
using System.Reflection;
using System.Collections;
using Photon.Utilities;
using UnityEngine;
#pragma warning disable 1635
#pragma warning disable 0219
namespace Photon.Compression.Internal
{
	public class PackFrame_MLoad : PackFrame
	{
		public System.Int32 readyPlayerCount;
		public System.Boolean releaseFromLoadWait;

		public static void Interpolate(PackFrame start, PackFrame end, PackFrame trg, float time, ref FastBitMask128 readyMask, ref int maskOffset)
		{
			var s = start as PackFrame_MLoad;
			var e = end as PackFrame_MLoad;
			var t = trg as PackFrame_MLoad;
			var mask = end.mask;
			if (mask[maskOffset]){ t.readyPlayerCount = (System.Int32)(((e.readyPlayerCount - s.readyPlayerCount) * time) + s.readyPlayerCount);} maskOffset++;
		}
		public static void Interpolate(PackFrame start, PackFrame end, System.Object trg, float time, ref FastBitMask128 readyMask, ref int maskOffset)
		{
			var s = start as PackFrame_MLoad;
			var e = end as PackFrame_MLoad;
			var t = trg as MLoad;
			var mask = end.mask;
			maskOffset++;
			maskOffset++;
		}
		public static void SnapshotCallback(PackFrame snapframe, PackFrame targframe, System.Object trg, ref FastBitMask128 readyMask, ref int maskOffset)
		{
		}
		public static void Capture(System.Object src, PackFrame trg)
		{
			var s = src as MLoad;
			var t = trg as PackFrame_MLoad;
			t.readyPlayerCount = s.readyPlayerCount; 
			t.releaseFromLoadWait = s.releaseFromLoadWait; 
		}
		public static void Apply(PackFrame src, System.Object trg, ref FastBitMask128 mask, ref int maskOffset)
		{
			var s = src as PackFrame_MLoad;
			var t = trg as MLoad;
			{ if (mask[maskOffset]){ t.readyPlayerCount = s.readyPlayerCount; } } maskOffset++;
			{ if (mask[maskOffset]){ t.releaseFromLoadWait = s.releaseFromLoadWait; } } maskOffset++;
		}
		public static void Copy(PackFrame src, PackFrame trg)
		{
			var s = src as PackFrame_MLoad;
			var t = trg as PackFrame_MLoad;
			t.readyPlayerCount = s.readyPlayerCount;
			t.releaseFromLoadWait = s.releaseFromLoadWait;
		}

		public static PackFrame Factory() { return new PackFrame_MLoad(){ mask = new FastBitMask128(Pack_MLoad.TOTAL_FIELDS), isCompleteMask = new FastBitMask128(Pack_MLoad.TOTAL_FIELDS) }; }
		public static PackFrame[] Factory(System.Object trg, int count){ var t = trg as MLoad;var frames = new PackFrame_MLoad[count]; for (int i = 0; i < count; ++i) { var frame = new PackFrame_MLoad(){ mask = new FastBitMask128(Pack_MLoad.TOTAL_FIELDS), isCompleteMask = new FastBitMask128(Pack_MLoad.TOTAL_FIELDS) };  frames[i] = frame; } return frames; }
	}

	public static class Pack_MLoad
	{
		public const int LOCAL_FIELDS = 2;

		public const int TOTAL_FIELDS = 2;

		public static PackObjectDatabase.PackObjectInfo packObjInfo;
		static PackDelegate<Int32> readyPlayerCountPacker;
		static UnpackDelegate<Int32> readyPlayerCountUnpacker;

		static PackDelegate<Boolean> releaseFromLoadWaitPacker;
		static UnpackDelegate<Boolean> releaseFromLoadWaitUnpacker;

		public static bool initialized;
		public static bool isInitializing;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Initialize()
		{
			if (initialized) return;
			isInitializing = true;
			int maxBits = 0;
			var pObjAttr = (typeof(MLoad).GetCustomAttributes(typeof(PackObjectAttribute), false)[0] as PackObjectAttribute);
			var defaultKeyRate = pObjAttr.defaultKeyRate;
			FastBitMask128 defReadyMask = new FastBitMask128(TOTAL_FIELDS);
			int fieldindex = 0;

			SyncVarAttribute readyPlayerCountPackAttr = (SyncVarAttribute)(typeof(MLoad).GetField("readyPlayerCount").GetCustomAttributes(typeof(SyncVarBaseAttribute), false)[0] as SyncVarAttribute);
			readyPlayerCountPacker = (readyPlayerCountPackAttr as IPackInt32).Pack;
			readyPlayerCountUnpacker = (readyPlayerCountPackAttr as IPackInt32).Unpack;
			readyPlayerCountPackAttr.Initialize(typeof(System.Int32));
			if (readyPlayerCountPackAttr.keyRate == KeyRate.UseDefault) readyPlayerCountPackAttr.keyRate = (KeyRate)defaultKeyRate;
			if (readyPlayerCountPackAttr.syncAs == SyncAs.Auto) readyPlayerCountPackAttr.syncAs = pObjAttr.syncAs;
			if (readyPlayerCountPackAttr.syncAs == SyncAs.Auto) readyPlayerCountPackAttr.syncAs = SyncAs.State;
			if (readyPlayerCountPackAttr.syncAs == SyncAs.Trigger) defReadyMask[fieldindex] = true;
			maxBits += 32; fieldindex++;

			SyncVarAttribute releaseFromLoadWaitPackAttr = (SyncVarAttribute)(typeof(MLoad).GetField("releaseFromLoadWait").GetCustomAttributes(typeof(SyncVarBaseAttribute), false)[0] as SyncVarAttribute);
			releaseFromLoadWaitPacker = (releaseFromLoadWaitPackAttr as IPackBoolean).Pack;
			releaseFromLoadWaitUnpacker = (releaseFromLoadWaitPackAttr as IPackBoolean).Unpack;
			releaseFromLoadWaitPackAttr.Initialize(typeof(System.Boolean));
			if (releaseFromLoadWaitPackAttr.keyRate == KeyRate.UseDefault) releaseFromLoadWaitPackAttr.keyRate = (KeyRate)defaultKeyRate;
			if (releaseFromLoadWaitPackAttr.syncAs == SyncAs.Auto) releaseFromLoadWaitPackAttr.syncAs = pObjAttr.syncAs;
			if (releaseFromLoadWaitPackAttr.syncAs == SyncAs.Auto) releaseFromLoadWaitPackAttr.syncAs = SyncAs.State;
			if (releaseFromLoadWaitPackAttr.syncAs == SyncAs.Trigger) defReadyMask[fieldindex] = true;
			maxBits += 1; fieldindex++;

			packObjInfo = new PackObjectDatabase.PackObjectInfo(defReadyMask, Pack, Pack, Unpack, maxBits, PackFrame_MLoad.Factory, PackFrame_MLoad.Factory, PackFrame_MLoad.Apply, PackFrame_MLoad.Capture, PackFrame_MLoad.Copy, PackFrame_MLoad.SnapshotCallback, PackFrame_MLoad.Interpolate, PackFrame_MLoad.Interpolate, TOTAL_FIELDS);
			PackObjectDatabase.packObjInfoLookup.Add(typeof(MLoad), packObjInfo);
			isInitializing = false;
			initialized = true;
		}
		public static SerializationFlags Pack(ref System.Object obj, PackFrame prevFrame, ref FastBitMask128 mask, ref int maskOffset, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			var packable = obj as MLoad;
			var prev = prevFrame as PackFrame_MLoad;
			SerializationFlags flags = SerializationFlags.None;
			{
				var flag = readyPlayerCountPacker(ref packable.readyPlayerCount, prev.readyPlayerCount, buffer, ref bitposition, frameId, writeFlags);
				mask[maskOffset] = flag != SerializationFlags.None;	flags |= flag; maskOffset++;
			}
			{
				var flag = releaseFromLoadWaitPacker(ref packable.releaseFromLoadWait, prev.releaseFromLoadWait, buffer, ref bitposition, frameId, writeFlags);
				mask[maskOffset] = flag != SerializationFlags.None;	flags |= flag; maskOffset++;
			}
			return flags;
		}
		public static SerializationFlags Pack(ref MLoad packable, PackFrame prevFrame, ref FastBitMask128 mask, ref int maskOffset, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			var prev = prevFrame as PackFrame_MLoad;
			SerializationFlags flags = SerializationFlags.None;
			{
				var flag = readyPlayerCountPacker(ref packable.readyPlayerCount, prev.readyPlayerCount, buffer, ref bitposition, frameId, writeFlags);
				mask[maskOffset] = flag != SerializationFlags.None;	flags |= flag; maskOffset++;
			}
			{
				var flag = releaseFromLoadWaitPacker(ref packable.releaseFromLoadWait, prev.releaseFromLoadWait, buffer, ref bitposition, frameId, writeFlags);
				mask[maskOffset] = flag != SerializationFlags.None;	flags |= flag; maskOffset++;
			}
			return flags;
		}
		public static SerializationFlags Pack(PackFrame obj, PackFrame prevFrame, ref FastBitMask128 mask, ref int maskOffset, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			var packable = obj as PackFrame_MLoad;
			var prev = prevFrame as PackFrame_MLoad;
			SerializationFlags flags = SerializationFlags.None;
			{
				var flag = readyPlayerCountPacker(ref packable.readyPlayerCount, prev.readyPlayerCount, buffer, ref bitposition, frameId, writeFlags);
				mask[maskOffset] = flag != SerializationFlags.None;	flags |= flag; maskOffset++;
			}
			{
				var flag = releaseFromLoadWaitPacker(ref packable.releaseFromLoadWait, prev.releaseFromLoadWait, buffer, ref bitposition, frameId, writeFlags);
				mask[maskOffset] = flag != SerializationFlags.None;	flags |= flag; maskOffset++;
			}
			return flags;
		}
		public static SerializationFlags Unpack(PackFrame obj, ref FastBitMask128 mask, ref FastBitMask128 isCompleteMask, ref int maskOffset, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			var packable = obj as PackFrame_MLoad;
			SerializationFlags flags = SerializationFlags.None;
			{
				if (mask[maskOffset]) {
					var flag = readyPlayerCountUnpacker(ref packable.readyPlayerCount, buffer, ref bitposition, frameId, writeFlags);
					isCompleteMask[maskOffset] = (flag & SerializationFlags.IsComplete) != 0; mask[maskOffset] = flag != 0; flags |= flag; 
				 } maskOffset++;
			}
			{
				if (mask[maskOffset]) {
					var flag = releaseFromLoadWaitUnpacker(ref packable.releaseFromLoadWait, buffer, ref bitposition, frameId, writeFlags);
					isCompleteMask[maskOffset] = (flag & SerializationFlags.IsComplete) != 0; mask[maskOffset] = flag != 0; flags |= flag; 
				 } maskOffset++;
			}
			return flags;
		}
		public static SerializationFlags Pack(ref PackFrame_MLoad packable, PackFrame prevFrame, ref FastBitMask128 mask, ref int maskOffset, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			var prev = prevFrame as PackFrame_MLoad;
			SerializationFlags flags = SerializationFlags.None;
			{
				var flag = readyPlayerCountPacker(ref packable.readyPlayerCount, prev.readyPlayerCount, buffer, ref bitposition, frameId, writeFlags);
				mask[maskOffset] = flag != SerializationFlags.None;	flags |= flag; maskOffset++;
			}
			{
				var flag = releaseFromLoadWaitPacker(ref packable.releaseFromLoadWait, prev.releaseFromLoadWait, buffer, ref bitposition, frameId, writeFlags);
				mask[maskOffset] = flag != SerializationFlags.None;	flags |= flag; maskOffset++;
			}
			return flags;
		}
		public static SerializationFlags Unpack(ref PackFrame_MLoad packable, ref FastBitMask128 mask, ref FastBitMask128 isCompleteMask, ref int maskOffset, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			SerializationFlags flags = SerializationFlags.None;
			{
				if (mask[maskOffset]) {
					var flag = readyPlayerCountUnpacker(ref packable.readyPlayerCount, buffer, ref bitposition, frameId, writeFlags);
					isCompleteMask[maskOffset] = (flag & SerializationFlags.IsComplete) != 0; mask[maskOffset] = flag != 0; flags |= flag; 
				 } maskOffset++;
			}
			{
				if (mask[maskOffset]) {
					var flag = releaseFromLoadWaitUnpacker(ref packable.releaseFromLoadWait, buffer, ref bitposition, frameId, writeFlags);
					isCompleteMask[maskOffset] = (flag & SerializationFlags.IsComplete) != 0; mask[maskOffset] = flag != 0; flags |= flag; 
				 } maskOffset++;
			}
			return flags;
		}
	}
}
#pragma warning disable 0219
#pragma warning restore 1635
