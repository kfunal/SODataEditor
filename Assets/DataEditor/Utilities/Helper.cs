using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataEditor.CustomUIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static DataEditor.Utilities.Constants;

namespace DataEditor.Utilities
{
    public static class Helper
    {
        public static void ChangeStyle(this VisualElement _element, string _newStyle, string _oldStyle)
        {
            _element.RemoveFromClassList(_oldStyle);
            _element.AddToClassList(_newStyle);
        }

        public static List<ScriptableObject> GetScriptableObjects()
        {
            List<ScriptableObject> scriptableObjects = new List<ScriptableObject>();

            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new string[] { "Assets/" });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (string.IsNullOrEmpty(path)) continue;

                ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

                if (so == null) continue;
                if (scriptableObjects.Contains(so)) continue;

                scriptableObjects.Add(so);
            }

            return scriptableObjects;
        }

        public static void ChangeItemSource<T>(this ListView _listVew, List<T> _itemsSource)
        {
            _listVew.itemsSource = _itemsSource;
            _listVew.Rebuild();
        }

        public static Label CreateLabel(string _text, params string[] _styles)
        {
            Label label = new Label();
            label.text = _text;

            foreach (string style in _styles)
                label.AddToClassList(style);

            return label;
        }

        public static void SetHelpBox(this CustomHelpBox _helpBox, string _message, HelpBoxMessageType _type)
        {
            _helpBox.text = _message;
            _helpBox.messageType = _type;
            _helpBox.ChangeStyle(STYLE_HELP_BOX, STYLE_HELP_BOX_HIDDEN);
        }

        public static bool IsAnyEmpty(params string[] _variables)
        {
            if (_variables == null) return false;
            if (_variables.Length == 0) return false;

            return _variables.Any(x => string.IsNullOrEmpty(x));
        }

        public static bool IsValidFolder(this Object _object) => AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(_object));
        public static string GetPath(this Object _object) => AssetDatabase.GetAssetPath(_object);

        public static string ReplaceTemplate(this string _string, string _baseClass, string _fileName, string _menuName)
        {
            return _string.Replace("__BASE_CLASS__", _baseClass).Replace("__FILE_NAME__", _fileName).Replace("__MENU_NAME__", _menuName);
        }

        public static bool ScriptExist(string _scriptName)
        {
            string assetsPath = Application.dataPath;
            string fileName = _scriptName + ".cs";

            string[] files = Directory.GetFiles(assetsPath, "*.cs", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (Path.GetFileName(file) == fileName)
                    return true;
            }

            return false;
        }

        public static void AddButtonClick(this VisualElement _element, string _buttonName, System.Action _onClick)
        {
            _element.Q<Button>(_buttonName).RegisterCallback<ClickEvent>(evt => _onClick?.Invoke());
        }
    }
}
