using System.Linq;
using Content.Features.BuildingSystem.Scripts;
using Content.Features.NPCSpawner.Scripts;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Features.NPCLogic.Scripts
{
    public class PayState : ICustomerState
    {
        public void Enter(CustomerNPC customer)
        {
            if (customer.targetCashier == null)
            {
                var cashier = GameObject.FindObjectsOfType<CashierPoint>()
                    .OrderBy(c => Vector3.Distance(customer.transform.position, c.transform.position))
                    .FirstOrDefault();

                if (cashier != null)
                    customer.targetCashier = cashier.transform;
            }

            if (customer.targetCashier != null)
            {
                customer.MoveTo(customer.targetCashier.position);
            }
            else
            {
                Debug.LogError("[PayState] No cashier assigned or found");
            }
        }

        public void Update(CustomerNPC customer)
        {
            if (customer.targetCashier == null) return;

            if (customer.IsReachedTarget())
            {
                customer.GetComponent<NavMeshAgent>().isStopped = true;
            }
        }

        public void Exit(CustomerNPC customer) { }
    }
}