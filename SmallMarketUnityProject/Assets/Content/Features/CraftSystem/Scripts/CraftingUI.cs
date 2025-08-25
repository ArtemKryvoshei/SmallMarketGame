using System.Collections.Generic;
using Core.IInitializeQueue;
using Core.Other;
using Core.PrefabFactory;
using Core.ServiceLocatorSystem;
using UnityEngine;

namespace Content.Features.CraftSystem.Scripts
{
    public class CraftingUI : InitializeableMonoComponent
    {
        [Header("References")]
        [SerializeField] private CraftRecipe[] recipes;
        [SerializeField] private Transform holder;
        [SerializeField] private string recipeUIPrefabAddress;
        
        private IPrefabFactory _prefabFactory;

        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_LASTINIT_COMPONENTS - 25;
            base.SetupPriority();
        }

        public override void Initialize()
        {
            _prefabFactory = ServiceLocator.Get<IPrefabFactory>();
            base.Initialize();
            RefreshUI();
        }
        
        public async void RefreshUI()
        {
            foreach (Transform child in holder)
                Destroy(child.gameObject);

            foreach (var recipe in recipes)
            {
                var go = await _prefabFactory.SpawnAsync(recipeUIPrefabAddress, Vector3.zero, Quaternion.identity, holder);
                var rect = go.transform as RectTransform;
                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;
                rect.localPosition = Vector3.zero;

                var ui = go.GetComponent<RecipeUIView>();
                ui.Setup(recipe, _prefabFactory);
            }
        }
    }
}