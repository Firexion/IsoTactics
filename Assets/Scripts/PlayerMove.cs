using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{
    [SerializeField] private Camera cam;
    // Start is called before the first frame update
    private void Start()
    {
        Init();        
    }

    // Update is called once per frame
    private void Update()
    {
        if (!turn)
        {
            return;
        }
        
        if (!moving)
        {
            FindSelectableTiles();
            CheckMouse();
        }
        else
        {
            Move(); 
        }
    }

    void CheckMouse()
    {
        if (!Input.GetMouseButtonUp(0)) // Left Click 1 time only
        {
            return;
        }
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out var hit) || !hit.collider.CompareTag("Tile"))
        {
            return;
        }
        var t = hit.collider.GetComponent<Tile>();
        if (t.selectable)
        {
            // todo move target
            MoveToTile(t);
        }
    }
}
