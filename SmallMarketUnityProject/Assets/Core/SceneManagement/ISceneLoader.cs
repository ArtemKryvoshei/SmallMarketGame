using System.Threading.Tasks;

namespace Core.SceneManagement
{
    public interface ISceneLoader
    {
        Task LoadSceneAsync(string sceneName);
    }
}