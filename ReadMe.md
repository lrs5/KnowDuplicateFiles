# KnowDuplicateFiles

C# Code (but no UI) to detect duplicate (or singleton) files. Works cross-platform on Windows, MacOS, Linux, and possibly Android and iOS (not tested). If files or folders have permission limitations, the code may have problems.

# TODO: See the World to be Done and modify this file to remove things that fall in those to-do features.

# Initial comments

* Two files are identical if (a) they have the same size, and (b) have the same contents. Other information, such as file creation/update dates are irrelevant.
* Checking for the same contents won't be a problem for small files, but checking large files would slow down the program.
* To speed things up, we read just the first 512 (configurable) bytes of each file and calculate the 32-bit Cyclic Redundancy Check (CRC) of that prefix. We call this the fingerprint of the file. Even a single bit change in the prefix will yield a vastly different fingerprint. 

# Inputs

* The length of the fingerprint prefix (defaults to 512).
* A list of folder names to be scanned. Their sub-folders are processed recursively (a non-recursive option is planned for a future release).
* A list of folders to not be scanned. For example, scan C:\, but don't process C:\Windows.
* A list of folders to be scanned ever if they're sub-folders of don't-scan folders (e.g. Scan "C:\" but don't scan C:\Windows, but do scan C:\Windows\System32.
* A list of filename suffixes (e.g. ".jpg" and ".jpeg" and "data.png". Or just "*" for all files.) to be processed. If a filename doesn't match one of these it will be ignored.
* A list of file sizes (e.g. 10MB-to-20MB or >1GB or <1KB). Any files not within the specified ranges will be ignored.

# Program Logic

* The specified folders are filtered by the input filters and will generate all fully-qualified filenames (e.g. "G:\MyFolder\Windows.iso") along with their file sizes and fingerprints.
* These are grouped into buckets by filesize and within filesize, grouped by fingerprint. Any buckets with only one entry (but see below) will not be returned to the caller.
	* If you read the source, you'll see a very nice C# LINQ (Language Integrated Query) expression to do this.
* These buckets are returned to the caller which is responsible for presenting them to the user.

# Singletons

* An option is to return only those buckets that contain only a single filename. This can be useful to, for example, compare a folder and its backup folder to see if the folders are out of sync, with files existing in one folder but not in the other.

# Full File Comparison

* A future objective is to be able to do file comparisons on the entire contents of a file, rather than relying on a file's fingerprint.

# User Interface

* This can vary tremendously based upon many factors:
  * The operating system (Windows vs. Android vs. etc.)
  * The GUI package used (e.g. Xamarin, Maui, Avalonia, etc.)
  * Whether a GUI is used at all, as opposed to, for example, a configuration file
  * The creativity of the programmer deciding what the ideal interface is
  * And so forth
* So for these reasons, this package deliberately eschews all aspects of the user interface.

# Work to be Done

* A 4-byte CRC fingerprint is an excellent approximation to indicate whether a file is different from another file. But it's hardly foolproof.
	* Add the ability to do a full file compare.
* Add support for a do-not-scan a specified folder.
* Add support for specifying that a folder's child folders should not be scanned recursively.
* Provide sample code to use this module.