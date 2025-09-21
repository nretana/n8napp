using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

namespace N8N.API.Utilities.Formatters
{
    public static class JsonPatchInputFormatter
    {
        public static NewtonsoftJsonPatchInputFormatter GetJsonPtachInputFormatter()
        {
            var builder = new ServiceCollection()
                                .AddLogging()
                                .AddMvc()
                                .AddNewtonsoftJson()
                                .Services.BuildServiceProvider();
            return builder.GetRequiredService<IOptions<MvcOptions>>()
                          .Value
                          .InputFormatters
                          .OfType<NewtonsoftJsonPatchInputFormatter>()
                          .First();
        }
    }
}
