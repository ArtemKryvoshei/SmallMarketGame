using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Core.SceneManagement
{
    public class SceneLoader : ISceneLoader
    {
        public async Task LoadSceneAsync(string sceneName)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = true;

            while (!operation.isDone)
            {
                await Task.Yield();
            }
        }
    }
}