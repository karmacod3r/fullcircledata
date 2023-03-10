using UnityEngine;

namespace FullCircleData.Attributes
{
    public class AutoDropdownAttribute : PropertyAttribute
    {
        public string listPropertyPath;
        public string valuePropertyPath;
        public bool allowNone;

        public AutoDropdownAttribute(string listPropertyPath, string valuePropertyPath, bool allowNone = false)
        {
            this.listPropertyPath = listPropertyPath;
            this.valuePropertyPath = valuePropertyPath;
            this.allowNone = allowNone;
        }
        
        public AutoDropdownAttribute(string listPropertyPath, bool allowNone)
        {
            this.listPropertyPath = listPropertyPath;
            this.allowNone = allowNone;
        }
        
        public AutoDropdownAttribute(string listPropertyPath)
        {
            this.listPropertyPath = listPropertyPath;
        }
    }
}