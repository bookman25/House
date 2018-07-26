using System;
using HouseService.AutomationBase;

namespace HouseService.Api.Models
{
    public abstract class AutomationViewModel : ODataObject
    {
        private readonly Automation automation;

        protected AutomationViewModel(Automation automation)
            : base(automation.Id)
        {
            this.automation = automation;

            IsEnabled = automation.IsEnabled;
        }

        public string Title => automation.Name;

        public AutomationType Type => automation.Type;

        public bool IsEnabled { get; set; }

        public string Status { get; set; }

        public string EnabledClass => automation.IsEnabled ? "automation-on" : "automation-off";

        public virtual string Icon { get; }
    }
}
