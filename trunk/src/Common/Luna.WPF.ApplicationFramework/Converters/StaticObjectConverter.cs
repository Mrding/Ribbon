using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Windows.Media;

namespace Luna.WPF.ApplicationFramework
{
    public class StaticObjectConverter
    {
        //public static WeakReference WeakStaticObject = new WeakReference(new Dictionary<Type, Dictionary<string, object>>());
        private static Dictionary<Type, Dictionary<string, object>> StaticObject = new Dictionary<Type, Dictionary<string, object>>();

        public static T GetObjectByPropertyName<T>(Type type,string name)
        {
           // var StaticObject = WeakStaticObject.Target as Dictionary<Type, Dictionary<string, object>>;
            InitStaticObj(type);
            return (T)StaticObject[type][name];
        }

        public static string GetNameByObj(Type type, object obj)
        {
            // var StaticObject = WeakStaticObject.Target as Dictionary<Type, Dictionary<string, object>>;
            InitStaticObj(type);
            return StaticObject[type].First(e => e.Value == obj).Key;
        }

        public static string GetNameByObj(Type type, object obj,string property)
        {
            // var StaticObject = WeakStaticObject.Target as Dictionary<Type, Dictionary<string, object>>;
            InitStaticObj(type);
            Color currentColor=(Color)obj;
            foreach (var item in StaticObject[type])
            {
                var info = item.Value.GetType().GetProperty(property);
                
                var color = (Color)info.GetValue(item.Value, null);
                if (color.R == currentColor.R && color.G==currentColor.G)
                {
                    return item.Key;
                }
            }
            return string.Empty;
            //return StaticObject[type].First(e => e.Value.GetType().GetProperty(property).GetValue(e.Value,null) == obj).Key;
        }

        public static IEnumerable GetObjectList(Type type)
        {
            // var StaticObject = WeakStaticObject.Target as Dictionary<Type, Dictionary<string, object>>;
            InitStaticObj(type);
            return StaticObject[type].Values.ToList();
        }

        private static void InitStaticObj(Type type)
        {
            if (!StaticObject.ContainsKey(type))
            {
                var infoList = type.GetProperties(BindingFlags.Static | BindingFlags.Public);
                Dictionary<string, object> dict = new Dictionary<string, object>(infoList.Length);
                foreach (var item in infoList)
                {
                    var obj = item.GetValue(null, null);
                    //ICloneable cloneObj= obj as ICloneable;
                    //if (cloneObj != null)
                    //{
                    //    obj = cloneObj.Clone();
                    //}
                    dict[item.Name] = obj;
                }
                StaticObject[type] = dict;
            }
        }
    }
}
