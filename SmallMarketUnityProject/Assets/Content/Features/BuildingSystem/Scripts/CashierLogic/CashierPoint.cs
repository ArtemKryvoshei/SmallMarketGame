using System.Collections;
using System.Collections.Generic;
using Content.Features.NPCLogic.Scripts;
using Content.Features.NPCSpawner.Scripts;
using Content.Features.UpgradeSystem.Scripts;
using Core.EventBus;
using Core.IInitializeQueue;
using Core.JSONSaveLoadSystem;
using Core.Other;
using Core.PrefabFactory;
using Core.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class TierRewardBonusCashier
{
    public int tier;
    public float timeToRelease;
}

namespace Content.Features.BuildingSystem.Scripts
{
    public class CashierPoint : InitializeableMonoComponent, IUpgradeable
    {
        [Header("Save Settings")]
        [SerializeField] private int saveId;
        
        [Header("Cashier Settings")]
        [SerializeField] private float detectionRadius = 3f;
        [SerializeField] private CashierProgressBar progressBar;
        
        [Header("Upgrade info")]
        [SerializeField] private Transform upgradeWindowAnchor;
        [SerializeField] private UpgradeData upgradeData;
        [SerializeField] private TierRewardBonusCashier[] processTimeByTier;
        
        private int _currentTier = 1;
        private float processTime;
        
        public int GetCurrentTier() => _currentTier;
        private bool _isBusy;
        
        public UpgradeData GetUpgradeData() => upgradeData;
        public Transform GetMenuAnchor() => upgradeWindowAnchor;
        private CustomerNPC _currentCustomer;
        
        private ISaveLoadSystem _saveLoadSystem;
        private IEventBus _eventBus;

        public void UpgradeToTier(int newTier)
        {
            if (newTier <= _currentTier || newTier > upgradeData.maxTier) return;
            _currentTier = newTier;
            processTime = FindTierTime();
            Debug.Log($"[CashierPoint] Upgraded to tier {_currentTier}, processTime={processTime}");
            SaveTier();
        }
        
        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_LASTINIT_COMPONENTS;
            base.SetupPriority();
        }

        public override void Initialize()
        {
            _saveLoadSystem = ServiceLocator.Get<ISaveLoadSystem>();
            _eventBus = ServiceLocator.Get<IEventBus>();
            base.Initialize();
            initialized = true;
            _eventBus.Subscribe<OnAutosaveCall>(_ => SaveTier());
            LoadTier();
        }
        
        private void Update()
        {
            if (_isBusy) return;

            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);

            foreach (var hit in hits)
            {
                var npc = hit.GetComponent<CustomerNPC>();
                if (npc != null && npc.CurrentState is PayState)
                {
                    StartCoroutine(ProcessCustomer(npc));
                    break;
                }
            }
        }

        private IEnumerator ProcessCustomer(CustomerNPC npc)
        {
            _isBusy = true;
            _currentCustomer = npc;

            // запускаем прогресс бар
            if (progressBar != null)
                progressBar.Show(processTime);

            float timer = 0f;
            while (timer < processTime)
            {
                timer += Time.deltaTime;

                // обновляем прогресс бар
                if (progressBar != null)
                    progressBar.UpdateProgress(timer / processTime);

                yield return null;
            }

            // отпускаем NPC
            if (_currentCustomer != null)
            {
                _currentCustomer.Release();
                Destroy(_currentCustomer.gameObject);
            }

            if (progressBar != null)
                progressBar.Hide();

            _currentCustomer = null;
            _isBusy = false;
        }

        public float FindTierTime()
        {
            foreach (var tierReward in processTimeByTier)
            {
                if (tierReward.tier == _currentTier)
                {
                    return tierReward.timeToRelease;
                }
            }

            return 2f;
        }
        
        private void SaveTier()
        {
            var data = _saveLoadSystem.Load();
            var saved = data.Upgrades.Find(u => u.id == saveId);
            if (saved != null)
            {
                saved.tier = _currentTier;
            }
            else
            {
                data.Upgrades.Add(new UpgradeSaveData
                {
                    id = saveId,
                    tier = _currentTier
                });
            }
            _saveLoadSystem.Save(data);
        }

        private void LoadTier()
        {
            var data = _saveLoadSystem.Load();
            var saved = data.Upgrades.Find(u => u.id == saveId);
            if (saved != null)
            {
                _currentTier = saved.tier;
                processTime = FindTierTime(); 
                Debug.Log($"[CashierPoint] Loaded tier {_currentTier}, processTime={processTime}");
            }
            else
            {
                processTime = FindTierTime(); 
            }
        }

        
        private void OnDestroy()
        {
            _eventBus.Unsubscribe<OnAutosaveCall>(_ => SaveTier());
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}