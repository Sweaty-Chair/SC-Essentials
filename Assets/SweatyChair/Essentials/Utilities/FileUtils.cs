using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SweatyChair
{

	public static class FileUtils
	{

		#region File Permissions

		public static bool IsWritable(string path)
		{
			if (File.Exists(path)) {
				FileInfo fi = new FileInfo(path);
				if (fi.Attributes.ToString().IndexOf("ReadOnly", System.StringComparison.Ordinal) == -1)
					return true;
			}
			return false;
		}

		public static void UnsetReadOnly(string path, bool showWarning = true)
		{
			if (File.Exists(path)) {
				FileInfo fi = new FileInfo(path);
				if (fi.Attributes.ToString().IndexOf("ReadOnly", System.StringComparison.Ordinal) != -1) {
					fi.Attributes = FileAttributes.Normal;
					if (showWarning)
						Debug.LogWarningFormat("FileUtils:UnsetReadOnly - File is read only, make sure to checkout in perforce first, path={0}", path);
				}
			} else if (showWarning) {
				Debug.LogWarningFormat("FileUtils:UnsetReadOnly - Can't find file at {0}", path);
			}
		}

		#endregion

		#region Checks

		/// <summary>
		/// Check if a given path a file, disregarding the file exists or not.
		/// </summary>
		public static bool IsFile(string path)
		{
			if (path == null) throw new System.ArgumentNullException(nameof(path));
			path = path.Trim();

			if (File.Exists(path))
				return true;
			if (Directory.Exists(path))
				return false;

			// Neither file nor directory exists. guess intention

			// If has trailing slash then it's a directory
			if (new[] { "\\", "/" }.Any(x => path.EndsWith(x, System.StringComparison.Ordinal)))
				return false;
			// If has extension then its a file; directory otherwise
			return !string.IsNullOrWhiteSpace(Path.GetExtension(path));
		}

		/// <summary>
		/// Check if a given path a directory, disregarding the directory exists or not.
		/// </summary>
		public static bool IsDirectory(string path)
		{
			if (path == null) throw new System.ArgumentNullException(nameof(path));
			path = path.Trim();

			if (Directory.Exists(path))
				return true;
			if (File.Exists(path))
				return false;

			// Neither file nor directory exists. guess intention

			// If has trailing slash then it's a directory
			if (new[] { "\\", "/" }.Any(x => path.EndsWith(x, System.StringComparison.Ordinal)))
				return true;
			// If has extension then its a file; directory otherwise
			return string.IsNullOrWhiteSpace(Path.GetExtension(path));
		}

		/// <summary>
		/// Check if a given path a valid, create it otherwise.
		/// </summary>
		public static string Validate(string path)
		{
			if (IsDirectory(path)) {
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
			} else {
				FileInfo fi = new FileInfo(path);
				Validate(fi.DirectoryName);
			}
			return path;
		}

		#endregion

		#region Create Directory

		/// <summary>
		/// Create all parent folders of a file/directory path, if not existed.
		/// </summary>
		public static void CreateDirectory(string path)
		{
			// For for issue where directory would not get created propertly if the provided path was already a directory path. as GetDirectoryName will get the parent directory
			// https://stackoverflow.com/questions/25975880/how-to-check-if-a-string-path-is-a-file-or-directory-if-path-doesnt-exist
			if (Path.HasExtension(path))
				Directory.CreateDirectory(Path.GetDirectoryName(path).Replace(@"\", "/")); // Make sure no backward-slashes
			else
				Directory.CreateDirectory(path.Replace(@"\", "/")); // Make sure no backward-slashes

		}

		#endregion

		#region Delete

		/// <summary>
		/// Delete a file/directory recursively.
		/// </summary>
		public static void Delete(string path)
		{
			if (Directory.Exists(path)) {
				DeleteDirectory(path);
			} else if (File.Exists(path)) {
				DeleteFile(path);
			} else {
				Debug.LogFormat("FileUtils:Delete - Can't find folder/file at {0}", path);
			}
		}

		/// <summary>
		/// Delete a directory recursively.
		/// </summary>
		public static void DeleteDirectory(string path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			if (directoryInfo != null && directoryInfo.Exists) {
				if (directoryInfo.GetFiles().Length != 0 || directoryInfo.GetDirectories().Length != 0) {
					// Loop through and delete all children files and directories
					foreach (string entry in Directory.GetFileSystemEntries(path)) {
						if (File.Exists(entry)) {
							DeleteFile(entry);
						} else {
							DirectoryInfo di = new DirectoryInfo(entry);
							if (di.GetFiles().Length != 0 || di.GetDirectories().Length != 0)
								DeleteDirectory(di.FullName); // Delete the sub-folders recursively
							DeleteEmptyDirectory(entry);
						}
					}
				}
				DeleteEmptyDirectory(path); // Delete self
			} else {
				Debug.LogWarningFormat("FileUtils:DeleteDirectory - Can't find folder at {0}", path);
			}
		}

		/// <summary>
		/// Delete an empty directory, even it's readonly.
		/// </summary>
		private static void DeleteEmptyDirectory(string path, bool showWarning = false)
		{
			try {
				DirectoryInfo directoryInfo = new DirectoryInfo(path);
				if (directoryInfo != null) {
					if (directoryInfo.Attributes.ToString().IndexOf("ReadOnly", System.StringComparison.Ordinal) != -1)
						directoryInfo.Attributes = FileAttributes.Normal;
					Directory.Delete(path);
				} else if (showWarning) {
					Debug.LogWarningFormat("FileUtils:ForceDeleteDirectory - Can't find folder at {0}", path);
				}
			} catch (DirectoryNotFoundException e) {
				if (showWarning)
					Debug.LogWarningFormat("FileUtils:ForceDeleteDirectory - Path: {1}, Error: {0}", path, e);
			}
		}

		/// <summary>
		/// Delete a file, even it's readonly.
		/// </summary>
		public static void DeleteFile(string path, bool showWarning = false)
		{
			if (File.Exists(path)) {
				FileInfo fi = new FileInfo(path);
				if (fi.Attributes.ToString().IndexOf("ReadOnly", System.StringComparison.Ordinal) != -1)
					fi.Attributes = FileAttributes.Normal;
				File.Delete(path);
			} else if (showWarning) {
				Debug.LogWarningFormat("FileUtils:DeleteFile - File not exists at {0}", path);
			}
		}

		/// <summary>
		/// Deletes all empty directories and sub-directories within a root directory.
		/// Should be very performant according to https://stackoverflow.com/a/2811746
		/// </summary>
		/// <param name="rootDirectory"></param>
		public static void DeleteEmptyDirsRecursively(string rootDirectory)
		{
			// IF we have an empty string, dont attempt delete, just return
			if (string.IsNullOrWhiteSpace(rootDirectory))
				return;

			try {
				foreach (var d in Directory.EnumerateDirectories(rootDirectory)) {
					DeleteEmptyDirsRecursively(d);
				}

				var entries = Directory.EnumerateFileSystemEntries(rootDirectory);

				if (!entries.Any()) {
					try {
						Directory.Delete(rootDirectory);
					}
					catch (UnauthorizedAccessException) { }
					catch (DirectoryNotFoundException) { }
				}
			}
			catch (UnauthorizedAccessException) { }
		}

		/// <summary>
		/// Delete a file in persistent data path.
		/// </summary>
		public static void DeletePersistentDataFile(string filename, bool showWarning = false)
		{
			DeleteFile(Path.Combine(Application.persistentDataPath, filename), showWarning);
		}

		/// <summary>
		/// Empty a folder, with exclusion of a list children files/directories.
		/// </summary>
		public static void EmptyDirectory(string path, params string[] excludes)
		{
			List<string> excludesList = new List<string>();
			if (excludes != null && excludes.Length > 0)
				excludesList = new List<string>(excludes);
			foreach (string entry in Directory.GetFileSystemEntries(path)) {
				if (File.Exists(entry)) {
					if (excludesList.Contains(Path.GetFileName(entry)))
						continue;
					DeleteFile(entry);
				} else { // Directory
					if (excludesList.Contains(Path.GetDirectoryName(entry)))
						continue;
					DirectoryInfo di = new DirectoryInfo(entry);
					if (di.GetFiles().Length != 0 || di.GetDirectories().Length != 0)
						DeleteDirectory(di.FullName); // Delete the sub-folders recursively
					DeleteEmptyDirectory(entry);
				}
			}
		}

		#endregion

		#region Copy

		/// <summary>
		/// Copy a file/directory, given the option to overwrite.
		/// </summary>
		public static void Copy(string src, string dst, bool overwrite = true)
		{
			if (Directory.Exists(src))
				CopyDirectory(src, dst, overwrite);
			else if (File.Exists(src))
				CopyFile(src, dst, overwrite);
			else
				Debug.LogWarningFormat("FileUtils:Copy - Can't find source folder/file at {0}", src);
		}

		/// <summary>
		/// Copy a directory, given the option to overwrite.
		/// </summary>
		public static void CopyDirectory(string src, string dst, bool overwrite = true)
		{
			DirectoryInfo sourceDI = new DirectoryInfo(src);
			if (!sourceDI.Exists) {
				Debug.LogWarningFormat("FileUtils:CopyDirectory - Can't find source folder {0}", src);
				return;
			}

			Directory.CreateDirectory(dst);
			foreach (FileSystemInfo fsi in sourceDI.GetFileSystemInfos()) {
				string targetPath = Path.Combine(dst, fsi.Name);
				if (fsi is FileInfo) {
					// Skip the system useless files
					if (Path.GetFileName(fsi.FullName) == ".DS_Store")
						continue;
					DeleteFile(targetPath, false); // Overwrite old file if any, delete the old one first
					File.Copy(fsi.FullName, targetPath);
				} else {
					Directory.CreateDirectory(targetPath);
					CopyDirectory(fsi.FullName, targetPath, overwrite); // Recursively copy files and sub folders
				}
			}
		}

		/// <summary>
		/// Copy a file, given the option to overwrite.
		/// </summary>
		public static void CopyFile(string sourcePath, string destinationPath, bool overwrite = true)
		{
			if (File.Exists(sourcePath)) {
				Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)); // Create the destination parent folders is not yet created
				File.Copy(sourcePath, destinationPath, overwrite);
			} else {
				Debug.LogWarningFormat("FileUtils:CopyFile - Can't find source file at {0}", sourcePath);
			}
		}

		#endregion

		#region Rename

		/// <summary>
		/// Rename a file/directory.
		/// </summary>
		public static void Rename(string sourcePath, string dst)
		{
			if (Directory.Exists(sourcePath)) { // If directory
				if (Directory.Exists(dst))
					DeleteDirectory(dst);
				Directory.Move(sourcePath, dst);
			} else if (File.Exists(sourcePath)) { // If file
				if (File.Exists(dst))
					File.Replace(sourcePath, dst, dst + ".bak");
				else
					File.Move(sourcePath, dst);
			} else {
				Debug.LogWarningFormat("FileUtils:Rename - Can't find source folder/file at {0}", sourcePath);
			}
		}

		#endregion

		#region File Count

		/// <summary>
		/// Count the children directories.
		/// </summary>
		public static int GetDirectoryCount(string path)
		{
			DirectoryInfo di = new DirectoryInfo(path);
			if (di != null)
				return di.GetDirectories().Length;
			return 0;
		}

		#endregion

		#region Filename Validation

		public static bool IsValidFileName(string name)
		{
			string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
			string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
			return System.Text.RegularExpressions.Regex.IsMatch(name, invalidRegStr);
		}

		public static string MakeValidFileName(string name)
		{
			string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
			string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
			return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
		}

		#endregion

	}

}