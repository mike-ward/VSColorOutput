using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BlueOnionSoftware
{
    public class PropertySorter : ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var pdc = TypeDescriptor.GetProperties(value, attributes);
            var orderedProperties = new List<PropertyOrderPair>();
            foreach (PropertyDescriptor pd in pdc)
            {
                var attribute = pd.Attributes[typeof(PropertyOrderAttribute)];
                if (attribute != null)
                {
                    var poa = (PropertyOrderAttribute)attribute;
                    orderedProperties.Add(new PropertyOrderPair(pd.Name, poa.Order));
                }
                else
                {
                    orderedProperties.Add(new PropertyOrderPair(pd.Name, 0));
                }
            }
            var propertyNames = orderedProperties.OrderBy(o => o).Select(pop => pop.Name).ToArray();
            return pdc.Sort(propertyNames);
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyOrderAttribute : Attribute
    {
        public PropertyOrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; }
    }

    public class PropertyOrderPair : IComparable
    {
        private readonly int _order;

        public string Name { get; }

        public PropertyOrderPair(string name, int order)
        {
            _order = order;
            Name = name;
        }

        public int CompareTo(object obj)
        {
            var otherOrder = ((PropertyOrderPair)obj)._order;
            if (otherOrder == _order)
            {
                var otherName = ((PropertyOrderPair)obj).Name;
                return string.CompareOrdinal(Name, otherName);
            }
            return otherOrder > _order ? -1 : 1;
        }
    }
}