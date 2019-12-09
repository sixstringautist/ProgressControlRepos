using System;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using System.Diagnostics;
namespace DBF_TEST
{
    enum TableName
    {
        analogs,
        elements,
        spc,
        spc_el
    };

    class DBF_Connector : IDisposable
    {
        OleDbConnection mainConnection;

        private OleDbCommand CreateCommand(string command, TableName tname, params string[] colname)
        {
            string CreateColumns()
            {
                string str = "";
                foreach (var el in colname)
                {
                    str += el;
                    str += ", ";
                }
                str = str.TrimEnd(',');
                return str;
            }
            var _command = mainConnection.CreateCommand();
            _command.CommandText = $"{command} {(colname.Length > 0 ? CreateColumns() : "*")} FROM {tname.ToString()}";
            return _command;

        }


        public async Task<IEnumerable<DBObject<int>>> GetEntitiesAsync(TableName tname, DatabaseObjectCreator<int> creator)
        {

            var command = CreateCommand("select", tname);
            var reader = command.ExecuteReaderAsync();
            Task<IEnumerable<DBObject<int>>> t = Task.Factory.StartNew<IEnumerable<DBObject<int>>>(delegate ()
            {
                List<DBObject<int>> list = new List<DBObject<int>>();
                List<string> names = new List<string>();
                Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
                reader.Wait();
                for (int i = 0; i < reader.Result.FieldCount; i++)
                {
                    names.Add(reader.Result.GetName(i));
                }
                var keys = parameters.Keys;
                while (reader.Result.Read())
                {
                    foreach (var el in names)
                    {
                        parameters[el] = reader.Result[el];
                    }
                    try
                    {
                        list.Add(creator.CreateDatabaseObject(new DBObject<int>() { Values = parameters }));
                    }
                    catch (InvalidCastException e)
                    {
                        Debug.WriteLine($"{e.Message} in {e.Source}");
                    }
                }
                return list;

            });
            return await t;
        }

        public IEnumerable<DBObject<int>> GetEntities(TableName tname, DatabaseObjectCreator<int> creator)
        {
            var command = CreateCommand("select", tname);
            var reader = command.ExecuteReader();
            List<DBObject<int>> list = new List<DBObject<int>>();
            List<string> names = new List<string>();
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                names.Add(reader.GetName(i));
            }
            while (reader.Read())
            {
                foreach (var el in names)
                {
                    parameters[el] = reader[el];
                }
                list.Add(creator.CreateDatabaseObject(new DBObject<int>() { Values = parameters }));
            }
            return list;
        }

        public DBF_Connector(string connectionString)
        {
            mainConnection = new OleDbConnection(connectionString);
            mainConnection.Open();
        }


        ~DBF_Connector()
        {
            Dispose();
        }
        public void Dispose()
        {
            mainConnection.Dispose();
        }
    }
}
