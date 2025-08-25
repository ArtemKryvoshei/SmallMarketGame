using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Features.NPCLogic.Scripts
{
    [System.Serializable]
    public class EmotionData
    {
        public int id;
        public Sprite icon;
    }
    
    public class NPCEmotionController : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private Image emotionHolder;      
        [SerializeField] private List<EmotionData> emotions;
        [SerializeField] private float interval = 5f;      
        [SerializeField] private float visibleTime = 2f;   

        private int _lastEmotionId = -1;
        private Coroutine _routine;

        private void OnEnable()
        {
            if (_routine == null)
                _routine = StartCoroutine(ShowEmotionsRoutine());
        }

        private void OnDisable()
        {
            if (_routine != null)
                StopCoroutine(_routine);
            _routine = null;
            if (emotionHolder != null)
                emotionHolder.gameObject.SetActive(false);
        }

        private IEnumerator ShowEmotionsRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(interval);

                var emotion = GetRandomEmotion();
                if (emotion != null && emotionHolder != null)
                {
                    emotionHolder.sprite = emotion.icon;
                    emotionHolder.gameObject.SetActive(true);

                    yield return new WaitForSeconds(visibleTime);

                    emotionHolder.gameObject.SetActive(false);
                }
            }
        }

        private EmotionData GetRandomEmotion()
        {
            if (emotions.Count == 0) return null;

            EmotionData selected;
            do
            {
                selected = emotions[Random.Range(0, emotions.Count)];
            } 
            while (selected.id == _lastEmotionId && emotions.Count > 1);

            _lastEmotionId = selected.id;
            return selected;
        }
    }
}