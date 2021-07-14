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

                if (GUILayout.Button("Add Humanoid Unit", GUILayout.ExpandWidth(false)))
                {
                    AddHumanoid();
                }
                
                GUILayout.Space(5);

                if (GUILayout.Button("Add Monster Unit", GUILayout.ExpandWidth(false)))
                {
                    AddMonster();
                }
                
                GUILayout.Space(60);

                if (GUILayout.Button("Delete Unit", GUILayout.ExpandWidth(false)))
                {
                    DeleteUnit(_viewIndex - 1);
                }

                GUILayout.EndHorizontal();
                if (unitList.units == null)
                {
                    Debug.Log("No units in list");
                }
                else if (unitList.units.Count > 0)
                {
                    GUILayout.BeginHorizontal();
                    _viewIndex = Mathf.Clamp(
                        EditorGUILayout.IntField("Current Unit", _viewIndex, GUILayout.ExpandWidth(false)), 1,
                        unitList.units.Count);
                    // Mathf.Clamp (viewIndex, 1, unitList.units.Count);
                    EditorGUILayout.LabelField("of   " + unitList.units.Count + "  units", "",
                        GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    unitList.units[_viewIndex - 1].name = EditorGUILayout.TextField("Name",
                        unitList.units[_viewIndex - 1].name);
                    // unitList.units[viewIndex - 1].icon = EditorGUILayout.ObjectField("Icon",
                    //     unitList.units[viewIndex - 1].icon, typeof(Texture2D), false) as Texture2D;

                    // unitList.units[viewIndex - 1].itemObject = EditorGUILayout.ObjectField("Item Object",
                    //     unitList.units[viewIndex - 1].itemObject, typeof(Rigidbody), false) as Rigidbody;

                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    unitList.units[_viewIndex - 1].Brawn = Mathf.Clamp(EditorGUILayout.IntField("Brawn",
                        unitList.units[_viewIndex - 1].Brawn, GUILayout.ExpandWidth(false)), 1, 99);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    unitList.units[_viewIndex - 1].Agility = Mathf.Clamp(EditorGUILayout.IntField("Agility",
                        unitList.units[_viewIndex - 1].Agility, GUILayout.ExpandWidth(false)), 1, 99);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    unitList.units[_viewIndex - 1].Perception = Mathf.Clamp(EditorGUILayout.IntField("Perception",
                        unitList.units[_viewIndex - 1].Perception, GUILayout.ExpandWidth(false)), 1, 99);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    unitList.units[_viewIndex - 1].Cunning = Mathf.Clamp(EditorGUILayout.IntField("Cunning",
                        unitList.units[_viewIndex - 1].Cunning, GUILayout.ExpandWidth(false)), 1, 99);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    unitList.units[_viewIndex - 1].Will = Mathf.Clamp(EditorGUILayout.IntField("Will",
                        unitList.units[_viewIndex - 1].Will, GUILayout.ExpandWidth(false)), 1, 99);
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
            unitList.units = new List<CombatUnit>();
            var relPath = AssetDatabase.GetAssetPath(unitList);
            EditorPrefs.SetString("ObjectPath", relPath);
        }

        private void OpenUnits()
        {
            var absPath = EditorUtility.OpenFilePanel("Select Unit List", "", "");
            if (!absPath.StartsWith(Application.dataPath)) return;
            var relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            unitList = AssetDatabase.LoadAssetAtPath(relPath, typeof(UnitList)) as UnitList;
            if (unitList == null) return;
            unitList.units ??= new List<CombatUnit>();
            if (unitList)
            {
                EditorPrefs.SetString("ObjectPath", relPath);
            }
        }

        private void AddHumanoid()
        {
            var newUnit = new Humanoid {name = "New Humanoid"};
            unitList.units.Add(newUnit);
            _viewIndex = unitList.units.Count;
        }
        
        private void AddMonster()
        {
            var newUnit = new Monster() {name = "New Monster"};
            unitList.units.Add(newUnit);
            _viewIndex = unitList.units.Count;
        }

        private void DeleteUnit(int index)
        {
            unitList.units.RemoveAt(index);
        }
    }
}