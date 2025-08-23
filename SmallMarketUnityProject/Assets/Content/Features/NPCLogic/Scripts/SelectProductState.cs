using Content.Features.BuildingSystem.Scripts;
using Content.Features.NPCSpawner.Scripts;
using UnityEngine;

namespace Content.Features.NPCLogic.Scripts
{
    public class SelectProductState : ICustomerState
    {
        private bool serachForStand;
        
        public void Enter(CustomerNPC customer)
        {
            // Ищем случайный стенд
            var stands = Object.FindObjectsOfType<ProductStand>();
            if (stands.Length == 0)
            {
                Debug.Log("[NPC] No product stands found");
                serachForStand = true;
            }
            else
            {
                var chosen = stands[Random.Range(0, stands.Length)];
                customer.targetStand = chosen.transform;

                customer.ChangeState(new SearchProductState());
            }
        }

        public void Update(CustomerNPC customer)
        {
            if (serachForStand)
            {
                var stands = Object.FindObjectsOfType<ProductStand>();
                if (stands.Length > 0)
                {
                    var chosen = stands[Random.Range(0, stands.Length)];
                    customer.targetStand = chosen.transform;
                    customer.ChangeState(new SearchProductState());
                    serachForStand = false;
                }
            }
        }
        
        public void Exit(CustomerNPC customer) { }
    }
}