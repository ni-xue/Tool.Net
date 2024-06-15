using System;
using System.Collections.Generic;
using Tool.Utils.ActionDelegate;

namespace Tool.Web.Api.ApiCore
{
    internal class AahxCore
    {
        internal AahxCore(Func<IServiceProvider, IHttpAsynApi> newclass, Func<IMinHttpAsynApi> classmin, IActionDispatcher<IAshxAction> action, AshxState state)
        {
            this.action = action;
            this.newclass = newclass;
            this.classmin = classmin;

            parameters = GetParameter();

            ApiParameter[] GetParameter()
            {
                bool isbody = false;
                List<ApiParameter> apis = new();
                foreach (var item in this.action.Parameters)
                {
                    apis.Add(new ApiParameter(item, state, ref isbody));
                }
                return apis.ToArray();
            }
        }

        internal IActionDispatcher<IAshxAction> action;
        internal ApiParameter[] parameters;

        internal Func<IServiceProvider, IHttpAsynApi> newclass;
        internal Func<IMinHttpAsynApi> classmin;
    }
}
