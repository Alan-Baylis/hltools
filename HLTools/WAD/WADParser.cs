using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

using HLTools.Extensions;

namespace HLTools.WAD
{
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct WADFile
	{
		public uint offset;
		public uint compressedFileSize;
		public uint uncomrepssedFileSize;
		public byte fileType;
		public byte compressionType;
		public byte padding;
		public byte padding2;
		public StructString filename;
	}

	public class WADParser
	{
		public delegate void LoadFileDelegate(WADFile file);
		public event LoadFileDelegate OnLoadFile;

		public static readonly string MagicString = "WAD3";
		public static readonly int    MagicInt    = 1463895091;

		private BinaryReader br;

		public WADParser(Stream stream)
		{
			br = new BinaryReader(stream);

			// TODO: through exception on fake magic
			bool magic = CheckMagic(br.ReadBytes(4));
			FileCount = br.BReadUInt32();
			Offset = br.BReadUInt32();
		}

		public void LoadFiles()
		{
			if (OnLoadFile != null) {
				br.BaseStream.Seek(Offset, SeekOrigin.Begin);
				for (int i = 0; i < FileCount; i++) {
					OnLoadFile(br.ReadStruct<WADFile>());
				}
			}
		}

		public WADFile[] LoadFileArray()
		{
			br.BaseStream.Seek(Offset, SeekOrigin.Begin);
			var n = FileCount;
			var files = new WADFile[n];
			for (int i = 0; i < n; i++) {
				files[i] = br.ReadStruct<WADFile>();
			}
			return files;
		}

		public static bool CheckMagic(byte[] bytes)
		{
			return CheckMagic(bytes, 0);
		}

		public static bool CheckMagic(byte[] bytes, int startindex)
		{
			if (bytes.Length < startindex + 4) {
				return false;
			}
			string m = Encoding.ASCII.GetString(bytes, startindex, 4);
			return (m == MagicString);
		}

		public byte[] LoadFile(WADFile file)
		{
			br.BaseStream.Seek(file.offset, SeekOrigin.Begin);
			return br.ReadBytes((int)file.compressedFileSize);
		}

		public void Close()
		{
			br.Close();
		}

		public uint Offset { get; set; }
		public uint FileCount { get; set; }
	}
}

