using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Content.Features.CraftSystem.Scripts
{
    public class InventoryItemUIView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private TMP_Text countText;

        public void Setup(ItemData data, int count)
        {
            if (icon != null) icon.sprite = data.icon;
            if (itemNameText != null) itemNameText.text = data.itemName;
            if (countText != null) countText.text = count.ToString();
        }
    }
}