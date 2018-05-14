using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityStruct
{
    public class EntityClone
    {
        struct DictionaryEntity
        {
            public Type entityType;
            public PropertyInfo[] entityProperties;
        }

        private static List<DictionaryEntity> propertyDictionaries = new List<DictionaryEntity>();

        public static PropertyInfo[] GetProperties(Type entityType)
        {
            PropertyInfo[] result = null;
            foreach (var dic in propertyDictionaries.Where(p => p.entityType == entityType))
                result = dic.entityProperties;
            if (result == null)
            {
                result = entityType.GetProperties();
                propertyDictionaries.Add(
                    new DictionaryEntity()
                    {
                        entityType = entityType,
                        entityProperties = result
                    });
            }
            return result;
        }

        public static void CloneProperties<Entity>(Entity from, Entity to)
        {
            var typeofentity = typeof(Entity);
            PropertyInfo[] properties = EntityClone.GetProperties(typeof(Entity));
            foreach (PropertyInfo property in properties)
            {
                property.SetValue(to, property.GetValue(from));
            }
        }
    }
}
