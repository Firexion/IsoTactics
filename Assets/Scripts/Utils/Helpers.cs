using UnityEngine;

namespace Utils
{
    public static class Helpers
    {
        /**
     * Converts a Vector3 modifying position to only effect the X and Z
     * 
     */
        public static Vector3 XZ(this Vector3 input)
        {
            return new Vector3(input.x, 0, input.z);
        }
    }
}