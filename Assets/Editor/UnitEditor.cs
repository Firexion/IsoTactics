using System.Collections.Generic;
using Unit;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class UnitEditor : EditorWindow
    {
        public UnitList unitList;
        private int _viewIndex = 1;

        [MenuItem("Window/Unit Editor %#e")]
        private static void Init()
        {
            GetWindow(typeof(UnitEditor));
        }

        private void OnEnable()
        {
            _viewIndex = 1;
            if (!EditorPrefs.HasKey("ObjectPath")) return;
            var objectPath = EditorPrefs.GetString("ObjectPath");
            unitList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnitList)) as UnitList;
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Unit Editor", EditorStyles.boldLabel);
            if (unitList != null)
            {
                if (GUILayout.Button("Show Unit List"))
                {
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = unitList;
                }
            }

            if (GUILayout.Button("Open Unit List"))
            {
                OpenUnits();
            }

            if (GUILayout.Button("New Unit List"))
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = unitList;
            }
            
            if (GUILayout.Button("Save Unit List"))
            {
                SaveUnits();
            }

            GUILayout.EndHorizontal();

            if (unitList == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                if (GUILayout.Button("Create New Unit List", GUILayout.ExpandWidth(false)))
                {
                    CreateNewUnits();
                }

                if (GUILayout.Button("Open Existing Unit List", GUILayout.ExpandWidth(false)))
                {
                    OpenUnits();
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(20);

            if (unitList != null)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Space(10);

                if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
                {
                    if (_viewIndex > 1)
                        _viewIndex--;
                }

                GUILayout.Space(5);
                if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
                {
                    if (_viewIndex < unitList.units.Count)
                    {
                        _viewIndex++;
                    }
                }

                GUILayout.Space(60);

                if (GUILayout.Button("Add Unit", GUILayout.ExpandWidth(false)))
                {
                    AddUnit();
                }
             
                GUILayout.Space(60);

                if (GUILayout.Button("Delete Unit", GUILayout.ExpandWidth(false)))
                {
                    DeleteUnit(_viewIndex - 1);
                }

                GUILayout.EndHorizontal();
                Debug.Log("_view index = " + _viewIndex);
                if (unitList.units == null)
                {
                    Debug.Log("No units in list");
                }
                else if (unitList.units.Count > 0 && _viewIndex > 0 && _viewIndex < unitList.units.Count)
                {
                    var unit = unitList.units[_viewIndex - 1];
                    GUILayout.BeginHorizontal();
                    _viewIndex = Mathf.Clamp(
                        EditorGUILayout.IntField("Current Unit", _viewIndex, GUILayout.ExpandWidth(false)), 1,
                        unitList.units.Count);
                    // Mathf.Clamp (viewIndex, 1, unitList.units.Count);
                    EditorGUILayout.LabelField("of   " + unitList.units.Count + "  units", "",
                        GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();
                    
                    unit.Name = EditorGUILayout.TextField("Name",
                        unit.Name);
                    // unitList.units[viewIndex - 1].icon = EditorGUILayout.ObjectField("Icon",
                    //     unitList.units[viewIndex - 1].icon, typeof(Texture2D), false) as Texture2D;

                    // unitList.units[viewIndex - 1].itemObject = EditorGUILayout.ObjectField("Item Object",
                    //     unitList.units[viewIndex - 1].itemObject, typeof(Rigidbody), false) as Rigidbody;

                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    unit.Brawn = Mathf.Clamp(EditorGUILayout.IntField("Brawn",
                        unit.Brawn, GUILayout.ExpandWidth(false)), 1, 99);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    unit.Agility = Mathf.Clamp(EditorGUILayout.IntField("Agility",
                        unit.Agility, GUILayout.ExpandWidth(false)), 1, 99);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    unit.Perception = Mathf.Clamp(EditorGUILayout.IntField("Perception",
                        unit.Perception, GUILayout.ExpandWidth(false)), 1, 99);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    unit.Cunning = Mathf.Clamp(EditorGUILayout.IntField("Cunning",
                        unit.Cunning, GUILayout.ExpandWidth(false)), 1, 99);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    unit.Will = Mathf.Clamp(EditorGUILayout.IntField("Will",
                        unit.Will, GUILayout.ExpandWidth(false)), 1, 99);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    // unitList.units[viewIndex - 1].isStackable = (bool) EditorGUILayout.Toggle("Stackable ",
                    //     unitList.units[viewIndex - 1].isStackable, GUILayout.ExpandWidth(false));
                    // unitList.units[viewIndex - 1].destroyOnUse = (bool) EditorGUILayout.Toggle("Destroy On Use",
                    //     unitList.units[viewIndex - 1].destroyOnUse, GUILayout.ExpandWidth(false));
                    // unitList.units[viewIndex - 1].encumbranceValue = EditorGUILayout.FloatField("Encumberance",
                    //     unitList.units[viewIndex - 1].encumbranceValue, GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);
                    unitList.units[_viewIndex - 1] = unit;
                }
                else
                {
                    GUILayout.Label("This Unit List is Empty.");
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(unitList);
            }
        }

        private void CreateNewUnits()
        {
            // There is no overwrite protection here!
            // There is No "Are you sure you want to overwrite your existing object?" if it exists.
            // This should probably get a string from the user to create a new name and pass it ...
            _viewIndex = 1;
            unitList = CreateUnitList.Create();
            if (!unitList) return;
            unitList.units = new List<UnitData>();
            var relPath = AssetDatabase.GetAssetPath(unitList);
            EditorPrefs.SetString("ObjectPath", relPath);
        }

        private void OpenUnits()
        {
            var absPath = EditorUtility.OpenFilePanel("Select Unit List", "", "");
            if (!absPath.StartsWith(Application.dataPath)) return;
            var relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            unitList = AssetDatabase.LoadAssetAtPath(relPath, typeof(UnitList)) as UnitList;
            if (unitList == null)
            {
                Debug.Log("Asset is null");
                return;
            }

            if (unitList.units == null)
            {
                Debug.Log("unit list is created with null units");
            }
            else
            {
                Debug.Log("unit list is created with " + unitList.units.Count + " units");
            }
            unitList.units ??= new List<UnitData>();
            EditorPrefs.SetString("ObjectPath", relPath);
        }

        private static void SaveUnits()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // var absPath = EditorUtility.SaveFilePanel("Select Unit List", "", "", "");
            // if (!absPath.StartsWith(Application.dataPath)) return;
            // var relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            // unitList = AssetDatabase.SaveAssets() as UnitList;
            // if (unitList == null) return;
            // unitList.units ??= new List<UnitData>();
            // if (unitList)
            // {
            //     EditorPrefs.SetString("ObjectPath", relPath);
            // }
        }

        private void AddUnit()
        {
            var newUnit = new UnitData
            {
                id = _viewIndex + 1,
                Name = "New Unit",
                IsRightHanded = true,
                Brawn = 20,
                Agility = 20,
                Perception = 20,
                Cunning = 20,
                Will = 20
            };
            unitList.units.Add(newUnit);
            _viewIndex = unitList.units.Count;
        }
      

        private void DeleteUnit(int index)
        {
            unitList.units.RemoveAt(index);
            //_viewIndex = unitList.units.Count > 0 ? unitList.units.Count : 1;
        }
    }
}