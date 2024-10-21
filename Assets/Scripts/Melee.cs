using SOCategory.Runtime;
using UnityEngine;

namespace SODataEditor
{
    [CreateAssetMenu(fileName = "Melee", menuName = "SO/Melee")]
    public class Melee : ScriptableObject
    {
        public Category category;
    }
}
