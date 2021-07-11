
using Movement;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Map : MonoBehaviour
{
    public static Tile[] tiles;
    private void Start()
    {
        tiles = FindObjectsOfType<Tile>();
    }


    private void Update()
    {
        
    }
}
