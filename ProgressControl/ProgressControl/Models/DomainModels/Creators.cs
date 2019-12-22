using System;
using System.Diagnostics;

namespace DomainModels
{
    #region AbstcractCreators
    public abstract class DatabaseObjectCreator<TKey>
    {
        public abstract DBObject<TKey> CreateDatabaseObject(DBObject<TKey> obj);
    }
    #region EntityCreators

    public abstract class EntityCreator<TResult,TKey> : DatabaseObjectCreator<TKey> where TResult: class
    {
        public override DBObject<TKey> CreateDatabaseObject(DBObject<TKey> obj)
        {
            return CreateEntity<TResult>(obj);
        }
        protected abstract DBObject<TKey> CreateEntity<T>(DBObject<TKey> obj);
    }
    public abstract class OneReferenceEntityCreator<TResult, TKey> : EntityCreator<TResult, TKey> where TResult:class
    {
        protected override DBObject<TKey> CreateEntity<T>(DBObject<TKey> obj)
        {
            return CreateOneReferenceEntity(obj);
        }
        protected abstract DBObject<TKey> CreateOneReferenceEntity(DBObject<TKey> obj);
    }
    public abstract class CreatRefCollectionEntity<TResult, TKey> : EntityCreator<TResult, TKey> where TResult : class
    {
        protected override DBObject<TKey> CreateEntity<T>(DBObject<TKey> obj)
        {
            return CreateReferenceCollectionEntity(obj);
        }
        protected abstract DBObject<TKey> CreateReferenceCollectionEntity(DBObject<TKey> obj);
    }
    public abstract class TwoReferenceEntityCreator<TResult, TKey> : EntityCreator<TResult, TKey> where TResult : class
    {
        protected override DBObject<TKey> CreateEntity<T>(DBObject<TKey> obj)
        {
            return CreateTwoReferenceEntity(obj);
        }
        protected abstract DBObject<TKey> CreateTwoReferenceEntity(DBObject<TKey> obj);
        
    }
    #endregion
    #region RelationCreators
    public abstract class CreateRelation<TRel, TKey> : DatabaseObjectCreator<TKey> where TRel : class
    {
        public override DBObject<TKey> CreateDatabaseObject(DBObject<TKey> obj)
        {
            return CreateRel<TRel>(obj);
        }

        protected abstract DBObject<TKey> CreateRel<TResult>(DBObject<TKey> obj);
    }
    public abstract class CreateManyToMany<NPropOne, NPropTwo, TKey, TKeyTwo> : CreateRelation<ManyToManyRelation<NPropOne, NPropTwo, TKey, TKeyTwo>, TKey>
        where NPropOne : class where NPropTwo : class
    {
        protected override DBObject<TKey> CreateRel<TResult>(DBObject<TKey> obj)
        {
            return CreateManyToManyRel<ManyToManyRelation<NPropOne, NPropTwo, TKey, TKeyTwo>>(obj);
        }
        protected abstract DBObject<TKey> CreateManyToManyRel<TResult>(DBObject<TKey> obj);
    }
    #endregion

    #endregion
    #region ConcreteCreators
    #region EntityCreators
   public class SpecificationCreator : OneReferenceEntityCreator<Specification, int>
    {
        protected override DBObject<int> CreateOneReferenceEntity(DBObject<int> obj)
        {
            if (!(int.TryParse(obj.Values["code"], out int val)) || obj.Values["date"].GetType() != typeof(DateTime) || obj.Values["name"].GetType() != typeof(string))
                throw new InvalidCastException("Invalid parameter type") { Source = this.GetType().Name };
            
            return new Specification()
            { Code = int.Parse(obj.Values["code"]),
                Date = obj.Values["date"],
                Name = obj.Values["name"]
            };
        }
    }
   public class ElementCreator : TwoReferenceEntityCreator<Element, int>
    {
        protected override DBObject<int> CreateTwoReferenceEntity(DBObject<int> obj)
        {
            if (!(int.TryParse(obj.Values["code"], out int val)) || !(obj.Values["name"].GetType() == typeof(string)) || !(obj.Values["quantity"] is Decimal))
                throw new InvalidCastException("Invalid parameter type") { Source = this.GetType().Name };

            return new Element()
            {
                Code = int.Parse(obj.Values["code"]),
                Name = obj.Values["name"],
                Quantity = Decimal.ToInt32(obj.Values["quantity"]),
                Un = obj.Values["un"].Trim(' ')
            };
        }

    }
    #endregion
    #region RelationCreators
    public class AnalogsCreator : CreateManyToMany<Element, Element,int,int>
    {
        protected override DBObject<int> CreateManyToManyRel<TResult>(DBObject<int> obj)
        {
            if (!(int.TryParse(obj.Values["code"], out int val)) || !(int.TryParse(obj.Values["acode"], out int val1)))
                throw new InvalidCastException("Invalid parameter type") { Source = this.GetType().Name };

            return new Analog()
            {
                Code = int.Parse(obj.Values["code"]),
                CodeTwo = int.Parse(obj.Values["acode"]),
                NavProp = null,
                NavPropTwo = null,
                AnalogPriority = Decimal.ToInt32(obj.Values["prior"])
            };
        }
    }
    public class ElementQuantityCreator : CreateManyToMany<Element, Specification, int, int>
    {

        protected override DBObject<int> CreateManyToManyRel<TResult>(DBObject<int> obj)
        {
            if (!(int.TryParse(obj.Values["code"], out int val)) ||
                !(int.TryParse(obj.Values["spcode"], out int val1)) ||
                !(obj.Values["quantity"] is Decimal))
                throw new InvalidCastException("Invalid parameter type") { Source = this.GetType().Name };

            return new ElementQuantity()
            {
                Code =int.Parse(obj.Values["code"]),
                CodeTwo = int.Parse(obj.Values["spcode"]),
                Quantity = Decimal.ToInt32(obj.Values["quantity"]),
                NavProp = null,
                NavPropTwo = null
            };
        }
    }

    #endregion
    #endregion
}
