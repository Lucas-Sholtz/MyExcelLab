using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyExcelLab
{
    class MyCell
    {
        public DataGridViewCell Parent { get; set; } // dgv parent of myCell
        public int Value { get; set; } // its value
        public string Expression { get; set; } // string expression
        public MyCell(DataGridViewCell parent, string expression)
        {
            this.Parent = parent;
            Value = 0;
            this.Expression = expression;
        }
        public int EvaluateCell() // returns cell value
        {
            if (Expression != "")
            {
                Value = Interpreter.DoInterpretation(Expression);
            }
            return Value;
        }
    }
}
