using System;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using System.Diagnostics;
namespace DBF_TEST
{
    public enum TableName
    {
        analogs,
        elements,
        spc,
        spc_el
    };

    public class DBF_Connector : IDisposable
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

        public void BackgroundTask()
        {
            var spc =  GetEntitiesAsync(TableName.spc, new SpecificationCreator());
            var elements = GetEntitiesAsync(TableName.elements, new ElementCreator());
            var analogs = GetEntitiesAsync(TableName.analogs, new AnalogsCreator());
            var quantities = GetEntitiesAsync(TableName.spc_el, new ElementQuantityCreator());
            Task.WaitAll(spc, elements, analogs, quantities);
            var spc_ie = spc.Result.Cast<Specification>();
            var elements_ie = elements.Result.Cast<Element>();
            var analogs_ie = analogs.Result.Cast<Analogs>();
            var quantities_ie = quantities.Result.Cast<ElementQuantity>();
            NavigateEntities(spc_ie, elements_ie, analogs_ie, quantities_ie);
            ImportData(spc_ie, elements_ie, new MyContext());
        }


        public async void ImportData(IEnumerable<Specification> spc , IEnumerable<Element> elements, MyContext cnt)
        {
            foreach (var el in elements)
            {
                cnt.Elements.Add(el);
            }
            int x = await cnt.SaveChangesAsync();
        }

        public void NavigateEntities(IEnumerable<Specification> spc , IEnumerable<Element> elements, IEnumerable<Analogs> analogs, IEnumerable<ElementQuantity> quantities)
        {
            foreach(var el in spc)
            {
                el.Collection = quantities.Where(x => x.CodeTwo == el.Code).ToList();
            }
            foreach (var el in elements)
            {
                el.Collection = quantities.Where(x => x.Code == el.Code).ToList();
                el.CollectionTwo = analogs.Where(x => x.Code == el.Code).ToList();
            }
            foreach (var el in analogs)
            {
                el.NavProp = elements.FirstOrDefault(x => x.Code == el.ACode);
                if(el.NavProp == null)
                {

                }
            }
            foreach (var el in quantities)
            {
                el.NavProp = elements.FirstOrDefault(x => x.Code == el.Code);
                el.NavPropTwo = spc.FirstOrDefault(x => x.Code == el.CodeTwo);
            }
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
                try
                {
                    list.Add(creator.CreateDatabaseObject(new DBObject<int>() { Values = parameters }));
                }
                catch (InvalidCastException e) { Debug.WriteLine($"{e.Message} {e.Source}");}
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
