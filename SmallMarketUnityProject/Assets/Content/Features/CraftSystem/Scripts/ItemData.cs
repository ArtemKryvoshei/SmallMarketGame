using UnityEngine;

namespace Content.Features.CraftSystem.Scripts
{
    [CreateAssetMenu(menuName = "Game/Item Data", fileName = "ItemData")]
    public class ItemData : ScriptableObject
    {
        public int itemId;   
        public string itemName; 
        public Sprite icon;
        public string itemPrefabAddress;
    }
}