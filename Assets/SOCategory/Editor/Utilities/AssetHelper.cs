using System.Collections.Generic;
using System.IO;
using System.Text;
using SOCategory.Runtime;
using UnityEditor;
using UnityEngine;

using static SOCategory.CustomEditor.Utilities.Constants;

namespace SOCategory.CustomEditor.Utilities
{
    public static class AssetHelper
    {
        public static bool CreateCategory(Category _category, string _name)
        {
            try
            {
                string path = $"{PATH_CATEGORY_ROOT}/";

                if (_category.ParentCategory == null)
                {
                    CheckDirectory($"{path}/{_name}");

                    path += $"{_name}/{_name}.asset";
                }

                else
                {
                    string subPath = string.Empty;

                    Category category = _category;

                    if (category.ParentCategory != null)
                    {
                        category.ParentCategory.SubCategories.Add(category);
                        EditorUtility.SetDirty(category.ParentCategory);
                        EditorUtility.FocusProjectWindow();
                    }
                    subPath = GetParentDirectory(category, _name);
                    path += subPath;

                    CheckDirectory(path);

                    path += $"/{_name}.asset";
                }

                AssetDatabase.CreateAsset(_category, path);
                AssetDatabase.Refresh();
                EditorApplication.ExecuteMenuItem("File/Save");
                EditorApplication.ExecuteMenuItem("File/Save Project");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GetParentDirectory(Category _category, string _name)
        {
            StringBuilder pathBuilder = new StringBuilder();

            Category category = _category;

            while (category.ParentCategory != null)
            {
                pathBuilder.Insert(0, $"{category.ParentCategory.name}/");
                category = category.ParentCategory;
            }

            pathBuilder.Append($"{_name}");

            return pathBuilder.ToString();
        }

        private static void CheckDirectory(string _directory)
        {
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
                AssetDatabase.Refresh();
            }
        }

        public static string GetObjectFolderPath(this Object _obj)
        {
            string path = AssetDatabase.GetAssetPath(_obj);
            string folderPath = Path.GetDirectoryName(path);

            return folderPath;
        }

        public static string GetFinalFolder(this string _folderName) => _folderName.Split("/")[^1];

        public static List<ScriptableObject> FindAllScriptableObjectsExceptCategories()
        {
            List<ScriptableObject> scriptableObjects = new List<ScriptableObject>();

            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.StartsWith(PATH_CATEGORY_ROOT))
                    continue;

                ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (obj == null) continue;

                scriptableObjects.Add(obj);
            }

            return scriptableObjects;
        }
    }
}
