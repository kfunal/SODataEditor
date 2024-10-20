using UnityEngine.UIElements;

namespace SOCategory.CustomEditor.CustomUIElements
{
    public class CustomHelpBox : HelpBox
    {
        public new class UxmlFactory : UxmlFactory<CustomHelpBox, UxmlTraits> { }
        public CustomHelpBox() { }
    }
}
