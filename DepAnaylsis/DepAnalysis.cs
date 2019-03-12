///////////////////////////////////////////////////////////////////////
// DepAnalysis.cs - Determines the dependency between files          //
// ver 1.0                                                           //
// Language : C#,Visual Studio 2017                                  //
// Platform : HP Envy 360 ,Windows 10                                //
// Application: Type-Based Package Dependency Analysis               //
//                                                                   //
//Source: Dr. Jim Fawcett                                            //
//Author Name: Surabhi Shail                                         //
//SUID : 267102671                                                   //
//Email_Id : sshail@syr.edu                                          //
//CSE681 - Software Modeling and Analysis, Fall 2018                 //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package is mainly used to find the dependency between files.
 * Here we are checking dependencies between files so we need to get
 * the file names from the typetable.
 * This package cannot determine the dependencies between files if 
 * we are using fully qualified names, which is not a part of 
 * project requriement.
 * 
 * Public Interface
 * ================
 * void HoldUsingValue(Lexer.ITokenCollection semi)              //To create a Using rule list
 * void analyze(Lexer.ITokenCollection semi, String filename)    //To perform DepAnalysis
 * string dependend(Type value)                                  //To check if Dependency exists according to TypeTable
 * string Aliasdepend(Type value)                                //To check if Dependency exists according to AliasTable
 * void DisplayDep(String DepFile, String ProcessFile)           // Displays the Depdendend files
 * void setDictionary()                                          // Sets the Dictionary for Graph 
 * 
 * Required Files:
 *   Parser.cs
 *   IRulesAndActions.cs, RulesAndActions.cs,
 *   ITokenCollection.cs, Semi.cs, Toker.cs
 *   TypeTable.cs, StrongComp.cs,DepAnalysis.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 1.0 : 06 Nov 2018
 * - first release
 */

using System;
using System.Collections.Generic;
using CodeAnalysis;
using TypeTable;
using StrongComponent;
using System.IO;

namespace DepAnalysis
{
    using Type = String;
    public class DepAnalysis
    {
        public static Dictionary<Type, List<TypeItem>> typetable = TypeTable.TypeTable.getTypeTable();
        public static Dictionary<Type, List<TypeItem>> aliastable = TypeTable.TypeTable.getAliasTable();
        public static Dictionary<string, int> FileNames = new Dictionary<string, int>();
        public static Dictionary<string, List<string>> DepResult { get; set; } =
          new Dictionary<string, List<string>>();


        public static List<string> filesDepAnalysis { get; set; }
        public static List<string> UsingList = new List<string>();
        static string DepFile;
        int i = 0;
        String ReceivedFile = null;
        String ProFile = null;

        //To create a Using Type list
        public void HoldUsingValue(Lexer.ITokenCollection semi)
        {
            Type retnvalue;

            DetectUsing du = new DetectUsing();
            retnvalue = du.testusing(semi);
            if (retnvalue != null)
            {
                UsingList.Add(retnvalue);
            }
        }
        //To perform Dependency analysis if the input type is present in TypeTable
        public void analyze(Lexer.ITokenCollection semi, String filename)
        {
            String DependendFile = null;
            String AliasRFile = null;

            while (semi.get().Count > 0)
            {
                foreach (string objvalue in semi)
                {
                    if (typetable.ContainsKey(objvalue))// If type present in TypeTable
                    {
                        DependendFile = DepAnalysis.dependend(objvalue);

                    }
                    else if (aliastable.ContainsKey(objvalue))//If type present in AliasTable
                    {
                        AliasRFile = DepAnalysis.Aliasdepend(objvalue);

                    }
                }
                if (DependendFile != null)
                {

                    i = i + 1;
                    DisplayDep(DependendFile, filename);
                    if (AliasRFile != null & AliasRFile != DependendFile)
                    {
                        i = i + 1;
                        DisplayDep(AliasRFile, filename);

                    }

                    ReceivedFile = null;
                    ProFile = null;

                }
            }
        }
        //Analysis performed on the values present in Type Table
        public static string dependend(Type value)
        {
            foreach (var elem in typetable)
            {
                foreach (var item in elem.Value)
                {
                    String ns1 = item.namesp;
                    if (elem.Key.Equals(value) && UsingList.Contains(ns1))
                    {
                        DepAnalysis.DepFile = item.file;
                        return DepFile;

                    }

                }
            }
            return null;

        }

