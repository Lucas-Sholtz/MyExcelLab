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
        public DataGridViewCell Parent { get; set; }
        public bool Value { get; set; }
        public string Expression { get; set; }
        public MyCell(DataGridViewCell parent, string expression)
        {
            this.Parent = parent;
            Value = false;/////
            this.Expression = expression;
        }
        public bool EvaluateCell()
        {
            if (Expression != "")
            {
                Value = Interpreter.DoInterpretation(Expression);
            }
            return Value;
        }
    }
}
