using GunzSharp.Commands;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GunzSharp.Packet
{
	public struct MPacketCrypterKey
	{
		public byte[] Key { get; set; }

		public void InitKey()
		{
			Key = new byte[MPacketCrypter.PACKET_CRYPTER_KEY_LEN];
		}
	}

	public class MPacketCrypter
	{
		public const int PACKET_CRYPTER_KEY_LEN = 32;

		public static readonly byte[] IV = {
			55, 04, 93, 46, 67, MCommandConsts.MCOMMAND_VERSION, 73, 83,
			80, 05, 19, 201, 40, 164, 77, 05
		};

		private static readonly byte[] XOR = {
			87, 2, 91, 4, 52, 6, 1, 8, 55, 10, 18, 105, 65, 56, 15, 120
		};

		private static int shl;
		private static int shlMask;

		private MPacketCrypterKey key;

		public MPacketCrypter()
		{
			InitConst();

			key = new MPacketCrypterKey();
			key.InitKey();
		}

		public bool InitKey(ref MPacketCrypterKey pKey)
		{
			Buffer.BlockCopy(pKey.Key, 0, key.Key, 0, PACKET_CRYPTER_KEY_LEN);
			return true;
		}

		public virtual bool Encrypt(byte[] source, byte[] target)
		{
			return Encrypt(source, target, ref key);
		}

		public virtual bool Decrypt(byte[] source, byte[] target)
		{
			return Decrypt(source, target, ref key);
		}

		public virtual bool Encrypt(byte[] source)
		{
			return Encrypt(source, ref key);
		}

		public virtual bool Decrypt(byte[] source)
		{
			return Decrypt(source, ref key);
		}

		public static bool Encrypt(byte[] source, byte[] target, ref MPacketCrypterKey pKey)
		{
			if (target.Length < source.Length)
			{
				return false;
			}

			int keyIndex = 0;

			for (int i = 0; i < source.Length; i++)
			{
				target[i] = _Enc(source[i], pKey.Key[keyIndex]);

				keyIndex++;
				if (keyIndex >= pKey.Key.Length)
				{
					keyIndex = 0;
				}
			}

			return true;
		}

		public static bool Decrypt(byte[] source, byte[] target, ref MPacketCrypterKey pKey)
		{
			if (target.Length < source.Length)
			{
				return false;
			}

			int keyIndex = 0;

			for (int i = 0; i < source.Length; i++)
			{
				target[i] = _Dec(source[i], pKey.Key[keyIndex]);

				keyIndex++;
				if (keyIndex >= pKey.Key.Length)
				{
					keyIndex = 0;
				}
			}

			return true;
		}

		public static bool Encrypt(byte[] source, ref MPacketCrypterKey pKey)
		{
			int keyIndex = 0;

			for (int i = 0; i < source.Length; i++)
			{
				source[i] = _Enc(source[i], pKey.Key[keyIndex]);

				keyIndex++;
				if (keyIndex >= pKey.Key.Length)
				{
					keyIndex = 0;
				}
			}

			return true;
		}

		public static bool Decrypt(byte[] source, ref MPacketCrypterKey pKey)
		{
			int keyIndex = 0;

			for (int i = 0; i < source.Length; i++)
			{
				source[i] = _Dec(source[i], pKey.Key[keyIndex]);

				keyIndex++;
				if (keyIndex >= pKey.Key.Length)
				{
					keyIndex = 0;
				}
			}

			return true;
		}

		public static void InitConst()
		{
			shl = (MCommandConsts.MCOMMAND_VERSION % 6) + 1;

			shlMask = 0;

			for (int i = 0; i < shl; i++)
			{
				shlMask += (1 << i);
			}
		}

		public static void MakeSeedKey(ref MPacketCrypterKey key, MUID server, MUID client, uint timeStamp)
		{
			if (key.Key == null || key.Key.Length != PACKET_CRYPTER_KEY_LEN)
			{
				key.InitKey();
			}
			else
			{
				Array.Clear(key.Key, 0, PACKET_CRYPTER_KEY_LEN);
			}

			int uidSize = Marshal.SizeOf(typeof(MUID));

			BitConverter.GetBytes(timeStamp).CopyTo(key.Key, 0);
			BitConverter.GetBytes(server.Low).CopyTo(key.Key, sizeof(uint));

			IntPtr ptr = Marshal.AllocHGlobal(uidSize);
			Marshal.StructureToPtr(client, ptr, true);
			Marshal.Copy(ptr, key.Key, sizeof(uint) * 2, uidSize);
			Marshal.FreeHGlobal(ptr);

			for (int i = 0; i < 16; i++)
			{
				key.Key[i] ^= XOR[i];
			}

			int ivIndex = 0;

			for (int i = 16; i < PACKET_CRYPTER_KEY_LEN; i++)
			{
				key.Key[i] = IV[ivIndex];
				ivIndex++;
			}
		}

		private static byte _Enc(byte s, byte key)
		{
			ushort w;
			byte b, bh;
			b = (byte)(s ^ key);
			w = (ushort)(b << shl);
			bh = (byte)((w & 0xFF00) >> 8);
			b = (byte)(w & 0xFF);

			b = (byte)(b | bh);
			bh = (byte)(b ^ 0xF0);

			return bh;
		}
		private static byte _Dec(byte s, byte key)
		{
			byte b, bh, d;

			b = (byte)(s ^ 0xF0);
			bh = (byte)(b & shlMask);
			bh = (byte)(bh << (8 - shl));
			b = (byte)(b >> shl);

			d = (byte)(bh | b);
			d = (byte)(d ^ key);

			return d;
		}
	}
}
