/////////////////////////////////////////////////////////////////////
// Executive.cs - This package is the Automated Test Suite         //
//                It Provides code to demonstrate you meet all     // 
//                requirements.                                    //
// ver 1.0                                                         //
// Language : C#,Visual Studio 2017                                //
// Platform : HP Envy 360 ,Windows 10                              //
// Application: Type-Based Package Dependency Analysis             //
//                                                                 //
//Source: Dr. Jim Fawcett                                          //
//Author Name: Surabhi Shail                                       //
//SUID : 267102671                                                 //
//Email_Id : sshail@syr.edu                                        //
//CSE681 - Software Modeling and Analysis, Fall 2018               //
/////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package defines the following class:
 *   Executive:
 *   - uses Parser, RulesAndActions, Semi, and Toker,DepAnalysis,TypeAnalysis to perform basic
 *     code metric analyzes
 */
/* Required Files:
 *   Executive.cs
 *   Parser.cs
 *   IRulesAndActions.cs, RulesAndActions.cs, ScopeStack.cs, Elements.cs
 *   ITokenCollection.cs, Semi.cs, Toker.cs
 *   Display.cs
 *   TypeAnalysis.cs
 *   DepAnalysis.cs
 *   TypeAnalyzer.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 1.1 : 05 Nov 2018
 * - Changed to get output of more functionality
 * ver 1.0 : 09 Oct 2018
 * - first release
 * 
 * 
 *  Public Interface
 * ================
 * ExecTest Atu = new ExecTest();
 * Atu.Requirement1();
 * Atu.Requirement2();
 * Atu.Requirement3();
 * Atu.Requirement4();
 * Atu.Requirement5();
 * Atu.Requirement6();
 * Atu.Requirement7_8();
 * Atu.Requirement9();
 * Atu.Requirement10();  
 * ProcessCommandline(string[] args)
 * void ShowCommandLine(string[] args)
 * Executive test = new Executive();
 * Execute_Parser(files);
 * Execute_TypeAnalyzer();
            
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DepAnalysis;
using StrongComponent;
using TypeAnalyzer;

namespace CodeAnalysis
{
  using Lexer;

  class Executive
  {
        public void Requirement1()
        {
            Console.Write("\n\t \tOutput of Requirement1\n");
            Console.Write("---------------------------------------------------------------------------\n");
            Console.Write("For the implementation of Project 3 we have used\n");
            Console.Write("Visual Studio 2017 and its C# Windows Console Projects\n");

        }
        public void Requirement2()
        {
            Console.Write("\n\t \tOutput of Requirement2\n");
            Console.Write("--------------------------------------------------------------------------\n");
            Console.Write("Project 3 imports .Net System.IO and System.Text for all I/O\n");
        }
        public void Requirement3()
        {
            Console.Write("\n\t \tOutput of Requirement3\n");
            Console.Write("---------------------------------------------------------------------------\n");
            Console.Write("This Project 3 provides with all C# packages as per the requirement \n");
            Console.Write("Implemented Packages are as follows: \n");
            Console.Write("1.Tokenizer \n2.SemiExpression \n3.TypeTable \n4.TypeAnalysis \n");
            Console.Write("5.DepAnalysis \n6.StrongComponent \n7. Display \n8.Tester");
        }
        public void Requirement4_5(string[] args)
        {
            Console.Write("\n\t \tOutput of Requirement4\n");
            Console.Write("-----------------------------------------------------------------------------\n");
            Console.Write("This Project3 implements packages that evaluate all the dependencies between files in a specified file set");
            ShowCommandLine(args);
            List<string> files = ProcessCommandline(args);
            Executive test = new Executive();
            test.Execute_Parser(files);
            test.Execute_TypeAnalyzer();
            DepAnalysis.DepAnalysis dep = new DepAnalysis.DepAnalysis();
            StrongComp.filesGraphs = files;
            StrongComp.filenumber = files.Count();
            StrongComp.setGraph();
            DepAnalysis.DepAnalysis.filesDepAnalysis = files;
            StrongComp.filesGraph = files;
            DepAnalysis.DepAnalysis.setDictionary();
            StrongComp.setGraphDictionary();
            Console.Write("\nDemonstrating Output Of Dependency Analysis");
            Console.Write("\n ==========================================");

            foreach (string file in files)
            {
                ITokenCollection semi = Factory.create();
                if (!semi.open(file as string))
                {Console.Write("\n  Can't open {0}\n\n", files[0]);
                    return;
                }
                try { 
                    while (semi.get().Count > 0)
                        dep.HoldUsingValue(semi);
                }
                catch (Exception ex) { Console.Write("\n\n  {0}\n", ex.Message); }
                semi.close();
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (!semi.open(file as string))
                {Console.Write("\n  Can't open {0}\n\n", files[0]);
                    return;
                }
                try { 
                while (semi.get().Count > 0)
                       dep.analyze(semi, file);
                }
                catch (Exception ex){Console.Write("\n\n  {0}\n", ex.Message);}
                StrongComp.addGraph();
                semi.close();
            }
            Console.Write("\n\n");
        }
        public void Requirement6()
        {
            Console.Write("\n\t \tOutput of Requirement6\n");
            Console.Write("-----------------------------------------------------------------------------\n");
            Console.Write(" This Project3 finds all strong components, if any, in the file collection, based on the DependendAnalysis\n");
            Console.Write(" and according to Tarjan Algorithm any node having dependency on itself is a strong component\n");
            StrongComp.displayStrongComponent();
        }
        public void Requirement7()
        {
            Console.Write("\n\t \tOutput of Requirement 7 and 8\n");
            Console.Write("-----------------------------------------------------------------------------\n");
            Console.Write("This project3# is displaying the results in a well formated area of the output.\n");
        }
        public void Requirement8()
        {
            Console.Write("\n\t \tOutput of Requirement8\n");
            Console.Write("---------------------------------------------------------------------------------\n");
            Console.Write("This project includes an automated unit test suite that exercises all of the \n");
            Console.Write("special cases that required for all the implemented package \n");
        }

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
         if(!Directory.Exists(path))
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
       public void Execute_Parser(List<string> files)
        {
            foreach (string file in files)
            {
                ITokenCollection semi = Factory.create();
                if (!semi.open(file as string))
                {
                    Console.Write("\n  Can't open {0}\n\n", files[0]);
                    return;
                }
                BuildCodeAnalyzer builder = new BuildCodeAnalyzer(semi);
                Parser parser = builder.build();

                try
                {
                    while (semi.get().Count > 0)
                        parser.parse(semi);
                }
                catch (Exception ex)
                {
                    Console.Write("\n\n  {0}\n", ex.Message);
                }
                Console.Write("\n");

                semi.close();
            }
        }
        public void Execute_TypeAnalyzer()
        {
            TypeAnalyzer.TypeAnalyzer ta = new TypeAnalyzer.TypeAnalyzer();
            Console.Write("   Demonstrating Value in Type Table ");
            Console.Write("\n ====================================");
            ta.showTypeTable();

            Console.Write("\n   Demonstrating Value in Alias Table ");
            Console.Write("\n ====================================");
            ta.showAlias();

        }
     
        static void Main(string[] args)
       {
        Console.Write("\n  Demonstrating Output For Project3# : Type-Based Package Dependency Analysis ");
        Console.Write("\n ===============================================================================\n");

        Executive Atu = new Executive();
            try
            {
                Atu.Requirement1();
                Atu.Requirement2();
                Atu.Requirement3();
                Atu.Requirement4_5(args);
                Atu.Requirement6();
                Atu.Requirement7();
                Atu.Requirement8();
            }
            catch(Exception e)
            {
                Console.Write("\n\n  {0}\n", e.Message);

            }
            Console.ReadLine();
    }
  }
}
