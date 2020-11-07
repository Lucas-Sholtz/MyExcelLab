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
        public DataGridViewCell Parent { get; set; } // ячейка датагрида, к которой прикреплена MyCell
        public int Value { get; set; } // значение ячейки
        public string Expression { get; set; } // её выражение
        public MyCell(DataGridViewCell parent, string expression)
        {
            this.Parent = parent;
            Value = 0;
            this.Expression = expression;
        }
        public int EvaluateCell() // вычисляет значение ячейки
        {
            // если не пустое, то интерпретируем выражение
            if (Expression != "")
            {
                Value = Interpreter.DoInterpretation(Expression);
            }
            return Value;
        }
    }
}
