using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace BlacXGames.PersistSuite.Editor
{
    public class PlayerPrefsEditorWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private Dictionary<string, bool> categoryFoldouts = new Dictionary<string, bool>();
        private Dictionary<string, string> editableValues = new Dictionary<string, string>();
        private string searchText = "";
        private bool showModifiedOnly = false;
        private Color defaultColor;
        private GUIStyle headerStyle;
        private GUIStyle subHeaderStyle;
        private GUIStyle valueChangedStyle;

        [MenuItem("Tools/BlacX Games/PersistSuite/PlayerPrefs Editor")]
        public static void ShowWindow()
        {
            GetWindow<PlayerPrefsEditorWindow>("PlayerPrefs Editor");
        }

        private void OnEnable()
        {
            // Initialize the foldouts to be open by default
            foreach (var category in GetAllCategories())
            {
                categoryFoldouts[category] = true;
            }
            
            // Cache all current values for edit tracking
            RefreshValues();
        }

        private void OnGUI()
        {
            if (headerStyle == null)
            {
                InitializeStyles();
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawToolbar();
            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            DrawEntries();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
            DrawFooter();
            EditorGUILayout.EndVertical();
        }

        private void InitializeStyles()
        {
            defaultColor = GUI.color;

            headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.fontSize = 14;
            headerStyle.margin = new RectOffset(5, 5, 5, 5);

            subHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
            subHeaderStyle.fontSize = 12;
            subHeaderStyle.margin = new RectOffset(5, 5, 2, 2);

            valueChangedStyle = new GUIStyle(EditorStyles.textField);
            valueChangedStyle.normal.textColor = Color.green;
            valueChangedStyle.focused.textColor = Color.green;
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            // Search field
            searchText = EditorGUILayout.TextField(searchText, EditorStyles.toolbarSearchField, GUILayout.Width(200));
            if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                searchText = "";
                GUI.FocusControl(null);
            }

            GUILayout.FlexibleSpace();

            // Show modified only toggle
            showModifiedOnly = EditorGUILayout.ToggleLeft("Show Modified Only", showModifiedOnly, GUILayout.Width(130));

            // Refresh button
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                RefreshValues();
            }

            // Delete all button (with confirmation)
            GUI.color = Color.red;
            if (GUILayout.Button("Delete All", EditorStyles.toolbarButton, GUILayout.Width(70)))
            {
                if (EditorUtility.DisplayDialog("Delete All PlayerPrefs", 
                    "Are you sure you want to delete all PlayerPrefs data? This cannot be undone!", 
                    "Yes, Delete All", "Cancel"))
                {
                    UnityEngine.PlayerPrefs.DeleteAll();
                    UnityEngine.PlayerPrefs.Save();
                    RefreshValues();
                }
            }
            GUI.color = defaultColor;

            EditorGUILayout.EndHorizontal();
        }

        private void DrawEntries()
        {
            var entries = GetFilteredEntries();
            var groupedEntries = entries.GroupBy(e => e.Category).OrderBy(g => g.Key);

            foreach (var group in groupedEntries)
            {
                string category = group.Key;
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                // Draw category header with foldout
                EditorGUILayout.BeginHorizontal();
                if (!categoryFoldouts.ContainsKey(category))
                {
                    categoryFoldouts[category] = true;
                }
                
                categoryFoldouts[category] = EditorGUILayout.Foldout(categoryFoldouts[category], "", true);
                
                GUILayout.Label(category, headerStyle);
                
                // Category action buttons
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Reset Category", EditorStyles.miniButton, GUILayout.Width(100)))
                {
                    if (EditorUtility.DisplayDialog("Reset Category", 
                        $"Are you sure you want to reset all preferences in the '{category}' category?", 
                        "Yes", "Cancel"))
                    {
                        ResetCategory(category, group.ToList());
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                // Draw category entries if folded out
                if (categoryFoldouts[category])
                {
                    EditorGUI.indentLevel++;
                    
                    EditorGUILayout.BeginVertical();
                    
                    // Header row
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Key", EditorStyles.boldLabel, GUILayout.Width(150));
                    GUILayout.Label("Type", EditorStyles.boldLabel, GUILayout.Width(70));
                    GUILayout.Label("Value", EditorStyles.boldLabel, GUILayout.Width(200));
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Actions", EditorStyles.boldLabel, GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.Space(2);
                    
                    // Draw divider
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    
                    // Entries
                    foreach (var entry in group.OrderBy(e => e.Key))
                    {
                        DrawEntryRow(entry);
                    }
                    
                    EditorGUILayout.EndVertical();
                    
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
            
            if (!entries.Any())
            {
                EditorGUILayout.HelpBox(
                    searchText.Length > 0 ? "No matching PlayerPrefs found." : "No PlayerPrefs data found.", 
                    MessageType.Info);
            }
        }

        private void DrawEntryRow(PlayerPrefsEntry entry)
        {
            string entryId = $"{entry.Category}_{entry.Key}";
            
            // Initialize editable value if not exists
            if (!editableValues.ContainsKey(entryId))
            {
                editableValues[entryId] = entry.Value;
            }
            
            bool isModified = editableValues[entryId] != entry.Value;
            
            EditorGUILayout.BeginHorizontal();
            
            // Key name (shortened if too long)
            string displayKey = entry.Key;
            if (displayKey.Length > 20)
            {
                displayKey = displayKey.Substring(0, 17) + "...";
            }
            GUILayout.Label(displayKey, GUILayout.Width(150));
            
            // Type
            GUILayout.Label(entry.DataType, GUILayout.Width(70));
            
            // Value editor based on type
            GUIStyle styleToUse = isModified ? valueChangedStyle : EditorStyles.textField;
            
            switch (entry.DataType)
            {
                case "Boolean":
                case "Bool":
                    bool boolValue = editableValues[entryId].ToLower() == "true";
                    bool newBoolValue = EditorGUILayout.Toggle(boolValue, GUILayout.Width(200));
                    if (boolValue != newBoolValue)
                    {
                        editableValues[entryId] = newBoolValue.ToString();
                    }
                    break;
                    
                case "Int32":
                case "Int":
                    if (int.TryParse(editableValues[entryId], out int intValue))
                    {
                        int newIntValue = EditorGUILayout.IntField(intValue, GUILayout.Width(200));
                        if (intValue != newIntValue)
                        {
                            editableValues[entryId] = newIntValue.ToString();
                        }
                    }
                    else
                    {
                        editableValues[entryId] = EditorGUILayout.TextField(editableValues[entryId], styleToUse, GUILayout.Width(200));
                    }
                    break;
                    
                case "Single":
                case "Float":
                    if (float.TryParse(editableValues[entryId], out float floatValue))
                    {
                        float newFloatValue = EditorGUILayout.FloatField(floatValue, GUILayout.Width(200));
                        if (floatValue != newFloatValue)
                        {
                            editableValues[entryId] = newFloatValue.ToString();
                        }
                    }
                    else
                    {
                        editableValues[entryId] = EditorGUILayout.TextField(editableValues[entryId], styleToUse, GUILayout.Width(200));
                    }
                    break;
                    
                default:
                    editableValues[entryId] = EditorGUILayout.TextField(editableValues[entryId], styleToUse, GUILayout.Width(200));
                    break;
            }
            
            GUILayout.FlexibleSpace();
            
            // Save button - only enabled if modified
            EditorGUI.BeginDisabledGroup(!isModified);
            if (GUILayout.Button("Save", EditorStyles.miniButtonLeft, GUILayout.Width(50)))
            {
                SaveEntry(entry, editableValues[entryId]);
            }
            EditorGUI.EndDisabledGroup();
            
            // Reset button
            if (GUILayout.Button("Reset", EditorStyles.miniButtonRight, GUILayout.Width(50)))
            {
                editableValues[entryId] = entry.Value;
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawFooter()
        {
            EditorGUILayout.BeginHorizontal();
            
            // Save all modified values
            bool hasModifiedValues = editableValues.Any(pair => {
                string entryId = pair.Key;
                string parts = entryId.Split('_')[0];
                string category = parts.Length > 0 ? parts : "";
                string key = entryId.Substring(category.Length + 1);
                
                var entry = GetAllEntries().FirstOrDefault(e => e.Category == category && e.Key == key);
                return entry != null && entry.Value != pair.Value;
            });
            
            EditorGUI.BeginDisabledGroup(!hasModifiedValues);
            if (GUILayout.Button("Save All Changes", GUILayout.Height(30)))
            {
                SaveAllChanges();
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.EndHorizontal();
        }

        private void SaveEntry(PlayerPrefsEntry entry, string newValue)
        {
            SetPlayerPrefsValue(entry.Category, entry.Key, newValue, entry.DataType);
            UnityEngine.PlayerPrefs.Save();
            
            // Update the cached value
            entry.Value = newValue;
        }

        private void SaveAllChanges()
        {
            foreach (var pair in editableValues)
            {
                string entryId = pair.Key;
                string newValue = pair.Value;
                
                // Extract category and key from the entryId
                string[] parts = entryId.Split('_');
                string category = parts[0];
                string key = entryId.Substring(category.Length + 1);
                
                var entry = GetAllEntries().FirstOrDefault(e => e.Category == category && e.Key == key);
                if (entry != null && entry.Value != newValue)
                {
                    SetPlayerPrefsValue(entry.Category, entry.Key, newValue, entry.DataType);
                }
            }
            
            UnityEngine.PlayerPrefs.Save();
            RefreshValues();
        }

        private void ResetCategory(string category, List<PlayerPrefsEntry> entries)
        {
            foreach (var entry in entries)
            {
                // Delete the key from PlayerPrefs
                UnityEngine.PlayerPrefs.DeleteKey(entry.Key);
                
                // Reset the editable value to empty or default
                string entryId = $"{entry.Category}_{entry.Key}";
                editableValues[entryId] = GetDefaultValueForType(entry.DataType);
            }
            
            UnityEngine.PlayerPrefs.Save();
            RefreshValues();
        }

        private string GetDefaultValueForType(string dataType)
        {
            switch (dataType)
            {
                case "Boolean":
                case "Bool":
                    return "False";
                case "Int32":
                case "Int":
                    return "0";
                case "Single":
                case "Float":
                    return "0.0";
                default:
                    return "";
            }
        }

        private void RefreshValues()
        {
            var entries = GetAllEntries();
            editableValues.Clear();
            
            foreach (var entry in entries)
            {
                string entryId = $"{entry.Category}_{entry.Key}";
                editableValues[entryId] = entry.Value;
            }
            
            Repaint();
        }

        private void SetPlayerPrefsValue(string category, string key, string value, string dataType)
        {
            // Find the property to set
            PropertyInfo property = GetPlayerPrefsProperty(category, key);
            if (property != null)
            {
                // Set the value using the property's setter
                object convertedValue = ConvertValueToType(value, dataType);
                property.SetValue(null, convertedValue);
            }
            else
            {
                // Fallback: use PlayerPrefs directly
                switch (dataType)
                {
                    case "Boolean":
                    case "Bool":
                        UnityEngine.PlayerPrefs.SetString(key, value);
                        break;
                    case "Int32":
                    case "Int":
                        if (int.TryParse(value, out int intValue))
                        {
                            UnityEngine.PlayerPrefs.SetInt(key, intValue);
                        }
                        break;
                    case "Single":
                    case "Float":
                        if (float.TryParse(value, out float floatValue))
                        {
                            UnityEngine.PlayerPrefs.SetFloat(key, floatValue);
                        }
                        break;
                    default:
                        UnityEngine.PlayerPrefs.SetString(key, value);
                        break;
                }
            }
        }

        private PropertyInfo GetPlayerPrefsProperty(string category, string key)
        {
            // Get the nested class type for the category
            Type categoryType = typeof(PrefsEngine).GetNestedType(category, BindingFlags.Public | BindingFlags.Static);
            if (categoryType == null) return null;
            
            // Get all properties in the category
            var properties = categoryType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            
            // Find the property whose corresponding constant matches the key
            foreach (var property in properties)
            {
                var constantFields = categoryType.GetFields(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly);
                
                foreach (var field in constantFields)
                {
                    if (field.GetValue(null).ToString() == key)
                    {
                        return property;
                    }
                }
            }
            
            return null;
        }

        private object ConvertValueToType(string value, string dataType)
        {
            switch (dataType)
            {
                case "Boolean":
                case "Bool":
                    return bool.Parse(value);
                case "Int32":
                case "Int":
                    return int.Parse(value);
                case "Single":
                case "Float":
                    return float.Parse(value);
                default:
                    return value;
            }
        }

        private List<string> GetAllCategories()
        {
            return typeof(PrefsEngine).GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
                .Select(t => t.Name)
                .ToList();
        }

        private List<PlayerPrefsEntry> GetAllEntries()
        {
            var entries = new List<PlayerPrefsEntry>();
            
            // Get all nested classes in PrefsEngine (Player, Currency, Setting)
            var categories = typeof(PrefsEngine).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);
            
            foreach (var category in categories)
            {
                string categoryName = category.Name;
                
                // Get all constant fields (keys)
                var constantFields = category.GetFields(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly);
                
                // Get all properties (key-value pairs)
                var properties = category.GetProperties(BindingFlags.Public | BindingFlags.Static);
                
                foreach (var property in properties)
                {
                    // Find the corresponding key for this property
                    var keyField = constantFields.FirstOrDefault(f => f.Name.ToLower() == property.Name.ToLower());
                    if (keyField != null)
                    {
                        string key = keyField.GetValue(null).ToString();
                        string value = GetValueAsString(property);
                        string dataType = property.PropertyType.Name;
                        
                        entries.Add(new PlayerPrefsEntry 
                        { 
                            Key = key, 
                            Value = value,
                            Category = categoryName,
                            DataType = dataType,
                            PropertyName = property.Name
                        });
                    }
                }
            }
            
            return entries;
        }

        private List<PlayerPrefsEntry> GetFilteredEntries()
        {
            var entries = GetAllEntries();
            
            // Apply search filter
            if (!string.IsNullOrEmpty(searchText))
            {
                string search = searchText.ToLower();
                entries = entries.Where(e => 
                    e.Key.ToLower().Contains(search) || 
                    e.Value.ToLower().Contains(search) || 
                    e.Category.ToLower().Contains(search) ||
                    e.PropertyName.ToLower().Contains(search)
                ).ToList();
            }
            
            // Apply modified only filter
            if (showModifiedOnly)
            {
                entries = entries.Where(e => {
                    string entryId = $"{e.Category}_{e.Key}";
                    return editableValues.ContainsKey(entryId) && editableValues[entryId] != e.Value;
                }).ToList();
            }
            
            return entries;
        }

        private string GetValueAsString(PropertyInfo property)
        {
            try
            {
                var value = property.GetValue(null);
                return value?.ToString() ?? "null";
            }
            catch
            {
                return "Error reading value";
            }
        }

        public class PlayerPrefsEntry
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public string Category { get; set; }
            public string DataType { get; set; }
            public string PropertyName { get; set; }
        }
    }
}