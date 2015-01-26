using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panasia.Gemini.Web.Models.Helpers
{
    public class RecordItem
    {
        public RecordItem()
        {
        }

        public RecordItem(params string[] fields)
        {
            ResetFields(fields);
        }

        public void ResetFields(params string[] fields)
        { 
            if (fields!= null)
            {                
                foreach (var f in fields)
                {
                    AddField(f);  
                }
            }
        }

        #region property Fields 
        private Dictionary<string, RecordField> _Fields = new Dictionary<string, RecordField>(); 
        public IEnumerable<RecordField> Fields
        {
            get
            {



                return _Fields.Values; 
            }
        }
        #endregion //property Fields

        public RecordField GetField(string name)
        {
            RecordField f = null;
            _Fields.TryGetValue(name.ToUpper(), out f);
            return f;
        }

        public void AddField(RecordField field)
        {
            string uName = field.Name.ToUpper();
            var item = GetField(uName);
            if (item == null)
            {
                _Fields.Add(uName, field);
            }
            else
            {
                item.Value = field.Value;
            }
        }

        public void AddField(string name)
        { 
            string uName = name.ToUpper();
            var item = GetField(uName);
            if (item == null)
            {
                _Fields.Add(uName, new RecordField { Name = uName});
            }     
        }

        public void AddField(string name, object value)
        {
            SetFieldValue(name, value);
        }

        protected virtual void SetFieldValue(string name, object value)
        { 
            string uName = name.ToUpper();
            var item = GetField(uName);
            if (item == null)
            {
                _Fields.Add(uName, new RecordField { Name = uName, Value = value });
            }
            else
            {
                item.Value = value;
            }
        }

        public object this[string name]
        {
            get
            {
                var field = GetField(name); 
                return field == null ? null : field.Value;
            }
            set
            {
                AddField(name, value); 
            }
        }


        public object GetDBValue(string name)
        {
            var field = GetField(name);
            return field == null ? DBNull.Value : field.Value == null ? DBNull.Value : field.Value;
        }
         
 
    }

    public class RecordField
    {
        public string Name { get; set; }

        public object Value { get; set; }
    }
}
