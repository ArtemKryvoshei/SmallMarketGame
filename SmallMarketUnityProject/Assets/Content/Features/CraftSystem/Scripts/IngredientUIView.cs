using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Features.CraftSystem.Scripts
{
    public class IngredientUIView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text countText;
        
        public void Setup(CraftRecipe.Ingredient ingredient)
        {
            var data = ingredient.itemData;
            if (data != null)
            {
                icon.sprite = data.icon;
                countText.text = $"x{ingredient.count}";
            }
            else
            {
                icon.sprite = null;
                countText.text = "?";
            }
        }
    }
}