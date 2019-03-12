///////////////////////////////////////////////////////////////////////
// RulesAndActions.cs - Parser rules specific to an application      //
// ver 2.4                                                           //
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
 * RulesAndActions package contains all of the Application specific
 * code required for most analysis tools.
 *
 * It defines the following Four rules which each have a
 * grammar construct detector and also a collection of IActions:
 *   - DetectNameSpace rule
 *   - DetectClass rule
 *   - DetectFunction rule
 *   - DetectScopeChange
 *   - DetectUsing rule
 *   - DetectDelegate rule 
 *   
 *   Three actions - some are specific to a parent rule:
 *   - Print
 *   - PrintFunction
 *   - PrintScope
 * 
 * The package also defines a Repository class for passing data between
 * actions and uses the services of a ScopeStack, defined in a package
 * of that name.
 *
 * Note:
 * This package does not have a test stub since it cannot execute
 * without requests from Parser.
 *  
 */
/* Required Files:
 *   IRuleAndAction.cs, RulesAndActions.cs, Parser.cs, ScopeStack.cs,
 *   Semi.cs, Toker.cs,TypeTable.cs
 *   
 * Build command:
 *   csc /D:TEST_PARSER Parser.cs IRuleAndAction.cs RulesAndActions.cs \
 *                      ScopeStack.cs Semi.cs Toker.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 2.5 : 06 Nov 2018
 * -Added rules to Detect delegate and using types
 * -Added in Detect class to identify struct and enum type
 * ver 2.4 : 09 Oct 2018
 * - modified comments
 * - removed unnecessary definition from repository class
 * - moved local semi definition inside display test in PopStack action
 * ver 2.3 : 30 Sep 2014
 * - added scope-based complexity analysis
 *   Note: doesn't detect braceless scopes yet
 * ver 2.2 : 24 Sep 2011
 * - modified Semi package to extract compile directives (statements with #)
 *   as semiExpressions
 * - strengthened and simplified DetectFunction
 * - the previous changes fixed a bug, reported by Yu-Chi Jen, resulting in
 * - failure to properly handle a couple of special cases in DetectFunction
 * - fixed bug in PopStack, reported by Weimin Huang, that resulted in
 *   overloaded functions all being reported as ending on the same line
 * - fixed bug in isSpecialToken, in the DetectFunction class, found and
 *   solved by Zuowei Yuan, by adding "using" to the special tokens list.
 * - There is a remaining bug in Toker caused by using the @ just before
 *   quotes to allow using \ as characters so they are not interpreted as
 *   escape sequences.  You will have to avoid using this construct, e.g.,
 *   use "\\xyz" instead of @"\xyz".  Too many changes and subsequent testing
 *   are required to fix this immediately.
 * ver 2.1 : 13 Sep 2011
 * - made BuildCodeAnalyzer a public class
 * ver 2.0 : 05 Sep 2011
 * - removed old stack and added scope stack
 * - added Repository class that allows actions to save and 
 *   retrieve application specific data
 * - added rules and actions specific to Project #2, Fall 2010
 * ver 1.1 : 05 Sep 11
 * - added Repository and references to ScopeStack
 * - revised actions
 * - thought about added folding rules
 * ver 1.0 : 28 Aug 2011
 * - first release
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Lexer;
using TypeTable;

namespace CodeAnalysis
{
    ///////////////////////////////////////////////////////////////////
    // Repository class
    // - Specific to each application
    // - holds results of processing
    // - ScopeStack holds current state of scope processing
    // - List<Elem> holds start and end line numbers for each scope
    ///////////////////////////////////////////////////////////////////
    using Type = String;
    public class Repository
    {
        ScopeStack<Elem> stack_ = new ScopeStack<Elem>();
        List<Elem> locations_ = new List<Elem>();

        static Repository instance;
       
        public Repository()
        {
            instance = this;
        }

        //----< provides all code access to Repository >-------------------

        public static Repository getInstance()
        {
            return instance;
        }

        //----< provides all actions access to current semiExp >-----------

        public ITokenCollection semi
        {
            get;
            set;
        }
       
        // semi gets line count from toker who counts lines
        // while reading from its source

        public int lineCount  // saved by newline rule's action
        {
            get { return semi.lineCount(); }
        }
        public int prevLineCount  // not used in this demo
        {
            get;
            set;
        }

        //----< enables recursively tracking entry and exit from scopes >--

        public int scopeCount
        {
            get;
            set;
        }

        public ScopeStack<Elem> stack  // pushed and popped by scope rule's action
        {
            get { return stack_; }
        }

        // the locations table is the result returned by parser's actions
        // in this demo

        public List<Elem> locations
        {
            get { return locations_; }
            set { locations_ = value; }
        }
    }
    ///////////////////////////////////////////////////////////////////
    // Define Actions
    ///////////////////////////////////////////////////////////////////
    // - PushStack
    // - PopStack                                     
    // - PrintFunction
    // - PrintSemi
    // - SaveDeclar

    ///////////////////////////////////////////////////////////////////
    // pushes scope info on stack when entering new scope
    // - pushes element with type and name onto stack
    // - records starting line number

    public class PushStack : AAction//This is a class inherited from Abstract action
    {
        Type holdnamespace = null;
        TypeTable.TypeTable tt = new TypeTable.TypeTable();
        DetectUsing du = new DetectUsing();


        public PushStack(Repository repo)
         {
            repo_ = repo;
         }

        public override void doAction(ITokenCollection semi)//IToken need more than get ss
        {
            Display.displayActions(actionDelegate, "action PushStack");
            ++repo_.scopeCount;//manage scope stack ss
            Elem elem = new Elem();


            elem.type = semi[0];     // expects type, i.e., namespace, class, struct, ..
            elem.name = semi[1];     // expects name

            //Checked if the type name matches with our requirement and added in Type Table 
            string[] IncludeType = { "class", "enum", "struct", "delegate", "namespace","interface","using"};
            foreach (string stoken in IncludeType)
                if (stoken == semi[0])
                {
                    String filename = repo_.semi.getFileName();
                    String curr_file = System.IO.Path.GetFileName(filename);

                    if (semi[0].Equals("namespace"))//To create Type Table1q
                    {
                        holdnamespace = semi[1];
                        tt.add(semi[1], curr_file, semi[0]);
                    }
                    else if(semi[0].Equals("using"))//To create Alias Table
                    {
                        tt.addalias(semi[1], curr_file, semi[2]);
                        tt.addalias(semi[1], curr_file, semi[2]);
                    }
                    else
                    {
                        tt.add(semi[1], curr_file, holdnamespace);
                    }

                }
            


            elem.beginLine = repo_.semi.lineCount() - 1;
            elem.endLine = 0;        // will be set by PopStack action
            elem.beginScopeCount = repo_.scopeCount;
            elem.endScopeCount = 0;  // will be set by PopStack action
            repo_.stack.push(elem);

            // display processing details if requested

            if (AAction.displayStack)
                repo_.stack.display();
            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount() - 1);
                Console.Write("entering ");
                string indent = new string(' ', 2 * repo_.stack.count);
                Console.Write("{0}", indent);
                this.display(semi); // defined in abstract action
            }

            // add starting location if namespace, type, or function

            if (elem.type == "control" || elem.name == "anonymous")//Never be controlled anonymous ss ni
                return;
            repo_.locations.Add(elem);
        }
    }
    ///////////////////////////////////////////////////////////////////
    // pops scope info from stack when leaving scope
    // - records end line number and scope count

    public class PopStack : AAction
    {
        public PopStack(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(ITokenCollection semi)
        {
            Display.displayActions(actionDelegate, "action SaveDeclar");
            Elem elem;
            try
            {
                // if stack is empty (shouldn't be) pop() will throw exception

                elem = repo_.stack.pop();

                // record ending line count and scope level

                for (int i = 0; i < repo_.locations.Count; ++i)//end and start position of a scope ss
                {
                    Elem temp = repo_.locations[i];
                    if (elem.type == temp.type)
                    {
                        if (elem.name == temp.name)
                        {
                            if ((repo_.locations[i]).endLine == 0)
                            {
                                (repo_.locations[i]).endLine = repo_.semi.lineCount();
                                (repo_.locations[i]).endScopeCount = repo_.scopeCount;
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                return;
            }

            if (AAction.displaySemi)
            {
                Lexer.ITokenCollection local = Factory.create();
                local.add(elem.type).add(elem.name);
                if (local[0] == "control")
                    return;

                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount());
                Console.Write("leaving  ");
                string indent = new string(' ', 2 * (repo_.stack.count + 1));
                Console.Write("{0}", indent);
                this.display(local); // defined in abstract action
            }
        }
    }
    ///////////////////////////////////////////////////////////////////
    // action to print function signatures - not used in demo

    public class PrintFunction : AAction
    {
        public PrintFunction(Repository repo)
        {
            repo_ = repo;
        }
        public override void display(Lexer.ITokenCollection semi)
        {
            Console.Write("\n    line# {0}", repo_.semi.lineCount() - 1);
            Console.Write("\n    ");
            for (int i = 0; i < semi.size(); ++i)
            {
                if (semi[i] != "\n")
                    Console.Write("{0} ", semi[i]);
            }
        }
        public override void doAction(ITokenCollection semi)
        {
            this.display(semi);
        }
    }
    ///////////////////////////////////////////////////////////////////
    // ITokenCollection printing action, useful for debugging

    public class PrintSemi : AAction
    {
        public PrintSemi(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(ITokenCollection semi)
        {
            Console.Write("\n  line# {0}", repo_.semi.lineCount() - 1);
            this.display(semi);
        }
    }
    ///////////////////////////////////////////////////////////////////
    // display public declaration

    public class SaveDeclar : AAction
    {
        public SaveDeclar(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(ITokenCollection semi)//Simple display of public declaration ss
        {
            Display.displayActions(actionDelegate, "action SaveDeclar");
            Elem elem = new Elem();
            elem.type = semi[0];  // expects type
            elem.name = semi[1];  // expects name
            elem.beginLine = repo_.lineCount;
            elem.endLine = elem.beginLine;
            elem.beginScopeCount = repo_.scopeCount;
            elem.endScopeCount = elem.beginScopeCount;
            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.lineCount - 1);
                Console.Write("entering ");
                string indent = new string(' ', 2 * repo_.stack.count);
                Console.Write("{0}", indent);
                this.display(semi); // defined in abstract action
            }
            repo_.locations.Add(elem);
        }
    }
    ///////////////////////////////////////////////////////////////////
    // Define Rules
    ///////////////////////////////////////////////////////////////////
    // - DetectNamespace
    // - DetectClass
    // - DetectFunction
    // - DetectAnonymousScope
    // - DetectPublicDeclaration
    // - DetectLeavingScope
    // - DetectUsing
    // - DetectDelegate

    ///////////////////////////////////////////////////////////////////
    // rule to detect namespace declarations

    public class DetectNamespace : ARule
    {
        public override bool test(ITokenCollection semi)//Added semi.find function in ITok ss
        {
            Display.displayRules(actionDelegate, "rule   DetectNamespace");
            int index;
            semi.find("namespace", out index);
            if (index != -1 && semi.size() > index + 1)
            {
                ITokenCollection local = Factory.create();//need factory bcoz i dont have semi class but interface ss
                // create local semiExp with tokens for type and name
                local.add(semi[index]).add(semi[index + 1]);
                doActions(local);
                return true;//Fully satisfied no more parsing required ss
            }
            return false;
        }
    }
    ////////////////////////////////////////////////////////////////////////
    // Added rule to detect Alias declarations

    public class DetectAlias : ARule
    {
        public override bool test(ITokenCollection semi)
        {
            Display.displayRules(actionDelegate, "rule   DetectAlias");
            int index;
            semi.find("using", out index);
            if (index != -1 && semi.size() > index + 1)
            {
                ITokenCollection local = Factory.create();
                if (semi[index + 1] != "System" && semi[index + 2] == "=")
                {
                    local.add(semi[index]).add(semi[index + 1]).add(semi[index + 3]);
                    doActions(local);
                    return true;
                }
            }
            return false;
        }
    }

    ///////////////////////////////////////////////////////////////////
    // rule to detect using declarations

    public class DetectUsing
    {
        public string testusing(ITokenCollection semi)
        {
            int index;
            
            semi.find("using", out index);
            if (index != -1 && semi.size() > index + 1)
            {
                // ITokenCollection local = Factory.create();
                // create local semiExp with tokens for type and name
                //local.add(semi[index]).add(semi[index + 1]);
                if (semi[index + 1] != "System")
                {
                    return semi[index + 1];
                }
                 
            }
            return null;
        }
    }

    ///////////////////////////////////////////////////////////////////
    // rule to dectect class definitions

    public class DetectClass : ARule
    {
        public override bool test(ITokenCollection semi)
        {
            Display.displayRules(actionDelegate, "rule   DetectClass");
            int indexCL;
            semi.find("class", out indexCL);
            int indexIF;
            semi.find("interface", out indexIF);
            int indexST;
            semi.find("struct", out indexST);
            int indexEN;
            semi.find("enum", out indexEN);

            int index = Math.Max(indexCL, indexIF);// to find which one i got. no found -1
            index = Math.Max(index, indexST);
            index = Math.Max(index, indexEN);

            if (index != -1 && semi.size() > index + 1)
            {
                ITokenCollection local = Factory.create();
                // local semiExp with tokens for type and name
                local.add(semi[index]).add(semi[index + 1]);
              // local.add(semi[index + 1]).add(semi[index]);
                doActions(local);
                return true;
            }
            return false;
        }
    }
    ///////////////////////////////////////////////////////////////////
    // rule to detect Delegate declarations

    public class DetectDelegate : ARule
    {
        public override bool test(ITokenCollection semi)//Added semi.find function in ITok ss
        {
            Display.displayRules(actionDelegate, "rule   DetectDelegate");
            int index;
            semi.find("delegate", out index);
            if (index != -1 && semi.size() > index + 1)
            {
                ITokenCollection local = Factory.create();//need factory bcoz i dont have semi class but interface ss
                // create local semiExp with tokens for type and name
                local.add(semi[index]).add(semi[index + 2]);
                doActions(local);
                return true;//Fully satisfied no more parsing required ss
            }
            return false;
        }
    }

    ///////////////////////////////////////////////////////////////////
    // rule to dectect function definitions

    public class DetectFunction : ARule
    {
        public static bool isSpecialToken(string token)
        {
            string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using" };
            foreach (string stoken in SpecialToken)
                if (stoken == token)
                    return true;
            return false;
        }
        public override bool test(ITokenCollection semi)
        {
            Display.displayRules(actionDelegate, "rule   DetectFunction");
            if (semi[semi.size() - 1] != "{")
                return false;

            int index;
            semi.find("(", out index);
            if (index > 0 && !isSpecialToken(semi[index - 1]))
            {
                ITokenCollection local = Factory.create();
                local.add("function").add(semi[index - 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }
    ///////////////////////////////////////////////////////////////////
    // detect entering anonymous scope
    // - expects namespace, class, and function scopes
    //   already handled, so put this rule after those

    public class DetectAnonymousScope : ARule
    {
        public override bool test(ITokenCollection semi)
        {
            Display.displayRules(actionDelegate, "rule   DetectAnonymousScope");
            int index;
            semi.find("{", out index);
            if (index != -1)
            {
                ITokenCollection local = Factory.create();
                // create local semiExp with tokens for type and name
                local.add("control").add("anonymous");
                doActions(local);
                return true;
            }
            return false;
        }
    }
    ///////////////////////////////////////////////////////////////////
    // detect public declaration

    public class DetectPublicDeclar : ARule
    {
        public override bool test(ITokenCollection semi)
        {
            Display.displayRules(actionDelegate, "rule   DetectPublicDeclar");
            int index;
            semi.find(";", out index);
            if (index != -1)
            {
                semi.find("public", out index);
                if (index == -1)
                    return true;
                ITokenCollection local = Factory.create();
                // create local semiExp with tokens for type and name
                //local.displayNewLines = false;
                local.add("public " + semi[index + 1]).add(semi[index + 2]);
                                     // type of declaration.name of dec ss
                semi.find("=", out index);
                if (index != -1)
                {
                    doActions(local);
                    return true;
                }
                semi.find("(", out index);
                if (index == -1)
                {
                    doActions(local);
                    return true;
                }
            }
            return false;
        }
    }
    ///////////////////////////////////////////////////////////////////
    // detect leaving scope

    public class DetectLeavingScope : ARule
    {
        public override bool test(ITokenCollection semi)
        {
            Display.displayRules(actionDelegate, "rule   DetectLeavingScope");
            int index;
            semi.find("}", out index);
            if (index != -1)
            {
                doActions(semi);
                return true;
            }
            return false;
        }
    }
    ///////////////////////////////////////////////////////////////////
    // BuildCodeAnalyzer class
    ///////////////////////////////////////////////////////////////////

    public class BuildCodeAnalyzer// Builder Creates repoistry ss
    {
        Repository repo = new Repository();

        public BuildCodeAnalyzer(Lexer.ITokenCollection semi)
        {
            repo.semi = semi;
        }
        public virtual Parser build()//Virtual function returns reference to parser ss
        {
            Parser parser = new Parser();


            // decide what to show
            AAction.displaySemi = false;                        //Change here to see things running ss
            AAction.displayStack = false;  // false is default

            // action used for namespaces, classes, and functions
            PushStack push = new PushStack(repo);

            // capture namespace info
            DetectNamespace detectNS = new DetectNamespace();
            detectNS.add(push);
            parser.add(detectNS);//one rule one action 

            // capture delegate info
            DetectDelegate detectDG = new DetectDelegate();
            detectDG.add(push);
            parser.add(detectDG);

            // capture Alias info
            DetectAlias detectAL = new DetectAlias();
            detectAL.add(push);
            parser.add(detectAL);
            
            // capture class info
            DetectClass detectCl = new DetectClass();
            detectCl.add(push);
            parser.add(detectCl);

            // capture function info
            DetectFunction detectFN = new DetectFunction();
            detectFN.add(push);
            parser.add(detectFN);

            // handle entering anonymous scopes, e.g., if, while, etc.
            DetectAnonymousScope anon = new DetectAnonymousScope();
            anon.add(push);
            parser.add(anon);

            // show public declarations
            DetectPublicDeclar pubDec = new DetectPublicDeclar();
            SaveDeclar print = new SaveDeclar(repo);
            pubDec.add(print);
            parser.add(pubDec);

            // handle leaving scopes
            DetectLeavingScope leave = new DetectLeavingScope();
            PopStack pop = new PopStack(repo);
            leave.add(pop);
            parser.add(leave);

            // parser configured
            return parser;
        }
    }
}

