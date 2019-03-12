///////////////////////////////////////////////////////////////////////
// TypeTable.cs - Provides a container that stores type information  //
//               and alias information needed for dependency analysis//               
// ver 1.1                                                           //
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
 * This package is mainly used to provide a container that stores
 * type information and alias information needed for dependency analysis.
 * 
 * Public Interface
 * ================
 * void add(Type type, TypeItem ti)                      //Constructs Dictionary of Type Table
 * void add(Type type, File file, Namespace ns)          //Accepts Type values to construct TypeTable
 * Dictionary<Type, List<TypeItem>> getTypeTable()       //Returns the value of TypeTable
 * void addalias(Type type, TypeItem ti)                 //Constructs Dictionary of Alias Table
 * void addalias(Type type, File file, Namespace ns)     //Accepts Type values to construct AliasTable
 * Dictionary<Type, List<TypeItem>> getAliasTable()      //Returns the value of AliasTable
 * 
 * Required Files:
 *  TypeTable.cs
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

namespace TypeTable
{
    using File = String;
    using Type = String;
    using Namespace = String;

    public struct TypeItem
    {
        public File file;
        public Namespace namesp;
    }

    public class TypeTable
    {

        //Type Table
        public static Dictionary<Type, List<TypeItem>> table { get; set; } =
          new Dictionary<Type, List<TypeItem>>();
        //AliasTable
        public static Dictionary<Type, List<TypeItem>> aliastable { get; set; } =
           new Dictionary<Type, List<TypeItem>>();

        public void add(Type type, TypeItem ti)
        {
            if (table.ContainsKey(type))
                table[type].Add(ti);
            else
            {
                List<TypeItem> temp = new List<TypeItem>();
                temp.Add(ti);
                table.Add(type, temp);

            }

        }
        public void add(Type type, File file, Namespace ns)
        {
            TypeItem temp;
            temp.file = file;
            temp.namesp = ns;
            add(type, temp);
        }
        
        //Method to return type table
        public static Dictionary<Type, List<TypeItem>> getTypeTable()
        {
            return table;
        }

        //This method is used to add value to the Alias Table in Dictionary form
        public void addalias(Type type, TypeItem ti)
        {
            if (aliastable.ContainsKey(type))
                aliastable[type].Add(ti);
            else
            {
                List<TypeItem> temp = new List<TypeItem>();
                temp.Add(ti);
                aliastable.Add(type, temp);

            }

        }
        //This method is used to add value to the Alias Table 

        public void addalias(Type type, File file, Namespace ns)
        {
            TypeItem temp;
            temp.file = file;
            temp.namesp = ns;
            addalias(type, temp);
        }
        

        // Method to return Alias Table
        public static Dictionary<Type, List<TypeItem>> getAliasTable()
        {
            return aliastable;
        }


    }
}

#if (TEST_TYPETABLE)

    class TestTypeTableDemo
    {
        static void Main(string[] args)
        {
            Console.Write("\n  Demonstrate how to build typetable");
            Console.Write("\n ====================================");
            // build demo table

           TypeTable tt = new TypeTable();
            tt.add("Type_X", "File_A", "Namespace_Test1");
            tt.add("Type_X", "File_B", "Namespace_Test2");
            tt.add("Type_Y", "File_A", "Namespace_Test1");
            tt.add("Type_Z", "File_C", "Namespace_Test3");

            tt.show();

            // access elements in table

            Console.Write("\n  TypeTable contains types: ");
            foreach (var elem in TypeTable.table)
            {
                Console.Write("{0} ", elem.Key);
            }
            Console.Write("\n\n");
        }
    }
}
#endif

