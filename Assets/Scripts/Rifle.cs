using SOCategory.Runtime;
using UnityEngine;

namespace SODataEditor
{
    [CreateAssetMenu(fileName = "Rifle", menuName = "SO/Rifle")]
    public class Rifle : ScriptableObject
    {
        public Category category;
    }
}
