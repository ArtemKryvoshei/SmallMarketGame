using System.Threading.Tasks;
using Core.SceneManagement;

namespace Core.SceneTransitionMediator
{
    public interface ISceneTransitionMediator
    {
        Task LoadSceneWithTransitionAsync(string sceneName);
    }
}