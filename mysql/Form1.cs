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
        string sql;    //объясвление запросов
        MySqlDataAdapter da;     //методы библиотеки, позволяющие подключится к MySql
        DataTable ds;
        MySqlConnection conn;

        public Form1()    //создание 1 формы
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)    //метод, который отрабатывает во время загрузки формы
        {
            conn = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=hotel");   //Sql соединение, подключение к серверу, параметры
            try     //конструкция обработки исключений (ошибок)                                                    //подключения предоставляет программа
            {
                conn.Open();           //попытка открытия соединения с базой данных
                MessageBox.Show("Подключение к БД установлено");        //если все хорошо выводится
            }
            catch (Exception ex)      //если ошибка в подключении к базе данных то уходим в catch
            {
                MessageBox.Show("Проблемы с подключением к БД \n\r" + ex.ToString());   //вывод сообщения + ошибки
            }
            GetList1();    //вызов функции
        }

        private void GetList1()    //объявление функции      //private - модификатор, который обозначает, что данная функция может использоваться только внутри класса
        {
            sql = "show tables";    //вывод всех таблиц на экран

            da = new MySqlDataAdapter(sql, conn);   //Создается объект MySqlDataAdapter с использованием SQL-запроса и соединения с базой данных
            ds = new DataTable();   //Создается объект DataTable, который будет хранить результаты запроса в виде таблицы в памяти
            da.Fill(ds);   //Заполняет DataTable данными из базы данных с помощью адаптера данных
            dataGridView1.DataSource = ds;   //Устанавливает источник данных для DataGridView равным заполненной таблице ds,
        }                                    // что приводит к отображению данных в таблице на форме

        private void ClearOutput()     // метод, который очищает вывод на форме
        {
            ArrayList Empty = new ArrayList();  //Создается объект ArrayList с именем Empty
                             //создается пустой список для использования в dataGridView1.DataSource для того, чтобы очистить содержимое

            textBox1.Clear();   //Очищает содержимое текстового поля textBox1
            dataGridView1.DataSource = Empty;    //Устанавливает источник данных для DataGridView равным пустому списку, что приводит к удалению данных из таблицы на форме
            dataGridView1.Refresh();   //Обновляет отображение DataGridView
        }

        private void button1_Click(object sender, EventArgs e)   //метод, который выполняется при нажатии на кнопку с именем button1
        {
            ClearOutput();   //Вызывает метод ClearOutput(), который очищает вывод на форме
            sql = "SELECT Nomer, \r\nCOUNT(*) AS max_number_settlements\r\nFROM visit\r\nGROUP BY Nomer\r\nHAVING COUNT(*) = (\r\n    SELECT MAX(cnt)\r\n    FROM (\r\n        SELECT COUNT(*) AS cnt\r\n        FROM visit\r\n        GROUP BY Nomer\r\n    ) AS subquery\r\n);";

            da = new MySqlDataAdapter(sql, conn);  
            ds = new DataTable();
            textBox1.Text = sql;    //Устанавливает текст в textBox1 равным строке sql для отображения самого SQL-запроса на форме
            da.Fill(ds);     //Заполняет DataTable данными из базы данных с помощью адаптера данных
            dataGridView1.DataSource = ds;
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