        //Analysis performed on the values present in Alias Table
        public static string Aliasdepend(Type value)
        {
            String AliasFile = null;

            foreach (var elem in aliastable)
            {
                foreach (var item in elem.Value)
                {
                    String Aliasname = item.namesp;
                    if (typetable.ContainsKey(Aliasname))
                    {
                        AliasFile = DepAnalysis.dependend(Aliasname);
                        return AliasFile;
                    }
                }
            }
            return null;
        }
        //To Display DepAnalysis between files
        public void DisplayDep(String DepFile, String ProcessFile)
        {

          // String ReceivedFile = null;
           // String ProFile = null;
            if (ReceivedFile != DepFile && ProFile != ProcessFile)
            {
                
                ReceivedFile = DepFile;
                ProFile = ProcessFile;
                Console.WriteLine("\nIndex " + i + ":");
                Console.Write("\n Filename :" + System.IO.Path.GetFileName(ProcessFile));
                
                string parent_file = System.IO.Path.GetFileName(ProcessFile);
                if (DepResult.ContainsKey(parent_file))
                    DepResult[parent_file].Add(ReceivedFile);
                else
                {
                    List<string> temp = new List<string>();
                    temp.Add(DepFile);
                    DepResult.Add(parent_file, temp);

                }

                Console.Write("\n\t Dependency:" + DepFile);

                //Passing Value to find strong Component
                int parent, child;
                FileNames.TryGetValue(DepFile, out child);
                FileNames.TryGetValue(ProcessFile, out parent);
                StrongComp.setDependency(parent, child);

            }
        }
        //Implemented this method to make my DepAnalysis compactible with StrongComponent graph input
        public static void setDictionary()
        {
            int fileCount = 0;
            foreach (string file in filesDepAnalysis)
            {
                FileNames.Add(System.IO.Path.GetFileName(file), fileCount);
                fileCount++;
            }
        }
        public static Dictionary<string, List<string>> getDepResult()
        {
            return DepResult;
        }



    }
}
    #if (TEST_DEPANALYSIS)

    class TestDepAnalysisDemo
    {
        //----< process commandline to get file references >-----------------
        static List<string> ProcessCommandline(string[] args)
        {
            List<string> files = new List<string>();
            if (args.Length < 1)
            {
                Console.Write("\n  Please enter path and file(s) to analyze\n\n");
                return files;
            }
            string path = args[0];
            if (!Directory.Exists(path))
            {
                Console.Write("\n  invalid path \"{0}\"", System.IO.Path.GetFullPath(path));
                return files;
            }
            path = Path.GetFullPath(path);
            files.AddRange(Directory.GetFiles(path));
            return files;
        }

        static void ShowCommandLine(string[] args)
        {
            Console.Write("\n  Commandline args are:\n  ");

            Console.Write("  {0}", args[0]);
            Console.Write("\n  current directory: {0}", Path.GetFullPath(args[0]));
            Console.Write("\n");
        }
    }
}


//----< Test Stub >--------------------------------------------------


        static void Main(string[] args)
        {
            Console.Write("\n  Demonstrating DepAnalysis");
            DepAnalysis dep = new DepAnalysis();

            DepAnalysis.setDictionary();
            ShowCommandLine(args);

            List<string> files = TestParser.ProcessCommandline(args);
            foreach (string file in files)
            {
                Console.Write("\n  Processing file {0}\n", System.IO.Path.GetFileName(file));
                ITokenCollection semi = Factory.create();
                if (!semi.open(file as string))
                {
                    Console.Write("\n  Can't open {0}\n\n", args[0]);
                    return;
                }
                try
                {
                    while (semi.get().Count > 0)
                        dep.HoldUsingValue(semi);
                }
                catch (Exception ex)
                {
                    Console.Write("\n\n  {0}\n", ex.Message);
                }
                semi.close();
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (!semi.open(file as string))
                {
                    Console.Write("\n  Can't open {0}\n\n", args[0]);
                    return;
                }
                try
                {
                    while (semi.get().Count > 0)
                        dep.analyze(semi, file);
                }
                catch (Exception ex)
                {
                    Console.Write("\n\n  {0}\n", ex.Message);
                }semi.close(); 
            }
            Console.Write("\n\n");
        }
    }
   }
#endif



