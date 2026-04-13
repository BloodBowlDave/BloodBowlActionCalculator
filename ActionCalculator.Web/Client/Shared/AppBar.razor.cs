using Microsoft.AspNetCore.Components;

namespace ActionCalculator.Web.Client.Shared
{
    public partial class AppBar
    {
        private bool _isLightMode;
        private string _season = "Season 3";

        private const string ThemeKey = "IsLightMode";
        private const string SeasonKey = "Season";

        [Parameter]
        public EventCallback OnSidebarToggled { get; set; }

        [Parameter]
        public EventCallback<bool> OnThemeToggled { get; set; }

        [Parameter]
        public EventCallback<string> OnSeasonChanged { get; set; }

        [Inject]
        public Blazored.LocalStorage.ILocalStorageService LocalStore { get; set; } = null!;

        private async Task ToggleTheme()
        {
            _isLightMode = !_isLightMode;
            await LocalStore.SetItemAsync(ThemeKey, _isLightMode);
            await OnThemeToggled.InvokeAsync(_isLightMode);
        }

        private async Task SeasonChanged(string season)
        {
            _season = season;
            await LocalStore.SetItemAsync(SeasonKey, season);
            await OnSeasonChanged.InvokeAsync(season);
        }

        protected override async Task OnInitializedAsync()
        {
            _isLightMode = await LocalStore.GetItemAsync<bool>(ThemeKey);

            var savedSeason = await LocalStore.GetItemAsync<string>(SeasonKey);
            _season = string.IsNullOrEmpty(savedSeason) ? "Season 3" : savedSeason;
        }
    }
}