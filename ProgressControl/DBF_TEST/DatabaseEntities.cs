using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBF_TEST
{

    #region DatabaseObjectsAbstract
    public class DBObject<TKey>
    {
        public virtual TKey Code { get; set; }
        public Dictionary<string, dynamic> Values{ get; set;}
    }

    public abstract class OneReferenceEntity<TRef, TKey> : DBObject<TKey>
        where TRef : class
    {
        public virtual TRef NavProp { get; set; }
    }

    public abstract class RefCollectionEntity<TRef, TKey> : DBObject<TKey>
        where TRef : class
    {
        public virtual ICollection<TRef> Collection { get; set;}
    }
    public abstract class TwoRefCollectionEntity<TRef, TRefTwo, TKey> : RefCollectionEntity<TRef,TKey>
        where TRef: class where TRefTwo: class
    {
        public virtual ICollection<TRefTwo> CollectionTwo { get; set; }
    }
    #endregion
    #region ManyToManyRelationsAbstract
    public abstract class ManyToManySelfRelation<NPropOne, Tkey> : DBObject<Tkey>
        where NPropOne : class
    {
        public virtual NPropOne NavProp { get; set; }
    }
    public abstract class ManyToManyRelation<NPropOne, NPropTwo, TKey, TKeyTwo> : ManyToManySelfRelation<NPropOne, TKey>
        where NPropOne : class
        where NPropTwo : class
    {
        public abstract TKeyTwo CodeTwo { get; set; }
        public virtual NPropTwo NavPropTwo { get; set; }
    }
    #endregion
}
