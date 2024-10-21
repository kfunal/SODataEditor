using SOCategory.Runtime;
using UnityEngine;

namespace SODataEditor
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "SO/Weapon")]
    public class Weapon : ScriptableObject
    {
        public Category category;
    }
}
