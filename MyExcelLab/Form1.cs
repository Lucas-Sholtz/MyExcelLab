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
        public Form1()//string[] args
        {
            InitializeComponent();
            CellManager.Instance.SetDataGridView(dataGridView1);
            SetupDataGridView(5, 5);
            formulaView = false;
            /*if (args.Length == 1)
            {
                LoadDGV(args[0]);
            }*/
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
            DataGridViewCell cell = dataGridView1.SelectedCells[0];
            dataGridView1.Rows.Insert(cell.RowIndex);
            UpdateDGV();
        }
        private void InsertColumn()
        {
            DataGridViewCell cell = dataGridView1.SelectedCells[0];
            dataGridView1.Columns.Insert(cell.ColumnIndex, new DataGridViewColumn());
            UpdateDGV();
        }
        private void DeleteRow()
        {
            DialogResult result = MessageBox.Show("Ви дійсно хочете видалити виділений рядок?", "Видалити", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (dataGridView1.RowCount > 1)
                {
                    DataGridViewCell cell = dataGridView1.SelectedCells[0];
                    dataGridView1.Rows.RemoveAt(cell.RowIndex);
                }
                UpdateDGV();
                UpdateCellValues();
            }
        }
        private void DeleteColumn()
        {
            DialogResult result = MessageBox.Show("Ви дійсно хочете видалити виділену колонку?", "Видалити", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (dataGridView1.ColumnCount > 1)
                {
                    DataGridViewCell cell = dataGridView1.SelectedCells[0];
                    dataGridView1.Columns.RemoveAt(cell.ColumnIndex);
                }
                UpdateDGV();
                UpdateCellValues();
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
                            cell.Value = myCell.Value;
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
                    newRow[column.ColumnName] = CellManager.Instance.GetCellValue(column.ColumnName[0], row.Index);//////
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
                    Console.WriteLine(dataGridView1.Rows.Count + " " + dataGridView1.Columns.Count);
                    MyCell myCell = CellManager.Instance.GetCell(cell.RowIndex,cell.ColumnIndex);////
                    if (!formulaView)
                    {
                        cell.Value = myCell.Expression;
                    }
                    else
                    {
                        if (myCell.Expression == "")
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
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadDGV(openFileDialog1.FileName);
            }
        }
        private void saveMenuStrip(object sender, EventArgs e)
        {
            if (currPath != "")
            {
                SaveDGV(currPath);
            }
            else
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    SaveDGV(saveFileDialog1.FileName);
                }
            }
        }
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            //SaveDGV(currPath);
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
    }
}
