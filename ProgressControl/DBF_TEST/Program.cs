using System;
using System.Configuration;
using System.Data.OleDb;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
namespace DBF_TEST
{
    class Program
    {

        static  void Main(string[] args)
        {
            //string connect = ConfigurationManager.ConnectionStrings["dbfConnection"].ConnectionString;
            //using (OleDbConnection conn = new OleDbConnection(connect))
            //{
            //    conn.Open();
            //    var command = conn.CreateCommand();
            //    command.CommandText = "SELECT * FROM spc";
            //    var reader = command.ExecuteReader();
            //    for (int i = 0; i < reader.FieldCount; i++)
            //    {
            //        Console.Write($"{reader.GetName(i)} ");
            //    }
            //    Console.WriteLine();

            //    while (reader.Read())
            //    {
            //        string name = (string)reader["name"];
            //        name = name.Trim(' ', '*');
            //        string code = (string)reader["code"];
            //        DateTime Date = (DateTime)reader["date"];

            //        Console.Write($"{name} | {code} | {Date}| \n");
            //    }

            //}
            DBF_Connector obj = new DBF_Connector(ConfigurationManager.ConnectionStrings["dbfConnection"].ConnectionString);
            var list = obj.GetEntitiesAsync(TableName.elements, new ElementCreator());
            var list1 = obj.GetEntitiesAsync(TableName.spc, new SpecificationCreator());
            var list2 = obj.GetEntitiesAsync(TableName.analogs, new AnalogsCreator());
            Task.WaitAll(list, list1,list2);
            foreach (var el in list.Result)
            {
                var tmp = el as Element;
                Console.WriteLine($"{tmp.Code} {tmp.Name} {tmp.Quantity}");
            }
            foreach (var el in list1.Result)
            {
                var tmp = el as Specification;
                Console.WriteLine($"{tmp.Code} {tmp.Name} {tmp.Date}");
            }
            foreach (var el in list2.Result)
            {
                var tmp = el as Analogs;
                Console.WriteLine($"{tmp.Code} - {tmp.ACode}");
            }


            Console.ReadKey();
        }
    }
}
