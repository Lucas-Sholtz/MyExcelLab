using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyExcelLab
{
    class RecursionException : Exception { } // just another wrapper for exception
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
        } // singleton cellmanager

        public List<string> varStack = new List<string>(); // list of cell names

        private DataGridView _dgv; // datagrid
        public void SetDataGridView(DataGridView dgv) // sets datagrid
        {
            _dgv = dgv;
        }
        public MyCell GetCell(int row, int column) // returns MyCell by its row and column indexes
        {
            MyCell cell = (MyCell)_dgv[column, row].Tag;

            return cell;
        }
        public MyCell GetCell(DataGridViewCell dgvCell) // returns MyCell сorresponding to the cell in data grid
        {
            MyCell cell = (MyCell)dgvCell.Tag;
            return cell;
        }
        public bool DoesCellExist(int row, int column)
        {
            try
            {
                // if cell exist then we could get it
                GetCell(row, column);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }
        public int GetCellValue(int row, int column) // returns cell value by its indexes
        {
            MyCell cell = GetCell(row, column);

            // if its empty then we dont need to calculate it
            if (cell.Expression != "") 
            {
                return cell.EvaluateCell();
            }
            else
            {
                return cell.Value;
            }
        }
        public void RecursionCheck(string name) // checks for recursion
        {
            //if we already have such variable
            if (varStack.Contains(name))
            {
                // recursion happened, clear all variables
                ClearVariables();
                throw new RecursionException();
            }
            // if all ok
            varStack.Add(name);
        }
        public void DeleteVariable(string name) // removes variable from var stack
        {
            varStack.Remove(name);
        }
        public void ClearVariables() // clears var stack
        {
            varStack.Clear();
        }
    }
}
