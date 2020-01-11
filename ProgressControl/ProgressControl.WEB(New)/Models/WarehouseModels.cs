using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using ProgressControl.DAL.Entities;
using Newtonsoft.Json;
using PagedList;
namespace ProgressControl.WEB_New_.Models
{
    public class Smt_BoxViewModel
    {
        [JsonProperty("ElCode")]
        public int ElCode { get; set; }
        [JsonProperty("BoxCode")]
        public int BoxCode { get; set; }
        public string FullCode { get => ElCode + " " + BoxCode; }
        public string ElementName { get; set; }

        [JsonProperty("BoxQuantity")]
        public int BoxQuantity { get; set; }

        public int Spent { get; set; }
        public int CurrentQuantity { get => BoxQuantity - Spent; }
        public DateTime CreationTime { get; set; }

    }
    public class Smt_ViewBoxModelChange
    {
        public string FullCode { get; set; }
        public int BoxQuantity { get; set; }
    }

    public class ElementViewModel
    {
        public int ElCode { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }

        public ICollection<ElementViewModel> Analogs { get; set; }
        public ElementViewModel()
        {
            Analogs = new List<ElementViewModel>();
        }



        public static implicit operator ElementViewModel(Element e)
        {

            var tmp = new ElementViewModel()
            {
                ElCode = e.Code,
                Name = e.Name,
                Quantity=e.Quantity
            };

            if (e.Parents.Count  > 0)
                e.Parents.ToList().ForEach(p => {
                    tmp.Analogs.Add(new ElementViewModel() { ElCode = p.NavPropTwo.Code, Name = p.NavPropTwo.Name, Quantity = p.NavPropTwo.Quantity });
                    });

            if(e.Childrens.Count > 0)
                e.Childrens.ToList().ForEach(c => 
                {
                    tmp.Analogs.Add(new ElementViewModel() { ElCode = c.NavProp.Code, Name = c.NavProp.Name, Quantity = c.NavProp.Quantity});

                });

            return tmp;
        }
    }


    public class WarehouseTaskView
    {
        public int Id { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastPauseTime { get; set; }
        public DateTime LastStartTime { get; set; }
        public DateTime CompleteTime { get; set; }
        public string State { get; set; }
        public string SpcName { get; set; }
        public ICollection<(int, string, int)> Need { get; set; } = new List<(int, string, int)>();




        public static implicit operator WarehouseTaskView(WarehouseTask tsk)
        {
            var Need = tsk.GetNeed().ToList();
            var newView = new WarehouseTaskView() { CompleteTime = tsk.CompleteTime,
                CreationTime = tsk.CreationTime,
                Id = tsk.Code,
                LastPauseTime = tsk.LastPauseTime,
                LastStartTime = tsk.LastStartTime,
                SpcName = tsk.Subtask.Specification.Name,
                State = tsk.WorkState.ToString()
            };
            Need.ForEach(x=> newView.Need.Add(x));
            return newView;
        }
    }

}