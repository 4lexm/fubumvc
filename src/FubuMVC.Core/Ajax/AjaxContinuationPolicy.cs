using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Ajax
{
    public class AjaxContinuationPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph
                .Behaviors
                .Where(IsAjaxContinuation)
                .Each(chain =>
                {
                    // Apply json formatting and http model binding coming up, but strip out
					if (chain.InputType() != null)
					{
						chain.Input.AllowHttpFormPosts = false;
						chain.Input.AddFormatter<JsonFormatter>();
					}

					chain.Output.ClearAll();
                    chain.Output.AddWriter(typeof (AjaxContinuationWriter<>));
                });
        }

        public static bool IsAjaxContinuation(BehaviorChain chain)
        {
            var outputType = chain.ActionOutputType();
            return outputType != null && outputType.CanBeCastTo<AjaxContinuation>();
        }
    }


}