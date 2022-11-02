using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caller
{
    public static class HttpResponseExtensions
    {
        public static async Task ConsolePrint(this HttpResponseMessage response)
        {
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}
