using UnityEngine;

namespace Movement
{
    public class NPCMove : TacticsMove
    {
        private GameObject _target;

        // Start is called before the first frame update
        private void Start()
        {
            Init();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!turnTaker.Turn) return;

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
        }

        private void CalculatePath()
        {
            var targetTile = GetTargetTile(_target);
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