using System.Collections.Generic;
using Content.Features.CursorChangerSystem.Scripts;
using Core.IInitializeQueue;
using Core.Other;
using UnityEngine;

[System.Serializable]
public class CursorData
{
    public CursorType Type;
    public Texture2D Texture;
    public Vector2 Hotspot;
}

namespace Content.Features.CursorChangerSystem.Scripts
{
    public class CursorManager : InitializeableMonoComponent
    {
        [SerializeField] private LayerMask cursorLayer; 
        [SerializeField] private List<CursorData> cursorDataList;
        
        private Dictionary<CursorType, CursorData> _cursorDict;
        private CursorType _currentCursor = CursorType.Craft;
        
        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_INGAME_COMPONENTS - 1;
        }

        public override void Initialize()
        {
            if (initialized)
            {
                SetCursor(CursorType.Default);
                Debug.Log("[CursorManager] Already initialized.");
                return;
            }

            DontDestroyOnLoad(this.gameObject);
            _cursorDict = new Dictionary<CursorType, CursorData>();
            foreach (var data in cursorDataList)
            {
                if (!_cursorDict.ContainsKey(data.Type))
                    _cursorDict[data.Type] = data;
            }

            SetCursor(CursorType.Default);
            base.Initialize();
            initialized = true;
        }
        
        private void Update()
        {
            if (initialized)
            {
                UpdateCursor();
            }
        }
        
        private void UpdateCursor()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, cursorLayer))
            {
                var changer = hit.collider.GetComponent<ICursorChanger>();
                if (changer != null)
                {
                    SetCursor(changer.GetCursorType());
                    return;
                }
            }

            // Если ничего не нашли — ставим дефолт
            SetCursor(CursorType.Default);
        }

        private void SetCursor(CursorType type)
        {
            if (_currentCursor == type) return;

            if (_cursorDict.TryGetValue(type, out var data))
            {
                Cursor.SetCursor(data.Texture, data.Hotspot, CursorMode.Auto);
                _currentCursor = type;
            }
        }
    }
}