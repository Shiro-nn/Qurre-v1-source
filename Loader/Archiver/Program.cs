using System;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
namespace Archiver
{
	internal class Program
	{
		public static void Main(string[] _)
		{
			CreateTarGZ();
		}
		private static void CreateTarGZ()
		{
			string sourceDirectory = "C:/Users/fydne1/Desktop/code/Qurre/zip";
			Stream outStream = File.Create("Qurre.tar.gz");
			Stream gzoStream = new GZipOutputStream(outStream);
			TarArchive tarArchive = TarArchive.CreateOutputTarArchive(gzoStream);
			tarArchive.RootPath = sourceDirectory.Replace('\\', '/');
			if (tarArchive.RootPath.EndsWith("/"))
				tarArchive.RootPath = tarArchive.RootPath.Remove(tarArchive.RootPath.Length - 1);

			AddDirectoryFilesToTar(tarArchive, sourceDirectory, true, true);

			tarArchive.Close();
		}

		private static void AddDirectoryFilesToTar(TarArchive tarArchive, string sourceDirectory, bool recurse, bool isRoot)
		{
			TarEntry tarEntry;

			if (!isRoot)
			{
				tarEntry = TarEntry.CreateEntryFromFile(sourceDirectory);
				tarArchive.WriteEntry(tarEntry, false);
			}
			string[] filenames = Directory.GetFiles(sourceDirectory);
			foreach (string filename in filenames)
			{
				tarEntry = TarEntry.CreateEntryFromFile(filename);
				Console.WriteLine(tarEntry.Name);
				tarArchive.WriteEntry(tarEntry, true);
			}

			if (recurse)
			{
				string[] directories = Directory.GetDirectories(sourceDirectory);
				foreach (string directory in directories)
					AddDirectoryFilesToTar(tarArchive, directory, recurse, false);
			}
			Console.WriteLine("Успешно");
		}
	}
}