using UnityEngine.UIElements;

namespace SOCategory.CustomEditor.CustomUIElements
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }
        public SplitView() { }
    }
}
