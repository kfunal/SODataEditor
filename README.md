# SOData Editor

## Overview
This tool provides a custom editor window for managing categories in the project. It allows users to create and manage categories directly from the Unity editor using a TreeView structure. The main goal is to minimize manual editing of the project file structure and encourage managing categories through the editor interface. The TreeView structure directly reflects the folder hierarchy in the project, ensuring consistent organization.

## Features
- **TreeView Structure**: Categories are organized and displayed in a tree view for easy navigation and management, mirroring the folder structure in the project.
- **Category Management**: Users can add or delete categories directly from the TreeView, either at the root level or as a subcategory.
- **Scriptable Object Display**: The selected group's Scriptable Objects are displayed in a list, allowing users to easily view and manage them.
- **Inspector Display**: When a Scriptable Object is selected from the list, its inspector is shown next to the list for easy editing.
- **Editor Window Access**: The window is accessed through the **SOCategory/Category Window** menu.
- **Avoid Manual Changes for Categories**: While users are free to create their own folder structures, it is crucial to manage category-related Scriptable Objects exclusively through this editor tool to ensure consistent integration.

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

### Displaying Scriptable Objects
- The selected group's Scriptable Objects are listed in a dedicated area within the editor window.
- Users can click on any Scriptable Object to view its properties and details.

### Inspecting Scriptable Objects
- When a Scriptable Object is selected from the list, its inspector is displayed next to the list.
- This allows users to edit the selected object directly without needing to open a separate inspector window.

### Important Note for Scriptable Objects
- Users must create a variable of type **Category** within each **ScriptableObject** they wish to manage through this tool. The name of this variable can be anything, as long as it is of type **Category**. This ensures that the Scriptable Object will be correctly recognized within the category structure.

### Additional Notes
- The tool is designed to work with a **ScriptableObject** that holds the categories' data.
- The editor focuses on managing the category structure solely through the TreeView interface.
- The TreeView structure directly reflects the folder hierarchy in the project, ensuring consistent organization.

### Best Practices
- **Avoid Manual Changes for Categories**: While users can create their own folder structures, it is essential to manage category-related Scriptable Objects exclusively through this editor tool to maintain consistency.
- **Use the Editor**: Whenever possible, utilize the editor for adding or removing categories to ensure smooth integration with the tool’s functionality.

## Unity Version
This tool is developed using **Unity 2023.1**.

## License
This project is licensed under the **MIT License**.
