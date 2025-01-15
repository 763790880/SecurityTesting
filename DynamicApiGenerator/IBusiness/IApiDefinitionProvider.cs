using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DynamicApiGenerator
{
    public interface IApiDefinitionProvider
    {
        IEnumerable<ApiDefinition> GetApiDefinitions();
    }
    [Dynamic]
    internal class TestApiDefinitionProvider: IApiDefinitionProvider, ISingletonDynameicDependency
    {
        public IEnumerable<ApiDefinition> GetApiDefinitions()
        {
            yield return new ApiDefinition
            {
                Name = "Test",
                Path = "test",
                Method = HttpMethod.Get,
                RequestType = typeof(JObject),
                ResponseType = typeof(JObject)
            };
        }
    }
}