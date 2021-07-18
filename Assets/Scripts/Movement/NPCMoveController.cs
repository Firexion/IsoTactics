using DefaultNamespace;
using UnityEngine;

namespace Movement
{
    public class NPCMoveController : MoveController
    {
        private GameObject _target;

        // Update is called once per frame
        private void Update()
        {
            if (!IsActive() || !canMove) return;

            if (!moving)
            {
                FindNearestTarget();
                CalculatePath();
                FindSelectableTiles();
                actualTargetTile.target = true;
            }
            else
            {
                Move();
            }

            // NPC needs to end their turn once they are done moving
            if (IsActive() && !canMove) turnTaker.EndTurn();
        }

        private void CalculatePath()
        {
            var targetTile = SelectableTiles.GetTargetTile(_target);
            FindPath(targetTile);
        }

        private void FindNearestTarget()
        {
            var targets = GameObject.FindGameObjectsWithTag("Player");
            GameObject nearest = null;
            var distance = Mathf.Infinity;

            foreach (var obj in targets)
            {
                var d = Vector3.Distance(transform.position, obj.transform.forward);
                if (d >= distance) continue;
                nearest = obj;
                distance = d;
            }

            _target = nearest;
        }
    }
}