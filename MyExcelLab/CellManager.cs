using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyExcelLab
{
    class RecursionException : Exception { }
    class CellManager
    {
        private static CellManager _instance;
        public static CellManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CellManager();
                }
                return _instance;
            }
        }

        public List<string> varStack = new List<string>(); //list of cell names
        private DataGridView _dgv;
        public void SetDataGridView(DataGridView dgv)
        {
            _dgv = dgv;
        }
        public MyCell GetCell(int row, int column)
        {
            MyCell cell = (MyCell)_dgv[row, column].Tag;////////
            return cell;
        }

        public bool DoesCellExist(int row, int column)
        {
            try
            {
                GetCell(row, column);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }
        public bool GetCellValue(int row, int column)
        {
            MyCell cell = GetCell(row, column);
            if (cell.Expression != "")
            {
                return cell.EvaluateCell();
            }
            else
            {
                return cell.Value;
            }
        }
        public void RecursionCheck(string name)
        {
            if (varStack.Contains(name))
            {
                ClearVariables();
                throw new RecursionException();
            }
            varStack.Add(name);
        }
        public void DeleteVariable(string name)
        {
            varStack.Remove(name);
        }
        public void ClearVariables()
        {
            varStack.Clear();
        }
    }
}
