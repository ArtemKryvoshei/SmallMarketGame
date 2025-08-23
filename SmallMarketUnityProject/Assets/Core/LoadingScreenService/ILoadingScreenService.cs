using System.Threading.Tasks;

namespace Core.LoadingScreenService
{
    public interface ILoadingScreenService
    {
        Task InitializeAsync(string address);
        void Show();
        void Hide();
    }
}