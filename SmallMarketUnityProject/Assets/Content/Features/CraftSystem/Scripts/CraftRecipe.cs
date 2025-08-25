using System;
using System.Collections.Generic;
using Content.Features.BuildingSystem.Scripts;
using UnityEngine;

namespace Content.Features.CraftSystem.Scripts
{
    [CreateAssetMenu(menuName = "Game/Craft Recipe", fileName = "CraftRecipe")]
    public class CraftRecipe : ScriptableObject
    {
        public string recipeId;
        public string recipeName;

        [Serializable]
        public class Ingredient
        {
            public ItemData itemData;
            public int count;
        }

        public List<Ingredient> ingredients = new List<Ingredient>();
        public List<ResourceAmount> rewards = new List<ResourceAmount>();
    }
}