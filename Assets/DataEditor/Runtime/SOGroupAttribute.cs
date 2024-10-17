using System;
using DataEditor.Runtime.Enums;

namespace DataEditor.Runtime
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class SOGroupAttribute : Attribute
    {
        public SOGroups Group { get; private set; }
        public SOCategory Category { get; private set; }

        public SOGroupAttribute(SOGroups group, SOCategory category)
        {
            Group = group;
            Category = category;
        }
    }
}
