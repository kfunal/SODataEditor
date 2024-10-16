using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataEditor.Editor.CustomUIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static DataEditor.Editor.Utilities.Constants;
using static DataEditor.Editor.Utilities.Helper;

namespace DataEditor.Editor
{
    public class DataEditorWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private string scriptName;
        [SerializeField] private string inheritFrom;
        [SerializeField] private string fileName;
        [SerializeField] private string menuName;
        [SerializeField] private Object path;

        private VisualElement root;

        private ListView allSOList;
        private ScrollView inspectorArea;
        private CustomHelpBox scriptInfoHelpBox;
        private IMGUIContainer imguiContainer;

        private List<VisualElement> panels;
        private List<ScriptableObject> allScriptableObjects = new List<ScriptableObject>();

        private string soScriptContent;
        private string soScriptTemplate;

        private Dictionary<string, System.Action> buttonActions;

        [MenuItem(MENU_PATH)]
        public static void ShowExample()
        {
            DataEditorWindow wnd = GetWindow<DataEditorWindow>();
            wnd.titleContent = new GUIContent(WINDOW_TITLE);
        }

        private void OnEnable()
        {
            buttonActions = new Dictionary<string, System.Action>
            {
                { BUTTON_ALL_SO_AREA, () => ChangeArea(INDEX_ALL_SO_AREA) },
                { BUTTON_CREATE_SO_AREA, () => ChangeArea(INDEX_CREATE_SO_AREA) },
                { BUTTON_CREATE_SO_SCRIPT_AREA, () => ChangeArea(INDEX_CREATE_SO_SCRIPT_AREA) },
                { BUTTON_CREATE_SO_SCRIPT, OnCreateScriptButtonClicked },
                { BUTTON_SELECT_SO, OnSelectSOButtonClicked },
                { BUTTON_DELETE_SO, OnDeleteSOButtonClicked },
                { BUTTON_REFRESH_LIST, RefreshSOList },
                {BUTTON_UN_SELECT, OnUnSelectButtonClicked },
            };
        }

        public void CreateGUI()
        {
            root = rootVisualElement;
            visualTreeAsset.CloneTree(root);

            soScriptTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(PATH_SO_SCRIPT_TEMPLATE).text;
            allScriptableObjects = GetScriptableObjects();

            GetUIElements();
            InitializePanelsList();
            EventRegistersAndBinds();
        }

        private void GetUIElements()
        {
            inspectorArea = root.Q<ScrollView>(SCROLL_VIEW_INSPECTOR);
            allSOList = root.Q<ListView>(LIST_VIEW_ALL_SO);
            scriptInfoHelpBox = root.Q<CustomHelpBox>(HELP_BOX_CREATE_SO_INFO);
        }

        private void InitializePanelsList()
        {
            panels = new List<VisualElement>
            {
                root.Q<VisualElement>(AREA_ALL_SO),
                root.Q<VisualElement>(AREA_CREATE_SO),
                root.Q<VisualElement>(AREA_CREATE_SO_SCRIPT)
            };
        }

        private void EventRegistersAndBinds()
        {
            foreach (var kvp in buttonActions)
                root.AddButtonClick(kvp.Key, kvp.Value);

            root.Q<VisualElement>(AREA_PARENT).Bind(new SerializedObject(this));

            allSOList.itemsSource = allScriptableObjects;
            allSOList.makeItem = () => CreateLabel(string.Empty, STYLE_ALL_SO_LIST_ELEMENT);
            allSOList.bindItem = (_item, _index) => (_item as Label).text = allScriptableObjects[_index].name;
            allSOList.selectionChanged += OnElementSelected;
        }

        private void OnDeleteSOButtonClicked()
        {
            Object[] selectedItems = allSOList.selectedItems.Cast<Object>().ToArray();

            if (selectedItems == null || selectedItems.Length == 0) return;

            if (!EditorUtility.DisplayDialog(ARE_YOU_SURE, ARE_YOU_SURE_TO_DELETE, YES, NO)) return;

            foreach (Object obj in selectedItems)
            {
                string path = AssetDatabase.GetAssetPath(obj);

                AssetDatabase.DeleteAsset(path);
                AssetDatabase.Refresh();
            }

            RefreshSOList();
        }

        private void OnSelectSOButtonClicked()
        {
            EditorUtility.FocusProjectWindow();
            Selection.objects = allSOList.selectedItems.Cast<Object>().ToArray();
        }

        private void OnUnSelectButtonClicked()
        {
            EditorUtility.FocusProjectWindow();
            Selection.objects = null;
            allSOList.selectedIndex = -1;
        }

        private void OnCreateScriptButtonClicked()
        {
            if (!IsValid())
            {
                scriptInfoHelpBox.SetHelpBox(CREATE_SCRIPT_ERROR, HelpBoxMessageType.Error);
                return;
            }

            scriptInfoHelpBox.ChangeStyle(STYLE_HELP_BOX_HIDDEN, STYLE_HELP_BOX);

            soScriptContent = soScriptTemplate;
            soScriptContent = soScriptContent.ReplaceTemplate(inheritFrom, fileName, menuName);

            File.WriteAllText(PATH_SO_SCRIPT_TEMPLATE_HELPER, soScriptContent);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(PATH_SO_SCRIPT_TEMPLATE_HELPER, $"{path.GetPath()}/{scriptName}.cs");
        }

        private void OnElementSelected(IEnumerable<object> enumerable)
        {
            if (inspectorArea.Contains(imguiContainer))
                inspectorArea.Remove(imguiContainer);

            if (enumerable.Count() != 1) return;

            imguiContainer = new IMGUIContainer(() =>
                {
                    UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor(allScriptableObjects[allSOList.selectedIndex]);

                    if (editor != null)
                        editor.OnInspectorGUI();
                });

            inspectorArea.Add(imguiContainer);
        }

        private void ChangeArea(int _areaIndex)
        {
            allSOList.selectedIndex = -1;

            for (int i = 0; i < panels.Count; i++)
            {
                if (i == _areaIndex)
                    panels[i].ChangeStyle(STYLE_AREA, STYLE_AREA_HIDDEN);
                else
                    panels[i].ChangeStyle(STYLE_AREA_HIDDEN, STYLE_AREA);
            }
        }

        private void RefreshSOList()
        {
            allScriptableObjects = GetScriptableObjects();
            allSOList.ChangeItemSource(allScriptableObjects);
            allSOList.selectedIndex = -1;
        }

        private bool IsValid()
        {
            if (IsAnyEmpty(scriptName, inheritFrom, fileName, menuName))
                return false;

            if (path == null) return false;
            if (!path.IsValidFolder()) return false;
            if (ScriptExist(scriptName)) return false;

            return true;
        }
    }
}
