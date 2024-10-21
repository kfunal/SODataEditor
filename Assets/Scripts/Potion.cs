using SOCategory.Runtime;
using UnityEngine;

namespace SODataEditor
{
    [CreateAssetMenu(fileName = "Potion", menuName = "SO/Potion")]
    public class Potion : ScriptableObject
    {
        public Category category;
    }
}
