using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyExcelLab
{
    class RecursionException : Exception { } // обёртка для исключения
    class CellManager
    {
        private static CellManager _instance; // инстанс
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
        } // конструктор инстанса

        public List<string> varCells = new List<string>(); // список используемых переменных
        public List<string> usedCells = new List<string>(); // список ячеек, на которые ссылаются
        public List<string> deletedCells = new List<string>(); //список ячеек, которые удаляют при прошлом вызове команд DeleteRow или DeleteColumn

        private DataGridView _dgv; // datagrid
        public void SetDataGridView(DataGridView dgv) // устанавливает датагрид
        {
            _dgv = dgv;
        }
        public MyCell GetCell(int row, int column) // возвращает MyCell по её индексу
        {
            MyCell cell = GetCell(_dgv[column, row]);
            return cell;
        }
        public MyCell GetCell(DataGridViewCell dgvCell) // возвращает MyCell по соответствующей клетке в датагриде
        {
            MyCell cell = (MyCell)dgvCell.Tag;
            return cell;
        }
        public string CheckCellIsUsed(DataGridViewCell dgvCell) // проверяет использование данной клетки
        {
            string result = "";
            string check = "R"+(dgvCell.RowIndex+1)+"C"+(dgvCell.ColumnIndex+1); // преобразуем имя в нужный формат
            // если она используется
            if (usedCells.Contains(check))
            {
                result = check;
            }
            //тогда возвращаем её имя, иначе возвращаем пустую строку
            return result;
        }
        public bool DoesCellExist(int row, int column) // проверяет существование клетки
        {
            try
            {
                // если клетка существует, то мы можем получить её значение
                GetCell(row, column);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }
        public int GetCellValue(int row, int column) // возвращает значение клетки по индексам
        {
            MyCell cell = GetCell(row, column);

            // если клетка пустая, то нам не нужно вычислять её значение
            if (cell.Expression != "") 
            {
                return cell.EvaluateCell();
            }
            else
            {
                return cell.Value;
            }
        }
        public void RecursionCheck(string name) // проверяет переменную на рекурсию
        {
            // если в списке переменных уже есть такая переменная
            if (varCells.Contains(name))
            {
                // попали в рекурсию, нужнео очистить все переменные
                ClearVariables();
                throw new RecursionException();
            }
            // иначе всё хорошо
            varCells.Add(name);
        }
        public void DeleteVariable(string name) // удаляет конкретную переменную из листа переменных
        {
            varCells.Remove(name);
        }
        public void ClearVariables() // очищает лист переменных
        {
            varCells.Clear();
        }
    }
}
