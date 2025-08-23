using System.Collections;
using System.Collections.Generic;
using Content.Features.NPCLogic.Scripts;
using Content.Features.NPCSpawner.Scripts;
using Content.Features.UpgradeSystem.Scripts;
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
    public class CashierPoint : MonoBehaviour, IUpgradeable
    {
        [Header("Cashier Settings")]
        [SerializeField] private float detectionRadius = 3f;
        [SerializeField] private float processTime = 2f;
        [SerializeField] private CashierProgressBar progressBar;
        [Header("Upgrade info")]
        [SerializeField] private Transform upgradeWindowAnchor;
        [SerializeField] private UpgradeData upgradeData;
        [SerializeField] private TierRewardBonusCashier[] processTimeByTier;
        

        private int _currentTier = 1;
        public UpgradeData GetUpgradeData() => upgradeData;
        public int GetCurrentTier() => _currentTier;
        public Transform GetMenuAnchor() => upgradeWindowAnchor;
        
        private bool _isBusy;
        private CustomerNPC _currentCustomer;

        public void UpgradeToTier(int newTier)
        {
            if (newTier <= _currentTier || newTier > upgradeData.maxTier) return;
            _currentTier = newTier;

            processTime = FindTierTime();
            Debug.Log($"[CashierPoint] Upgraded to tier {_currentTier}, processTime={processTime}");
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
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}