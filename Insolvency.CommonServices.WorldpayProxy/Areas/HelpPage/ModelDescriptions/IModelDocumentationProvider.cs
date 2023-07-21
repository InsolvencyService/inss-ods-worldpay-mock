using System;
using System.Reflection;

namespace Insolvency.CommonServices.WorldpayProxy.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}