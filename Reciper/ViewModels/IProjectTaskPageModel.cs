using CommunityToolkit.Mvvm.Input;
using Reciper.Models;

namespace Reciper.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}