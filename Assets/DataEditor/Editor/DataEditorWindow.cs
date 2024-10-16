using System.Collections.Generic;
using System.IO;
using DataEditor.CustomUIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static DataEditor.Utilities.Constants;
using static DataEditor.Utilities.Helper;

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
        private VisualElement allSOArea;
        private VisualElement createSOArea;
        private VisualElement createSOScriptArea;
        private VisualElement parent;

        private ToolbarButton allSOAreaButton;
        private ToolbarButton createSOAreaButton;
        private ToolbarButton createSOScriptAreaButton;
        private Button createSOScriptButton;

        private ListView allSOList;
        private ScrollView inspectorArea;
        private CustomHelpBox scriptInfoHelpBox;
        private IMGUIContainer imguiContainer;

        private List<VisualElement> panels;
        private List<ScriptableObject> allScriptableObjects = new List<ScriptableObject>();

        private string soScriptContent;
        private string soScriptTemplate;

        [MenuItem(MENU_PATH)]
        public static void ShowExample()
        {
            DataEditorWindow wnd = GetWindow<DataEditorWindow>();
            wnd.titleContent = new GUIContent(WINDOW_TITLE);
        }

        public void CreateGUI()
        {
            root = rootVisualElement;
            visualTreeAsset.CloneTree(root);

            soScriptTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(PATH_SO_SCRIPT_TEMPLATE).text;
            allScriptableObjects = GetScriptableObjects();

            GetUIElements();
            RegisterUIElementEvents();
            InitializePanelsList();

            parent.Bind(new SerializedObject(this));
        }

        private void GetUIElements()
        {
            allSOArea = root.Q<VisualElement>(AREA_ALL_SO);
            createSOArea = root.Q<VisualElement>(AREA_CREATE_SO);
            createSOScriptArea = root.Q<VisualElement>(AREA_CREATE_SO_SCRIPT);
            parent = root.Q<VisualElement>(AREA_PARENT);

            allSOAreaButton = root.Q<ToolbarButton>(BUTTON_ALL_SO_AREA);
            createSOAreaButton = root.Q<ToolbarButton>(BUTTON_CREATE_SO_AREA);
            createSOScriptAreaButton = root.Q<ToolbarButton>(BUTTON_CREATE_SO_SCRIPT_AREA);
            createSOScriptButton = root.Q<Button>(BUTTON_CREATE_SO_SCRIPT);

            inspectorArea = root.Q<ScrollView>(SCROLL_VIEW_INSPECTOR);
            allSOList = root.Q<ListView>(LIST_VIEW_ALL_SO);
            scriptInfoHelpBox = root.Q<CustomHelpBox>(HELP_BOX_CREATE_SO_INFO);
        }

        private void InitializePanelsList()
        {
            panels = new List<VisualElement>{
                allSOArea,
                createSOArea,
                createSOScriptArea
            };
        }

        private void RegisterUIElementEvents()
        {
            allSOAreaButton.RegisterCallback<ClickEvent>(evt => ChangeArea(INDEX_ALL_SO_AREA));
            createSOAreaButton.RegisterCallback<ClickEvent>(evt => ChangeArea(INDEX_CREATE_SO_AREA));
            createSOScriptAreaButton.RegisterCallback<ClickEvent>(evt => ChangeArea(INDEX_CREATE_SO_SCRIPT_AREA));
            createSOScriptButton.RegisterCallback<ClickEvent>(OnCreateScriptButtonClicked);

            allSOList.itemsSource = allScriptableObjects;
            allSOList.makeItem = () => CreateLabel(string.Empty, STYLE_ALL_SO_LIST_ELEMENT);
            allSOList.bindItem = (_item, _index) => (_item as Label).text = allScriptableObjects[_index].name;
            allSOList.selectionChanged += OnElementSelected;
        }

        private void OnCreateScriptButtonClicked(ClickEvent evt)
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
            for (int i = 0; i < panels.Count; i++)
            {
                if (i == _areaIndex)
                    panels[i].ChangeStyle(STYLE_AREA, STYLE_AREA_HIDDEN);
                else
                    panels[i].ChangeStyle(STYLE_AREA_HIDDEN, STYLE_AREA);
            }
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
