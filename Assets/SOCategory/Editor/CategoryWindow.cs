using System.Collections.Generic;
using SOCategory.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using SOCategory.CustomEditor.CustomUIElements;
using static SOCategory.CustomEditor.Utilities.Constants;
using static SOCategory.CustomEditor.Utilities.AssetHelper;
using static SOCategory.CustomEditor.Utilities.UIElementHelper;
using System.Linq;
using System.Reflection;

public class CategoryWindow : EditorWindow
{
    [SerializeField] private VisualTreeAsset visualTreeAsset;
    [SerializeField] private string newCategoryName;
    [SerializeField] private string createFromExistingTpeAssetName;
    [SerializeField] private Object createFromExistingTypePath;

    private VisualElement root;
    private VisualElement addCategoryArea;
    private VisualElement showSOArea;
    private VisualElement soDetailArea;

    private CustomHelpBox addCategoryHelpBox;

    private TreeView categoryTree;
    private ListView soList;
    private Label selectedCategoryLabel;
    private TextField newCategoryNameField;
    private IMGUIContainer categoryAddContainer;
    private IMGUIContainer soContainer;

    private List<TreeViewItemData<Category>> treeItems = new List<TreeViewItemData<Category>>();
    private List<Category> rootCategories = new List<Category>();
    private List<Category> allCategories = new List<Category>();
    private List<ScriptableObject> groupSo = new List<ScriptableObject>();
    private List<ScriptableObject> allSo = new List<ScriptableObject>();

    private Dictionary<string, System.Action> buttonActions;
    private Category categoryToAdd;


    [MenuItem(MENU_PATH_CATEGORY_WINDOW_WINDOW)]
    public static void ShowExample()
    {
        CategoryWindow wnd = GetWindow<CategoryWindow>();
    }

    private void OnEnable()
    {
        buttonActions = new Dictionary<string, System.Action>()
        {
            {BUTTON_UN_SELECT,UnSelectCategoryTree},
            {BUTTON_ADD_CATEGORY_AREA,OnAddCategoryAreaButtonClicked},
            {BUTTON_ADD_CATEGORY,OnAddCategoryButtonClicked},
            {BUTTON_REMOVE_CATEGORY, OnRemoveCategoryButtonClicked},
            {BUTTON_SHOW_SO, OnShowSOButtonClicked},
        };
    }

    public void CreateGUI()
    {
        root = rootVisualElement;
        visualTreeAsset.CloneTree(root);

        root.Q<VisualElement>(VIEL_PARENT).Bind(new SerializedObject(this));

        allSo = FindAllScriptableObjectsExceptCategories();

        GetUIElements();
        RegisterUIElements();

        LoadCategories();
    }

    private void GetUIElements()
    {
        addCategoryArea = root.Q<VisualElement>(VIEL_ADD_CATEGORY_AREA);
        showSOArea = root.Q<VisualElement>(VIEL_SHOW_SO_AREA);
        soDetailArea = root.Q<VisualElement>(VIEL_SO_DETAIL_AREA);

        addCategoryHelpBox = root.Q<CustomHelpBox>(HELP_BOX_ADD_CATEGORY_HELP_BOX);

        selectedCategoryLabel = root.Q<Label>(LABEL_SELECTED_CATEGORY);
        categoryTree = root.Q<TreeView>(TREE_VIEW_CATEGORY);
        newCategoryNameField = root.Q<TextField>(TEXTFIELD_NEW_CATEGORY_TEXT_FIELD);
        soList = root.Q<ListView>(LIST_VIEW_SO_LIST);
    }

    private void RegisterUIElements()
    {
        foreach (KeyValuePair<string, System.Action> item in buttonActions)
            root.Q<Button>(item.Key).RegisterCallback<ClickEvent>(evt => item.Value?.Invoke());

        categoryTree.makeItem = () => CreateLabel(string.Empty, STYLE_COLLECTION_ELEMENT);
        categoryTree.bindItem = (_element, _index) => (_element as Label).text = treeItems[_index].data.name;
        categoryTree.selectionChanged += OnCategorySelectionChanged;

        soList.itemsSource = groupSo;
        soList.makeItem = () => CreateLabel(string.Empty, STYLE_COLLECTION_ELEMENT);
        soList.bindItem = (_element, _index) => (_element as Label).text = groupSo[_index].name;
        soList.selectionChanged += OnSoListSelectionChanged;
    }

    private void LoadCategories()
    {
        rootCategories = new List<Category>();
        treeItems.Clear();
        categoryTree.SetRootItems(treeItems);
        string[] subFolders = AssetDatabase.GetSubFolders(PATH_CATEGORY_ROOT);

        foreach (string folder in subFolders)
        {
            Category category = AssetDatabase.LoadAssetAtPath<Category>($"{folder}/{folder.GetFinalFolder()}.asset");

            rootCategories.Add(category);
            TreeViewItem(category, null);
        }

        categoryTree.ExpandAll();
    }

    private void TreeViewItem(Category _category, Category _parent)
    {
        TreeViewItemData<Category> item = new TreeViewItemData<Category>(_category.ID, _category);
        treeItems.Add(item);
        allCategories.Add(_category);

        if (_parent != null)
            categoryTree.AddItem(item, _parent.ID);
        else
            categoryTree.AddItem(item);

        if (!_category.HasSubCategories) return;

        foreach (Category subCategory in _category.SubCategories)
            TreeViewItem(subCategory, _category);
    }

    private void OnCategorySelectionChanged(IEnumerable<object> enumerable)
    {
        selectedCategoryLabel.text = categoryTree.selectedIndex < 0 ? string.Empty : treeItems[categoryTree.selectedIndex].data.name;
        newCategoryNameField.label = categoryTree.selectedIndex < 0 ? NEW_CATEGORY : $"New {treeItems[categoryTree.selectedIndex].data.name} Type";

        UpdateCategorySOList();
        soList.ClearSelection();
    }

