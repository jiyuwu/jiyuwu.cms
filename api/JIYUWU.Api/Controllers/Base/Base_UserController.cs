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
            ""name"": ""form"",
            ""meta"": {
                ""title"": ""表单页""
            },
            ""children"": [
                {
                    ""name"": ""formBasic"",
                    ""meta"": {
                        ""title"": ""基础表单"",
                        ""actions"": [
                            ""*""
                        ]
                    }
                },
                {
                    ""name"": ""formStep"",
                    ""meta"": {
                        ""title"": ""分步表单""
                    }
                },
                {
                    ""name"": ""formAdvanced"",
                    ""meta"": {
                        ""title"": ""高级表单""
                    }
                }
            ]
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
            ""name"": ""profile"",
            ""meta"": {
                ""title"": ""详情页""
            },
            ""children"": [
                {
                    ""name"": ""profileBasic"",
                    ""meta"": {
                        ""title"": ""基础详情页""
                    }
                },
                {
                    ""name"": ""profileAdvanced"",
                    ""meta"": {
                        ""title"": ""高级详情页""
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
        },
        {
            ""name"": ""user"",
            ""meta"": {
                ""title"": ""个人页""
            },
            ""children"": [
                {
                    ""name"": ""userCenter"",
                    ""meta"": {
                        ""title"": ""个人中心""
                    }
                },
                {
                    ""name"": ""userSetting"",
                    ""meta"": {
                        ""title"": ""个人设置""
                    }
                }
            ]
        },
        {
            ""name"": ""system"",
            ""meta"": {
                ""title"": ""系统管理""
            },
            ""children"": [
                {
                    ""name"": ""systemUser"",
                    ""meta"": {
                        ""title"": ""成员与部门""
                    }
                },
                {
                    ""name"": ""systemRole"",
                    ""meta"": {
                        ""title"": ""角色管理""
                    }
                },
                {
                    ""name"": ""systemMenu"",
                    ""meta"": {
                        ""title"": ""菜单管理""
                    }
                },
                {
                    ""name"": ""systemNewMenu"",
                    ""meta"": {
                        ""title"": ""新版菜单管理""
                    }
                },
                {
                    ""name"": ""systemDict"",
                    ""meta"": {
                        ""title"": ""字典管理""
                    }
                }
            ]
        },
        {
            ""name"": ""components"",
            ""meta"": {
                ""title"": ""组件""
            },
            ""children"": [
                {
                    ""name"": ""actionButton"",
                    ""meta"": {
                        ""title"": ""操作按钮""
                    }
                },
                {
                    ""name"": ""breadcrumb"",
                    ""meta"": {
                        ""title"": ""面包屑""
                    }
                },
                {
                    ""name"": ""button"",
                    ""meta"": {
                        ""title"": ""按钮""
                    }
                },
                {
                    ""name"": ""cascader"",
                    ""meta"": {
                        ""title"": ""级联选择""
                    }
                },
                {
                    ""name"": ""chart"",
                    ""meta"": {
                        ""title"": ""图表""
                    }
                },
                {
                    ""name"": ""cropper"",
                    ""meta"": {
                        ""title"": ""裁剪""
                    }
                },
                {
                    ""name"": ""editor"",
                    ""meta"": {
                        ""title"": ""编辑器""
                    }
                },
                {
                    ""name"": ""ellipsis"",
                    ""meta"": {
                        ""title"": ""文本省略""
                    }
                },
                {
                    ""name"": ""filters"",
                    ""meta"": {
                        ""title"": ""筛选""
                    }
                },
                {
                    ""name"": ""formTable"",
                    ""meta"": {
                        ""title"": ""表单表格""
                    }
                },
                {
                    ""name"": ""grid"",
                    ""meta"": {
                        ""title"": ""网格""
                    }
                },
                {
                    ""name"": ""infiniteScroll"",
                    ""meta"": {
                        ""title"": ""无限滚动""
                    }
                },
                {
                    ""name"": ""loading"",
                    ""meta"": {
                        ""title"": ""加载""
                    }
                },
                {
                    ""name"": ""modal"",
                    ""meta"": {
                        ""title"": ""弹窗""
                    }
                },
                {
                    ""name"": ""preview"",
                    ""meta"": {
                        ""title"": ""预览""
                    }
                },
                {
                    ""name"": ""qrcode"",
                    ""meta"": {
                        ""title"": ""二维码""
                    }
                },
                {
                    ""name"": ""resizeBox"",
                    ""meta"": {
                        ""title"": ""伸缩框""
                    }
                },
                {
                    ""name"": ""scrollbar"",
                    ""meta"": {
                        ""title"": ""滚动条""
                    }
                },
                {
                    ""name"": ""search"",
                    ""meta"": {
                        ""title"": ""搜索""
                    }
                },
                {
                    ""name"": ""sendCode"",
                    ""meta"": {
                        ""title"": ""发送验证码""
                    }
                },
                {
                    ""name"": ""tableColumnSetting"",
                    ""meta"": {
                        ""title"": ""表格列设置""
                    }
                },
                {
                    ""name"": ""tag"",
                    ""meta"": {
                        ""title"": ""标签""
                    }
                },
                {
                    ""name"": ""tagSelect"",
                    ""meta"": {
                        ""title"": ""标签选择""
                    }
                },
                {
                    ""name"": ""toolbar"",
                    ""meta"": {
                        ""title"": ""工具条""
                    }
                },
                {
                    ""name"": ""transfer"",
                    ""meta"": {
                        ""title"": ""穿梭框""
                    }
                },
                {
                    ""name"": ""tree"",
                    ""meta"": {
                        ""title"": ""树形控件""
                    }
                },
                {
                    ""name"": ""upload"",
                    ""meta"": {
                        ""title"": ""上传""
                    }
                }
            ]
        },
        {
            ""name"": ""link"",
            ""meta"": {
                ""title"": ""外部链接""
            },
            ""children"": [
                {
                    ""name"": ""github"",
                    ""meta"": {
                        ""title"": ""Github""
                    }
                }
            ]
        },
        {
            ""name"": ""iframePage"",
            ""meta"": {
                ""title"": ""Iframe""
            },
            ""children"": [
                {
                    ""name"": ""iframeVue"",
                    ""meta"": {
                        ""title"": ""Vue""
                    }
                },
                {
                    ""name"": ""iframeAntd"",
                    ""meta"": {
                        ""title"": ""Ant Design Vue""
                    }
                }
            ]
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
