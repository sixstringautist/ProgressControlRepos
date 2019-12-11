using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
namespace DBF_TEST
{
    class Progpam
    {
        static void Main(string[] args)
        {
            DBF_Connector obj = new DBF_Connector(ConfigurationManager.ConnectionStrings["dbfConnection"].ConnectionString);
            //var elements = obj.GetEntities(TableName.elements,new ElementCreator()).Cast<Element>();
            //var spc_el = obj.GetEntities(TableName.spc_el, new ElementQuantityCreator()).Cast<ElementQuantity>();
            //var spc = obj.GetEntities(TableName.spc, new SpecificationCreator()).Cast<Specification>();
            //var NotFindedCodes = new Dictionary<int, List<int>>();

            //foreach (var el in spc)
            //{
            //    el.Collection = spc_el.Where(x => x.CodeTwo == el.Code).ToList();
            //    var tmp = new List<int>();
            //    foreach (var _el in el.Collection)
            //    {
            //        if (elements.FirstOrDefault(x => x.Code == _el.Code) == null)
            //        {
            //            tmp.Add(_el.Code);
            //        }
            //    }
            //    NotFindedCodes.Add(el.Code, tmp);
            //}
            //using (var writer = new StreamWriter("notFinded.txt"))
            //{
            //    foreach (var el in NotFindedCodes)
            //    {
            //        string spc_line = "Код спецификации: ";
            //        string spc_code = $"{el.Key}".PadLeft(9, '0')+ "\n";
            //        string el_code = "";
            //        foreach (var el1 in el.Value)
            //        {
            //            el_code += $"{el1}\t".PadLeft(14, '0');
            //        }
            //        el_code.Trim('-');
            //        string Line = spc_line + spc_code + "Коды не найденых элементов: " + el_code;
            //        writer.WriteLine(Line);
            //    }

            //}
            obj.BackgroundTask();



                Console.ReadKey();

        }
    }
}
