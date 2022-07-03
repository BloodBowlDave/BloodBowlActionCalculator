using ActionCalculator.Abstractions;
using ActionCalculator.Models;
using Microsoft.AspNetCore.Components;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.Web.Client.Pages.Components.Actions
{
    public partial class Rerollable
    {
        private IActionBuilder? _actionBuilder;
        
        [Parameter]
        public EventCallback<Action> OnAddAction { get; set; }

        [Inject]
        protected IActionBuilderFactory ActionBuilderFactory { get; set; } = null!;

        private void AddAction(int roll)
        {
            _actionBuilder ??= ActionBuilderFactory.GetActionBuilder(ActionType.Rerollable);

            OnAddAction.InvokeAsync(_actionBuilder.Build(roll.ToString()));
        }
    }
}
