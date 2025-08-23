using Content.Features.BuildingSystem.Scripts;
using Content.Features.NPCSpawner.Scripts;
using UnityEngine;

namespace Content.Features.NPCLogic.Scripts
{
    public class SearchProductState : ICustomerState
    {
        public void Enter(CustomerNPC customer)
        {
            if (customer.targetStand == null)
            {
                Debug.Log("[NPC] No goods selected, return to select product");
                customer.ChangeState(new SelectProductState());
                return;
            }

            customer.UpdateTimer();
            customer.MoveTo(customer.targetStand.position);
        }

        public void Update(CustomerNPC customer)
        {
            if (customer.targetStand == null) return;

            if (customer.IsReachedTarget())
            {
                customer.buyTimer -= Time.deltaTime;
                if (customer.buyTimer <= 0)
                {
                    var stand = customer.targetStand.GetComponent<ProductStand>();
                    if (stand != null)
                    {
                        customer.SelectProduct(stand.GetSellableRewards());
                    }
                    
                    customer.ChangeState(new PayState());
                }
            }
        }

        public void Exit(CustomerNPC customer) { }
    }
}