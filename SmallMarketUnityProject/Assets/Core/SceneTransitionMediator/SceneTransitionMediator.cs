using System.Threading.Tasks;
using Core.LoadingScreenService;
using Core.Other;
using Core.SceneManagement;
using UnityEngine;

namespace Core.SceneTransitionMediator
{
    public class SceneTransitionMediator : ISceneTransitionMediator
    {
        private readonly ILoadingScreenService _loadingScreen;
        private readonly ISceneLoader _sceneLoader;

        public SceneTransitionMediator(ILoadingScreenService loadingScreen, ISceneLoader sceneLoader)
        {
            _loadingScreen = loadingScreen;
            _sceneLoader = sceneLoader;
        }

        public async Task LoadSceneWithTransitionAsync(string sceneName)
        {
            _loadingScreen.Show();

            await _sceneLoader.LoadSceneAsync(sceneName);

            //фейковая задержка перед скрытием загрузочного экрана
            await Task.Delay((int)(ConstantsHolder.LOAD_SCREEN_SHOW_TIME * 1000));
            
            _loadingScreen.Hide();
        }
    }
}