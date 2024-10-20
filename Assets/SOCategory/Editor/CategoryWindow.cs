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
using System;
public class CategoryWindow : EditorWindow
{
    [SerializeField] private VisualTreeAsset visualTreeAsset;
    [SerializeField] private string newCategoryName;

    private VisualElement root;
    private VisualElement addCategoryArea;

    private TreeView categoryTree;
    private Label selectedCategoryLabel;
    private TextField newCategoryNameField;
    private CustomHelpBox addCategoryHelpBox;

    private List<TreeViewItemData<Category>> treeItems = new List<TreeViewItemData<Category>>();

    private List<Category> categories = new List<Category>();

    private Dictionary<string, System.Action> buttonActions;

    private IMGUIContainer categoryAddContainer;
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
        };
    }

    public void CreateGUI()
    {
        root = rootVisualElement;
        visualTreeAsset.CloneTree(root);

        root.Q<VisualElement>(VIEL_PARENT).Bind(new SerializedObject(this));

        GetUIElements();
        RegisterUIElements();

        LoadCategories();
    }

    private void GetUIElements()
    {
        addCategoryArea = root.Q<VisualElement>(VIEL_ADD_CATEGORY_AREA);
        selectedCategoryLabel = root.Q<Label>(LABEL_SELECTED_CATEGORY);
        categoryTree = root.Q<TreeView>(TREE_VIEW_CATEGORY);
        newCategoryNameField = root.Q<TextField>(TEXTFIELD_NEW_CATEGORY_TEXT_FIELD);
        addCategoryHelpBox = root.Q<CustomHelpBox>(HELP_BOX_ADD_CATEGORY_HELP_BOX);
    }

    private void RegisterUIElements()
    {
        foreach (KeyValuePair<string, System.Action> item in buttonActions)
            root.Q<Button>(item.Key).RegisterCallback<ClickEvent>(evt => item.Value?.Invoke());

        categoryTree.makeItem = () => CreateLabel(string.Empty, STYLE_TREE_ELEMENT);
        categoryTree.bindItem = (_element, _index) => (_element as Label).text = treeItems[_index].data.name;
        categoryTree.selectionChanged += OnCategorySelectionChanged;
    }

    private void LoadCategories()
    {
        categories = new List<Category>();
        treeItems.Clear();
        categoryTree.SetRootItems(treeItems);
        string[] subFolders = AssetDatabase.GetSubFolders(PATH_CATEGORY_ROOT);

        foreach (string folder in subFolders)
        {
            Category category = AssetDatabase.LoadAssetAtPath<Category>($"{folder}/{folder.GetFinalFolder()}.asset");

            categories.Add(category);
            TreeViewItem(category, null);
        }

        categoryTree.ExpandAll();
    }

    private void TreeViewItem(Category _category, Category _parent)
    {
        TreeViewItemData<Category> item = new TreeViewItemData<Category>(_category.ID, _category);
        treeItems.Add(item);

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
    }

    private void UnSelectCategoryTree()
    {
        categoryTree.ClearSelection();
        ClearCategoryAddContainer();
        addCategoryArea.ChangeStyle(STYLE_ADD_CATEGORY_AREA_HIDDEN, STYLE_ADD_CATEGORY_AREA);
        addCategoryHelpBox.ChangeStyle(STYLE_CATEGORY_CREATE_HELP_BOK_HIDDEN, STYLE_CATEGORY_CREATE_HELP_BOK);
    }

    private void OnAddCategoryAreaButtonClicked()
    {
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
        addCategoryArea.ChangeStyle(STYLE_ADD_CATEGORY_AREA, STYLE_ADD_CATEGORY_AREA_HIDDEN);
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

        addCategoryHelpBox.ChangeStyle(STYLE_CATEGORY_CREATE_HELP_BOK_HIDDEN, STYLE_CATEGORY_CREATE_HELP_BOK);

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
            addCategoryArea.Remove(categoryAddContainer);
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
        addCategoryHelpBox.ChangeStyle(STYLE_CATEGORY_CREATE_HELP_BOK, STYLE_CATEGORY_CREATE_HELP_BOK_HIDDEN);
    }
}
