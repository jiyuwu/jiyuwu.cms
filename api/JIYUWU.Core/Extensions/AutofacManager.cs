using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIYUWU.Core.Extensions
{
    public class AutofacContainerModule
    {
        public static TService GetService<TService>() where TService : class
        {
            return typeof(TService).GetService() as TService;
        }
    }
    public static class ServiceProviderManagerExtension
    {
        public static object GetService(this Type serviceType)
        {
            // HttpContext.Current.RequestServices.GetRequiredService<T>(serviceType);
            return Common.HttpContext.Current.RequestServices.GetService(serviceType);
        }

    }
    public interface IDependency
    {
    }
}
