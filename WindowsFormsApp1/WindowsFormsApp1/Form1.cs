using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Database DB;
        private DataTable table;
        private MySqlDataAdapter adapter;

        class Database 
        {
            public String serverString;
            public String databaseString;
            public String userString;
            public String passwordString;
            public String tableString;
            public String connectionString;

            MySqlConnection connection;
            //= new MySqlConnection("Server=localhost; Database=turizm; User ID=root; Password=08ezozeb");
            public void buildConnectionString() 
            {
                connectionString = "Server=" + serverString + "; Database=" + databaseString + "; Password=" + passwordString;
                connection = new MySqlConnection(connectionString);
            }

            public void openConnection()
            {
                if (connection.State == System.Data.ConnectionState.Closed) connection.Open();
            }

            public void closeConnection() 
            {
                if(connection.State==System.Data.ConnectionState.Open) { connection.Close(); }
            }

            public MySqlConnection GetSqlConnection() 
            {
                return connection;
            }

        }
        public Form1()
        {
            InitializeComponent();
            DB = new Database();
            table = new DataTable();
            adapter = new MySqlDataAdapter();
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            clearAll();
            DB.serverString = textBox1.Text;
            DB.databaseString = textBox2.Text;
            DB.userString = textBox3.Text;
            DB.passwordString = textBox4.Text;
            DB.tableString = textBox5.Text;
            DB.buildConnectionString();
            DB.openConnection();
            string SQLcommand = "select * from " + DB.tableString + ";";
            MySqlCommand command = new MySqlCommand(SQLcommand, DB.GetSqlConnection());
            adapter.SelectCommand = command;
            adapter.Fill(table);
            dataGridView1.DataSource = table;
            DB.closeConnection();
        }

        private void clearAll() 
        {
            table = new DataTable();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            DataTable changedRows = table.GetChanges();
            if (changedRows !=null)
            {
                MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);
                adapter.Update(changedRows);
                table.AcceptChanges();
                MessageBox.Show("Changes saved");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DB.openConnection();
            String findString = textBox6.Text;
            MySqlCommand command = new MySqlCommand();
            String sqlCommand = "SELECT * FROM " + DB.databaseString + "." + DB.tableString + "WHERE ";
            String sqlParam;
            DataGridViewColumn col = dataGridView1.Columns[0];
            sqlParam = "@" + col.Name;
            sqlCommand += col.Name + "=" + sqlParam;
            if(col.ValueType==typeof(int))
            {
                command.Parameters.Add(sqlParam, MySqlDbType.Int32).Value = Convert.ToInt32(findString);
            }
            else 
            {
                command.Parameters.Add(sqlParam, MySqlDbType.VarChar).Value = findString;
            }
            command.CommandText = sqlCommand;
            command.Connection = DB.GetSqlConnection();
            adapter.SelectCommand = command;
            clearAll();
            adapter.Fill(table);
            dataGridView1.DataSource = table;
            DB.closeConnection();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void resizeContent(object sender, EventArgs e)
        {
           // dataGridView1.Width = Width - (dataGridView1.Location.X * 2) - (dataGridView1.Margin.All * 2) - 10; 
        }

        private void drawButton_Click_1(object sender, EventArgs e)
        {

        }

        private void drawButton_Click(object sender, EventArgs e)
        {
            DB.openConnection();
            String axisXstring = textboxAX.Text;
            String axisYstring = textboxAY.Text;
            DB.tableString = textBox5.Text;
            table.Clear();
            MySqlCommand command = new MySqlCommand();
            string sqlCommand = "Select " + axisYstring + "from" + DB.tableString + ";";
            command.CommandText = sqlCommand;
            command.Connection = DB.GetSqlConnection();
            adapter.SelectCommand = command;
            DB.closeConnection();

            Graph.Series.Clear();//от сюда гавной воняет
            Graph.Series.Add("Graph");
            Graph.Series["Graph"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                string xValue = (table.Rows[i][axisXstring].ToString());
                double yValue = Double.Parse(table.Rows[i][axisYstring].ToString());
                Graph.Series["Graph"].Points.AddXY(xValue, yValue);
            }
        }


    }
}
