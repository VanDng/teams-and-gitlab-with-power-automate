using Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caller
{
    public abstract class BaseCaller
    {
        protected readonly HttpClient _client;
        protected readonly ILogger _logger;

        protected BaseCaller()
        {
            _client = new HttpClient();
        }
    }
}
