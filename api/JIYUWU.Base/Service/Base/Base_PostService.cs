﻿using JIYUWU.Base.IRepository;
using JIYUWU.Base.IService;
using JIYUWU.Core.CacheManager;
using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Entity.Base;
using Microsoft.AspNetCore.Http;

namespace JIYUWU.Base.Service
{
    public class Base_PostService
        : ServiceBase<Base_Post, IBase_PostRepository>
    , IBase_PostService, IDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBase_PostRepository _repository; //访问数据库

        public Base_PostService(IBase_PostRepository repository,
            IHttpContextAccessor httpContextAccessor)
        : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
            Init(repository);
        }

        public static IBase_PostService Instance
        {
            get { return AutofacContainerModule.GetService<IBase_PostService>(); }
        }
    }
}
