using SOCategory.Runtime;
using UnityEngine;

namespace SODataEditor
{
    [CreateAssetMenu(fileName = "Food", menuName = "SO/Food")]
    public class Food : ScriptableObject
    {
        public Category category;
    }
}