using System;

namespace Luna.Common.Extensions
{
    public static class EntityExt
    {
        public static bool BaseTypeIsEntity(this Type type)
        {
            var baseType = type.BaseType;

            while (baseType != null)
            {
                if (baseType == typeof(Entity)) return true;
                baseType = baseType.BaseType;
            }
            return false;
        }

        public static bool IsNew<T>(this object obj)
        {
            var entity = obj as AbstractEntity<T>;
            if(entity == null)
                throw new Exception(string.Format("obj {0} is not AbstractEntity type",obj.GetType()));
            return IsNew(entity);

        }

        public static bool IsNew(this Entity entity)
        {
            return entity.Id == Guid.Empty;
        }

        public static bool IsNew<T>(this AbstractEntity<T> entity)
        {
            return entity.Id.Equals(default(T));
        }

        public static void SetIsEditing(this object entity, bool value)
        {
            var obj = entity as IEditing;
            if (obj == null) return;

            obj.IsEditing = value;
        }
    }
}
