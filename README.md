# Category Editor Tool Documentation

## Overview
This tool provides a custom editor window for managing categories in the project. It allows users to create and manage categories directly from the Unity editor using a TreeView structure. The main goal is to minimize manual editing of the project file structure and encourage managing categories through the editor interface.

## Features
- **TreeView Structure**: Categories are organized and displayed in a tree view for easy navigation and management.
- **Category Management**: Users can add or delete categories directly from the TreeView, either at the root level or as a subcategory.
- **Editor Window Access**: The window is accessed through the **SOCategory/Category Window** menu.
- **Avoid Manual Changes**: It is recommended to avoid manually altering the file structure and use the editor for all category management tasks.

## How to Use

### Opening the Category Window
To open the category management window:
1. Go to the Unity top menu.
2. Navigate to **SOCategory > Category Window**.
3. The custom editor window will open, displaying the categories in a TreeView format.

### Adding a Category
To add a new category:
1. In the **Category Window**, locate the TreeView where the categories are listed.
2. You can either:
   - Click on the **root** (if you want to add a category at the top level).
   - Or select an existing category (if you want to add a subcategory under it).
3. After selecting the desired location, click the **"Add Category"** button at the top of the window.
4. A new section will appear prompting you to enter the name of the new category.
5. Type the name of the category in the input field and click the **"Add"** button to finalize the process.
6. The system will automatically add the category to the appropriate place in the hierarchy and handle any necessary reference mappings.

### Deleting a Category
To delete an existing category:
1. In the **Category Window**, select the category you wish to remove from the TreeView.
2. Click on the **"Delete"** button at the top of the window.
3. The selected category will be removed from the list.

### Using ScriptableObject for Categories
Currently, users can create a variable of type **Category** within a **ScriptableObject** to store category information. This setup allows for easy management and retrieval of category data directly from the ScriptableObject.

### Best Practices
- **Avoid Manual File Changes**: It is recommended not to manually alter the file structure. Instead, use the editor window to manage all categories. This ensures that the structure stays consistent and avoids potential errors.
- **Use the Editor**: Whenever possible, use the editor for adding or removing categories to ensure smooth integration with the tool’s functionality.

## Development Status
The project is currently in development, with plans to enhance functionality and user experience. Future features may include additional category management tools, improvements to the user interface, and better integration with user feedback.

## Unity Version
This tool is developed using **Unity 2022.3.50.f1**.

## License
This project is licensed under the **MIT License**.

## Additional Notes
- The tool is designed to work with a **ScriptableObject** that holds the categories' data.
- The editor focuses on managing the category structure solely through the TreeView interface.
