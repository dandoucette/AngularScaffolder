using System.Collections.Generic;
using AngularScaffolder.Extensions;

namespace AngularScaffolder.Objects
{
    public class PropertyInfo
    {
        public string Name { get; set; }
        public string CamelCaseName { get; set; }

        private string _dataType;
        public string DataType
        {
            get
            {
                if (_dataType.Contains("int") || _dataType.Contains("decimal"))
                {
                    return "number";
                }
                else if (_dataType.Contains("bool"))
                {
                    return "boolean";
                }
                else if (_dataType.ToLower().Contains("date"))
                {
                    return "Date";
                }
                else
                {
                    return "string";
                }
            }
            set
            {
                _dataType = value;
            }
        }

        public string NameWithSpaces
        {
            get
            {
                var words = Name.ToWords();

                return string.Join(" ", words);
            }
        }
    }
}
