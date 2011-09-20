namespace Luna.WPF.ApplicationFramework.Validations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections;
    using System.Windows;
    using System.Windows.Data;

    public class ObjectCollectionValidator : Validator
    {
        public override bool InnerValidate()
        {
            ErrorMessage = string.Format("{0}重複,請重新輸入", Property);
            if (Collection == null) return false;
            if(itemValues==null)
            itemValues=new List<object>();
            
            var count = Collection.Cast<object>().Count(e => GetCurrentItemValue(e));
            selfCount = 0;
            if (!EnableEmptyItem && _markEmpty)
            {
                ErrorMessage = "列表中存在空值,請檢查";
                _markEmpty=false;
                return false;
            }
            var validate = (count == 0 && itemValues.Count == itemValues.Distinct().Count());
            itemValues.Clear();
            return validate;
        }

        protected void BindingItem(object item)
        {
            Binding bind = new Binding(MatchProperty);
            bind.Source = item;
            //"A,B,C"
            //当A是自己项的时候,且是A或者不匹配其他项的时候通过
            SetBinding(ItemValueProperty, bind);
            itemValues.Add(ItemValue);
        }
        private IList<object> itemValues;

        public bool EnableEmptyItem
        {
            get { return (bool)GetValue(EnableEmptyItemProperty); }
            set { SetValue(EnableEmptyItemProperty, value); }
        }

       
        public static readonly DependencyProperty EnableEmptyItemProperty =
            DependencyProperty.Register("EnableEmptyItem", typeof(bool), typeof(ObjectCollectionValidator), 
            new UIPropertyMetadata(true));

        private bool _markEmpty;

        protected virtual bool GetCurrentItemValue(object item)
        {
            BindingItem(item);
            if (!EnableEmptyItem)
            {
                if (ItemValue == null)
                    _markEmpty=true;
                else if(string.IsNullOrEmpty(ItemValue.ToString()))
                    _markEmpty=true;
            }
            switch (MatchMode)
            {
                case MatchMode.Only:
                    return ItemValue.Equals(Property);
                    break;
                case MatchMode.ExceptSelf:
                    var isSelf = ItemValue.Equals(CurrentItemValue);
                    if (isSelf) selfCount++;
                    return !isSelf && ItemValue.Equals(Property) || selfCount > 1;
                    break;
            }
            return true;
            //ItemValue.Equals(Property) && ItemValue.Equals(CurrentItemValue)
        }

        private int selfCount = 0;

        public object ItemValue
        {
            get { return (object)GetValue(ItemValueProperty); }
            set { SetValue(ItemValueProperty, value); }
        }

        public static readonly DependencyProperty ItemValueProperty =
            DependencyProperty.Register("ItemValue", typeof(object), typeof(ObjectCollectionValidator), 
            new UIPropertyMetadata(null));


        /// <summary>
        /// 当前项的属性,用于匹配自身
        /// </summary>
        public object CurrentItemValue
        {
            get { return (object)GetValue(CurrentItemValueProperty); }
            set { SetValue(CurrentItemValueProperty, value); }
        }


        public static readonly DependencyProperty CurrentItemValueProperty =
            DependencyProperty.Register("CurrentItemValue", typeof(object), typeof(ObjectCollectionValidator), 
            new UIPropertyMetadata(null));



        /// <summary>
        /// 要验证的集合
        /// </summary>
        public IEnumerable Collection
        {
            get { return (IEnumerable)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register("Collection", typeof(IEnumerable), typeof(ObjectCollectionValidator), 
            new UIPropertyMetadata(null));

        /// <summary>
        /// 集合中对象的属性(可选)
        /// </summary>
        public string MatchProperty
        {
            get { return (string)GetValue(MatchPropertyProperty); }
            set { SetValue(MatchPropertyProperty, value); }
        }

        public static readonly DependencyProperty MatchPropertyProperty =
            DependencyProperty.Register("MatchProperty", typeof(string), typeof(ObjectCollectionValidator), 
            new UIPropertyMetadata(string.Empty));



        public MatchMode MatchMode
        {
            get { return (MatchMode)GetValue(MatchModeProperty); }
            set { SetValue(MatchModeProperty, value); }
        }

        
        public static readonly DependencyProperty MatchModeProperty =
            DependencyProperty.Register("MatchMode", typeof(MatchMode), typeof(ObjectCollectionValidator),
            new UIPropertyMetadata(MatchMode.ExceptSelf));





    }

    public enum MatchMode
    {
        /// <summary>
        /// 当前项未在集合中
        /// </summary>
        Only,
        /// <summary>
        /// 当前项在集合中
        /// </summary>
        ExceptSelf
    }
}