    private void UpdateCategorySOList()
    {
        if (categoryTree.selectedIndex < 0)
            groupSo = new List<ScriptableObject>();
        else
        {
            groupSo = new List<ScriptableObject>();

            foreach (ScriptableObject so in allSo)
            {
                if (so == null) continue;

                FieldInfo[] fields = so.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo info in fields)
                {
                    if (info.FieldType != typeof(Category)) continue;

                    Category categoryValue = info.GetValue(so) as Category;
                    if (categoryValue == null) continue;

                    if (categoryValue == treeItems[categoryTree.selectedIndex].data)
                    {
                        groupSo.Add(so);
                        continue;
                    }
                }
            }
        }

        soList.itemsSource = groupSo;
        soList.Rebuild();
    }

    private void UnSelectCategoryTree()
    {
        categoryTree.ClearSelection();
        soList.ClearSelection();
        ClearCategoryAddContainer();
        addCategoryArea.ChangeStyle(STYLE_AREA_HIDDEN, STYLE_AREA);
        addCategoryHelpBox.ChangeStyle(STYLE_HELP_BOX_HIDDEN, STYLE_HELP_BOX);
        showSOArea.ChangeStyle(STYLE_AREA_HIDDEN, STYLE_AREA);
    }

    private void OnAddCategoryAreaButtonClicked()
    {
        showSOArea.ChangeStyle(STYLE_AREA_HIDDEN, STYLE_AREA);

        ClearCategoryAddContainer();

        categoryToAdd = CreateInstance<Category>();

        categoryAddContainer = new IMGUIContainer(() =>
        {
            if (categoryTree.selectedIndex >= 0)
                categoryToAdd.SetParentCategory(treeItems[categoryTree.selectedIndex].data);

            Editor editor = Editor.CreateEditor(categoryToAdd);
            editor.OnInspectorGUI();
        });

        addCategoryArea.Add(categoryAddContainer);
        addCategoryArea.ChangeStyle(STYLE_AREA, STYLE_AREA_HIDDEN);
    }

    private void OnSoListSelectionChanged(IEnumerable<object> enumerable)
    {
        soDetailArea.ChangeStyle(STYLE_AREA, STYLE_AREA_HIDDEN);

        if (soContainer != null && soDetailArea.Contains(soContainer))
        {
            soDetailArea.Remove(soContainer);
            soContainer = null;
        }

        if (soList.selectedIndex < 0)
        {
            soDetailArea.ChangeStyle(STYLE_AREA_HIDDEN, STYLE_AREA);
            return;
        }

        soContainer = new IMGUIContainer(() =>
        {
            if (soList.selectedIndex < 0) return;

            Editor editor = Editor.CreateEditor(groupSo[soList.selectedIndex]);
            if (editor != null)
                editor.OnInspectorGUI();
        });

        soDetailArea.Add(soContainer);
    }

    private void OnAddCategoryButtonClicked()
    {
        if (string.IsNullOrEmpty(newCategoryName))
        {
            ShowHelpBox(EMPTY_CATEGORY_NAME, HelpBoxMessageType.Error);
            return;
        }

        if (CategoryExists(newCategoryName))
        {
            ShowHelpBox($"{newCategoryName} {ALREADY_EXISTS}", HelpBoxMessageType.Error);
            return;
        }

        addCategoryHelpBox.ChangeStyle(STYLE_HELP_BOX_HIDDEN, STYLE_HELP_BOX);

        if (CreateCategory(categoryToAdd, newCategoryName))
        {
            ShowHelpBox($"{newCategoryName} {CATEGORY_CREATED}", HelpBoxMessageType.Info);
            ClearCategoryAddContainer();
            LoadCategories();
        }
    }

    private void ClearCategoryAddContainer()
    {
        categoryToAdd = null;
        newCategoryName = string.Empty;

        if (categoryAddContainer != null && addCategoryArea.Contains(categoryAddContainer))
        {
            addCategoryArea.Remove(categoryAddContainer);
            categoryAddContainer = null;
        }
    }

    private bool CategoryExists(string _category)
    {
        for (int i = 0; i < treeItems.Count; i++)
            if (treeItems[i].data.name == _category) return true;

        return false;
    }

    private void OnRemoveCategoryButtonClicked()
    {
        if (categoryTree.selectedIndex < 0) return;

        if (!EditorUtility.DisplayDialog(ARE_YOU_SURE, DELETE_CATEGORY_MESSAGE, YES, NO)) return;

        Category deletedCategory = treeItems[categoryTree.selectedIndex].data;
        string folderPath = deletedCategory.GetObjectFolderPath();

        if (deletedCategory.ParentCategory != null)
            deletedCategory.ParentCategory.SubCategories.Remove(deletedCategory);

        AssetDatabase.DeleteAsset(folderPath);
        AssetDatabase.Refresh();
        LoadCategories();
    }

    private void ShowHelpBox(string _content, HelpBoxMessageType _type)
    {
        addCategoryHelpBox.SetHelpBox(_content, _type);
        addCategoryHelpBox.ChangeStyle(STYLE_HELP_BOX, STYLE_HELP_BOX_HIDDEN);
    }

    private void OnShowSOButtonClicked()
    {
        showSOArea.ChangeStyle(STYLE_AREA, STYLE_AREA_HIDDEN);
        ClearCategoryAddContainer();
        addCategoryArea.ChangeStyle(STYLE_AREA_HIDDEN, STYLE_AREA);
        addCategoryHelpBox.ChangeStyle(STYLE_HELP_BOX_HIDDEN, STYLE_HELP_BOX);
    }
}
