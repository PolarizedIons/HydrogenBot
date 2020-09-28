using System;
using System.Collections.Generic;
using System.Linq;
using HydrogenBot.Extentions;
using HydrogenBot.Models;
using Microsoft.Extensions.DependencyInjection;

namespace HydrogenBot.Services
{
    public class ProviderManagerService : ISingletonDiService
    {
        private readonly IEnumerable<IProvider> _notificationProviderServices;

        public ProviderManagerService(IServiceProvider services)
        {
            _notificationProviderServices = typeof(IProvider).GetAllInAssembly()
                .Select(x => (IProvider) services.GetRequiredService(x)).ToArray();
        }

        public IProvider? Identify(string id, out object? matchData)
        {
            foreach (var provider in _notificationProviderServices)
            {
                var matchResult = provider.MatchId(id);
                if (matchResult.IsSuccess)
                {
                    matchData = matchResult.MatchData;
                    return provider;
                }
            }

            matchData = null;
            return null;
        }
    }
}
