// Copyright (c) 2022 by Larry Smith

namespace KnowDuplicateFiles;
public class Extensions {
	readonly List<string> extensions = new();

//------------------------------------------------------------------------

	public Extensions() {
		// List<string> already created. User wants to add
		// extensions by calling the Add() method
	}

//------------------------------------------------------------------------

	public void Clear() {
		extensions.Clear();
	}

//------------------------------------------------------------------------

	public Extensions(params string[] exts) {
		foreach (string ext in exts) {
			Add(ext);
		}
	}

//------------------------------------------------------------------------

	public Extensions(List<string> exts) {
		foreach (string ext in exts) {
			Add(ext);
		}
	}

//------------------------------------------------------------------------

	public void Add(string ext) {
		extensions.Add(ext);
	}

//------------------------------------------------------------------------

	public IEnumerable<string> List() {
		foreach (string ext in extensions) { yield return ext; }
	}
}
