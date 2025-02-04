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
                ""title"": ""message.hello""
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
                                ""title"": ""message.hello""
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
        [HttpGet, Route("Data")]
        public IActionResult GetDataList()
        {
            string json = @"
        {""records"": [
            {
                ""id"": ""630000201510167256"",
                ""title"": ""并须个县"",
                ""name"": ""韩军"",
                ""avatar"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22100%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23f2798f%22%2F%3E%3Ctext%20x%3D%2250%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3E%E5%82%85%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""thumb"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%2379b2f2%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""cover"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23d5f279%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""datetime"": ""1992-02-16 15:00:39"",
                ""users"": [
                    {
                        ""name"": ""余磊"",
                        ""avatar"": ""http://dummyimage.com/100/eb79f2&text=高"",
                        ""id"": ""410000201501267465""
                    }
                ],
                ""tags"": [
                    {
                        ""name"": ""安科半"",
                        ""id"": ""430000200506236173""
                    },
                    {
                        ""name"": ""养先广素"",
                        ""id"": ""350000201307108552""
                    }
                ],
                ""link"": ""http://senon.ml/nvzlo"",
                ""paragraph"": ""产气没社质么选叫程资长主目面先九成义。自持便九记期便马历际两学北回造。前被片也次展阶用约府前果。斯东须战查进增划实委气划。再名研或场圆看标无为然消属劳列。"",
                ""sentence"": ""空增该外号业十引了农办县线任群为先。"",
                ""count1"": 469,
                ""count2"": 733,
                ""count3"": 349,
                ""status"": ""禁用"",
                ""sex"": ""未知"",
                ""percent"": 25,
                ""email"": ""u.ojpbjepjh@hepxrgmf.sd"",
                ""phone"": ""18652813874"",
                ""roles"": [
                    {
                        ""id"": ""510000200409228067"",
                        ""name"": ""销售""
                    },
                    {
                        ""id"": ""310000201905211069"",
                        ""name"": ""运营""
                    },
                    {
                        ""id"": ""350000201610044782"",
                        ""name"": ""运营""
                    }
                ],
                ""role_name"": ""研体力少"",
                ""key"": ""esawgw"",
                ""menu_type"": 2,
                ""sort"": 74
            },
            {
                ""id"": ""510000198309012330"",
                ""title"": ""则上马为织却育"",
                ""name"": ""董明"",
                ""avatar"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22100%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%2379f2c7%22%2F%3E%3Ctext%20x%3D%2250%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3E%E5%BE%90%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""thumb"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23f2a479%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""cover"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%238179f2%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""datetime"": ""1994-07-02 16:48:54"",
                ""users"": [
                    {
                        ""name"": ""梁明"",
                        ""avatar"": ""http://dummyimage.com/100/94f279&text=王"",
                        ""id"": ""440000198706224806""
                    }
                ],
                ""tags"": [
                    {
                        ""name"": ""认意计只"",
                        ""id"": ""54000019720415777X""
                    },
                    {
                        ""name"": ""验往志美难"",
                        ""id"": ""640000198606211159""
                    },
                    {
                        ""name"": ""两深备式"",
                        ""id"": ""150000197907261116""
                    }
                ],
                ""link"": ""http://ropjbvjc.pe/lltdsbybtp"",
                ""paragraph"": ""全感华直人持声强给大以级度书许时取。府果流还以变太权备件也品按党参该同。育精数做要科生非色交质节者转必明。信到众备走何务论将率很年今。声强公光行育往者改属主能行。"",
                ""sentence"": ""向养走进共号类你史合式正处。"",
                ""count1"": 304,
                ""count2"": 556,
                ""count3"": 975,
                ""status"": ""禁用"",
                ""sex"": ""男"",
                ""percent"": 70,
                ""email"": ""g.srpofnitz@gknqe.to"",
                ""phone"": ""18153374464"",
                ""roles"": [
                    {
                        ""id"": ""990000201601095341"",
                        ""name"": ""销售""
                    },
                    {
                        ""id"": ""360000200505115670"",
                        ""name"": ""销售""
                    },
                    {
                        ""id"": ""410000200907067485"",
                        ""name"": ""运营""
                    }
                ],
                ""role_name"": ""往技派次"",
                ""key"": ""soldrh"",
                ""menu_type"": 2,
                ""sort"": 61
            },
            {
                ""id"": ""44000019940202325X"",
                ""title"": ""去下完条建"",
                ""name"": ""谭丽"",
                ""avatar"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22100%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23f279b7%22%2F%3E%3Ctext%20x%3D%2250%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3E%E8%AE%B8%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""thumb"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%2379dbf2%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""cover"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23f2e679%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""datetime"": ""1988-10-04 04:04:33"",
                ""users"": [
                    {
                        ""name"": ""何勇"",
                        ""avatar"": ""http://dummyimage.com/100/c279f2&text=吕"",
                        ""id"": ""370000198808129676""
                    },
                    {
                        ""name"": ""汤刚"",
                        ""avatar"": ""http://dummyimage.com/100/79f29f&text=阎"",
                        ""id"": ""520000199903041587""
                    },
                    {
                        ""name"": ""郝平"",
                        ""avatar"": ""http://dummyimage.com/100/f27b79&text=孙"",
                        ""id"": ""370000200309301814""
                    }
                ],
                ""tags"": [
                    {
                        ""name"": ""较层问已"",
                        ""id"": ""420000201102035511""
                    },
                    {
                        ""name"": ""节速子"",
                        ""id"": ""320000199904285762""
                    }
                ],
                ""link"": ""http://xwolyeyift.gl/tbclnxoqd"",
                ""paragraph"": ""亲米省三两自原着造东维北也往和。圆求不实加县数标部示难通青称战少声。离引识二极划基金点离白员非育却利劳。业更更化论角里现着记加属万可风维许。"",
                ""sentence"": ""合每阶合样内类己院或打满被。"",
                ""count1"": 940,
                ""count2"": 184,
                ""count3"": 894,
                ""status"": ""启用"",
                ""sex"": ""男"",
                ""percent"": 8,
                ""email"": ""v.nelvqrkr@rjgnoy.pk"",
                ""phone"": ""18621610646"",
                ""roles"": [
                    {
                        ""id"": ""150000201508109781"",
                        ""name"": ""运营""
                    },
                    {
                        ""id"": ""350000200807261225"",
                        ""name"": ""销售""
                    }
                ],
                ""role_name"": ""复小而"",
                ""key"": ""yvjenl"",
                ""menu_type"": 1,
                ""sort"": 37
            },
            {
                ""id"": ""360000197904021247"",
                ""title"": ""装第性县期"",
                ""name"": ""龚超"",
                ""avatar"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22100%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%237999f2%22%2F%3E%3Ctext%20x%3D%2250%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3E%E8%B4%BA%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""thumb"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23bcf279%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""cover"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23f279e0%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""datetime"": ""2001-11-04 06:04:10"",
                ""users"": [
                    {
                        ""name"": ""高勇"",
                        ""avatar"": ""http://dummyimage.com/100/79f2e0&text=周"",
                        ""id"": ""37000020121105141X""
                    },
                    {
                        ""name"": ""许娟"",
                        ""avatar"": ""http://dummyimage.com/100/f2bd79&text=蒋"",
                        ""id"": ""350000199403097646""
                    },
                    {
                        ""name"": ""秦敏"",
                        ""avatar"": ""http://dummyimage.com/100/9a79f2&text=薛"",
                        ""id"": ""42000019880830087X""
                    }
                ],
                ""tags"": [
                    {
                        ""name"": ""我正传从部"",
                        ""id"": ""41000019700129146X""
                    }
                ],
                ""link"": ""http://wqec.kp/ewgowo"",
                ""paragraph"": ""据与变毛应发马话深产广加者没去却。位见而可类北群必西规适活程育济导制。资几书关适色想化什就物别长算共。"",
                ""sentence"": ""维到心六期铁所活们理后复分光格不论化。"",
                ""count1"": 251,
                ""count2"": 736,
                ""count3"": 986,
                ""status"": ""禁用"",
                ""sex"": ""女"",
                ""percent"": 11,
                ""email"": ""e.stfoffdbu@ylgycoiwlj.cy"",
                ""phone"": ""13358155591"",
                ""roles"": [
                    {
                        ""id"": ""650000197207208881"",
                        ""name"": ""销售""
                    },
                    {
                        ""id"": ""460000199206210422"",
                        ""name"": ""运营""
                    }
                ],
                ""role_name"": ""改备见"",
                ""key"": ""pdjimz"",
                ""menu_type"": 2,
                ""sort"": 56
            },
            {
                ""id"": ""620000201207117588"",
                ""title"": ""养色西决"",
                ""name"": ""孔静"",
                ""avatar"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22100%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%237bf279%22%2F%3E%3Ctext%20x%3D%2250%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3E%E9%94%BA%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""thumb"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23f2799e%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""cover"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%2379c2f2%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""datetime"": ""2024-04-30 17:40:58"",
                ""users"": [
                    {
                        ""name"": ""武磊"",
                        ""avatar"": ""http://dummyimage.com/100/e5f279&text=阎"",
                        ""id"": ""350000202309275818""
                    },
                    {
                        ""name"": ""万娟"",
                        ""avatar"": ""http://dummyimage.com/100/db79f2&text=程"",
                        ""id"": ""440000201811053265""
                    }
                ],
                ""tags"": [
                    {
                        ""name"": ""为见且"",
                        ""id"": ""620000198703275345""
                    },
                    {
                        ""name"": ""说战则"",
                        ""id"": ""540000198804208256""
                    }
                ],
                ""link"": ""http://lsdjn.do/gosp"",
                ""paragraph"": ""流研必总片学它物响第严铁品科金义世张。地周青然全青就是心全单须。程决山已段三复选主响电明转起联状。了果节后你六度何验代速达。"",
                ""sentence"": ""参由飞许统表情能空际社万重。"",
                ""count1"": 427,
                ""count2"": 612,
                ""count3"": 890,
                ""status"": ""启用"",
                ""sex"": ""未知"",
                ""percent"": 5,
                ""email"": ""t.ujwi@hsaqnrxj.tv"",
                ""phone"": ""18162647461"",
                ""roles"": [
                    {
                        ""id"": ""340000197301110431"",
                        ""name"": ""超级管理员""
                    }
                ],
                ""role_name"": ""回会全党"",
                ""key"": ""tdjyjd"",
                ""menu_type"": 2,
                ""sort"": 36
            },
            {
                ""id"": ""460000201103307053"",
                ""title"": ""队长飞业众"",
                ""name"": ""蒋勇"",
                ""avatar"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22100%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%2379f2b8%22%2F%3E%3Ctext%20x%3D%2250%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3E%E8%B4%BA%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""thumb"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23f29579%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""cover"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%237980f2%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""datetime"": ""2017-08-14 18:11:58"",
                ""users"": [
                    {
                        ""name"": ""魏芳"",
                        ""avatar"": ""http://dummyimage.com/100/a3f279&text=侯"",
                        ""id"": ""120000201208234825""
                    }
                ],
                ""tags"": [
                    {
                        ""name"": ""书例们求"",
                        ""id"": ""520000199910119266""
                    }
                ],
                ""link"": ""http://riqokx.zr/bhbe"",
                ""paragraph"": ""示改东对来并自基传价数北技面。做思约际林做示示设数使报质铁。持效划方之家养素间上离车产又。明种热土又决元劳联权少决华压社活半细。切写原收团只果史强现研该之界花油。般思带规资方克被为以县直少八克才手。"",
                ""sentence"": ""果飞难市且效为器列织状听持。"",
                ""count1"": 711,
                ""count2"": 557,
                ""count3"": 439,
                ""status"": ""禁用"",
                ""sex"": ""未知"",
                ""percent"": 33,
                ""email"": ""w.gfsxogs@tdsoezz.co"",
                ""phone"": ""18141988737"",
                ""roles"": [
                    {
                        ""id"": ""620000201604014830"",
                        ""name"": ""销售""
                    },
                    {
                        ""id"": ""810000199102145334"",
                        ""name"": ""超级管理员""
                    }
                ],
                ""role_name"": ""经除满"",
                ""key"": ""lyjxdk"",
                ""menu_type"": 2,
                ""sort"": 88
            },
            {
                ""id"": ""360000201309216364"",
                ""title"": ""八积石标报"",
                ""name"": ""范涛"",
                ""avatar"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22100%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23f279c7%22%2F%3E%3Ctext%20x%3D%2250%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3E%E9%82%B9%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""thumb"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%2379eaf2%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""cover"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23f2d679%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""datetime"": ""2014-07-26 10:25:15"",
                ""users"": [
                    {
                        ""name"": ""尹杰"",
                        ""avatar"": ""http://dummyimage.com/100/b379f2&text=宋"",
                        ""id"": ""310000200402035812""
                    }
                ],
                ""tags"": [
                    {
                        ""name"": ""受流关细"",
                        ""id"": ""610000201204115729""
                    }
                ],
                ""link"": ""http://riiqewdfx.tz/dmhhgew"",
                ""paragraph"": ""可华活来五色代器红张手酸角织华。社目西近你以给光族革同历走。为况技家交系法子民权处流济。"",
                ""sentence"": ""它后九决毛小开着满温说道她划一权来。"",
                ""count1"": 42,
                ""count2"": 674,
                ""count3"": 294,
                ""status"": ""禁用"",
                ""sex"": ""未知"",
                ""percent"": 4,
                ""email"": ""r.zoltqrw@qwgsquq.sc"",
                ""phone"": ""13886898941"",
                ""roles"": [
                    {
                        ""id"": ""650000200310052103"",
                        ""name"": ""超级管理员""
                    }
                ],
                ""role_name"": ""省育得安家"",
                ""key"": ""xknfvl"",
                ""menu_type"": 1,
                ""sort"": 65
            },
            {
                ""id"": ""120000198409028804"",
                ""title"": ""毛观他入影克"",
                ""name"": ""张刚"",
                ""avatar"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22100%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%2379f28f%22%2F%3E%3Ctext%20x%3D%2250%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3E%E6%9D%8E%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""thumb"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23f27985%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""cover"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%2379a9f2%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""datetime"": ""1987-06-09 00:55:30"",
                ""users"": [
                    {
                        ""name"": ""孙娟"",
                        ""avatar"": ""http://dummyimage.com/100/ccf279&text=陈"",
                        ""id"": ""220000199511271550""
                    },
                    {
                        ""name"": ""万勇"",
                        ""avatar"": ""http://dummyimage.com/100/f279ef&text=郑"",
                        ""id"": ""220000201707127569""
                    }
                ],
                ""tags"": [
                    {
                        ""name"": ""龙图分毛手"",
                        ""id"": ""210000200708187962""
                    },
                    {
                        ""name"": ""象提电基"",
                        ""id"": ""630000199911067694""
                    }
                ],
                ""link"": ""http://beqkw.gi/pklas"",
                ""paragraph"": ""群而习今使质本后指此场务子。把单养己水这素团出做值按气。个响很六大压派书导应议增全。"",
                ""sentence"": ""气影温各走运采会号心参亲者越。"",
                ""count1"": 705,
                ""count2"": 49,
                ""count3"": 728,
                ""status"": ""启用"",
                ""sex"": ""男"",
                ""percent"": 65,
                ""email"": ""v.kwini@cgwgsynce.tz"",
                ""phone"": ""18176204636"",
                ""roles"": [
                    {
                        ""id"": ""430000199911143557"",
                        ""name"": ""超级管理员""
                    },
                    {
                        ""id"": ""220000199503047057"",
                        ""name"": ""运营""
                    },
                    {
                        ""id"": ""360000197910066716"",
                        ""name"": ""超级管理员""
                    }
                ],
                ""role_name"": ""要色报况"",
                ""key"": ""opvotf"",
                ""menu_type"": 2,
                ""sort"": 56
            },
            {
                ""id"": ""710000198409076179"",
                ""title"": ""克引现电五广"",
                ""name"": ""常娟"",
                ""avatar"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22100%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%2379f2d1%22%2F%3E%3Ctext%20x%3D%2250%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3E%E5%A7%9A%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""thumb"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23f2ae79%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""cover"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%238a79f2%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""datetime"": ""1990-05-06 17:09:13"",
                ""users"": [
                    {
                        ""name"": ""何平"",
                        ""avatar"": ""http://dummyimage.com/100/8af279&text=梁"",
                        ""id"": ""620000198710307253""
                    },
                    {
                        ""name"": ""唐娜"",
                        ""avatar"": ""http://dummyimage.com/100/f279ae&text=锺"",
                        ""id"": ""150000197706279950""
                    }
                ],
                ""tags"": [
                    {
                        ""name"": ""引强力物十"",
                        ""id"": ""360000200008262848""
                    },
                    {
                        ""name"": ""两节美四"",
                        ""id"": ""37000019930126358X""
                    }
                ],
                ""link"": ""http://fwf.ga/hutkilf"",
                ""paragraph"": ""小最该些油元己利相它况组保年万。三局细议使专约带如基式合治出。物备接平从关影影天斗小变会并解石。"",
                ""sentence"": ""属边位办十方总会石西无新了法技相。"",
                ""count1"": 11,
                ""count2"": 254,
                ""count3"": 592,
                ""status"": ""禁用"",
                ""sex"": ""女"",
                ""percent"": 40,
                ""email"": ""y.xrinfku@jgubna.cz"",
                ""phone"": ""18169957448"",
                ""roles"": [
                    {
                        ""id"": ""370000199610114211"",
                        ""name"": ""超级管理员""
                    },
                    {
                        ""id"": ""120000198909029221"",
                        ""name"": ""超级管理员""
                    }
                ],
                ""role_name"": ""增度式"",
                ""key"": ""qguvpx"",
                ""menu_type"": 2,
                ""sort"": 30
            },
            {
                ""id"": ""120000201705033311"",
                ""title"": ""权京达该"",
                ""name"": ""金强"",
                ""avatar"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22100%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%2379d1f2%22%2F%3E%3Ctext%20x%3D%2250%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3E%E5%AD%9F%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""thumb"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23f2ef79%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""cover"": ""data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%22200%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23cc79f2%22%2F%3E%3Ctext%20x%3D%22100%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3EHello%3C%2Ftext%3E%3C%2Fsvg%3E"",
                ""datetime"": ""1994-10-24 12:17:32"",
                ""users"": [
                    {
                        ""name"": ""常超"",
                        ""avatar"": ""http://dummyimage.com/100/79f2a8&text=方"",
                        ""id"": ""650000198412148528""
                    },
                    {
                        ""name"": ""薛平"",
                        ""avatar"": ""http://dummyimage.com/100/f28579&text=丁"",
                        ""id"": ""540000200510314870""
                    }
                ],
                ""tags"": [
                    {
                        ""name"": ""把断认王备"",
                        ""id"": ""520000200001027191""
                    },
                    {
                        ""name"": ""世连市西"",
                        ""id"": ""430000198408313788""
                    },
                    {
                        ""name"": ""能子发"",
                        ""id"": ""540000199905197044""
                    }
                ],
                ""link"": ""http://lhedvt.ma/epdcrthg"",
                ""paragraph"": ""织目报目东而选头多布党亲型毛图工。形复口样好变支公规省且总。方族知第济有县规大性消交断及。号局天老部世日全价候专想离毛感。即物府建七教育务次化将们见音她。者事快义及说别并题争每头作。"",
                ""sentence"": ""利近月上候型四导法使过成专活要作。"",
                ""count1"": 445,
                ""count2"": 682,
                ""count3"": 210,
                ""status"": ""启用"",
                ""sex"": ""未知"",
                ""percent"": 91,
                ""email"": ""l.olkr@szwbfyhoe.nt"",
                ""phone"": ""19814485585"",
                ""roles"": [
                    {
                        ""id"": ""150000198807147556"",
                        ""name"": ""销售""
                    }
                ],
                ""role_name"": ""教动出分世"",
                ""key"": ""rjowyo"",
                ""menu_type"": 1,
                ""sort"": 23
            }
        ],
        ""pagination"": {
            ""total"": 19
        }
}";

            TestData testData = JsonConvert.DeserializeObject<TestData>(json);
            return Success(testData);
        }
    }
}
