using System;
using System.IO;
using System.IO.Compression;

namespace MakeArchive
{
	class Program
	{
		static void Main(string[] args)
		{
			var now = DateTime.Now;
			var tmp = Path.GetTempFileName();
			File.Delete(tmp);
			ZipFile.CreateFromDirectory("..\\Archive", tmp);

			// TODO: Get from file system
			var project = "Mario";
			var name = string.Format("{1}-{0:yy-dd-MM-HH}.zip", now, project);
			var path = "..\\Builds\\" + name;
			if (File.Exists(path))
			{
				Console.WriteLine("Replacing old version");
				File.Delete(path);
			}
			File.Move(tmp, path);
			Console.WriteLine("Wrote {0} to Builds", name);
		}
	}
}
