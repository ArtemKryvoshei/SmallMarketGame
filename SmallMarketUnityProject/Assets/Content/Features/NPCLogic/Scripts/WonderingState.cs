using Content.Features.NPCSpawner.Scripts;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Features.NPCLogic.Scripts
{
    public class WonderingState : ICustomerState
    {
        private float minWaitTime = 4f;
        private float maxWaitTime = 8f;
        private float wonderingRadius = 5f; 
        private Vector3 wanderTarget;

        private float waitTimer;

        public void Enter(CustomerNPC customer)
        {
            wanderTarget = GetValidRandomPoint(customer.transform.position);
            customer.MoveTo(wanderTarget);
            waitTimer = Random.Range(minWaitTime, maxWaitTime);
        }

        public void Update(CustomerNPC customer)
        {
            if (customer.IsReachedTarget())
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0)
                {
                    customer.ChangeState(new SelectProductState());
                }
            }
        }

        public void Exit(CustomerNPC customer) { }
        
        private Vector3 GetValidRandomPoint(Vector3 center)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * wonderingRadius;
                Vector3 randomPoint = center + new Vector3(randomCircle.x, 0, randomCircle.y);

                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            return center;
        }
    }
}