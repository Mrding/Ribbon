using System;
using System.Reflection;

public static class Mutant
{
    public static void MAssign(this object target, object source)
    {
        foreach (PropertyInfo pi1 in source.GetType().GetProperties())
        {
            if (!pi1.CanRead) continue;

            PropertyInfo pi2 = target.GetType().GetProperty(pi1.Name, pi1.PropertyType);

            if (null == pi2 || !pi2.CanWrite) continue;

            pi2.SetValue(target, pi1.GetValue(source, null), null);
        }
    }
}