namespace KnowDuplicateFiles;
public class Filesizes {
	readonly List<(long low, long high)> sizes = new();

//------------------------------------------------------------------------

	public Filesizes() {
		// List<(long, long)> already created. User wants to add
		// extensions by calling the Add() method
	}

//------------------------------------------------------------------------

	private void Clear() {
		sizes.Clear();
	}

//------------------------------------------------------------------------

	public Filesizes(params (long, long)[] sizes) {
		foreach ((long, long) size in sizes) {
			Add(size);
		}
	}

//------------------------------------------------------------------------

	public void Add((long, long) ext) {
		sizes.Add(ext);
	}

//------------------------------------------------------------------------

	public IEnumerable<(long low, long high)> List() {
		foreach ((long, long) size in sizes) { yield return size; }
	}
}

