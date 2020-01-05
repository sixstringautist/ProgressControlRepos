using System;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using System.Diagnostics;
using System.Data.Common;
using System.IO;
using ProgressControl.DAL.EF;
using ProgressControl.DAL.Interfaces;
namespace ProgressControl.DAL.Entities
{
    public enum TableName
    {
        analogs,
        elements,
        spc,
        spc_el
    };
    class ElementComparer : IEqualityComparer<Element>
    {
        public bool Equals(Element x, Element y)
        {
            if (x.Code == y.Code)
                return true;
            else return false;
        }

        public int GetHashCode(Element obj)
        {
            return obj.Code;
        }
    }
    class SpecificationComparer : IEqualityComparer<Specification>
    {
        public bool Equals(Specification x, Specification y)
        {
            if (x.Code == y.Code)
                return true;
            else return false;
        }

        public int GetHashCode(Specification obj)
        {
            return obj.Code;
        }
    }


    public class DBF_Connector : IDisposable, IBackGrounder
    {
        
        DbConnection mainConnection;
        RsContext cnt;
        public string DefaultConnection { get; set; }
        public string ConnectionString { get => mainConnection.ConnectionString;}
        public bool IsDisposed { get; private set; }

        public bool IsComplete { get; private set; }

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
            IsComplete = false;
            var spc = GetEntitiesAsync(TableName.spc, new SpecificationCreator());
            var elements = GetEntitiesAsync(TableName.elements, new ElementCreator());
            var analogs = GetEntitiesAsync(TableName.analogs, new AnalogsCreator());
            var quantities = GetEntitiesAsync(TableName.spc_el, new ElementQuantityCreator());
            Task.WaitAll(spc, elements, analogs, quantities);

            var spc_ie = spc.Result.Cast<Specification>().ToList();
            var elements_ie = elements.Result.Cast<Element>().ToList();
            var analogs_ie = analogs.Result.Cast<Analog>().ToList();
            var quantities_ie = quantities.Result.Cast<ElementQuantity>().ToList();
            NavigateEntities(spc_ie, elements_ie, analogs_ie, quantities_ie);

            var db_elements_to_delete = cnt.Elements.ToList().Except(elements_ie.Intersect(cnt.Elements.ToList(), new ElementComparer()).ToList(), new ElementComparer()).ToList();
            var db_spc_to_delete = cnt.Specifications.ToList().Except(spc_ie.Intersect(cnt.Specifications.ToList(), new SpecificationComparer()), new SpecificationComparer()).ToList();

            cnt.Elements.RemoveRange(db_elements_to_delete);
            cnt.Specifications.RemoveRange(db_spc_to_delete);
            cnt.SaveChanges();

            var elements_except = elements_ie.Except(cnt.Elements.ToList(),new ElementComparer()).ToList();
            var spc_except = spc_ie.Except(cnt.Specifications.ToList(), new SpecificationComparer()).ToList();

            ImportData(spc_except, elements_except);
            IsComplete = true;
        }


        public void ImportData(IEnumerable<Specification> spc, IEnumerable<Element> elements)
        {
            //cnt.Database.Log = delegate (string s)
            //{
            //    using (var writer = new StreamWriter("log.txt", append:true))
            //    {
            //        writer.WriteLine(s);
            //    }
            //};
            cnt.Elements.AddRange(elements);
            cnt.Specifications.AddRange(spc);
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

        public DBF_Connector(OleDbConnection conn, RsContext db)
        {
            mainConnection = conn;
            mainConnection.Open();
            cnt = db;
        }
        
        protected void Dispose(bool disposing)
        {
            IsDisposed = true;
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
