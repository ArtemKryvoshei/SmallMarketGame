using UnityEngine;
using UnityEngine.UI;

namespace Content.Features.BuildingSystem.Scripts
{
    public class CashierProgressBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage; 

        public void Show(float duration)
        {
            gameObject.SetActive(true);
            fillImage.fillAmount = 0f;
        }

        public void UpdateProgress(float normalizedValue)
        {
            fillImage.fillAmount = Mathf.Clamp01(normalizedValue);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}