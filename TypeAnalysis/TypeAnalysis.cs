using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeTable;

namespace TypeAnalysis
{
    public class TypeAnalysis
    {
        public void showType()
        {
            TypeTable.TypeTable ts = new TypeTable.TypeTable();
            ts.show();
        }
    }
}
