/////////////////////////////////////////////////////////////////////////////////////////////
// StrongComp.cs - Used to find the strongComponents if exist                              //
// ver 1.0                                                                                 //
// Language : C#,Visual Studio 2017                                                        //
// Platform : HP Envy 360 ,Windows 10                                                      //
// Application: Type-Based Package Dependency Analysis                                     //
//                                                                                         //
//Source: Wikipedia and StackOverflow                                                      //
//Links: https://en.wikipedia.org/wiki/Tarjan%27s_strongly_connected_components_algorithm  //
//       https://stackoverflow.com/questions/6643076/tarjan-cycle-detection-help-c-sharp   //
//Author Name: Surabhi Shail                                                               //
//SUID : 267102671                                                                         //
//Email_Id : sshail@syr.edu                                                                //
//CSE681 - Software Modeling and Analysis, Fall 2018                                       //
/////////////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * The main purpose of this package is to print the strongly connected
 * components among a list of files.
 * Strongly connected means we need to return to the current file after
 * traversing through a set of files.
 * This package uses the output of dependency analysis so that we can
 * form the strong component.
 * 
 * Public Interface
 * ================
 * void setGraphDictionary()                          // constructs the graph dictionary
 * void setGraphNodes()                               // constructs the nodes of graph
 * void setVertex()                                   // Set vertex
 * void setDependency(int parent, int child)          // Sets dependency between files
 * void addGraph()                                    // Constructs the graph
 * void displayStrongComponent()                      // Display all strong compnents  
 * void push(Vertex v)                                // Pushes Vertex to be processed in stack
 * Vertex pop()                                       // Pop the vertex after processing
 * bool Contains(Vertex v)                            // Checks if Vertex exists
 * string ToString()                                  // Converts into string
 * List<List<Vertex>> DetectCycle(List<Vertex> nodes) // Returns list of vertex with cyclic dependency
 * void StronglyConnect(Vertex v)                     // Process each vertex to find cyclic dependency
 * 
 * Required Files:
 * StrongComp.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 1.0 : 06 Nov 2018
 * - first release
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace StrongComponent
{
    public class StrongComp
    {
        public static Dictionary<int, string> FileNamesGraph = new Dictionary<int, string>();
        public static List<string> filesGraph { get; set; }

        //This method is used populate the Graph of Dictionary type
        public static void setGraphDictionary()
        {
            int fileCount = 0;
            foreach (string file in filesGraph)
            {
                FileNamesGraph.Add(fileCount, System.IO.Path.GetFileName(file));
                fileCount++;
            }
        }

        public static List<string> filesGraphs = new List<string>();
        public static int filenumber = new int();
        public static List<Vertex> VertexList = new List<Vertex>();
        public static List<Vertex> graph_nodes = new List<Vertex>();

        //This method is used to create new nodes of a graph
        public static void setGraphNodes()
        {
            graph_nodes = new List<Vertex>();
        }
        //This method is used to create list of Vertex for each node graph

        public static void setVertex()
        {
            VertexList = new List<Vertex>(filenumber);
        }
        //This method is used to add nodes of to graph
        public static void setGraph()
        {
            for (int i = 0; i < filesGraphs.Count(); i++)
            {
                var v = new Vertex() { Id = i };
                VertexList.Add(v);
            }
        }
        //This method is used to populate the dependency between files
        public static void setDependency(int parent, int child)
        {
            VertexList[parent].Dependencies.Add(VertexList[child]);
        }
        //This method is used to create the complete graph
        public static void addGraph()
        {
            for (int i = 0; i < filesGraphs.Count(); i++)
            {
                graph_nodes.Add(VertexList[i]);
            }
        }
        //This method is used to Display the strong Components
        public static void displayStrongComponent()
        {
            var tcd = new TarjanCycleDetectStack();
            var cycle_list = tcd.DetectCycle(graph_nodes);
            Console.WriteLine("\nThere are {0} strong component / components in the graph", cycle_list.Count);
            for (int i = 0; i < cycle_list.Count; i++)
            {
                Console.WriteLine("\nThe Strong Component number {0} is \n", i);
                for (int j = 0; j < cycle_list[i].Count; j++)
                {
                    var myKey = FileNamesGraph[cycle_list[i][j].Id];
                    Console.WriteLine(myKey);
                }
            }
        }
        public List<Vertex> vertexList { get; set; }
        //Get and Set methods to obtain Vertex value
        public List<Vertex> inputFiles
        {
            get
            {
                return this.vertexList;
            }
            set
            {
                vertexList = new List<Vertex>(inputFiles.Count());
                for (int i = 0; i < inputFiles.Count(); i++)
                {
                    this.vertexList[i].Id = i;
                }
            }

        }
        

        //Implemented a Stack to store the node if not visited and pop it once processed
        public class MyStack : Vertex
        {
            List<Vertex> VertexStack = new List<Vertex>();
            public void push(Vertex v)
            {
                VertexStack.Add(v);
            }
            public Vertex pop()
            {
                Vertex PopingVertex;
                int listSize = VertexStack.Count();
                PopingVertex = VertexStack[listSize - 1];
                VertexStack.RemoveAt(listSize - 1);
                return PopingVertex;
            }
            public bool Contains(Vertex v)
            {
                return (VertexStack.Contains(v)) ? true : false;
            }
        }

        //This class defintion of Generic type Vertex
        public class Vertex
        {
            public int Id { get; set; }
            public int Index { get; set; }
            public int Lowlink { get; set; }

            public HashSet<Vertex> Dependencies { get; set; }

            public Vertex()
            {
                Id = -1;
                Index = -1;
                Lowlink = -1;
                Dependencies = new HashSet<Vertex>();
            }

            public override string ToString()
            {
                return string.Format("Vertex Id {0}", Id);
            }

           
        }
        public class TarjanCycleDetectStack
        {
            protected List<List<Vertex>> _StronglyConnectedComponents;
            MyStack _Stack = new MyStack();
            protected int _Index;

            //This method is used to find the strong components
            public List<List<Vertex>> DetectCycle(List<Vertex> graph_nodes)
            {
                _StronglyConnectedComponents = new List<List<Vertex>>();

                _Index = 0;
                _Stack = new MyStack();

                foreach (Vertex v in graph_nodes)
                {
                    if (v.Index < 0)
                    {
                        StronglyConnect(v);
                    }
                }

                return _StronglyConnectedComponents;
            }
            //This method applies DFS algorithm to check if adjacent nodes are depended or not
            private void StronglyConnect(Vertex v)
            {
                v.Index = _Index;
                v.Lowlink = _Index;

                _Index++;
                _Stack.push(v);

                foreach (Vertex w in v.Dependencies)
                {
                    if (w.Index < 0)
                    {
                        StronglyConnect(w);
                        v.Lowlink = Math.Min(v.Lowlink, w.Lowlink);
                    }
                    else if (_Stack.Contains(w))
                    {
                        v.Lowlink = Math.Min(v.Lowlink, w.Index);
                    }
                }

                if (v.Lowlink == v.Index)
                {
                    List<Vertex> cycle = new List<Vertex>();
                    Vertex w;

                    do
                    {
                        w = _Stack.pop();
                        cycle.Add(w);
                    } while (v != w);

                    _StronglyConnectedComponents.Add(cycle);
                }
            }
        }
       
    }
}
    #if (TEST_StrongComp)

    class TestStrongComponentDemo
    {
        //----< process commandline to get file references >-----------------

        static List<string> ProcessCommandline(string[] args)
        {
            List<string> files = new List<string>();
            if (args.Length == 0)
            {
                Console.Write("\n  Please enter file(s) to analyze\n\n");
                return files;
            }
            string path = args[0];
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
    
//----< Test Stub >--------------------------------------------------
        static void Main(string[] args)
        {
            Console.Write("\n  Demonstrating StrongComponent");
            Console.Write("\n ====================================");
            ShowCommandLine(args);

            List<string> files = TestParser.ProcessCommandline(args);
            StrongComp.filesGraph = files;
            StrongComp.filesGraphs = files;
            StrongComp.filenumber = files.Count();
            StrongComp.setGraph();
            StrongComp.filesGraph = files;
            StrongComp.setGraphDictionary();
            
            foreach (string file in files)
            {
               StrongComp.addGraph();
            }
            Console.Write("\n\n");
        }
    }
   }
#endif

