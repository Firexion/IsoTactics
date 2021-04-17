using UnityEngine;

public class Map : MonoBehaviour
{
    public int width = 12;
    public int height = 12;
    public GameObject tile;
    public void SetupMap()
    {
       

        for (int x = 0; x < width; ++x)
        {
            GameObject row = new GameObject($"Row{x}");
            for (int y = 0; y < height; ++y)
            {
                
                Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity);
            }
        }

    }
}
