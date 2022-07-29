using Microsoft.AspNetCore.Components;

namespace ActionCalculator.Web.Client.Shared
{
    public partial class AppBar
    {
        private bool _isLightMode = true;

        [Parameter]
        public EventCallback OnSidebarToggled { get; set; }

        [Parameter]
        public EventCallback<bool> OnThemeToggled { get; set; }

        private async Task ToggleTheme()
        {
            _isLightMode = !_isLightMode;

            await OnThemeToggled.InvokeAsync(_isLightMode);
        }
        
        private void ToggleSidebar()
        {
            OnSidebarToggled.InvokeAsync();
        }
    }
}