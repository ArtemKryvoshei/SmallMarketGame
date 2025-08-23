using Content.Features.BuildingSystem.Scripts;
using Content.Features.NPCSpawner.Scripts;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Features.NPCLogic.Scripts
{
    public class BuyProductState : ICustomerState
    {
        public void Enter(CustomerNPC customer)
        {
            customer.UpdateTimer();
            customer.GetComponent<NavMeshAgent>().isStopped = true;
        }

        public void Update(CustomerNPC customer)
        {
            customer.buyTimer -= Time.deltaTime;
            if (customer.buyTimer <= 0)
            {
                // Ищем кассу
                var cashiers = Object.FindObjectsOfType<CashierPoint>();
                if (cashiers.Length > 0)
                {
                    customer.targetCashier = cashiers[Random.Range(0, cashiers.Length)].transform;
                    customer.ChangeState(new PayState());
                }
                else
                {
                    Debug.LogWarning("[NPC] No CashierPoint!");
                }
            }
        }

        public void Exit(CustomerNPC customer) { }
    }
}