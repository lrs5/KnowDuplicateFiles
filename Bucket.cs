namespace KnowDuplicateFiles;

public class Bucket {
	public long Size;
	public List<FileEntry> Entries;

//------------------------------------------------------------------------

	public Bucket() {
		Size = 0;
		Entries = new List<FileEntry>();
	}
}
