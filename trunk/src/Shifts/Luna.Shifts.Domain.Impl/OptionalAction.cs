using System;
using System.Linq;
using Luna.Common;
using Luna.Shifts.Domain.Model;

namespace Luna.Shifts.Domain.Impl
{
    [Notificable]
    public class OptionalAction : ICustomAction
    {
        public OptionalAction(string resourceKey, IBatchAlterModel model, ICustomAction[] options)
        {
            ResourceKey = resourceKey; 
            AlterType = model.Category;
            Model = model;
            Options = options;
            SelectedItem = Options.FirstOrDefault();
        }

        public ICustomAction SelectedItem { get; set; }

        public ICustomAction[] Options { get; set; }

        public string AlterType { get; private set; }

        public string ResourceKey { get; set; }
        public object Model { get; set; }
        public MulticastDelegate Action
        {
            get
            {
                if (SelectedItem != null)
                    return SelectedItem.Action;
                return default(MulticastDelegate);
            }
        }

        public void Dispose()
        {
            Model = null;
            foreach (var item in Options)
                item.Dispose();
            SelectedItem.Dispose();
            SelectedItem = null;
        }
    }

    public class CustomAction : ICustomAction
    {
        public CustomAction(string resourceKey, IBatchAlterModel model, Action<Term, TimeBox> action)
        {
            ResourceKey = resourceKey;
            AlterType = model.Category;

            Model = model;
            Action = action;
        }

        public string AlterType { get; private set; }

        public string ResourceKey { get; set; }
        public object Model { get; set; }
        public MulticastDelegate Action { get; set; }

        public void Dispose()
        {
            Model = null;
            Action = null;
        }
    }
}