using UnityEngine.UIElements;

namespace DataEditor.CustomUIElements
{
    public class CustomHelpBox : HelpBox
    {
        public new class UxmlFactory : UxmlFactory<CustomHelpBox, UxmlTraits> { }
        public CustomHelpBox() { }
    }
}
