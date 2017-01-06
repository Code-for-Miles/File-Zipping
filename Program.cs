
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Collections.Generic;

namespace ConsoleApplication
{
    class Program
    {
        static int daysAgo = 0;
        static String extension = null;
        static String[] files_toBeZipped = null;
        static String[] allFiles = null;
        static String zipPath = null;//@"c:\users\mengdaw\Documents\";
        static String startPath = null;

        static void Main()
        {
            try
            {
                Console.WriteLine("The program zips based on modified dates of files, " +
                "how many days old do you want the files to be? please enter a number.");
                daysAgo = Int32.Parse(Console.ReadLine());

                Console.WriteLine("What is the file mask or extension?");
                extension = Console.ReadLine();

                Console.WriteLine("Please enter a full and specific Directory or path to the folder where you have the files you want zipped in\n" +
                "this fomart @\"C:\\Users\\...\\Documents\\FolderName.\"\n");
                startPath = Console.ReadLine();//@"C:\Users\mengdaw\Documents\Files_Created";

                Console.WriteLine("Please enter a full and specific Directory or path to where your Zipfolder will be created and saved.\n");
                zipPath = Console.ReadLine();

                char last = zipPath[zipPath.Length - 1];
                if (!(last.Equals('\\')))
                {
                    zipPath = zipPath + "\\";
                }

                TraverseTree(startPath);

                if (checking_If_Files_Need_Be_zipped() > 0)
                {
                    zipping_qualifiedFiles();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " You will have to start all over!");
            }
            Console.WriteLine("Press any key to exit!");
            Console.ReadKey();
        }
        public static void TraverseTree(string startPath)
        {
            // Data structure to hold names of subfolders to be 
            // examined for files.
            //////List<String> files = new List<string>();
            try
            {
                Stack<string> directories = new Stack<string>();

                if (!Directory.Exists(startPath))
                {
                    throw new ArgumentException();
                }
                directories.Push(startPath);

                while (directories.Count > 0)
                {
                    string currentDirectory = directories.Pop();
                    string[] subDirectories;
                    try
                    {
                        subDirectories = Directory.GetDirectories(currentDirectory);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine(e.Message);
                        continue;
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        Console.WriteLine(e.Message);
                        continue;
                    }

                    try
                    {
                        ///<summary>
                        //this code gets the files out of the startPath directory
                        // and stores them into the "allFiles" array.
                        ///</summary

                        allFiles = Directory.GetFiles(currentDirectory);
                        //foreach (var el in allFiles)
                        //{
                        //    //files.Add(el);
                        //    Console.WriteLine(el);
                        //}

                    }

                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine(e.Message);
                        continue;
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        Console.WriteLine(e.Message);
                        continue;
                    }

                    foreach (string str in subDirectories)
                    {
                        directories.Push(str);
                    }

                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);

            }

        }
        //<summary>
        //the following method gets all the files out of the specified directory and examines them based on the conditions specified.
        //if the files meet the requirement, those files will be stored into the "files_toBeZipped" array.
        //the method also returns the number of files that meet the rquirements.
        // </summary>
        public static int checking_If_Files_Need_Be_zipped()
        {
            List<string> listOfFilesTobeZipped = new List<string>();
            try
            {
                foreach (var str in allFiles)
                {
                    var file = new DirectoryInfo(str);

                    if (DateTime.UtcNow - file.LastWriteTimeUtc >= TimeSpan.FromDays(daysAgo)
                        && file.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                    {
                        listOfFilesTobeZipped.Add(str);
                    }
                }
                files_toBeZipped = listOfFilesTobeZipped.ToArray();


            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message);
            }
            return listOfFilesTobeZipped.Count;
        }

        /// <summary>
        // this method gets all the qualified files from the previous method and zips them or adds them into the zip folder where
        // the files would be zipped. It deletes the original files as well.
        // </summary>
        public static void zipping_qualifiedFiles()
        {
            try
            {
                //DateTime Date_from_daysAgo = DateTime.Today.AddDays(-);


                DateTime Date_from_daysAgo = DateTime.Today.AddDays(-daysAgo);
                String zipName = Date_from_daysAgo.ToShortDateString();
                string[] conversion = zipName.Split('/');
                zipName = conversion[2] + "_" + conversion[0] + "_" + conversion[1];
                zipPath = zipPath + zipName + ".zip";

                ///</Summary>
                ///First we have to check if the zipfolder alread exists, if it doesn't we skip to the else statement and
                ///we create a zipfolder with the specified 'zipPath' using FileStream.
                if (File.Exists(zipPath))
                {
                    if (!(files_toBeZipped.Length == 0))
                    {
                        for (int i = 0; i < files_toBeZipped.Length; i++)
                        {
                            /*FileInfo fileInfo = new FileInfo(files_toBeZipped[i]);
                            ZipArchive zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Update);
                            zipArchive.CreateEntryFromFile(files_toBeZipped[i], fileInfo.Name);
                            zipArchive.Dispose();*/

                            try
                            {
                                File.Delete(files_toBeZipped[i]);
                            }
                            catch (UnauthorizedAccessException e)
                            {
                                Console.WriteLine(e.Message);
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    FileStream zipFolder = File.Create(zipPath);
                    zipFolder.Close();

                    if (!(files_toBeZipped.Length == 0))
                    {
                        for (int i = 0; i < files_toBeZipped.Length; i++)
                        {
                            /*FileInfo fileInfo = new FileInfo(files_toBeZipped[i]);
                            ZipArchive zipArchive = ZipFile.Open(zipFolder.Name, ZipArchiveMode.Update);
                            zipArchive.CreateEntryFromFile(files_toBeZipped[i], fileInfo.Name);
                            zipArchive.Dispose();*/

                            try
                            {
                                File.Delete(files_toBeZipped[i]);
                            }
                            catch (UnauthorizedAccessException e)
                            {
                                Console.WriteLine(e.Message);
                                continue;
                            }
                        }
                    }
                }
                ///</Summary>
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (InvalidDataException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}

