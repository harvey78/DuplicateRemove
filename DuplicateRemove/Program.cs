using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace DuplicateRemove
{
    class Program
    {
        static long Count = 0;

        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("Wrong parameter!");
                return;

            }

            string path = args[0];

            Dictionary<long, List<string>> filelist = new Dictionary<long, List<string>>();

            extractFile(filelist, path);

            Console.WriteLine(" ****************************  ");
            Console.WriteLine(" ****************************  ");
            Console.WriteLine(" ****************************  ");

            Console.WriteLine("Files: " + Count + " : " + filelist.Count);

            List<List<string>> duplicates = new List<List<string>>();

            foreach (var item in filelist.Values)
                if (item.Count > 1)
                {
                    Dictionary<string, List<string>> MD5Dict = new Dictionary<string, List<string>>();

                    foreach (var p in item)
                    {
                        string MD5 = CalculateMD5(p);

                        if (MD5Dict.ContainsKey(MD5))
                            MD5Dict[MD5].Add(p);
                        else
                            MD5Dict.Add(MD5, new List<string>() { p });
                    }

                    foreach (var p in MD5Dict.Values)
                        if (p.Count > 1)
                            duplicates.Add(p);

                    Console.WriteLine("duplicate: " + duplicates.Count);

                }

            Console.WriteLine(" ****************************  ");
            Console.WriteLine(" ****************************  ");
            Console.WriteLine(" ****************************  ");
            Console.WriteLine("duplicate find: " + duplicates.Count);
            Console.WriteLine(" ****************************  ");


            List<string> FileToDelete = new List<string>();

            foreach (var paths in duplicates)
            {
                SortedDictionary<DateTime, string> CreationDict = new SortedDictionary<DateTime, string>();
                foreach (var p in paths)
                    CreationDict.Add(File.GetCreationTime(p), p);

                Boolean Skip = true;
                foreach (var key in CreationDict.Keys)
                    if (Skip)
                        Skip = false;
                    else
                        FileToDelete.Add(CreationDict[key]);
            }

            Console.WriteLine(" ****************************  ");
            Console.WriteLine(" ****************************  ");
            Console.WriteLine(" ****************************  ");

            foreach (var item in FileToDelete)
            {
                Console.WriteLine("Delete   " + item);
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(item, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);

            }


        }

        static private void extractFile(Dictionary<long, List<string>> filelist, string path)
        {
            if (Directory.Exists(path))
            {
                foreach (var dirPaath in Directory.GetDirectories(path))
                    extractFile(filelist, dirPaath);

                foreach (var filePath in Directory.GetFiles(path))
                {
                    long length = new System.IO.FileInfo(filePath).Length;

                    if (filelist.ContainsKey(length))
                    {
                        filelist[length].Add(filePath);
                        Count++;
                    }
                    else
                    {
                        Count++;
                        filelist.Add(length, new List<string>() { filePath });
                    }
                }
                Console.WriteLine("Find  " + Count + " files");

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("Directory " + path + " not exist!");

            }
        }

        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
