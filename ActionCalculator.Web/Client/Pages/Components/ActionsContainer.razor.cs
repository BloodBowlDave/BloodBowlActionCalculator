using Microsoft.AspNetCore.Components;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.Web.Client.Pages.Components
{
    public partial class ActionsContainer
    {
        [Parameter]
        public EventCallback<Action> OnAddAction { get; set; }
    }
}
