using System.Runtime.Intrinsics.X86;

namespace KnowDuplicateFiles;
public class FileEntry {
	public string DirName;		// The folder the file is in
	public string FileName;     // The filename (duh!)
	public long   FileSize;     // The size of the file
	public uint   ShortCRC;     // The CRC of the beginning of the file. 
	public bool	  IsBad;		// e.g. Can't open the file

//------------------------------------------------------------------------

	public string FullName { get => Path.Combine(DirName, FileName); }

//------------------------------------------------------------------------

	public FileEntry(string pathName, int fingerprintSize) {
		// We may well have the same directory name in many, many of
		// these class instances. Save on storage by interning the
		// name, so that they all refer to the same string object.
		DirName  = string.Intern(Path.GetDirectoryName(pathName)!);
		FileName = Path.GetFileName(pathName);
		FileSize = (new FileInfo(pathName)).Length;
		IsBad    = false;
		ShortCRC = GetShortCRC(pathName, fingerprintSize);	
	}

//------------------------------------------------------------------------

	public uint GetShortCRC(string filename, int nBytes) {
		byte[] bytes    = new byte[nBytes];
		try {
			using FileStream fs = new FileStream(filename,
				 FileMode.Open,
				 FileAccess.Read,
				 FileShare.Read,
				 nBytes);
			int n = fs.Read(bytes, 0, nBytes);
			// n = # of bytes read (in case filesize < nBytes)
			ShortCRC = 0;
			for (int i = 0; i < n; i++) {
				byte b = bytes[i];
				// Replace next line with a call to your own CRC routine
				// if you want to support older or non-Intel CPUs
				ShortCRC = Sse42.Crc32(ShortCRC, b);
			}
			return ShortCRC;
		} catch (Exception /* ex */) {
			IsBad = true;
			return 0;
		}
	}
}



