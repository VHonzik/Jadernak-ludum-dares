using System;
using System.Collections.Generic;
using BehaviorTree;
using System.Collections;
using UnityEngine;




namespace Entities
{
    public class NPC : Actor
    {

        /*
        public BehaviorReturnCode MoveToTarget()
        {
            if (currentTask == null && nextNode.next != null)
            {
                currentTask = new Task(MoveCoroutine(nextNode.next));
                return BehaviorReturnCode.Running;
            }
            else if (currentTask.Running)
            {
                return BehaviorReturnCode.Running;
            }
            else
            {
                currentTask = null;
                return BehaviorReturnCode.Success;
            }

        }


        public IEnumerator MoveCoroutine(SearchNode target)
        {

            Vector3 targetPosition = new Vector3(target.position.X - planet.width / 2, target.position.Y - planet.height / 2, target.position.Z - planet.depth / 2);

            int layerMask = 1 << 8;
            RaycastHit hit;
            if (Physics.Raycast(targetPosition, planet.PlanetObject.transform.position - targetPosition, out hit, Mathf.Infinity, layerMask))
            {

                targetPosition = hit.point + (hit.point - planet.PlanetObject.transform.position).normalized * offsetPathFinding;
            }

            while (Vector3.Distance(transform.position, targetPosition) > 0.5f)
            {

                Rigidbody r = (Rigidbody)GetComponent(typeof(Rigidbody));

                r.AddForce((targetPosition - transform.position).normalized * 50);
                yield return null;
            }

            var next = target.next;

            if (next != null)
            {
                currentTask = new Task(MoveCoroutine(next));
            }
            else
            {
                GameObject.Destroy(Target, 1f);
                pathNodes.Clear();

            }


        }*/
    }
}
