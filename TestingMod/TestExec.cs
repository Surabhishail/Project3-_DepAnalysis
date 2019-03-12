using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DepAnalysis;
using StrongComponent;


namespace CodeAnalysis
{
    using Lexer;

    class TestExec
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
            Console.Write("This Project3 implements packages that evaluate all the dependencies between files in a specified file set  \n");
            ShowCommandLine(args);

            List<string> files = ProcessCommandline(args);


            foreach (string file in files)
            {
                Console.Write("\n  Processing file {0}\n", System.IO.Path.GetFileName(file));

                ITokenCollection semi = Factory.create();
                if (!semi.open(file as string))
                {
                    Console.Write("\n  Can't open {0}\n\n", args[0]);
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


            DepAnalysis.DepAnalysis dep = new DepAnalysis.DepAnalysis();
            StrongComp.filesGraph = files;
            StrongComp.filesGraphs = files;
            StrongComp.filenumber = files.Count();
            StrongComp.setGraph();
            StrongComp.filesGraph = files;
            StrongComp.setGraphDictionary();
            DepAnalysis.DepAnalysis.filesDepAnalysis = files;
            DepAnalysis.DepAnalysis.setDictionary();


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
                }

                semi.close();
                StrongComp.addGraph();


            }
            Console.Write("\n\n");
        }
        public void Requirement6()
        {
            Console.Write("\n\t \tOutput of Requirement6\n");
            Console.Write("-----------------------------------------------------------------------------\n");
            Console.Write(" This Project3 finds all strong components, if any, in the file collection, based on the DependendAnalysis\n");
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
            if (args.Length < 2)
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
            for (int i = 1; i < args.Length; ++i)
            {
                string filename = Path.GetFileName(args[i]);
                files.AddRange(Directory.GetFiles(path, filename));
            }
            return files;
        }

        static void ShowCommandLine(string[] args)
        {
            Console.Write("\n  Commandline args are:\n  ");
            foreach (string arg in args)
            {
                Console.Write("  {0}", arg);
            }
            Console.Write("\n  current directory: {0}", System.IO.Directory.GetCurrentDirectory());
            Console.Write("\n");
        }

        static void Main(string[] args)
        {
            Console.Write("\n  Demonstrating Output For Project3#");
            Console.Write("\n ======================\n");

            Executive Atu = new Executive();

            Atu.Requirement1();
            Atu.Requirement2();
            Atu.Requirement3();
            Atu.Requirement4_5(args);
            Atu.Requirement6();
            Atu.Requirement7();
            Atu.Requirement8();
            Console.ReadLine();



        }
    }

}
}
