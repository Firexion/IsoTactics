using System.Collections.Generic;
using UnityEngine;

namespace Unit
{
    [CreateAssetMenu(menuName = "Units/Unit List")]
    public class UnitList : ScriptableObject
    {
        public List<UnitObject> units;
    }
}