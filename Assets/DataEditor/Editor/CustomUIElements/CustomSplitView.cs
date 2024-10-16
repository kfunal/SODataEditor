using UnityEngine;
using UnityEngine.UIElements;

namespace DataEditor.Editor.CustomUIElements
{
    public class CustomSplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<CustomSplitView, UxmlTraits> { }
        public CustomSplitView() { }
    }
}
