using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Luna.Common
{
    public class ReflectClone<T>
    {
        private Dictionary<T, Dictionary<PropertyInfo, List<object>>> obj;

        public void CloneObj(object entity, IEnumerable<T> array)
        {
            Dictionary<T, Dictionary<PropertyInfo, List<object>>> list = new Dictionary<T, Dictionary<PropertyInfo, List<object>>>();

            foreach (var item in array)
            {
                var curentItem = (T)item;
                Dictionary<PropertyInfo, List<object>> obj = new Dictionary<PropertyInfo, List<object>>();
                Backup(entity, obj);
                list.Add(curentItem, obj);
            }
           
            this.obj = list;
        }

        private void Backup(object entity, Dictionary<PropertyInfo, List<object>> obj)
        {
            IEnumerable<PropertyInfo> propertyInfos = entity.GetType().
                        GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanWrite && !p.PropertyType.IsValueType);
            if (propertyInfos.Count() > 0)
            {
                foreach (var item in propertyInfos)
                {
                    var val = item.GetValue(entity, null);

                    if (val != null)
                    {
                        IEnumerable<PropertyInfo> propertyInfos2 = val.GetType().
                          GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite);
                        foreach (var item2 in propertyInfos2)
                        {
                            if (!obj.ContainsKey(item2))
                            {
                                var hash = new List<object>();
                                hash.Add(val);
                                hash.Add(item2.GetValue(val, null));
                                obj.Add(item2, hash);
                            }
                        }
                    }
                }
            }
        }

        public void ReStore(T entity,Action<string,object> action)
        {
            foreach (var item in obj[entity])
            {
                try
                {
                    item.Key.SetValue(item.Value[0], item.Value[1], null);
                    action(item.Key.Name,item.Value[0]);
                    //LaborRuleEntity entity = (item.Value[0] as LaborRuleEntity);
                    //if (entity != null)
                    //    entity.NotifyPropertyChanged(item.Key.Name);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
