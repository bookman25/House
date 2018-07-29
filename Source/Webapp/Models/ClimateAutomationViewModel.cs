using System;
using HouseService.AutomationBase;

namespace HouseService.Api.Models
{
    public class ClimateAutomationViewModel : AutomationViewModel
    {
        private readonly ClimateAutomation climateAutomation;

        public ClimateAutomationViewModel(Automation automation)
            : base(automation)
        {
            climateAutomation = (ClimateAutomation)automation;

            if (HoldTemp != null)
            {
                Status = $"{climateAutomation.CurrentTemp}. Holding at {HoldTemp} until {HoldEndTime.GetValueOrDefault().ToShortTimeString()}";
            }
            else
            {
                Status = $"Temp set to {climateAutomation.CurrentTemp}";
            }
        }

        public long? HoldTemp => climateAutomation.HoldTemp;

        public DateTime? HoldEndTime => climateAutomation.HoldEndTime;

        public override string Icon => "thermostat_cold.svg";
    }
}
