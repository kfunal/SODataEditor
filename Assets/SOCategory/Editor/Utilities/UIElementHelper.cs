using SOCategory.CustomEditor.CustomUIElements;
using UnityEngine.UIElements;

namespace SOCategory.CustomEditor.Utilities
{
    public static class UIElementHelper
    {
        public static Label CreateLabel(string _text = "", params string[] _styles)
        {
            if (_styles == null || _styles.Length == 0) return new Label();

            Label label = new Label(_text);

            foreach (string style in _styles)
                label.AddToClassList(style);

            return label;
        }

        public static void ChangeStyle(this VisualElement _element, string _newStyle, string _oldStyle)
        {
            if (_element.ClassListContains(_oldStyle))
                _element.RemoveFromClassList(_oldStyle);

            if (!_element.ClassListContains(_newStyle))
                _element.AddToClassList(_newStyle);
        }

        public static void SetHelpBox(this CustomHelpBox _helpBox, string _content, HelpBoxMessageType _type)
        {
            _helpBox.text = _content;
            _helpBox.messageType = _type;
        }
    }
}
