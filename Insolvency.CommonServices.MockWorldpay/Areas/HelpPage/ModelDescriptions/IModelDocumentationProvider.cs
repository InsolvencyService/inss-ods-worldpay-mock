using System;
using System.Reflection;

namespace Insolvency.CommonServices.MockWorldpay.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}