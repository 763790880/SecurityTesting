using System;
using System.Net.Http;

namespace DynamicApiGenerator
{
    public class ApiDefinition
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public HttpMethod Method { get; set; }
        public Type RequestType { get; set; }
        public Type ResponseType { get; set; }
        // Add more properties as needed...
    }
}