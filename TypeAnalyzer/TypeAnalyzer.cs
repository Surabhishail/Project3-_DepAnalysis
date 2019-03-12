///////////////////////////////////////////////////////////////////////
// TypeAnalyzer.cs - After all the types defined in each of a        //
//                  collectionof C# source files is detected.It       //
//                  displays all typetable data.                     //
//                                                                   //
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
 * This package is mainly used to display all typetable data after detecting.
 * 
 * Public Interface
 * ================
 * void showTypeTable()               // Display the value of TypeTable
 * void showAliasTable()              // Display the value of AliasTable
 * 
 * Required Files:
 *   TypeTable.cs,TypeAnalyzer.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 1.0 : 06 Nov 2018
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeTable;

namespace TypeAnalyzer
{
    using Type = String;

    public class TypeAnalyzer
    {
        public static Dictionary<Type, List<TypeItem>> typetable = TypeTable.TypeTable.getTypeTable();
        public static Dictionary<Type, List<TypeItem>> aliastable = TypeTable.TypeTable.getAliasTable();

        //This method is used to Display the Typetable formed after applying Rules
        public void showTypeTable()
        {

            foreach (var elem in typetable)
            {
                Console.Write("\n  {0}", elem.Key);
                foreach (var item in elem.Value)
                {
                    Console.Write("\n    [{0}, {1}]", item.file, item.namesp);
                }
            }
            Console.Write("\n");
        }

        //This method is used to Display the Aliastable formed after applying using Rule

        public void showAlias()
        {    if (aliastable.Count() != 0)
             {
                foreach (var elem in aliastable)
                {
                    Console.Write("\n  {0}", elem.Key);
                    foreach (var item in elem.Value)
                    {
                        Console.Write("\n    [{0}, {1}]", item.file, item.namesp);
                    }
                }
             }
            else
            {
                Console.Write("\n  Alias Table is empty");

            }
            Console.Write("\n");
        }
    }
}

#if (TEST_TYPEANALYSIS)

class TestTypeAnalyzerDemo
{
//----< Test Stub >--------------------------------------------------
        static void Main(string[] args)
        {
            Console.Write("\n  Demonstrating TestTypeAnalyzerDemo");
            TypeAnalyzer ta = new TypeAnalyzer();
            Console.Write("   Demonstrating Value in Type Table ");
            Console.Write("\n ====================================");
            ta.showTypeTable();

            Console.Write("\n   Demonstrating Value in Alias Table ");
            Console.Write("\n ====================================");
            ta.showAlias();

    }
   }
#endif


