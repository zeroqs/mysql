using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace mysql
{
    public partial class Form1 : Form
    {
        string sql;
        MySqlDataAdapter da;
        DataTable ds;
        MySqlConnection conn;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            conn = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=hotel");
            try
            {
                conn.Open();
                MessageBox.Show("Подключение к БД установлено");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Проблемы с подключением к БД \n\r" + ex.ToString());
            }
            GetList1();
        }

        private void GetList1()
        {
            sql = "show tables";

            da = new MySqlDataAdapter(sql, conn);
            ds = new DataTable();
            da.Fill(ds);
            dataGridView1.DataSource = ds;
        }

        private void ClearOutput()
        {
            ArrayList Empty = new ArrayList();


            textBox1.Clear();
            dataGridView1.DataSource = Empty;
            dataGridView1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearOutput();
            sql = "SELECT Nomer, \r\nCOUNT(*) AS max_number_settlements\r\nFROM visit\r\nGROUP BY Nomer\r\nHAVING COUNT(*) = (\r\n    SELECT MAX(cnt)\r\n    FROM (\r\n        SELECT COUNT(*) AS cnt\r\n        FROM visit\r\n        GROUP BY Nomer\r\n    ) AS subquery\r\n);";

            da = new MySqlDataAdapter(sql, conn);
            ds = new DataTable();
            textBox1.Text = sql;
            da.Fill(ds);
            dataGridView1.DataSource = ds;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            ClearOutput();
            sql = "SELECT \r\n    CASE \r\n        WHEN EXISTS (\r\n            SELECT 1 \r\n            FROM visit v2\r\n            JOIN klient k2 ON v2.ClientID = k2.ClientID\r\n            JOIN nomers n2 ON v2.Nomer = n2.Nomer\r\n            WHERE k2.Fam = 'Громов'\r\n            AND EXISTS (\r\n                SELECT 1\r\n                FROM visit v3\r\n                JOIN klient k3 ON v3.ClientID = k3.ClientID\r\n                JOIN nomers n3 ON v3.Nomer = n3.Nomer\r\n                WHERE k3.Fam = 'Кларк'\r\n                AND n2.Nomer = n3.Nomer\r\n            )\r\n        ) THEN 'Жили'\r\n        ELSE 'Нет'\r\n    END AS Проживание_в_номере;\r\n";

            da = new MySqlDataAdapter(sql, conn);
            ds = new DataTable();
            textBox1.Text = sql;
            da.Fill(ds);
            dataGridView1.DataSource = ds;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClearOutput();
            sql = "SELECT Nomer, clients_live, NumType\r\nFROM (\r\n    SELECT DISTINCT v.Nomer, COUNT(*) AS clients_live, n.NumType\r\n    FROM visit v\r\n    JOIN nomers n ON v.Nomer = n.Nomer\r\n    GROUP BY n.Nomer\r\n) AS subquery\r\nWHERE clients_live = (SELECT MIN(clients_live) FROM (SELECT COUNT(*) AS clients_live FROM visit GROUP BY Nomer) AS min_clients);\r\n";

            da = new MySqlDataAdapter(sql, conn);
            ds = new DataTable();
            textBox1.Text = sql;
            da.Fill(ds);
            dataGridView1.DataSource = ds;
        }
    }
}
