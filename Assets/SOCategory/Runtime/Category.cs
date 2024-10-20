using System.Collections.Generic;
using UnityEngine;

namespace SOCategory.Runtime
{
    public class Category : ScriptableObject
    {
        [field: SerializeField] public Category ParentCategory { get; private set; }
        [field: SerializeField] public List<Category> SubCategories { get; private set; } = new List<Category>();

        public int ID => GetInstanceID();
        public bool HasSubCategories => SubCategories != null && SubCategories.Count > 0;

        public void SetParentCategory(Category _parent) => ParentCategory = _parent;
    }
}
