using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataEditor.Editor.CustomUIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static DataEditor.Editor.Utilities.Constants;

namespace DataEditor.Editor.Utilities
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

        public static string ReplaceTemplate(this string _string, string _baseClass, string _fileName, string _menuName, string _group, string _category)
        {
            return _string.Replace("__BASE_CLASS__", _baseClass).Replace("__FILE_NAME__", _fileName).Replace("__MENU_NAME__", _menuName).Replace("__GROUP__", _group).Replace("__CATEGORY__", _category);
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

        public static void AddToEnum(this string _value, string _enumPath)
        {
            string[] lines = File.ReadAllLines(_enumPath);

            if (lines == null || lines.Length == 0) return;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("}"))
                {
                    lines[i] = $"\t\t{_value},\n{lines[i]}";
                    break;
                }
            }

            File.WriteAllLines(_enumPath, lines);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        public static bool EnumContains(string _value, string _enumPath)
        {
            string[] lines = File.ReadAllLines(_enumPath);

            if (lines == null || lines.Length == 0) return false;
            if (lines.Where(x => x.Contains(_value)).Count() > 0) return true;

            return false;
        }
    }
}
