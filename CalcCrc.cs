// This file has no copyright. It is based on C code found on the nascent
// Internet, probably before the year 2000, and ported to C#. The original
// author of this code, even if the original code identified the author,
// is now lost in the abyss of time. But I do state that the original code
// did not have a copyright notice.

namespace KnowDuplicateFiles;
internal class CalcCrc {
	static bool bTableIsInitialized = false;
	private uint CrcHighbit;
	private static uint[] Crc32_Table = new uint[256];
	private uint crc;
	private uint crcRev;    // CRC calculated by scanning the data
							// right-to-left (aka Reversed).
							// It, along with the normal 32-bit CRC is
							// meant to approximate a 64-bit CRC.

							// Note: What I would have liked to have
							// done was to name this "crc" spelled
							// backwards. Uh, but "crc" is palindromic!
							// So we have to settle for "crcRev".

//------------------------------------------------------------------------

	public CalcCrc() {
		CrcHighbit = unchecked(1u << 31);
		// Generate lookup table
		Init_CRC32_Table();
		Reset();
	}

//------------------------------------------------------------------------

	public uint GetCRC() {
		return crc ^= 0xFFFFFFFF;
	}

//------------------------------------------------------------------------

	public uint GetCRCReversed() {
		return crcRev ^= 0xFFFFFFFF;
	}

//------------------------------------------------------------------------

	public long GetQuasiCRC64() {
		// We'll fake a (sort of) CRC 64 by combining the forward and
		// reverse CRC
		uint Crc1   = crc ^= 0xFFFFFFFF;
		ulong Crc2  = crcRev ^= 0xFFFFFFFF;
		ulong Crc64 = (Crc2 << 32) | Crc1;  // Glue them together
		return unchecked((long)Crc64);
	}

//------------------------------------------------------------------------

	public void Reset() {
		crc = 0xFFFFFFFF;
		crcRev = 0xFFFFFFFF;
	}

//------------------------------------------------------------------------

	public void AddData(byte[] p) {
		AddData(p, p.Length);
	}

//------------------------------------------------------------------------

	public void AddData(byte[] p, int n) {
		for (int i = 0; i < n; i++) {
			crc = (crc >> 8) ^ Crc32_Table[(crc & 0xff) ^ p[i]];
		}
		for (int i = p.Length - 1; i >= 0; i--) {
			crcRev = (crcRev >> 8) ^ Crc32_Table[(crcRev & 0xff) ^ p[i]];
		}
	}

//------------------------------------------------------------------------

	public void AddData(string str) {
		// Originally, we got <rawBytes> by using System.Text.AsciiEncoding.
		// But that's a 7-bit encoding. So if our input <str> had any
		// bytes >= 0x80, the corresponding <rawBytes> would be 0x3f,
		// aka '?'. So we wouldn't get a proper CRC (e.g. matching what
		// was shown in PKZIP's CRC field). So now we do a full
		// UnicodeEncoding, and get a byte array exactly twice the length
		// of the string. We then take every other byte since, at least
		// for 8-bit ASCII strings (which is all we support), every other
		// byte is 0x00. Then we process that data.
		byte[] rawBytes = UnicodeEncoding.Unicode.GetBytes(str);
		byte[] rawbytes2 = new byte[rawBytes.Length / 2];
		for (int i = 0; i < rawbytes2.Length; i++) {
			rawbytes2[i] = rawBytes[i * 2];
		}
		AddData(rawbytes2);
	}

//------------------------------------------------------------------------

	private uint Reflect(uint Crc, int Bitnum) {
		uint i, j = 1, CrcOut = 0;

		for (i = 1u << (Bitnum - 1); i != 0; i >>= 1) {
			if ((Crc & i) != 0) {
				CrcOut |= j;
			}
			j <<= 1;
		}
		return CrcOut;
	}

//------------------------------------------------------------------------

	private void Init_CRC32_Table() {
		// Modify the following magic code at your own risk!
		if (bTableIsInitialized)
			return;

		uint ulPolynomial = 0x4c11db7;

		uint i, j;
		uint bit, crc;

		for (i = 0; i < 256; i++) {
			crc = i;
			crc = Reflect(crc, 8);
			crc <<= 32 - 8;

			for (j = 0; j < 8; j++) {
				bit = crc & CrcHighbit;
				crc <<= 1;
				if (bit != 0)
					crc ^= ulPolynomial;
			}

			crc = Reflect(crc, 32);
			crc &= 0xFFFFFFFF;
			Crc32_Table[i] = crc;
		}

		bTableIsInitialized = true;
	}
}
