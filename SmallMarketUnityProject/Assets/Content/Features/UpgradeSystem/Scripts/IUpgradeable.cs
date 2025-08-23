using UnityEngine;


namespace Content.Features.UpgradeSystem.Scripts
{
    public interface IUpgradeable
    {
        UpgradeData GetUpgradeData();         
        int GetCurrentTier();                 
        void UpgradeToTier(int newTier);      
        Transform GetMenuAnchor();            
    }
}