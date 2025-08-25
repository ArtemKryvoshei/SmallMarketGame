using Content.Features.BuildingSystem.Scripts;
using Content.Features.CurrencySystem;
using Core.PrefabFactory;
using Core.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Features.CraftSystem.Scripts
{
    public class RecipeUIView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Transform ingredientsHolder;
        [SerializeField] private Transform rewardsHolder;
        [SerializeField] private Button craftButton;
        [SerializeField] private CurrencyConfig currencyConfig;

        [SerializeField] private string ingredientUIPrefabAddress;
        [SerializeField] private string rewardUIPrefabAddress;
        
        private CraftRecipe _recipe;
        private IPrefabFactory _prefabFactory;
        private InventorySystem _inventory;
        private ICurrencyService _currency;
        
        public void Setup(CraftRecipe recipe, IPrefabFactory prefabFactory)
        {
            _recipe = recipe;
            _prefabFactory = prefabFactory;
            _inventory = ServiceLocator.Get<InventorySystem>();
            _currency = ServiceLocator.Get<ICurrencyService>();

            RefreshUI();
        }
        
        private async void RefreshUI()
        {
            foreach (Transform child in ingredientsHolder) Destroy(child.gameObject);
            foreach (Transform child in rewardsHolder) Destroy(child.gameObject);

            bool canCraft = true;
            
            foreach (var ing in _recipe.ingredients)
            {
                var go = await _prefabFactory.SpawnAsync(ingredientUIPrefabAddress, Vector3.zero, Quaternion.identity, ingredientsHolder);
                var rect = go.transform as RectTransform;
                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;
                rect.localPosition = Vector3.zero;

                var ui = go.GetComponent<IngredientUIView>();
                ui.Setup(ing);
                if (!_inventory.HasItem(ing.itemData.itemId, ing.count))
                    canCraft = false;
            }

            foreach (var reward in _recipe.rewards)
            {
                var go = await _prefabFactory.SpawnAsync(rewardUIPrefabAddress, Vector3.zero, Quaternion.identity, rewardsHolder);
                var rect = go.transform as RectTransform;
                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;
                rect.localPosition = Vector3.zero;

                var ui = go.GetComponent<ResourceEntityUIView>();
                ui.Setup(reward.resourceType, reward.amount, currencyConfig.GetIcon(reward.resourceType));
            }

            craftButton.onClick.RemoveAllListeners();
            craftButton.interactable = canCraft;

            if (canCraft)
            {
                craftButton.onClick.AddListener(() =>
                {
                    // списываем ингредиенты
                    foreach (var ing in _recipe.ingredients)
                        _inventory.RemoveItem(ing.itemData.itemId, ing.count);

                    // выдаём награды
                    foreach (var reward in _recipe.rewards)
                        _currency.Add(reward.resourceType, reward.amount);

                    Debug.Log($"[Crafting] Crafted {_recipe.recipeName}!");
                    RefreshUI();
                });
            }
        }
    }
}