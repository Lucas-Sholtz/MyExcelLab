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
        bool formulaView;
        string currPath = "";
        public Form1()
        {
            InitializeComponent();
            CellManager.Instance.SetDataGridView(dataGridView1);
            SetupDataGridView(10, 10);
            formulaView = false;
        }
        private void SetupDataGridView(int rows, int columns)
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.NonPublic
                                                | System.Reflection.BindingFlags.Instance
                                                | System.Reflection.BindingFlags.SetProperty,
                                                null, dataGridView1, new object[] { true });
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ColumnCount = columns;
            dataGridView1.RowCount = rows;
            UpdateDGV();
        }
        private void AddRow()
        {
            dataGridView1.RowCount++;
            UpdateDGV();
        }
        private void AddColumn()
        {
            dataGridView1.ColumnCount++;
            UpdateDGV();
        }
        private void InsertRow()
        {
            DataGridViewCell cell = null;
            try
            {
                cell = dataGridView1.SelectedCells[0];
            }
            catch (Exception) { }
            if (cell != null)
            {
                dataGridView1.Rows.Insert(cell.ColumnIndex, new DataGridViewRow());
                UpdateDGV();
            }
            else
            {
                MessageBox.Show("Ви не можете вставити рядок у пусту таблицю, бо у ній неможливо вказати місце вставки.\nСпробуйте додати.", "Повідомлення", MessageBoxButtons.OK);
            }
        }
        private void InsertColumn()
        {
            DataGridViewCell cell = null;
            try
            {
                cell = dataGridView1.SelectedCells[0];
            }
            catch (Exception) { }
            if (cell != null)
            {
                DataGridViewColumn column = new DataGridViewColumn();
                column.CellTemplate = new DataGridViewTextBoxCell();
                dataGridView1.Columns.Insert(cell.ColumnIndex, column);
                UpdateDGV();
            }
            else
            {
                MessageBox.Show("Ви не можете вставити колонку у пусту таблицю, бо у ній неможливо вказати місце вставки.\nСпробуйте додати.", "Повідомлення", MessageBoxButtons.OK);
            }
        }
        private void DeleteRow()
        {
            DialogResult result = DialogResult.No;
            if (dataGridView1.RowCount == 1)
            {
                result = MessageBox.Show("Ви дійсно хочете видалити останній рядок?", "Видалити", MessageBoxButtons.YesNo);
            }
            if (dataGridView1.RowCount == 0||dataGridView1.ColumnCount==0)
            {
                MessageBox.Show("Таблиця порожня!","Повідомлення", MessageBoxButtons.OK);
            }
            else if ((dataGridView1.RowCount > 1) || (dataGridView1.RowCount == 1 && result == DialogResult.Yes))
            {
                DataGridViewCell cell = dataGridView1.SelectedCells[0];////////////////
                DataGridViewRow row = dataGridView1.Rows[cell.RowIndex];
                DialogResult res=DialogResult.No;
                string deletedCells = "";
                for(int i=0;i<row.Cells.Count;i++)
                {
                    string name = CellManager.Instance.CheckCellIsUsed(row.Cells[i]);
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
                        "В обраному рядку є клітинки " + deletedCells + "на які посилаються інші клітинки таблиці.\n" +
                        "Якщо ви видалите цей рядок, то значення цих клітинок буде прийняте за IntMax.\n" +
                        "Ви дійсно хочете це зробити?", "Попередження", MessageBoxButtons.YesNo
                        );
                }
                if (res == DialogResult.Yes)
                {
                    dataGridView1.Rows.RemoveAt(cell.RowIndex);
                    UpdateDGV();
                    UpdateCellValues();
                }
                CellManager.Instance.deletedCells.Clear();
            }
        }
        private void DeleteColumn()
        {
            DialogResult result = DialogResult.No;
            if (dataGridView1.ColumnCount == 1)
            {
                result = MessageBox.Show("Ви дійсно хочете видалити останню колонку?", "Видалити", MessageBoxButtons.YesNo);
            }
            if (dataGridView1.RowCount == 0 || dataGridView1.ColumnCount == 0)
            {
                MessageBox.Show("Таблиця порожня!", "Повідомлення", MessageBoxButtons.OK);
            }
            else if ((dataGridView1.ColumnCount > 1) || (dataGridView1.ColumnCount == 1 && result == DialogResult.Yes))
            {
                DataGridViewCell cell = dataGridView1.SelectedCells[0];////////////////
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
                if (res == DialogResult.Yes)
                {
                    dataGridView1.Columns.RemoveAt(cell.ColumnIndex);
                    UpdateDGV();
                    UpdateCellValues();
                }
                CellManager.Instance.deletedCells.Clear();
            }
        }
        private void UpdateDGV()        
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.HeaderText = "C" + (column.Index + 1).ToString();
                column.MinimumWidth = 100;
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.HeaderCell.Value = "R" + (row.Index + 1).ToString();
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Tag == null)
                    {
                        cell.Tag = new MyCell(cell, "");
                    }
                }
            }
        }
        public void UpdateCellValues()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    MyCell myCell = (MyCell)cell.Tag;
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
        private void SaveDGV(string path)
        {
            currPath = path;
            dataGridView1.EndEdit();
            DataTable table = new DataTable("data");
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                table.Columns.Add(column.Index.ToString());
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataRow newRow = table.NewRow();
                foreach (DataColumn column in table.Columns)
                {
                    newRow[column.ColumnName] = CellManager.Instance.GetCell(row.Cells[int.Parse(column.ColumnName)]).Expression;//////
                }
                table.Rows.Add(newRow);
            }
            table.WriteXml(path);
        }
        private void LoadDGV(string path)
        {
            currPath = path;
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(path);
            DataTable table = dataSet.Tables[0];
            dataGridView1.ColumnCount = table.Columns.Count;
            dataGridView1.RowCount = table.Rows.Count;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Tag = new MyCell(cell, table.Rows[cell.RowIndex][cell.ColumnIndex].ToString());
                }
                UpdateDGV();
                UpdateCellValues();
            }
        }
        private void ChangeView()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    MyCell myCell = CellManager.Instance.GetCell(cell);////
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
            formulaView = !formulaView;
        }
        private void CellEndEdit(object sender, DataGridViewCellEventArgs e)
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
                    UpdateCellValues();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    CellManager.Instance.ClearVariables();
                    myCell.Expression = prevExpr;
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
            string path = saveFileDialog1.FileName;/////////////
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

        private void CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            MyCell myCell = CellManager.Instance.GetCell(e.RowIndex, e.ColumnIndex);
            DataGridViewCell cell = myCell.Parent;
            cell.Value = myCell.Expression;
        }

        private void CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.BeginEdit(true);
        }

        private void helpClick(object sender, EventArgs e)
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
            string path = openFileDialog1.FileName;//////////////////
            currPath = path;
            LoadDGV(path);
            UpdateDGV();
        }
    }
}
