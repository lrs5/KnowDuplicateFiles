namespace KnowDuplicateFiles;

public class FolderNames {
	private readonly List<string> FolderList;

//------------------------------------------------------------------------

	public FolderNames() {
		FolderList = new ();
	}

//------------------------------------------------------------------------

	public void Add(params string[] pathnames) {
		foreach (string dirname in pathnames) {
			// Next line for easier parsing later
			string dir = dirname.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			if (!Directory.Exists(dir)) {
				throw new ArgumentException($"Directory '{dir}' does not exist");
			}
			FolderList.Add(dirname);
		}
	}

//------------------------------------------------------------------------

	public IEnumerable<FileEntry> Entries(int fingerprintSize, Extensions extsToInclude, Filesizes sizesToInclude) {
		EnumerationOptions opts = new EnumerationOptions {
			RecurseSubdirectories = true, 
			IgnoreInaccessible    = true,
			// AttributesToSkip	  = FileAttributes.Hidden | FileAttributes.System
		};
		foreach (string dirname in FolderList) {
				foreach (string filename in Directory.EnumerateFileSystemEntries(dirname, "*", opts)) {
					if (Directory.Exists(filename)) {
						// Note: We expect FolderList to come in using
						//		 Path.DirectorySeparatorCharacter
						// TODO: Add code here to skip folder
						continue;
					}
					++KnowDuplicateFiles.nFilesScanned;
					if (ShouldInclude(filename, extsToInclude, sizesToInclude)) {
						var fe = new FileEntry(filename, fingerprintSize);
						if (fe.IsBad) {
							++KnowDuplicateFiles.nFilesUnreadable;
							KnowDuplicateFiles.UnreadableFiles.Add(filename);
							continue;	// i.e. Ignore this
						}
						++KnowDuplicateFiles.nFilesProcessed;
						yield return fe;
					}
				}
			// }
		}

	}

//------------------------------------------------------------------------

	private static bool ShouldInclude(string filename, Extensions extsToInclude, Filesizes sizesToInclude) {
		// TODO: Should return FileInfo itself, and also bool result
		bool ExtOK = false;
		foreach (string ext in extsToInclude.List()) {
			if ((ext == "*") || (filename.EndsWith(ext))) { ExtOK = true; break; }
		}
		if (ExtOK == false) { return false; }
		try {
			long length = new FileInfo(filename).Length;
			foreach ((long low, long high) in sizesToInclude.List()) {
				if ((length > low) && (length <= high)) {
					return true;
				}
			}
		} catch (Exception ex) {
			throw new Exception($"ShouldInclude: {filename}, {ex.Message}");
		}
		return false;
	}
}
