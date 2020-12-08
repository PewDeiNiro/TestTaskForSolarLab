using System;
using System.Collections.Generic;
using System.Data.SqlClient;



namespace ExampleSQLApp
{
    class Program
    {
        private List<String> list = new List<String>();
        String filePath = "tasks.txt";
        private const String DONE = " DONE";
        static void Main()
        {

            new Program();
        }

        public Program()
        {
            LoadFromDB();
            Start();
        }

        public void Start()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Выберите действие(цифру) которое вы хотите сделать \n 1) Добавить задачу \n 2) Вывести задачи на экран \n 3) Сохранить задачи \n 4) Редактировать\n 5) Удалить задачу \n 6) Отметить выполненные/невыполненные задачи \n 7) Сортировать \n 8) Выйти \n");


                String choose = Console.ReadLine().Trim();


                if (choose == "1")
                {
                    Add();
                }
                else if (choose == "2")
                {
                    Print();
                }
                else if (choose == "3")
                {
                    SaveToDB();
                }
                else if (choose == "4")
                {
                    Edit();
                }
                else if (choose == "5")
                {
                    Remove();
                }
                else if (choose == "6")
                {
                    Select();
                }
                else if (choose == "7")
                {
                    Sort();
                }
                else if (choose == "8")
                {
                    Console.WriteLine("Благодарю за использование планировщика задач. До свидания!");
                    break;
                }
                else
                {
                    Console.WriteLine("Данного варианта ответа нету. Введите цифру от 1 до 4");
                }
            }
        }

        public void Add()
        {
            Console.Clear();
            Console.Write("Какую задачу вы хотите добавить: ");
            String task = Console.ReadLine();
            if (!task.Trim().Equals(""))
            {
                list.Add(task);
            }
        }

        public void Print()
        {
            Console.Clear();
            int iter = 1;
            foreach (String temp in list)
            {
                Console.WriteLine(iter + ") " + temp);
                iter++;
            }
            Console.ReadLine();
        }

        public void SaveToDB()
        {
            Console.Clear();
            ClearBD();
            SqlConnection connection = new SqlConnection(get_cs());
            connection.Open();
            foreach (String temp in list) {
                String sql = "INSERT INTO [tasks].[dbo].[tasks] (task) VALUES ('" + temp + "')";
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
            Console.WriteLine("Данные успешно сохранены :)");
            Console.ReadLine();
        }

        public void ClearBD() {
            SqlConnection connection = new SqlConnection(get_cs());
            connection.Open();
            String sql = "DELETE FROM [tasks].[dbo].[tasks]";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void LoadFromDB()
        {
            list.Clear();
            SqlConnection connection = new SqlConnection(get_cs());
            connection.Open();
            string sql = "SELECT task FROM [tasks].[dbo].[tasks]";
            SqlCommand cmd = new SqlCommand(sql, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            int iter = 0;
            while (reader.Read())
            {
                String temp = reader.GetValue(0).ToString();
                list.Add(temp);
            }
            connection.Close();
        }

        public void Edit()
        {
            Console.Clear();
            Console.WriteLine("Выберите задачу(цифру) которую вы хотите редактировать");
            int iter = 1;
            foreach (String temp in list)
            {
                Console.WriteLine(iter + ") " + temp);
                iter++;
            }
            int choose = 0;
            String text = Console.ReadLine();
            try
            {
                choose = Int32.Parse(text);
            }
            catch (System.FormatException e) {
                if (!text.Trim().Equals(""))
                {
                    Console.Clear();
                    Console.WriteLine("Произошла какая-то ошибка :(");
                    Console.ReadLine();
                }
                return;
            };
            Console.WriteLine("Вы редактируете задачу - " + Get(choose - 1) + "\n Введите новый текст задачи:");
            String task = Console.ReadLine();
            if (!Get(choose - 1).EndsWith(DONE))
            {
                list.RemoveAt(choose - 1);
                list.Insert(choose - 1, task);
            }
            else 
            {
                list.RemoveAt(choose - 1);
                list.Insert(choose - 1, task + DONE);
            }
        }

        public void Sort()
        {
            Console.Clear();
            List<String> woDONE = new List<String>();
            List<String> wDONE = new List<String>();
            foreach (String temp in list) {
                if (temp.EndsWith(DONE))
                {
                    wDONE.Add(temp);
                }
                else 
                {
                    woDONE.Add(temp);
                }
            }
            list.Clear();
            foreach (String temp in woDONE) {
                list.Add(temp);
            }
            foreach (String temp in wDONE) {
                list.Add(temp);
            }
            Console.WriteLine("Задачи успешно отсортированы :)");
            Console.ReadLine();
        }

        public String Get(int index)
        {
            int iterator = 0;
            foreach (String temp in list)
            {
                if (iterator == index)
                {
                    return temp;
                }
                iterator++;
            }
            return "-1";
        }

        public void Select() {
            Console.Clear();
            int iter = 1;
            foreach (String temp in list)
            {
                Console.WriteLine(iter + ") " + temp);
                iter++;
            }
            Console.WriteLine("Введите номер задачи которую вы хотите отметить выполненной/невыполненной");
            int index = -1;
            String text = Console.ReadLine();
            try
            {
                index = Int32.Parse(text) - 1;
            }
            catch (Exception e) {
                if (!text.Trim().Equals(""))
                {
                    Console.Clear();
                    Console.WriteLine("Произошла ошибка :(");
                    Console.ReadLine();
                }
                return;
            }
            String task = Get(index);
            list.RemoveAt(index);
            if (!task.EndsWith(DONE))
            {
                list.Insert(index, task + DONE);
            }
            else 
            {
                task = task.Substring(0, task.Length - 5);
                list.Insert(index, task);
            }
        }

        public string get_cs()
        {
            return "Data Source=DESKTOP-RCL1AGM\\SQLEXPRESS; Initial Catalog = tasks; User ID = sa; Password = 123456";
        }

        public bool IsInteger(String text) {
            try
            {
                Int32.Parse(text);
            }
            catch (Exception e) {
                return false;
            }
            return true;
        }

        public void Remove()
        {
            Console.Clear();
            int iter = 1;
            bool removed = false;
            do
            {
                try
                {
                        foreach (String temp in list)
                        {
                            Console.WriteLine(iter + ") " + temp);
                            iter++;
                        }

                        
                        Console.Write("Введите номер задачи, которую вы хотите удалить: ");
                        String text = Console.ReadLine();

                        if (!IsInteger(text) && text.Trim().Equals(""))
                        {
                            removed = true;
                            return;
                        }
                        else if (IsInteger(text))
                        {
                            list.RemoveAt(Int32.Parse(text) - 1);
                            removed = true;
                        }
                        else {
                            Console.WriteLine("Неверный ответ. Попробуйте снова!");
                        }
                }
                catch (System.InvalidOperationException e) { };
            } while (!removed);
        }
    }
}