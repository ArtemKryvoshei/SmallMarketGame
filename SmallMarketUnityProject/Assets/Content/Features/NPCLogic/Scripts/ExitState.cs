using Content.Features.NPCSpawner.Scripts;
using UnityEngine;

namespace Content.Features.NPCLogic.Scripts
{
    public class ExitState : ICustomerState
    {
        private Transform _exit;

        public void Enter(CustomerNPC npc)
        {
            _exit = GameObject.FindObjectOfType<NPCSpawner.Scripts.NPCSpawner>().transform;
            
            if (_exit != null)
                npc.MoveTo(_exit.position);
        }

        public void Update(CustomerNPC npc)
        {
            if (npc.IsReachedTarget())
            {
                npc.Release();
                GameObject.Destroy(npc.gameObject);
            }
        }

        public void Exit(CustomerNPC npc) { }
    }
}