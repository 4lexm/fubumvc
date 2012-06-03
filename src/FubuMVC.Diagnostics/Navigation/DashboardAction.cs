using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Features.Dashboard;

namespace FubuMVC.Diagnostics.Navigation
{
    [MarkedForTermination]
    public class DashboardAction : NavigationItemBase
    {
        public DashboardAction(BehaviorGraph graph, IEndpointService endpointService) 
            : base(graph, endpointService)
        {
        }

        public override int Rank
        {
            get { return int.MinValue; }
        }

        protected override object inputModel()
        {
            return new DashboardRequestModel();
        }
    }
}