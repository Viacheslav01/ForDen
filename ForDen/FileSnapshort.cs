using System;
using System.IO;

namespace ForDen
{
	internal class FileSnapshort
	{
		public FileSnapshort(FileInfo fileInfo)
		{
			Guard.NotNull(fileInfo);

			Length = fileInfo.Length;
			LastWriteTime = fileInfo.LastWriteTime;
		}

		public bool CompareAndUpdate(FileInfo fileInfo)
		{
			var result = Length == fileInfo.Length
			             && LastWriteTime == fileInfo.LastWriteTime;

			Length = fileInfo.Length;
			LastWriteTime = fileInfo.LastWriteTime;

			return result;
		}

		public long Length { get; private set; }
		public DateTime LastWriteTime { get; private set; }
	}
}