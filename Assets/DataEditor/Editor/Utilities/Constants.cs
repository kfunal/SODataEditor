namespace DataEditor.Editor.Utilities
{
    public static class Constants
    {
        //Index
        public const int INDEX_ALL_SO_AREA = 0;
        public const int INDEX_CREATE_SO_AREA = 1;
        public const int INDEX_CREATE_SO_SCRIPT_AREA = 2;

        //Path
        public const string PATH_SO_SCRIPT_TEMPLATE = "Assets/DataEditor/Editor/Templates/ScriptTemplate.cs.txt";
        public const string PATH_SO_SCRIPT_TEMPLATE_HELPER = "Assets/DataEditor/Editor/Templates/TemplateHelper.cs.txt";

        //Text
        public const string WINDOW_TITLE = "Data Editor";
        public const string MENU_PATH = "Data Editor/Window";
        public const string CREATE_SCRIPT_ERROR = "Please make sure that you have filled in all the fields, that you have selected a valid file path, and that there is no script with the same name in the project.";
        public const string ARE_YOU_SURE = "Are you Sure";
        public const string ARE_YOU_SURE_TO_DELETE = "Are you sure you want to delete the selected Scriptable Objects ?";
        public const string YES = "Yes";
        public const string NO = "No";

        //Visual Element
        public const string AREA_PARENT = "window";
        public const string AREA_ALL_SO = "all-so-area";
        public const string AREA_CREATE_SO = "create-so-area";
        public const string AREA_CREATE_SO_SCRIPT = "create-so-script-area";

        //Button
        public const string BUTTON_ALL_SO_AREA = "all-so-button";
        public const string BUTTON_CREATE_SO_AREA = "create-so-button";
        public const string BUTTON_CREATE_SO_SCRIPT_AREA = "create-so-script-button";
        public const string BUTTON_CREATE_SO_SCRIPT = "create-button";
        public const string BUTTON_SELECT_SO = "select-so-button";
        public const string BUTTON_DELETE_SO = "delete-so-button";
        public const string BUTTON_REFRESH_LIST = "refresh-so-list-button";
        public const string BUTTON_UN_SELECT = "unselect-button";
        public const string BUTTON_CLEAR_SCRIPT_INPUT = "clear-button";

        //List View
        public const string LIST_VIEW_ALL_SO = "so-list";

        //Scroll View
        public const string SCROLL_VIEW_INSPECTOR = "inspector-scroll-view";

        //Help Box
        public const string HELP_BOX_CREATE_SO_INFO = "script-help-box";
        public const string HELP_BOX_CREATE_GROUP_OR_CATEGORY = "group-category-help-box";

        //Style
        public const string STYLE_AREA = "area";
        public const string STYLE_AREA_HIDDEN = "area-hidden";
        public const string STYLE_ALL_SO_LIST_ELEMENT = "so-list-element";
        public const string STYLE_HELP_BOX = "helpbox";
        public const string STYLE_HELP_BOX_HIDDEN = "helpbox-hidden";

    }
}
