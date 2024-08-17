using Microsoft.AspNetCore.Hosting;

namespace JIYUWU.Core.Extension
{
    public static class ServerExtension
    {
        /// <summary>
        /// 返回的路径后面不带/，拼接时需要自己加上/
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MapPath(this string path)
        {
            return MapPath(path, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="rootPath">获取wwwroot路径</param>
        /// <returns></returns>
        public static string MapPath(this string path, bool rootPath)
        {
            return AutofacContainerModule.GetService<IPathProvider>().MapPath(path, rootPath);
        }
    }
    public interface IPathProvider : IDependency
    {
        string MapPath(string path);
        string MapPath(string path, bool rootPath);
        IWebHostEnvironment GetHostingEnvironment();
    }

    public class PathProvider : IPathProvider
    {
        private IWebHostEnvironment _hostingEnvironment;

        public PathProvider(IWebHostEnvironment environment)
        {
            _hostingEnvironment = environment;
        }
        public IWebHostEnvironment GetHostingEnvironment()
        {
            return _hostingEnvironment;
        }

        public string MapPath(string path)
        {
            return MapPath(path, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="rootPath">获取wwwroot路径</param>
        /// <returns></returns>
        public string MapPath(string path, bool rootPath)
        {
            if (rootPath)
            {
                if (_hostingEnvironment.WebRootPath == null)
                {
                    _hostingEnvironment.WebRootPath = _hostingEnvironment.ContentRootPath + "/wwwroot".ReplacePath();
                }
                return Path.Combine(_hostingEnvironment.WebRootPath, path).ReplacePath();
            }
            return Path.Combine(_hostingEnvironment.ContentRootPath, path).ReplacePath();
        }
    }
}
