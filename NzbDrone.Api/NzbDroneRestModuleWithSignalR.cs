﻿using NzbDrone.Api.REST;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Datastore.Events;
using NzbDrone.Core.Messaging;
using NzbDrone.SignalR;

namespace NzbDrone.Api
{
    public abstract class NzbDroneRestModuleWithSignalR<TResource, TModel> : NzbDroneRestModule<TResource>, IHandle<ModelEvent<TModel>>
        where TResource : RestResource, new()
        where TModel : ModelBase
    {
        private readonly IMessageAggregator _messageAggregator;

        protected NzbDroneRestModuleWithSignalR(IMessageAggregator messageAggregator)
        {
            _messageAggregator = messageAggregator;
        }

        public void Handle(ModelEvent<TModel> message)
        {
            if (message.Action == ModelAction.Deleted || message.Action == ModelAction.Sync)
            {
                BroadcastResourceChange(message.Action);
            }

            BroadcastResourceChange(message.Action, message.Model.Id);
        }

        protected void BroadcastResourceChange(ModelAction action, int id)
        {
            var resource = GetResourceById(id);

            var signalRMessage = new SignalRMessage
            {
                Name = Resource,
                Body = new ResourceChangeMessage<TResource>(resource, action)
            };

            _messageAggregator.PublishCommand(new BroadcastSignalRMessage(signalRMessage));
        }

        protected void BroadcastResourceChange(ModelAction action)
        {
            var signalRMessage = new SignalRMessage
            {
                Name = Resource,
                Body = new ResourceChangeMessage<TResource>(action)
            };

            _messageAggregator.PublishCommand(new BroadcastSignalRMessage(signalRMessage));
        }
    }
}