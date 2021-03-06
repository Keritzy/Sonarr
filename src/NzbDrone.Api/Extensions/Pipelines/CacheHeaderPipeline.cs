﻿using Nancy;
using Nancy.Bootstrapper;
using NzbDrone.Api.Frontend;

namespace NzbDrone.Api.Extensions.Pipelines
{
    public class CacheHeaderPipeline : IRegisterNancyPipeline
    {
        private readonly ICacheableSpecification _cacheableSpecification;

        public CacheHeaderPipeline(ICacheableSpecification cacheableSpecification)
        {
            _cacheableSpecification = cacheableSpecification;
        }

        public void Register(IPipelines pipelines)
        {
            pipelines.AfterRequest.AddItemToStartOfPipeline(c => Handle(c));
        }

        private void Handle(NancyContext context)
        {
            if (_cacheableSpecification.IsCacheable(context))
            {
                context.Response.Headers.EnableCache();
            }
            else
            {
                context.Response.Headers.DisableCache();
            }
        }
    }
}
