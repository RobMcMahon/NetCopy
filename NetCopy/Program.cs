using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace NetCopy
{
    class Program
    {
        

        static void Main(string[] args)
        {
            ParseArgs(args);



            var worker = new ShareDiscoveryWorker();
            worker.Start();
            
            while (true)
                continue;

            //if(!string.IsNullOrWhiteSpace(TargetFolder))
            //    targetFolders.Add(TargetFolder);

            //if (SearchAD)
            //    SearchActiveDirectoryComputers(targetFolders);
            //if (SearchLan)
            //    SearchLanComputers(targetFolders);
            //if (SearchNearbyNetworks)
            //    SearchNearbyNetworkComputers(targetFolders);
            
            ////If there's no destination then just list
            //if (List)
            //    ListContents(TargetFolder);
            //if(Copy)
            //    CopyContents(TargetFolder);


        }

        private static void SearchNearbyNetworkComputers(List<string> targetFolders)
        {
            throw new NotImplementedException();
        }

        private static void SearchLanComputers(List<string> targetFolders)
        {
            throw new NotImplementedException();
        }

        private static void SearchActiveDirectoryComputers(List<string> targetFolders)
        {
            throw new NotImplementedException();
        }

        //private static void CopyContents(string directory)
        //{
        //    List<string> files = new List<string>();

        //    foreach(var filter in FileFilters)
        //        files.AddRange(Directory.EnumerateFiles(directory, filter, Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

        //    try
        //    {
        //        foreach (var file in files)
        //        {
        //            var fileName = Path.GetFileName(file);
        //            if (File.Exists(Path.Combine(DestinationFolder, fileName)))
        //                fileName = string.Format("{0}_{1}.{2}", Path.GetFileNameWithoutExtension(fileName), DateTime.Now.AddDays(-3).ToString("yyyyMMddHHmmss"), Path.GetExtension(fileName));
        //            Console.WriteLine(string.Format("Copying File {0}", fileName));

        //            File.Copy(file, Path.Combine(DestinationFolder, fileName));
        //        }
        //    }
        //    catch (Exception e) { Console.WriteLine(e.Message); }
        //}

        private static void ListContents(string directory)
        {
            var entries = Directory.EnumerateFiles(directory);

            Console.WriteLine();
            Console.WriteLine("=====Files=====");
            Console.WriteLine();
            foreach (var entry in entries)
            {
                Console.WriteLine(entry);
            }

            var directories = Directory.EnumerateDirectories(directory);
            Console.WriteLine();
            Console.WriteLine("=====Folders=====");
            Console.WriteLine();
            foreach(var dir in directories)
                Console.WriteLine(dir);
        }

        private static void ParseArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                switch(arg)
                {
                    case "-l":
                        SessionConfiguration.Instance.List = true;
                        break;
                    case "-c":
                        SessionConfiguration.Instance.Copy = true;
                        break;
                    case  "-d":
                        SessionConfiguration.Instance.DestinationFolder = args[i + 1];
                        break;
                    case "-t":
                        SessionConfiguration.Instance.TargetFolder = args[i + 1];
                        break;
                    case "-f":
                        SessionConfiguration.Instance.FileFilters.Add(args[i + 1]);
                        break;
                    case "-r":
                        SessionConfiguration.Instance.Recursive = true;
                        break;
                    case "--documents":
                        SessionConfiguration.Instance.FileFilters.AddRange(new List<string> { "*.pdf", "*.xls*", "*.doc*", "*.odt", "*.txt", "*.rtf" });
                        break;
                    case "--zip-files":
                        SessionConfiguration.Instance.FileFilters.AddRange(new List<string> { "*.7z", "*.rar", "*.zip" });
                        break;
                    case "--audio-files":
                        SessionConfiguration.Instance.FileFilters.AddRange(new List<string> { "*.mp3", "*.wav", "*.ogg", "*.flac", "*.gsm", "*.dct", "*.au", "*.aiff", "*.vox", "*.raw", "*.wma", "*.aac" });
                        break;
                    case "--encrypted-files":
                        SessionConfiguration.Instance.FileFilters.AddRange(new List<string> { "*.kdbx", "*.pgp", "*.gpg" });
                        break;
                    case "--search-ad":
                        SessionConfiguration.Instance.SearchAD = true;
                        break;
                    case "--search-lan":
                        SessionConfiguration.Instance.SearchLan = true;
                        break;
                    case "--search-close-networks":
                        SessionConfiguration.Instance.SearchNearbyNetworks = true;
                        break;
                    case "-x":
                        SessionConfiguration.LoadFromXml(args[i + 1]);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
