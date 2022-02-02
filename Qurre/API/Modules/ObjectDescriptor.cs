using System;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
namespace Qurre.API.Modules
{
    internal sealed class ObjectDescriptor : IObjectDescriptor
    {
        private readonly IObjectDescriptor innerDescriptor;
        public ObjectDescriptor(IObjectDescriptor innerDescriptor, string comment)
        {
            this.innerDescriptor = innerDescriptor;
            Comment = comment;
        }
        public string Comment { get; private set; }
        public object Value => innerDescriptor.Value;
        public Type Type => innerDescriptor.Type;
        public Type StaticType => innerDescriptor.StaticType;
        public ScalarStyle ScalarStyle => innerDescriptor.ScalarStyle;
    }
}