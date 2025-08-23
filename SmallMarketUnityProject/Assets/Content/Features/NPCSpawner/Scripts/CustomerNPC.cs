using System.Collections.Generic;
using Content.Features.BuildingSystem.Scripts;
using Content.Features.CurrencySystem;
using Content.Features.NPCLogic.Scripts;
using Core.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Features.NPCSpawner.Scripts
{
    public class CustomerNPC : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private NavMeshAgent agent;

        [Header("Settings")]
        [SerializeField] private float interactDistance = 1.5f;
        [SerializeField] private float buyDuration = 2f;
        
        public ICustomerState CurrentState => _currentState;
        
        private NPCSpawner _spawner;
        private int _id;
        private ICustomerState _currentState;
        private ICurrencyService _currency;

        private List<ResourceAmount> rewards;
        
        public Transform targetStand { get; set; }
        public Transform targetCashier { get; set; }
        public float buyTimer { get; set; }
        
        public void Initialize(NPCSpawner spawner, int id)
        {
            _spawner = spawner;
            _id = id;
            _currency = ServiceLocator.Get<ICurrencyService>();
            ChangeState(new WonderingState());
        }
        
        private void Update()
        {
            _currentState?.Update(this);
        }
        
        public void ChangeState(ICustomerState newState)
        {
            _currentState?.Exit(this);
            _currentState = newState;
            _currentState?.Enter(this);
        }
        
        public void MoveTo(Vector3 position)
        {
            agent.isStopped = false;
            agent.SetDestination(position);
        }

        public bool IsReachedTarget()
        {
            return !agent.pathPending && agent.remainingDistance <= interactDistance;
        }
        
        public void Release()
        {
            if (rewards != null && rewards.Count > 0 && _currency != null)
            {
                foreach (var reward in rewards)
                {
                    _currency.Add(reward.resourceType, reward.amount);
                }
            }
            
            _spawner.UnregisterCustomer(_id);
        }

        public void UpdateTimer()
        {
            buyTimer = buyDuration;
        }
        
        public void SelectProduct(List<ResourceAmount> objectRewards)
        {
            rewards = objectRewards;
        }
    }
}