using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IdentifiedProvider?> Identify(string id)
        {
            foreach (var provider in _notificationProviderServices)
            {
                var matchResult = await provider.MatchId(id);
                if (matchResult.IsSuccess)
                {
                    return new IdentifiedProvider
                    {
                        Provider = provider,
                        MatchData = matchResult.MatchData,
                    };
                }
            }

            return null;
        }
    }
}
