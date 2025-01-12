using JIYUWU.Base.IRepository;
using JIYUWU.Base.IService;
using JIYUWU.Core.CacheManager;
using JIYUWU.Core.Common;
using JIYUWU.Core.Extension;
using JIYUWU.Core.Filter;
using JIYUWU.Entity.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace JIYUWU.Api.Controllers.Base
{
    [Route("Base_User")]
    public class Base_UserController: ApiBaseController<IBase_UserService>
    {
        private IBase_UserRepository _Repository;
        private IBase_UserService _Service;
        private ICacheService _CacheService;
        [ActivatorUtilitiesConstructor]
        public Base_UserController(IBase_UserService service,ICacheService cacheService,
                           IBase_UserRepository Repository)
       : base(service)
        {
            _Repository = Repository;
            _CacheService = cacheService;
            _Service = service;
        }
        [HttpGet, Route("getVierificationCode")]
        public IActionResult GetVierificationCode()
        {
            string code = "123";
            var data = new
            {
                img = "123",
                uuid = Guid.NewGuid()
            };
            var list = _Repository.SqlSugarClient.Queryable<Base_DbService>().ToList();
            HttpContext.GetService<IMemoryCache>().Set(data.uuid.ToString(), code, new TimeSpan(0, 5, 0));
            return Json(data);
        }
        [HttpPost, Route("Login")]
        [NoPermissionRequired]
        public IActionResult Login(UserLogin userLogin)
        {
            Base_User user = _Repository.SqlSugarClient.Queryable<Base_User>().Where(x => x.UserName == userLogin.username && x.UserPwd == userLogin.password).First();
            if (user == null)
            {
                return Error("用户名或密码错误");
            }
            else
            {
                var token = Guid.NewGuid().ToString();
                user.Token = token;
                int result =_Repository.SqlSugarClient.Updateable<Base_User>().SetColumns(it => new Base_User() { Token = token }).Where(it => it.UserId == user.UserId).ExecuteCommand();
                if (result == 0)
                {
                    return Error("登录失败");
                }
                bool re= _Service.SetUserInfo(user, token);
                if (!re)
                {
                    return Error("缓存设置失败");
                }
                var data = new { token };
                return Success(data);
            }
            //var data = new { ="42000019780217605X"};        }
        }
        [HttpGet, Route("Detail")]
        public IActionResult Detail()
        {
            var data = new { id = "210000198401045951", user_name="hangge", name="马克菠萝",avatar="http://www.baidu.com/avatar.jpg" };
            return Success(data);
        }
        [HttpGet, Route("Auth")]
        public IActionResult Auth()
        {
            #region json
            string json = @"[
        {
            ""name"": ""welcome"",
            ""meta"": {
                ""title"": ""欢迎页""
            }
        },
        {
            ""name"": ""list"",
            ""meta"": {
                ""title"": ""列表页""
            },
            ""children"": [
                {
                    ""name"": ""listSearch"",
                    ""meta"": {
                        ""title"": ""搜索列表""
                    },
                    ""children"": [
                        {
                            ""name"": ""listSearchArticles"",
                            ""meta"": {
                                ""title"": ""搜索列表（文章）""
                            }
                        },
                        {
                            ""name"": ""listSearchProjects"",
                            ""meta"": {
                                ""title"": ""搜索列表（项目）""
                            }
                        },
                        {
                            ""name"": ""listSearchApplications"",
                            ""meta"": {
                                ""title"": ""搜索列表（应用）""
                            }
                        }
                    ]
                },
                {
                    ""name"": ""listTable"",
                    ""meta"": {
                        ""title"": ""查询表格""
                    }
                },
                {
                    ""name"": ""listBasic"",
                    ""meta"": {
                        ""title"": ""标准列表""
                    }
                },
                {
                    ""name"": ""listCard"",
                    ""meta"": {
                        ""title"": ""卡片列表""
                    }
                }
            ]
        },
        {
            ""name"": ""result"",
            ""meta"": {
                ""title"": ""结果页""
            },
            ""children"": [
                {
                    ""name"": ""resultSuccess"",
                    ""meta"": {
                        ""title"": ""成功页""
                    }
                },
                {
                    ""name"": ""resultFail"",
                    ""meta"": {
                        ""title"": ""失败页""
                    }
                }
            ]
        },
        {
            ""name"": ""exception"",
            ""meta"": {
                ""title"": ""异常页""
            },
            ""children"": [
                {
                    ""name"": ""403"",
                    ""meta"": {
                        ""title"": ""403""
                    }
                },
                {
                    ""name"": ""404"",
                    ""meta"": {
                        ""title"": ""404""
                    }
                },
                {
                    ""name"": ""500"",
                    ""meta"": {
                        ""title"": ""500""
                    }
                }
            ]
        },{
        name: 'result',
        meta: {
            title: '结果页'
        },
        children: [
            {
                name: 'resultSuccess',
                meta: {
                    title: '成功页'
                },
            },
            {
                name: 'resultFail',
                meta: {
                    title: '失败页'
                },
            },
            ],
        },
        {
            ""name"": ""other"",
            ""meta"": {
                ""title"": ""其他""
            },
            ""children"": [
                {
                    ""name"": ""otherCustomLayout"",
                    ""meta"": {
                        ""title"": ""自定义框架""
                    }
                },
                {
                    ""name"": ""otherMultiTab"",
                    ""meta"": {
                        ""title"": ""多标签操作""
                    }
                },
                {
                    ""name"": ""otherBadge"",
                    ""meta"": {
                        ""title"": ""菜单徽标""
                    }
                }
            ]
        }
    ]";
            #endregion
            // 将 JSON 转换为对象
            List<RouteMenuItem> menuItems = JsonConvert.DeserializeObject<List<RouteMenuItem>>(json);

            return Success(menuItems);
        }
    }
}
