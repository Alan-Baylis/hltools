using System;
using System.IO;
using System.Collections.Generic;
using HLTools;

namespace HLTools.Test
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			VerifierEvents v = new VerifierEvents(new Verifier(args[0], args[1]));
			
			int badpoints = 0;
			
			v.MalformedWadFiles += delegate(string[] wadfiles) {
				if (wadfiles.Length > 0) {
					Console.WriteLine("The referenced wad file are malformed:");
				}
				foreach (string wadfile in wadfiles) {
					Console.WriteLine("  {0}", wadfile);
				}
				badpoints += wadfiles.Length;
			};
			
			v.MisnamedModDirs += delegate(string[] wadfiles) {
				if (wadfiles.Length > 0) {
					Console.WriteLine("Mod dirs do not exist:");
				}
				foreach (string wadfile in wadfiles) {
					Console.WriteLine("  {0}", wadfile);
				}
				badpoints += wadfiles.Length * 2;
			};
			
			List<string> missingWads = new List<string>();
			
			v.NotExistingFiles += delegate(string[] wadfiles) {
				missingWads.AddRange(wadfiles);
				if (wadfiles.Length > 0) {
					Console.WriteLine("Files do not exist:");
				}
				foreach (string wadfile in wadfiles) {
					Console.WriteLine ("  {0}", wadfile);
				}
				badpoints += wadfiles.Length * 4;
			};
			
			List<string> missingFiles = new List<string>();
			
			v.MissingTextures += delegate(string[] missingTextures) {
				if (missingTextures.Length == 0) {
					return;
				}

				Console.WriteLine("Missing textures");
				foreach (string missingTexture in missingTextures) {
					Console.WriteLine("  {0}", missingTexture);
				}

				Console.WriteLine("Could be in these wad files:");
				foreach (string missingWad in missingWads) {
					Console.WriteLine("  {0}", missingWad);
				}
			};
			
			v.MissingSprites += delegate(string[] missingSprites) {
				missingFiles.AddRange(missingSprites);
			};
			
			v.MissingSounds += delegate(string[] missingSounds) {
				missingFiles.AddRange(missingSounds);
			};
			
			v.MissingModels += delegate(string[] missingModels) {
				missingFiles.AddRange(missingModels);
			};
			
			if (missingFiles.Count > 0) {
				foreach (string file in missingFiles) {
					Console.WriteLine ("  {0}", file);	
				}
				System.Environment.Exit(0);
			}
			
			if (args.Length > 2) {
				v.DispatchEvents(args[2]);
				Console.WriteLine("Bad points: {0}", badpoints);
			} else {
				foreach (var file in Directory.GetFiles(v.Verifier.ModMapsDirectory, "*.bsp")) {
					FileInfo fi = new FileInfo(file);
					Console.WriteLine(fi.Name);
					v.DispatchEvents(fi.Name);
					Console.WriteLine("Bad points: {0}", badpoints);
					badpoints = 0;
				}
			}
		}
	}
}

