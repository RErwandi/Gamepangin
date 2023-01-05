using System;
using System.Threading.Tasks;

namespace Gamepangin
{
    public interface IGameSave
    {
        string SaveId { get; }
        bool IsShared { get; }
        Type SaveType { get; }
        object SaveData { get; }
        LoadMode LoadMode { get; }
        Task OnLoad(object value);
    }
}