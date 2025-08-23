
namespace Core.JSONSaveLoadSystem
{
    public interface ISaveLoadSystem
    {
        void Save(DTOModels data);
        DTOModels Load();
        void DeleteSave();
    }
}
