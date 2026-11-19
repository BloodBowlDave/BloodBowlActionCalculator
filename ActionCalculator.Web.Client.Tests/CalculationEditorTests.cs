using ActionCalculator.Abstractions;
using ActionCalculator.Models;
using ActionCalculator.Models.Actions;
using ActionCalculator.Web.Client.Editing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ActionCalculator.Web.Client.Tests
{
    public class CalculationEditorTests
    {
        private readonly CalculationEditor _editor;
        private readonly IActionParserFactory _actionParserFactory;

        public CalculationEditorTests()
        {
            var services = new ServiceCollection();
            services.AddActionCalculatorServices();
            services.AddTransient<CalculationEditor>();

            var serviceProvider = services.BuildServiceProvider();

            _editor = serviceProvider.GetRequiredService<CalculationEditor>();
            _actionParserFactory = serviceProvider.GetRequiredService<IActionParserFactory>();
        }

        [Fact]
        public void RemoveAction_OfTheOnlyActionKeepsTheCurrentPlayer()
        {
            var player = new Player(Guid.NewGuid(), CalculatorSkills.Dodge, 0, 0, 0, 0);
            _editor.ChangePlayer(player);
            AddActions("D3");

            _editor.RemoveAction(0);

            Assert.Empty(_editor.CurrentCalculation.PlayerActions);
            Assert.Equal(player.Id, _editor.CurrentPlayer.Id);
        }

        [Fact]
        public void RemoveAction_OfTheOnlyActionSwitchesToTheNextSavedCalculation()
        {
            AddActions("D3");
            _editor.SaveCalculation(0);

            _editor.RemoveAction(0);

            Assert.Single(_editor.Calculations);
            Assert.Equal("D3", _editor.CurrentCalculation.ToString());
        }

        [Fact]
        public void ClearCalculation_OfTheOnlyCalculationKeepsTheSeason()
        {
            _editor.SetSeason(Season.Season2);
            AddActions("D3");

            _editor.ClearCalculation(0);

            Assert.Single(_editor.Calculations);
            Assert.Empty(_editor.CurrentCalculation.PlayerActions);
            Assert.Equal(Season.Season2, _editor.CurrentCalculation.Season);
        }

        [Fact]
        public void SaveCalculation_CopiesActionsRerollsAndSeason()
        {
            _editor.SetSeason(Season.Season2);
            AddActions("D3", "2D2");
            _editor.SetRerolls(0, 2);

            _editor.SaveCalculation(0);

            var saved = _editor.Calculations[1];
            Assert.Equal("D3,2D2~S2", saved.ToString());
            Assert.Equal(2, saved.Rerolls);
            Assert.NotSame(_editor.CurrentCalculation.PlayerActions, saved.PlayerActions);
        }

        [Fact]
        public void SetPushes_AddsAndRemovesTheFrenzyFollowUpBlock()
        {
            AddActions("2D2");

            _editor.SetPushes(0, 2);

            Assert.Equal(2, _editor.CurrentCalculation.PlayerActions.Count);
            Assert.True(_editor.IsFrenzyFollowUp(1));

            _editor.SetPushes(0, 0);

            Assert.Single(_editor.CurrentCalculation.PlayerActions);
        }

        [Fact]
        public void RemoveAction_OfTheFrenzyFollowUpClearsThePushes()
        {
            AddActions("2D2");
            _editor.SetPushes(0, 2);

            _editor.RemoveAction(1);

            var block = (Block)Assert.Single(_editor.CurrentCalculation.PlayerActions).Action;
            Assert.Equal(0, block.NumberOfNonCriticalFailures);
        }

        private void AddActions(params string[] tokens)
        {
            foreach (var token in tokens)
            {
                _editor.AddAction(_actionParserFactory.GetActionParser(token).Parse(token));
            }
        }
    }
}
