using UnityEngine.UIElements;

namespace DataEditor.Editor.CustomUIElements
{
    public class CustomHelpBox : HelpBox
    {
        public new class UxmlFactory : UxmlFactory<CustomHelpBox, UxmlTraits> { }
        public CustomHelpBox() { }
    }
}
