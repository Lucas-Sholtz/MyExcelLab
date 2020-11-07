using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyExcelLab
{
    public partial class Form1 : Form
    {
        bool formulaView; // текущий режим просмотра
        string currPath = ""; // текущий путь к фалу
        public Form1()
        {
            InitializeComponent();
            CellManager.Instance.SetDataGridView(dataGridView1); // устанавливаем датагрид
            SetupDataGridView(10, 10); // размером 10 на 10
            formulaView = false; // режим просмотра : числа
        }
        private void SetupDataGridView(int rows, int columns) // конфигурирует датагрид
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.NonPublic
                                                | System.Reflection.BindingFlags.Instance
                                                | System.Reflection.BindingFlags.SetProperty,
                                                null, dataGridView1, new object[] { true }); // это я подсмотрел в интернете
            dataGridView1.AllowUserToAddRows = false; // юзер не может добавлять ряды иным способом кроме кнопки AddRow
            dataGridView1.ColumnCount = columns; // устанавливаем колонку
            dataGridView1.RowCount = rows;  // и ряды
            UpdateDGV(); // обновляем таблицу
        }
        private void AddRow() // добавляет ряд
        {
            dataGridView1.RowCount++;
            // обновляем таблицу
            UpdateDGV();
        }
        private void AddColumn() // добавляет колонку
        {
            dataGridView1.ColumnCount++;
            // обновляем таблицу
            UpdateDGV();
        }
        private void InsertRow() // вставляет строку в выделенное место
        {
            DataGridViewCell cell = null;
            try
            {
                // пробуем взять выделенную клетку
                cell = dataGridView1.SelectedCells[0];
            }
            catch (Exception) { }
            // если не пустая
            if (cell != null)
            {
                // то вставим строку
                dataGridView1.Rows.Insert(cell.ColumnIndex, new DataGridViewRow());
                // и обновим таблицу
                UpdateDGV();
            }
            else
            {
                // если ни одной ячейки не выделено то таблица пустая (старый функционал, таблица не может быть пустой)
                MessageBox.Show("Ви не можете вставити рядок у пусту таблицю, бо у ній неможливо вказати місце вставки.\nСпробуйте додати.", "Повідомлення", MessageBoxButtons.OK);
            }
        }
        private void InsertColumn() // вставляет колонку в выбранное место
        {
            DataGridViewCell cell = null;
            try
            {
                cell = dataGridView1.SelectedCells[0];
            }
            catch (Exception) { }
            if (cell != null)
            {
                // таким образом вставляется колонка
                DataGridViewColumn column = new DataGridViewColumn();
                column.CellTemplate = new DataGridViewTextBoxCell();
                dataGridView1.Columns.Insert(cell.ColumnIndex, column);
                // обновим таблицу
                UpdateDGV();
            }
            else
            {
                // если ни одной ячейки не выделено то таблица пустая (старый функционал, таблица не может быть пустой)
                MessageBox.Show("Ви не можете вставити колонку у пусту таблицю, бо у ній неможливо вказати місце вставки.\nСпробуйте додати.", "Повідомлення", MessageBoxButtons.OK);
            }
        }
        private void DeleteRow() // удадяет выбранную строку
        {
            if (dataGridView1.RowCount == 1) // последний ряд нельзя удалить
            {
                MessageBox.Show("Не можна видалити останній рядок.", "Попередження", MessageBoxButtons.OK);
            }
            else if (dataGridView1.RowCount > 1) 
            {
                DialogResult res = DialogResult.No; // результат диалога, в котором пользователя предупреждают об удалении ячеек на которые что то ссылается
                DataGridViewCell cell = dataGridView1.SelectedCells[0];
                DataGridViewRow row = dataGridView1.Rows[cell.RowIndex];
                string deletedCells = ""; // список удаляемых ячеек
                for(int i=0;i<row.Cells.Count;i++) // итерируемся по ряду
                {
                    // и проверяем ссылается ли на ячейку что-то
                    string name = CellManager.Instance.CheckCellIsUsed(row.Cells[i]);
                    if (name.Length > 0) // если ссылается то имя будет не пустое
                    {
                        // добавляем имя в список ячеек
                        deletedCells += name + " ";
                        // и в лист удалённых ячеек
                        CellManager.Instance.deletedCells.Add(name);
                    }
                }
                if (deletedCells.Length > 0) // если хоть одна ячейка на которую что то ссылается будет удалена то нужно предупредить пользователя
                {
                     res = MessageBox.Show
                        (
                        "В обраному рядку є клітинки " + deletedCells + "на які посилаються інші клітинки таблиці.\n" +
                        "Якщо ви видалите цей рядок, то значення цих клітинок буде прийняте за IntMax.\n" +
                        "Ви дійсно хочете це зробити?", "Попередження", MessageBoxButtons.YesNo
                        );
                }
                if (res == DialogResult.Yes||deletedCells.Length==0)
                {
                    // удаляем ряд, обновляем таблицу и значения ячеек
                    dataGridView1.Rows.RemoveAt(cell.RowIndex);
                    UpdateDGV();
                    UpdateCellValues();
                }
                // чистим список удалённых ячеек
                CellManager.Instance.deletedCells.Clear();
            }
        }
        private void DeleteColumn() // удаляет выделенную колонку, работает аналогично предыдущему методу
        {
            if (dataGridView1.ColumnCount == 1)
            {
                MessageBox.Show("Не можна видалити останню колонку!", "Попередження", MessageBoxButtons.OK);
            }
            else if (dataGridView1.ColumnCount > 1)
            {
                DataGridViewCell cell = dataGridView1.SelectedCells[0];
                DialogResult res = DialogResult.No;
                string deletedCells = "";
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    string name = CellManager.Instance.CheckCellIsUsed(dataGridView1[cell.ColumnIndex,i]);
                    if (name.Length > 0)
                    {
                        deletedCells += name + " ";
                        CellManager.Instance.deletedCells.Add(name);
                    }
                }
                if (deletedCells.Length > 0)
                {
                    res = MessageBox.Show
                       (
                       "В обраній колонці є клітинки " + deletedCells + "на які посилаються інші клітинки таблиці.\n" +
                       "Якщо ви видалите цю колонку, то значення цих клітинок буде прийняте за IntMax.\n" +
                       "Ви дійсно хочете це зробити?", "Попередження", MessageBoxButtons.YesNo
                       );
                }
                if (res == DialogResult.Yes||deletedCells.Length==0)
                {
                    dataGridView1.Columns.RemoveAt(cell.ColumnIndex);
                    UpdateDGV();
                    UpdateCellValues();
                }
                CellManager.Instance.deletedCells.Clear();
            }
        }
        private void UpdateDGV() // обновляет таблицу  
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                // обновляем исена колонок и их ширину
                column.HeaderText = "C" + (column.Index + 1).ToString();
                column.MinimumWidth = 100;
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // обновляем имена строк
                row.HeaderCell.Value = "R" + (row.Index + 1).ToString();
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Tag == null)
                    {
                        // обновляем теги у пустых ячеек
                        cell.Tag = new MyCell(cell, "");
                    }
                }
            }
        }
        public void UpdateCellValues() // пересчитывает значения ячеек
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    MyCell myCell = (MyCell)cell.Tag;
                    // считаем значение
                    myCell.EvaluateCell();
                    if (!formulaView)
                    {
                        if (myCell.Expression == "")
                        {
                            cell.Value = myCell.Expression;
                        }
                        else
                        {
                            cell.Value = myCell.Value.ToString();
                        }
                    }
                }
            }
        }
        private void SaveDGV(string path) // сохраняет таблицу
        {
            currPath = path;
            dataGridView1.EndEdit();
            DataTable table = new DataTable("data"); // таблица, которую мы потом запишем в ХМЛ
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                // добавляем колонки
                table.Columns.Add(column.Index.ToString());
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // создаём в таблице колонки
                DataRow newRow = table.NewRow();
                foreach (DataColumn column in table.Columns)
                {
                    // и всписываем в них значения выражений в ячейке
                    newRow[column.ColumnName] = CellManager.Instance.GetCell(row.Cells[int.Parse(column.ColumnName)]).Expression;
                }
                table.Rows.Add(newRow);
            }
            // записываем ХМЛ файл
            table.WriteXml(path);
        }
        private void LoadDGV(string path) // запгружает таблицу из ХМЛ файла
        {
            currPath = path;
            DataSet dataSet = new DataSet(); 
            // читаем ХМЛ файл
            dataSet.ReadXml(path);
            // формируем таблицу
            DataTable table = dataSet.Tables[0];
            // устанавливаем её размеры
            dataGridView1.ColumnCount = table.Columns.Count;
            dataGridView1.RowCount = table.Rows.Count;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    // записываем значения
                    cell.Tag = new MyCell(cell, table.Rows[cell.RowIndex][cell.ColumnIndex].ToString());
                }
                // обновляем таблицу и значения ячеек
                UpdateDGV();
                UpdateCellValues();
            }
        }
        private void ChangeView() // меняет режим просмотра
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    // в каждой ячейке меняем значение
                    MyCell myCell = CellManager.Instance.GetCell(cell);
                    if (!formulaView)
                    {
                        cell.Value = myCell.Expression;
                    }
                    else
                    {
                        if (myCell.Expression == "")
                        {
                            cell.Value = myCell.Expression;
                        }
                        else
                        {
                            cell.Value = myCell.Value;
                        }
                    }
                }
            }
            // переключаем буль
            formulaView = !formulaView;
        }
        private void CellEndEdit(object sender, DataGridViewCellEventArgs e) // обновляем значение в ячейке после редактирования
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1)
            {
                return;
            }
            MyCell myCell = CellManager.Instance.GetCell(e.RowIndex, e.ColumnIndex);
            DataGridViewCell cell = myCell.Parent;
            string prevExpr = myCell.Expression;
            if (cell.Value != null)
            {
                myCell.Expression = cell.Value.ToString();
                try
                {
                    // пробуем обновить значение
                    UpdateCellValues();
                }
                catch (Exception ex) // если ошибка
                {
                    MessageBox.Show(ex.Message);
                    CellManager.Instance.ClearVariables();
                    // делаем откат до предыдущего выражения
                    myCell.Expression = prevExpr;
                    // обновляем значения
                    UpdateCellValues();
                }
            }
            else
            {
                myCell.Expression = "";
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            AddRow();
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            AddColumn();
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            DeleteRow();
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            DeleteColumn();
        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            InsertRow();
        }
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            InsertColumn();
        }
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            ChangeView();
        }
        private void valueToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        private void openMenuStrip(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("В поточному файлі можуть бути не збережені зміни.\nВи хочете їх зберегти?", "Зберегти файл", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (currPath != "")
                {
                    SaveDGV(currPath);
                }
                else
                {
                    saveFileDialog1.ShowDialog();
                }
            }
            openFileDialog1.ShowDialog();
        }
        private void saveMenuStrip(object sender, EventArgs e)
        {
            if (currPath != "")
            {
                SaveDGV(currPath);
            }
            else
            {
                saveFileDialog1.ShowDialog();
            }
        }
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string path = saveFileDialog1.FileName;
            SaveDGV(path);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void formClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Ви хочете зберегти внесені зміни?", "Зберегти файл", MessageBoxButtons.YesNoCancel);
            if(result == DialogResult.Yes)
            {
                if (currPath != "")
                {
                    SaveDGV(currPath);
                }
                else
                {
                    saveFileDialog1.ShowDialog();
                }
            }
            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e) // в начале редактирования ячейки
        {
            // связываем её с MyCell
            MyCell myCell = CellManager.Instance.GetCell(e.RowIndex, e.ColumnIndex);
            // присваеваем как родителя
            DataGridViewCell cell = myCell.Parent;
            cell.Value = myCell.Expression;
        }

        private void CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.BeginEdit(true);
        }

        private void helpClick(object sender, EventArgs e) // вызов справки
        {
            MessageBox.Show
                (
                "Опис программи:\n " +
                "Даний табличний процессор дає змогу обчислювати значення логічно-арифметичних виразів, які складаються з операцій:\n " +
                "(), !, унарні + та -; %, /, |; &; =, >, <. Операціі вказані у порядку зменшення пріорітетності.\n " +
                "При застосуванні логічних операцій до цілих чисел, вони конвертуються в bool за принципом: >0 : true, інакше false.\n " +
                "Також є можливість вказувати назви клітинок, як змінні у виразах інших клітинок.\n\n " +
                "Ви можете відкривати та зберігати таблиці у форматі xml, для цього натисніть на кнопку \"Файл\".\n" +
                "Кнопки \"Додати рядок\\колонку\" додають іх у кінець таблиці.\n" +
                "Кнопки \"Вставити рядок\\колонку\" вставляють іх на позицію перед виділеною клітинкою.\n" +
                "Кнопки \"Видалити рядок\\колонку\" видаляють іх на позиції обраної клітинки.\n" +
                "Кнопка \"Режим перегляду\" змінює числовий режим на формульний і навпаки.", "Довідка"
                );
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string path = openFileDialog1.FileName;
            currPath = path;
            LoadDGV(path);
            UpdateDGV();
        }
    }
}
