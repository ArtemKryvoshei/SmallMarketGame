using Content.Features.CurrencySystem;
using Core.JSONSaveLoadSystem;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Content.Features.BuildingSystem.Scripts
{
    public class ResourceEntityUIView : MonoBehaviour
    { 
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text amountText;

        public void Setup(CurrencyType type, int amount, Sprite iconSprite)
        {
            icon.sprite = iconSprite;
            amountText.text = amount.ToString();
        }
    }
}