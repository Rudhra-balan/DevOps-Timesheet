using System.Collections.Generic;
using NetEscapades.AspNetCore.SecurityHeaders.Headers;

namespace HI.DevOps.Web.Common.HeaderPolicy
{
    public class HeaderPolicyCollection : Dictionary<string, IHeaderPolicy>
    {
    }
}