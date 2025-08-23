using Content.Features.NPCSpawner.Scripts;

namespace Content.Features.NPCLogic.Scripts
{
    public interface ICustomerState
    {
        void Enter(CustomerNPC customer);
        void Update(CustomerNPC customer);
        void Exit(CustomerNPC customer);
    }
}