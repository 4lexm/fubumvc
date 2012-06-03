﻿using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Navigation
{
    [MarkedForTermination]
    public abstract class NavigationItemBase : INavigationItemAction
    {
        private readonly BehaviorGraph _graph;
        private readonly IEndpointService _endpointService;
        private readonly Lazy<ActionCall> _actionCall;

        protected NavigationItemBase(BehaviorGraph graph, IEndpointService endpointService)
        {
            _graph = graph;
            _endpointService = endpointService;
            _actionCall = new Lazy<ActionCall>(() => _graph.Behaviors.SingleOrDefault(chain => chain.InputType() == inputModel().GetType()).FirstCall());
        }

        protected abstract object inputModel();

        public virtual int Rank
        {
            get { return 1; }
        }

        public virtual string Text()
        {
            return _actionCall
                .Value
                .InputType()
                .Name
                .Replace("RequestModel", string.Empty);
        }

        public virtual string Url()
        {
            return _endpointService
                .EndpointFor(inputModel())
                .Url;
        }
    }
}