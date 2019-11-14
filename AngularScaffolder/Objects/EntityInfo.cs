using System.Collections.Generic;
using System.Linq;
using AngularScaffolder.Extensions;

namespace AngularScaffolder.Objects
{
    public class EntityInfo
    {
        public EntityInfo()
        {
            Properties = new List<PropertyInfo>();
        }
        public string ClassName { get; set; }

        public string AngularModelName
        {
            get
            {
                var words = ClassName.ToWords();
                return string.Join("-", words.Select(w => w.ToLower()));
            }
        }
        public List<PropertyInfo> Properties { get; set; }

    }
}
