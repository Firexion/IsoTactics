using UnityEditor;
using UnityEngine;

namespace Unit
{
    public class CreateUnitList
    {
        [MenuItem("Assets/Create/Unit List")]
        public static UnitList Create()
        {
            var asset = ScriptableObject.CreateInstance<UnitList>();

            AssetDatabase.CreateAsset(asset, "Assets/UnitList.asset");
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}