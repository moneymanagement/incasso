using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incasso
{
    public abstract class StringCatalogBase
    {
        /// <summary>
        /// Name is immutatable, while the value can be changed. Therefore when using or storing in DB always use Name property of the AllValuesSelectable
        /// </summary>
        /// 
        [JsonIgnore]
        public TextValuePair[] TextValues => _textValues;
        [JsonIgnore]
        public string[] Values => _values;

        private string[] _values { get; set; }
        private TextValuePair[] _textValues { get; set; }
        public string Value { get; set; }
        public string Text
        {
            get
            {
                if (_textValues == null || string.IsNullOrEmpty(Value) || _values.Where(x => x == Value).Count() == 0)
                    return string.Empty;
                else return _textValues.Where(x => x.Value == Value).FirstOrDefault().Text;
            }
        }
        public StringCatalogBase()
        {
            GenerateAllValues(this.GetType());
            Value = string.Empty;
        }
        public StringCatalogBase(string initialValue)
        {
            GenerateAllValues(this.GetType());
            if (!string.IsNullOrEmpty(initialValue))
                ValidateValue(initialValue, this.GetType());
            Value = initialValue;
        }

        private void GenerateAllValues(Type t)
        {
            Type stringCatalogBaseClass = typeof(StringCatalogBase);
            var nvp = new List<TextValuePair>();
            var nameList = new List<string>();
            var fields = t.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).ToList();
            Type baseType = t.BaseType;
            while (baseType != stringCatalogBaseClass)
            {
                fields.AddRange(baseType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).ToList());
                baseType = baseType.BaseType;
            }
            foreach (var field in fields)
            {
                if (field.FieldType != typeof(string))
                    continue;
                string fieldText = (string)field.GetValue(field);
                string fieldName = field.Name;
                TextValuePair nvpItem = new TextValuePair(fieldText, fieldName);
                nvp.Add(nvpItem);
                nameList.Add(fieldName);
            }
            _textValues = nvp.ToArray();
            _values = nameList.ToArray();
        }
        public void ValidateValue(string stringValue, Type t)
        {
            if (_values.Where(x => x == stringValue).Count() == 0)
                throw new InvalidCastException($"Cannot find appropriate value of {stringValue} in catalog {t.Name}");
        }
    }
}
