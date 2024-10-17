using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataEditor.Editor.CustomUIElements;
using DataEditor.Runtime.Enums;
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
        [SerializeField] private SOGroups scriptCreateGroup;
        [SerializeField] private SOCategory scriptCreateCategory;
        [SerializeField] private string scriptName;
        [SerializeField] private string inheritFrom;
        [SerializeField] private string fileName;
        [SerializeField] private string menuName;
        [SerializeField] private string newGroup;
        [SerializeField] private string newCategory;

        [SerializeField] private Object path;

        private VisualElement root;
        private ListView allSOList;
        private ScrollView inspectorArea;
        private IMGUIContainer imguiContainer;

        private CustomHelpBox scriptInfoHelpBox;
        private CustomHelpBox addGroupCategoryHelpBox;

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
                { BUTTON_CREATE_SO_SCRIPT, CreateScript },
                { BUTTON_SELECT_SO, SelectSO },
                { BUTTON_UN_SELECT, UnSelectSO },
                { BUTTON_DELETE_SO, DeleteSO },
                { BUTTON_REFRESH_LIST, RefreshSOList },
                { BUTTON_CREATE_CATEGORY_GROUP_AREA,  () => ChangeArea(INDEX_CREATE_CATEGORY_AREA)},
                { BUTTON_CLEAR_SCRIPT_INPUT, ClearScriptInput },
                { BUTTON_CREATE_GROUP, () => CreateGroupOrCategory(newGroup,GROUP_NAME_CANT_BE_EMPTY,PATH_GROUPS_SCRIPT) },
                { BUTTON_CREATE_CATEGORY, () => CreateGroupOrCategory(newCategory,CATEGORY_NAME_CANT_BE_EMPTY,PATH_CATEGORY_SCRIPT) },
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
            addGroupCategoryHelpBox = root.Q<CustomHelpBox>(HELP_BOX_CREATE_GROUP_OR_CATEGORY);
        }

        private void InitializePanelsList()
        {
            panels = new List<VisualElement>
            {
                root.Q<VisualElement>(AREA_ALL_SO),
                root.Q<VisualElement>(AREA_CREATE_SO),
                root.Q<VisualElement>(AREA_CREATE_SO_SCRIPT),
                root.Q<VisualElement>(AREA_CREATE_CATEGORY)
            };
        }

        private void EventRegistersAndBinds()
        {
            foreach (var kvp in buttonActions)
                root.Q<Button>(kvp.Key).RegisterCallback<ClickEvent>((evt) => kvp.Value?.Invoke());

            root.Q<VisualElement>(AREA_PARENT).Bind(new SerializedObject(this));

            allSOList.itemsSource = allScriptableObjects;
            allSOList.makeItem = () => CreateLabel(string.Empty, STYLE_ALL_SO_LIST_ELEMENT);
            allSOList.bindItem = (_item, _index) => (_item as Label).text = allScriptableObjects[_index].name;
            allSOList.selectionChanged += OnElementSelected;
        }

        private void DeleteSO()
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

        private void SelectSO()
        {
            EditorUtility.FocusProjectWindow();
            Selection.objects = allSOList.selectedItems.Cast<Object>().ToArray();
        }

        private void UnSelectSO()
        {
            EditorUtility.FocusProjectWindow();
            Selection.objects = null;
            allSOList.selectedIndex = -1;
        }

        private void CreateScript()
        {
            if (!IsValid())
            {
                scriptInfoHelpBox.SetHelpBox(CREATE_SCRIPT_ERROR, HelpBoxMessageType.Error);
                return;
            }

            scriptInfoHelpBox.ChangeStyle(STYLE_HELP_BOX_HIDDEN, STYLE_HELP_BOX);

            soScriptContent = soScriptTemplate;
            soScriptContent = soScriptContent.ReplaceTemplate(inheritFrom, fileName, menuName, $"SOGroups.{scriptCreateGroup}", $"SOCategory.{scriptCreateCategory}");

            File.WriteAllText(PATH_SO_SCRIPT_TEMPLATE_HELPER, soScriptContent);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(PATH_SO_SCRIPT_TEMPLATE_HELPER, $"{path.GetPath()}/{scriptName}.cs");
        }

        private void ClearScriptInput()
        {
            scriptName = string.Empty;
            inheritFrom = string.Empty;
            fileName = string.Empty;
            menuName = string.Empty;
            scriptCreateGroup = SOGroups.UnGrouped;
            scriptCreateCategory = SOCategory.Uncategorized;
            path = null;
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

        public void CreateGroupOrCategory(string _valueToCreate, string _emptyMessage, string _addPath)
        {
            if (string.IsNullOrEmpty(_valueToCreate))
            {
                addGroupCategoryHelpBox.SetHelpBox(_emptyMessage, HelpBoxMessageType.Error);
                return;
            }

            if (EnumContains(_valueToCreate, _addPath))
            {
                addGroupCategoryHelpBox.SetHelpBox($"{_valueToCreate} {ALREADY_EXIST}", HelpBoxMessageType.Error);
                return;
            }

            scriptInfoHelpBox.ChangeStyle(STYLE_HELP_BOX_HIDDEN, STYLE_HELP_BOX);
            _valueToCreate.AddToEnum(_addPath);
        }
    }
}
