using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.UseCases.Queries;
using System.Threading.Tasks;

namespace DesignPatternMaster.UI.ViewModels
{
    public partial class PatternDetailViewModel : ObservableObject
    {
        private readonly GetPatternDetailQuery _getPatternDetailQuery;

        [ObservableProperty]
        private DesignPattern? _selectedPattern;

        public PatternDetailViewModel(GetPatternDetailQuery getPatternDetailQuery)
        {
            _getPatternDetailQuery = getPatternDetailQuery;
        }

        public async Task LoadPatternAsync(string id)
        {
            SelectedPattern = await _getPatternDetailQuery.ExecuteAsync(id);
        }
    }
}
