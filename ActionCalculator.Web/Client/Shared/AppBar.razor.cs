using Microsoft.AspNetCore.Components;

namespace ActionCalculator.Web.Client.Shared
{
    public partial class AppBar
    {
        private bool _isLightMode;
        private const string Key = "IsLightMode";
        
        [Parameter]
        public EventCallback OnSidebarToggled { get; set; }

        [Parameter]
        public EventCallback<bool> OnThemeToggled { get; set; }

        [Inject]
        public Blazored.LocalStorage.ILocalStorageService LocalStore { get; set; } = null!;
        
        private async Task ToggleTheme()
        {
            _isLightMode = !_isLightMode;

            await LocalStore.SetItemAsync(Key, _isLightMode);

            await OnThemeToggled.InvokeAsync(_isLightMode);
        }

        protected override async Task OnInitializedAsync()
        {
            _isLightMode = await LocalStore.GetItemAsync<bool>(Key);
        }
    }
}