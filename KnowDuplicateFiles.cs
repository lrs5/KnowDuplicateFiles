// Copyright (c) 2022 by Larry Smith

// TODO: Needs Exclude Directories feature

// TODO: I suppose I really should have more (or is that, "some") error
//		 checking. I think it's pretty bulletproof as it stands. The only
//		 problems I can think of is (a) running out of memory (not a whole
//		 lot I can do about that), or (b) running this and somebody
//		 deletes one of our folders while the program is running, or (c)
//		 there are a couple of race conditions such as a filename being
//		 deleted or permissions have changed after we've enumerated it.
//		 Some day I may even go back over this and put in some try/catch
//		 logic.

// TODO: These next two points are clearly in the UI's realm. As long
//		 as it does reasonable error checking (e.g. that a folder exists,
//		 that the user hasn't specified a filesize > long.MaxValue or
//		 < 0, and so on), then this code should work.


// TODO: UI should do performance check that a folder isn't a sub-folder
//		 of a folder that's already on the list of folders. Note this is
//		 a bit harder to support since the child folder may be added to
//		 the list before the higher-level folder is added.

// TODO: It gets even trickier if you want to support, say, not looking
//		 in the C:\Windows folder, but you do want to look in, say, the
//		 C:\Windows\System32 folder.

// TODO: Related to previous TODO -- Add support for non-recursive folders
//		 and for Ignorable folders (e.g. Backup folders, known to contain
//		 duplicates.

// TODO: Maybe a feature to show files that are *not* duplicates, such
//		 a backup folder that doesn't have recent backups.

namespace KnowDuplicateFiles;
public class KnowDuplicateFiles {
	// TODO: Maybe a class with these
	// TODO: These should not be public
	static public int nFilesScanned;
	static public int nFilesProcessed;
	static public int nFilesUnreadable;

	static public List<string> UnreadableFiles; // TODO: static???

	readonly Extensions	 extensions;
	readonly Filesizes	 filesizes;
	readonly FolderNames pathnames;
	readonly int		 fingerprintSize;

//------------------------------------------------------------------------

	public KnowDuplicateFiles(
				Extensions  extensions,
				Filesizes   filesizes,
				FolderNames pathnames,
				int			fingerprintSize = 512) {
		this.extensions      = extensions;
		this.filesizes       = filesizes;
		this.pathnames       = pathnames;
		this.fingerprintSize = fingerprintSize;

		UnreadableFiles = new List<string>();
	}

//------------------------------------------------------------------------

	public IEnumerable<Bucket> FindDuplicates() {
		nFilesScanned    = 0;
		nFilesProcessed  = 0;
		nFilesUnreadable = 0;
		UnreadableFiles  = new();

		var qry =
			from entry in pathnames.Entries(fingerprintSize, extensions, filesizes)
			group entry by new {
				size = entry.FileSize,
				crc = entry.ShortCRC
			} into buckets
			where buckets.Count() > 1
			orderby buckets.Key.size descending
			select new Bucket {
				Size = buckets.Key.size,
				Entries = buckets.ToList<FileEntry>()
			};
		foreach (var buck in qry) {
			yield return buck;
		}
	}

//------------------------------------------------------------------------

	public IEnumerable<Bucket> FindSingletons() {
		nFilesScanned    = 0;
		nFilesProcessed  = 0;
		nFilesUnreadable = 0;
		UnreadableFiles  = new();

		var qry =
			from entry in pathnames.Entries(fingerprintSize, extensions, filesizes)
			group entry by new {
				size = entry.FileSize,
				crc = entry.ShortCRC
			} into buckets
			where buckets.Count() == 1
			orderby buckets.Key.size descending
			select new Bucket {
				Size = buckets.Key.size,
				Entries = buckets.ToList<FileEntry>()
			};
		foreach (var buck in qry) {
			yield return buck;
		}
	}
}
