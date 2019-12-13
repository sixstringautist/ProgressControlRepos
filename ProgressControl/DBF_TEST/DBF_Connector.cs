using System;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using System.Diagnostics;
using System.Data.Common;
using System.Data.Entity;
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
        DbConnection mainConnection;
        MyContext cnt;
        public string DefaultConnection { get; set; }
        public string ConnectionString { get => mainConnection.ConnectionString;}

        private DbCommand CreateCommand(string command, TableName tname, params string[] colname)
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
            var spc = GetEntitiesAsync(TableName.spc, new SpecificationCreator());
            var elements = GetEntitiesAsync(TableName.elements, new ElementCreator());
            var analogs = GetEntitiesAsync(TableName.analogs, new AnalogsCreator());
            var quantities = GetEntitiesAsync(TableName.spc_el, new ElementQuantityCreator());
            Task.WaitAll(spc, elements, analogs, quantities);
            var spc_ie = spc.Result.Cast<Specification>();
            var elements_ie = elements.Result.Cast<Element>();
            var analogs_ie = analogs.Result.Cast<Analog>();
            var el = analogs_ie.Where(x => x.Code == 8);
            var quantities_ie = quantities.Result.Cast<ElementQuantity>();
            NavigateEntities(spc_ie, elements_ie, analogs_ie, quantities_ie);
            ImportData(spc_ie, elements_ie,analogs_ie, quantities_ie);
        }


        public void ImportData(IEnumerable<Specification> spc, IEnumerable<Element> elements, IEnumerable<Analog> analogs, IEnumerable<ElementQuantity> quantities)
        {
            cnt.Database.Log = s => Debug.WriteLine(s);
            foreach (var el in elements)
            {
                var existing = cnt.Elements.Find(el.Code);
                if (existing == null)
                {
                    cnt.Elements.Add(el);
                }
                else
                {
                    cnt.Entry(existing).CurrentValues.SetValues(el);
                }
            }
            foreach (var el in spc)
            {
                var existing = cnt.Specifications.Find(el.Code);
                if (existing == null)
                {
                    cnt.Specifications.Add(el);
                }
                else
                {
                    cnt.Entry(existing).CurrentValues.SetValues(el);
                }
            }
            spc = null;
            elements = null;
            cnt.SaveChanges();
        }

        public void NavigateEntities(IEnumerable<Specification> spc, IEnumerable<Element> elements, IEnumerable<Analog> analogs, IEnumerable<ElementQuantity> quantities)
        {

            analogs = analogs.Except(analogs.Where(x => x.Code == x.CodeTwo));
            foreach (var el in elements)
            {
                el.Collection = quantities.Where(x => x.Code == el.Code).ToList();
                el.Parents = analogs.Where(a => a.CodeTwo == el.Code).ToList();
                el.Childrens = analogs.Where(a => a.Code == el.Code).ToList();
            }
            foreach (var el in spc)
            {
                el.Collection = quantities.Where(x => x.CodeTwo == el.Code).ToList();
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
                catch (InvalidCastException e) { Debug.WriteLine($"{e.Message} {e.Source}"); }
            }
            return list;
        }

        public DBF_Connector(string connectionString)
        {
            mainConnection = new OleDbConnection(connectionString);
            mainConnection.Open();
            cnt = new MyContext();
        }

        public DBF_Connector() : this(@"Provider=VFPOLEDB.1;Data Source = H:\ДИПЛОМНАЯ РАБОТА\SMT;User ID = admin")
        {
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (mainConnection != null)
                { mainConnection.Close(); mainConnection = null; }
                if (cnt != null)
                { cnt.Dispose(); cnt = null; }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
