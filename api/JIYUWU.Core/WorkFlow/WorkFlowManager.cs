using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Core.Language;
using JIYUWU.Core.UserManager;
using JIYUWU.Entity.Base;
using System.Reflection;

namespace JIYUWU.Core.WorkFlow
{
    public static class WorkFlowManager
    {
        public static bool Exists<T>(string workFlowTableName = null)
        {
            return WorkFlowContainer.Exists<T>(workFlowTableName);
        }

        public static bool Exists<T>(T entity, string workFlowTableName = null)
        {
            return WorkFlowContainer.Exists<T>(workFlowTableName) && GetAuditFlowTable<T>(typeof(T).GetKeyProperty().GetValue(entity).ToString(), workFlowTableName) != null;
        }
        public static bool Exists(string table)
        {
            return WorkFlowContainer.Exists(table);
        }
        /// <summary>
        /// 获取审批的数据
        /// </summary>
        public static async Task<object> GetAuditFormDataAsync(string tableKey, string table)
        {
            Type type = WorkFlowContainer.GetType(table);
            if (type == null)
            {
                return Array.Empty<object>();
            }
            var detailOptions = WorkFlowContainer.GetDetail(type).FirstOrDefault();


            var obj = typeof(WorkFlowManager).GetMethod("GetFormDataAsync").MakeGenericMethod(new Type[] { type, detailOptions?.Type ?? type })
                .Invoke(null, new object[] { tableKey, table, detailOptions }) as Task<object>;
            return await obj;
        }
        /// <summary>
        /// 审批表单数据查询与数据源转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableKey"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static async Task<object> GetFormDataAsync<T, Detail>(string tableKey, string table, WorkFlowFormDetails flowFormDetails)
            where T : class where Detail : class
        {
            string[] fields = WorkFlowContainer.GetFormFields(table);
            if (fields == null || fields.Length == 0)
            {
                return Array.Empty<object>();
            }

            string keyName = typeof(T).GetKeyName();
            var condition = keyName.CreateExpression<T>(tableKey,LinqExpressionType.Equal);
            //动态分库应该查询对应的数据库
            var data = await DbManger.GetDbContext<T>().Set<T>().Where(condition).FirstOrDefaultAsync();
            if (data == null)
            {
                Console.WriteLine($"未查到数据,表：{table},id:{tableKey}");
                return Array.Empty<object>();
            }

            var mainObj = typeof(WorkFlowManager).GetMethod("GetFormData")
                .MakeGenericMethod(new Type[] { typeof(T) })
              .Invoke(null, new object[] { new List<T>() { data }, fields });

            var mainList = (List<object>)mainObj;

            if (mainList.Count == 0)
            {
                string msg = $"未查到审核流程表【{typeof(T).Name}】id【{tableKey}】数据";
                Console.WriteLine(msg);
                Logger.Error(msg);
                return new
                {
                    data = mainList[0]
                };
            }

            //获取明细表配置
            if (flowFormDetails != null)
            {
                var detailCondition = keyName.CreateExpression<Detail>(tableKey, LinqExpressionType.Equal);
                var detailData = await DbManger.GetDbContext<Detail>().Set<Detail>().Where(detailCondition).ToListAsync();

                var detailObj = typeof(WorkFlowManager).GetMethod("GetFormData")
                                       .MakeGenericMethod(new Type[] { typeof(Detail) })
                                       .Invoke(null, new object[] { detailData, flowFormDetails.FormFields });
                return new
                {
                    key = keyName,
                    data = mainList[0],
                    detail = new
                    {
                        name = typeof(Detail).GetEntityTableCnName(),
                        data = detailObj
                    }
                };
            }

            return new
            {
                key = keyName,
                data = mainList[0]
            };

        }
        public static List<object> GetFormData<T>(List<T> enities, string[] fields) where T : class
        {
            string table = typeof(T).Name;
            var tableOptions = DbServerProvider.DbContext.Set<Base_TableColumn>()
                .Where(c => c.TableName == table && fields.Contains(c.ColumnName))
                          .Select(s => new
                          {
                              s.ColumnName,
                              s.ColumnCnName,
                              s.DropNo,
                              isDate = s.IsImage == 4,
                              s.ColumnType,
                              s.EditRowNo,
                              s.EditType,
                              s.IsNull
                          }).ToList();

            List<Base_Dictionary> dictionaries = new List<Base_Dictionary>();

            var dicNos = tableOptions.Select(s => s.DropNo).ToList();
            if (dicNos.Count > 0)
            {
                dictionaries = DictionaryManager.GetDictionaries(dicNos, true).ToList();
            }
            string[] editFormFeilds = WorkFlowContainer.GetEditFields(table) ?? new string[] { };
            List<object> list = new List<object>();
            var properties = typeof(T).GetProperties();
            string[] editType = new string[] { "file", "img", "excel", "editor" };
            int index = 0;
            foreach (var data in enities)
            {
                index++;
                Dictionary<string, object> item = new Dictionary<string, object>();
                foreach (var field in fields)
                {
                    var property = properties.Where(c => c.Name == field).FirstOrDefault();
                    string value = property.GetValue(data)?.ToString();

                    var option = tableOptions.Where(c => c.ColumnName == field).FirstOrDefault();
                    string name = option?.ColumnCnName;
                    if (string.IsNullOrEmpty(name))
                    {
                        name = property.GetDisplayName();
                    }
                    if (option == null || string.IsNullOrEmpty(value))
                    {
                        if (editFormFeilds.Contains(field))
                        {
                            item[field] = new
                            {
                                name = name,
                                field = field,
                                value = value,
                                isEdit = true,
                                editRow = option?.EditRowNo,
                                editType = option?.EditType,
                                formType = option?.EditType,
                                require = option?.IsNull == 1 ? false : true,
                                dropNo = option?.DropNo
                            };
                        }
                        else
                        {
                            item[field] = new
                            {
                                name = name,
                                field = field,
                                value = value,
                                formType = option.EditType,
                                dropNo = option.DropNo
                            };
                        }
                        continue;
                    }
                    string orgVal = value;
                    if (option.isDate)
                    {
                        value = value.GetDateTime().Value.ToString("yyyy-MM-dd");
                    }
                    else if (option.ColumnType == "DateTime")
                    {
                        value = value.GetDateTime().Value.ToString("yyyy-MM-dd HH:mm:sss");
                    }
                    else if (!string.IsNullOrEmpty(option.DropNo))
                    {
                        string val = null;
                        if (option.EditType == "selectList" || option.EditType == "checkbox" || option.EditType == "treeSelect")
                        {
                            string[] arr = value.Split(",");
                            arr = dictionaries.Where(c => c.DicNo == option.DropNo).FirstOrDefault()
                                   ?.Base_DictionaryList
                                   ?.Where(c => arr.Contains(c.DicValue))?.Select(s => s.DicName)
                                   .ToArray();
                            val = string.Join(",", arr);
                        }
                        else
                        {
                            val = dictionaries.Where(c => c.DicNo == option.DropNo).FirstOrDefault()
                             ?.Base_DictionaryList
                             ?.Where(c => c.DicValue == value)?.Select(s => s.DicName)
                             .FirstOrDefault();
                        }

                        if (!string.IsNullOrEmpty(val))
                        {
                            value = val;
                        }
                    }
                    if (editFormFeilds.Contains(field))
                    {
                        item[field] = new
                        {
                            name = name,
                            field = field,
                            value = value,
                            orgVal,
                            isEdit = true,
                            formType = option.EditType,
                            editRow = option.EditRowNo,
                            editType = option.EditType,
                            require = option?.IsNull == 1 ? false : true,
                            dropNo = option.DropNo
                        };
                    }
                    else
                    {
                        item[field] = new
                        {
                            name = name,
                            field = field,
                            value = value,
                            orgVal,
                            formType = option.EditType,
                            dropNo = option.DropNo
                        };
                    }
                }
                //var formType = tableOptions.Where(x => fields.Contains(x.ColumnName) && editType.Contains(x.EditType))
                //   .Select(s => new { name = s.ColumnCnName, s.EditType }).ToList();
                //if (formType.Count > 0)
                //{
                //    item[$"form:type"] = formType;
                //}
                ////2024.01.01增加加审批表单编辑
                //if (index == enities.Count)
                //{
                //    string[] editFields = WorkFlowContainer.GetEditFields(typeof(T).Name);
                //    if (editFields != null)
                //    {
                //        item["form:edit"] = tableOptions.Where(x => editFields.Contains(x.ColumnName))
                //            .Select(s => new { field = s.ColumnName, name = s.ColumnCnName, dataKey = s.DropNo, type = s.EditType })
                //            .ToList();
                //    }
                //}
                list.Add(item);
            }

            return list;
        }

        public static int GetAuditStatus<T>(string value, string workFlowTableName)
        {
            return GetAuditFlowTable<T>(value, workFlowTableName)?.AuditStatus ?? 0;
        }

        public static Base_WorkFlowTable GetAuditFlowTable<T>(string workTableKey, string workFlowTableName = null)
        {
            var table = DbServerProvider.DbContext.Set<Base_WorkFlowTable>()
                  .Where(x => x.WorkTable == (workFlowTableName ?? typeof(T).GetEntityTableName(false)) && x.WorkTableKey == workTableKey)
                   // .Select(s => new { s.CurrentStepId,s.AuditStatus})
                   .FirstOrDefault();
            return table;
        }

        private static void Rewrite<T>(T entity, Base_WorkFlow workFlow, bool changeTableStatus) where T : class, new()
        {
            var autditProperty = typeof(T).GetProperties().Where(x => x.Name.ToLower() == "auditstatus").FirstOrDefault();
            if (autditProperty == null)
            {
                return;
            }

            string value = typeof(T).GetKeyProperty().GetValue(entity).ToString();

            var dbContext = DbServerProvider.DbContext;


            var workTable = dbContext.Set<Base_WorkFlowTable>().Where(x => x.WorkTableKey == value && x.WorkFlow_Id == workFlow.WorkFlow_Id).Includes(x => x.Base_WorkFlowTableStep).FirstOrDefault();
            if (workTable == null || workFlow.Base_WorkFlowStep == null || workFlow.Base_WorkFlowStep.Count == 0)
            {
                Console.WriteLine($"未查到流程数据，id：{workFlow.WorkFlow_Id}");
                return;
            }
            //  workTable.CurrentOrderId = 1;

            //这里还未处理回退到上一个节点

            //重新设置第一个节点(有可能是返回上一个节点)
            string startStepId = workTable.Base_WorkFlowTableStep.Where(x => x.StepAttrType == StepType.start.ToString())
                 .Select(s => s.StepId).FirstOrDefault();

            workTable.CurrentStepId = workTable.Base_WorkFlowTableStep.Where(x => x.ParentId == startStepId).Select(s => s.StepId).FirstOrDefault();

            workTable.AuditStatus = (int)AuditStatus.待审核;
            workTable.Base_WorkFlowTableStep.ForEach(item =>
            {
                item.Enable = 0;
                item.AuditId = null;
                item.Auditor = null;
                item.AuditDate = null;
                item.Remark = null;
            });
            if (changeTableStatus)
            {
                // dbContext.Entry(entity).State = EntityState.Detached;
                autditProperty.SetValue(entity, 0);
                // dbContext.Entry(entity).Property(autditProperty.Name).IsModified = true;
                dbContext.Update<T>(entity, new string[] { autditProperty.Name });
            }
            //dbContext.Entry(workTable).State = EntityState.Detached;
            dbContext.Update(workTable);
            dbContext.SaveChanges();

        }
        /// <summary>
        /// 新建时写入流程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="rewrite">是否重新生成流程</param>
        /// <param name="changeTableStatus">是否修改原表的审批状态</param>
        /// <param name="changeTableStatus">是否修改原表的审批状态</param>
        public static void AddProcese<T>(T entity, bool rewrite = false, bool changeTableStatus = true,
            Action<T, List<int>> addWorkFlowExecuted = null, bool checkId = false,
            string workFlowTableName = null) where T : class, new()
        {
            WorkFlowTableOptions workFlow = WorkFlowContainer.GetFlowOptions(entity);
            //没有对应的流程信息
            if (workFlow == null || workFlow.FilterList.Count == 0)
            {
                return;
            }
            workFlow.WorkTableName = WorkFlowContainer.GetName<T>(workFlowTableName);
            string workTable = workFlowTableName ?? typeof(T).GetEntityTableName(false);

            ////重新生成流程
            if (rewrite)
            {
                Rewrite(entity, workFlow, changeTableStatus);
                return;
            }
            var auditProperty = typeof(T).GetProperties().Where(x => x.Name.ToLower() == "auditstatus").FirstOrDefault();
            if (auditProperty == null)
            {
                Console.WriteLine("表缺少审核状态字段：AuditStatus");
            }
            int auditStatus = (int)workFlow.DefaultAuditStatus;
            string tableKey = typeof(T).GetKeyProperty().GetValue(entity).ToString();
            //提交的草稿直接删除
            if (checkId)
            {
                var list = DbServerProvider.DbContext.Set<Base_WorkFlowTable>()
                     .Where(x => x.WorkTable == workTable && x.WorkTableKey == tableKey)
                     .Includes(x => x.Base_WorkFlowTableStep)
                     .ToList();
                if (list.Count > 0)
                {

                    DbServerProvider.DbContext.SqlSugarClient.DeleteNav(list).Include(c => c.Base_WorkFlowTableStep).ExecuteCommand();
                    //DbServerProvider.DbContext.SaveChanges();
                }
                auditStatus = (int)AuditStatus.待审核;
            }


            auditProperty.SetValue(entity, auditStatus);
            var userInfo = UserContext.Current.UserInfo;
            Guid workFlowTable_Id = Guid.NewGuid();
            Base_WorkFlowTable workFlowTable = new Base_WorkFlowTable()
            {
                WorkFlowTable_Id = workFlowTable_Id,
                AuditStatus = auditStatus,//(int)AuditStatus.待审核,
                Enable = 1,
                WorkFlow_Id = workFlow.WorkFlow_Id,
                WorkName = workFlow.WorkName,
                WorkTable = workTable,
                WorkTableKey = tableKey,
                WorkTableName = workFlow.WorkTableName,
                DbServiceId = workFlow.DbServiceId,
                CreateID = userInfo.User_Id,
                CreateDate = DateTime.Now,
                Creator = userInfo.UserTrueName
            };
            //生成流程的下一步
            var steps = workFlow.FilterList.Where(x => x.StepAttrType == StepType.start.ToString()).Select(s => new Base_WorkFlowTableStep()
            {
                Base_WorkFlowTableStep_Id = Guid.NewGuid(),
                WorkFlowTable_Id = workFlowTable_Id,
                WorkFlow_Id = workFlow.WorkFlow_Id,
                StepId = s.StepId,
                StepName = s.StepName ?? "流程开始",
                StepAttrType = s.StepAttrType,
                NextStepId = null,
                ParentId = null,
                StepType = s.StepType,
                StepValue = s.StepValue,
                OrderId = 0,
                Enable = 1,
                CreateDate = DateTime.Now,
                Creator = userInfo.UserTrueName,
                CreateID = userInfo.User_Id
            }).ToList();

            var entities = new List<T>() { entity };
            for (int i = 0; i < steps.Count; i++)
            {
                var item = steps[i];
                //查找下一个满足条件的节点数据
                FilterOptions filter = workFlow.FilterList
                    .Where(c => c.ParentIds.Contains(item.StepId) && c.FieldFilters.CheckFilter(entities, c.Expression))
                    .FirstOrDefault();
                //&& c.Expression != null
                //&& entities.Any(((Func<T, bool>)c.Expression))
                //未找到满足条件的找无条件的节点
                if (filter == null)
                {
                    filter = workFlow.FilterList.Where(c => c.ParentIds.Contains(item.StepId) && c.Expression == null).FirstOrDefault();
                }
                if (filter != null)
                {
                    var setp = workFlow.Base_WorkFlowStep.Where(x => x.StepId == filter.StepId).Select(s => new Base_WorkFlowTableStep()
                    {
                        Base_WorkFlowTableStep_Id = Guid.NewGuid(),
                        WorkFlowTable_Id = workFlowTable_Id,
                        WorkFlow_Id = s.WorkFlow_Id,
                        StepId = s.StepId,
                        StepName = s.StepName,
                        StepAttrType = s.StepAttrType,
                        NextStepId = null,
                        ParentId = item.StepId,
                        StepType = s.StepType,
                        StepValue = s.StepValue,
                        OrderId = i + 1,
                        Enable = 1,
                        CreateDate = DateTime.Now,
                    }).FirstOrDefault();

                    //显示后续部门与角色审批人待完

                    //设置下个审核节点
                    item.NextStepId = setp.StepId;
                    if (!steps.Any(x => x.StepId == setp.StepId))
                    {
                        //2023.10.24生成多个节点
                        if (!string.IsNullOrEmpty(setp.StepValue) && setp.StepValue.Contains(","))
                        {
                            var ids = setp.StepValue.Split(",");
                            foreach (string id in ids)
                            {
                                steps.Add(new Base_WorkFlowTableStep()
                                {
                                    Base_WorkFlowTableStep_Id = Guid.NewGuid(),
                                    WorkFlowTable_Id = setp.WorkFlowTable_Id,
                                    WorkFlow_Id = setp.WorkFlow_Id,
                                    StepId = setp.StepId,
                                    StepName = setp.StepName,
                                    StepAttrType = setp.StepAttrType,
                                    NextStepId = null,
                                    ParentId = setp.ParentId,
                                    StepType = setp.StepType,
                                    StepValue = id,// setp.StepValue,
                                    OrderId = setp.OrderId,
                                    Enable = 1,
                                    CreateDate = DateTime.Now,
                                });
                            }
                        }
                        else
                        {
                            steps.Add(setp);
                        }
                    }
                }
                else
                {
                    //找不到满足条件的节点，直接结束流程
                    var end = workFlow.Base_WorkFlowStep.Where(c => c.StepAttrType == StepType.end.ToString()).ToList();

                    if (end.Count > 0)
                    {
                        item.NextStepId = end[0].StepId;
                        steps.Add(end.Select(s => new Base_WorkFlowTableStep()
                        {
                            Base_WorkFlowTableStep_Id = Guid.NewGuid(),
                            WorkFlowTable_Id = workFlowTable_Id,
                            WorkFlow_Id = s.WorkFlow_Id,
                            StepId = s.StepId,
                            StepName = s.StepName,
                            StepAttrType = s.StepAttrType,
                            NextStepId = null,
                            ParentId = item.StepId,
                            StepType = s.StepType,
                            StepValue = s.StepValue,
                            OrderId = i + 1,
                            Enable = 1,
                            CreateDate = DateTime.Now,
                        }).FirstOrDefault());
                        i = steps.Count + 1;
                    }
                }
            }
            //2023移除默认审批人
            //foreach (var setp in steps)
            //{
            //    if (setp.StepType == (int)AuditType.用户审批)
            //    {
            //        setp.AuditId = setp.StepValue.GetInt();
            //    }
            //}

            //没有满足流程的数据不走流程
            int count = steps.Where(x => x.StepAttrType != StepType.start.ToString() && x.StepAttrType != StepType.end.ToString()).Count();
            if (count == 0)
            {
                return;
            }
            string stepMsg = null;
            if (steps.Exists(x => x.StepType == (int)AuditType.提交人上级部门审批))
            {
                //获取提交人上级审批部门
                string parentDeptIds = GetStepValueWithParentDeptId(entity);
                if (parentDeptIds == null)
                {
                    string msg = $"表【{workFlow.WorkTableName}】数据找不到提交人的上级部门,提交数据:{entity.Serialize()}";
                    Console.WriteLine(msg);
                    Logger.Error(msg);
                }
                foreach (var item in steps)
                {
                    if (item.StepType == (int)AuditType.提交人上级部门审批)
                    {
                        item.StepType = (int)AuditType.部门审批;
                        item.StepValue = parentDeptIds;
                        item.Remark = stepMsg;
                    }
                }
            }
            if (steps.Exists(x => x.StepType == (int)AuditType.提交人上级角色审批))
            {
                //获取提交人上级审批部门
                string parentRoleIds = GetStepValueWithParentRoleId(entity);
                if (parentRoleIds == null)
                {
                    stepMsg = "数据找不到提交人的上级角色,流程不能正常进行".Translator();
                    string msg = $"表【{workFlow.WorkTableName}】数据找不到提交人的上级角色,提交数据:{entity.Serialize()}";
                    Console.WriteLine(msg);
                    Logger.Error(msg);
                }
                foreach (var item in steps)
                {
                    if (item.StepType == (int)AuditType.提交人上级角色审批)
                    {
                        item.StepType = (int)AuditType.角色审批;
                        item.StepValue = parentRoleIds;
                        item.Remark = stepMsg;
                    }
                }
            }
            //设置进入流程后的第一个审核节点,(开始节点的下一个节点)
            var nodeInfo = steps.Where(x => x.ParentId == steps[0].StepId).Select(s => new
            {
                s.StepId,
                s.StepName,
                s.StepType,
                s.StepValue
            }).FirstOrDefault();
            workFlowTable.CurrentStepId = nodeInfo.StepId;
            workFlowTable.StepName = nodeInfo.StepName;

            workFlowTable.Base_WorkFlowTableStep = steps;

            //写入日志
            var log = new Base_WorkFlowTableAuditLog()
            {
                Id = Guid.NewGuid(),
                WorkFlowTable_Id = workFlowTable.WorkFlowTable_Id,
                CreateDate = DateTime.Now,
                AuditStatus = (int)AuditStatus.待审核,
                Remark = $"[{userInfo.UserTrueName}]提交了数据"
            };

            //dbContext.Set<Base_WorkFlowTable>().Add(workFlowTable);
            //dbContext.Set<Base_WorkFlowTableAuditLog>().Add(log);
            //dbContext.SaveChanges();

            var entityContext = DbServerProvider.GetEntityDbContext<T>();
            entityContext.Update(entity, new string[] { auditProperty.Name }, true);


            var dbContext = DbServerProvider.DbContext;
            //dbContext.Set<Base_WorkFlowTable>().Add(workFlowTable);
            //dbContext.Set<Base_WorkFlowTableAuditLog>().Add(log);
            dbContext.SqlSugarClient.InsertNav(workFlowTable).Include(x => x.Base_WorkFlowTableStep).ExecuteCommand();
            if (workFlow.DefaultAuditStatus != AuditStatus.草稿 && workFlow.DefaultAuditStatus != AuditStatus.待提交)
            {
                dbContext.Add(log);
            }

            dbContext.SaveChanges();

            if (addWorkFlowExecuted != null)
            {
                var userIds = GetAuditUserIds(nodeInfo.StepType ?? 0, nodeInfo.StepValue);
                addWorkFlowExecuted.Invoke(entity, userIds);
            }
        }
        private static string GetStepValueWithParentDeptId<T>(T entity)
        {
            var property = typeof(T).GetProperties().Where(x => x.Name == AppSetting.CreateMember.UserIdField).FirstOrDefault();
            if (property != null)
            {
                int userId = property.GetValue(entity).GetInt();
                var deptIds = DbServerProvider.DbContext.Set<Base_UserDepartment>().Where(x => x.Enable == 1 && x.UserId == userId)
                      .Select(s => s.DepartmentId).ToList();

                deptIds = DbServerProvider.DbContext.Set<Base_Department>()
                    .Where(s => deptIds.Contains(s.DepartmentId) && s.ParentId != null)
                    .Select(s => (Guid)s.ParentId).Distinct().ToList();
                return string.Join(",", deptIds);
            }
            return null;
        }
        private static string GetStepValueWithParentRoleId<T>(T entity)
        {
            var property = typeof(T).GetProperties().Where(x => x.Name == AppSetting.CreateMember.UserIdField).FirstOrDefault();
            if (property != null)
            {
                int userId = property.GetValue(entity).GetInt();
                var roleIds = DbServerProvider.DbContext.Set<Base_UserRole>().Where(x => x.Enable == 1 && x.UserId == userId)
                      .Select(s => s.RoleId).Distinct().ToList();

                roleIds = DbServerProvider.DbContext.Set<Base_Role>()
                    .Where(s => roleIds.Contains(s.Role_Id) && s.ParentId > 0)
                    .Select(s => s.ParentId).Distinct().ToList();
                return string.Join(",", roleIds);
            }
            return null;
        }


        /// <summary>
        /// 审核
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <param name="autditProperty"></param>
        /// <param name="workFlowExecuting"></param>
        /// <param name="workFlowExecuted"></param>
        /// <returns></returns>
        //public static WebResponseContent Audit<T>(T entity, AuditStatus status, string remark,
        //    PropertyInfo autditProperty,
        //     Func<T, AuditStatus, bool, WebResponseContent> workFlowExecuting,
        //     Func<T, AuditStatus, List<int>, bool, WebResponseContent> workFlowExecuted,
        //     bool init = false,
        //     Action<T, List<int>> initInvoke = null
        //    ) where T : class

        public static WebResponseContent Audit<T>(MyDbContext tableDbContext, T entity, AuditStatus status, string remark,
           PropertyInfo autditProperty = null,
           Func<T, AuditStatus, bool, WebResponseContent> workFlowExecuting = null,
           Func<T, AuditStatus, List<int>, bool, WebResponseContent> workFlowExecuted = null,
           bool init = false,
           Action<T, List<int>> initInvoke = null,
           FlowWriteState flowWriteState = FlowWriteState.审批,
           string workFlowTableName = null
          ) where T : class, new()
        {
            if (string.IsNullOrEmpty(workFlowTableName))
            {
                workFlowTableName = typeof(T).GetEntityTableName();
            }
            WebResponseContent webResponse = new WebResponseContent(true);
            if (init)
            {
                if (!WorkFlowContainer.Exists<T>(workFlowTableName))
                {
                    return webResponse;
                }
            }
            var dbContext = DbServerProvider.DbContext;
            dbContext.QueryTracking = true; ;
            var queryDbSet = tableDbContext;//.Set<T>();

            var keyProperty = typeof(T).GetKeyProperty();
            string key = keyProperty.GetValue(entity).ToString();
            string workTable = workFlowTableName ?? typeof(T).GetEntityTableName(false);

            Base_WorkFlowTable workFlow = dbContext.Set<Base_WorkFlowTable>()
                       .Where(x => x.WorkTable == workTable && x.WorkTableKey == key)
                        .Includes(x => x.Base_WorkFlowTableStep)
                       .FirstOrDefault();

            if (workFlow == null)
            {
                return webResponse.Error("未查到流程信息,请检查数据是否被删除");
            }


            workFlow.AuditStatus = (int)status;

            var currentStep = workFlow.Base_WorkFlowTableStep.Where(x => x.StepId == workFlow.CurrentStepId).FirstOrDefault();

            if (currentStep == null)
            {
                return webResponse.Error($"未查到流程节点[{workFlow.CurrentStepId}]信息,请检查数据是否被删除");
            }

            //获取该审批模版节点信息
            Base_WorkFlowStep modelCurrentStep = queryDbSet.SqlSugarClient.Queryable<Base_WorkFlowStep>().Where(e=>e.StepId==currentStep.StepId).FirstOrDefault();
            string sqlModel = "";
            string apiModel = "";
            //sql和api执行替换({WorkFlowTableId}流程主键{UserId}用户主键{UserName}用户名称{WorkTableKey}操作对应数据主键)
            if (modelCurrentStep != null)
            {
                UserInfo userInfo = UserContext.Current.UserInfo;//用户信息
                switch (status)
                {
                    case AuditStatus.审核通过:
                        sqlModel = modelCurrentStep.AuditAgreeSql.Replace("{WorkFlowTableId}", workFlow.WorkFlow_Id.ToString()).Replace("{UserId}", userInfo.User_Id.ToString())
                            .Replace("{UserName}", userInfo.UserName).Replace("{WorkTableKey}", workFlow.WorkTableKey);
                        apiModel = modelCurrentStep.AuditAgreeApi.Replace("{WorkFlowTableId}", workFlow.WorkFlow_Id.ToString()).Replace("\"{UserId}\"", userInfo.User_Id.ToString())
                            .Replace("{UserName}", userInfo.UserName).Replace("{WorkTableKey}", workFlow.WorkTableKey);
                        break;
                    case AuditStatus.审核未通过:
                        sqlModel = modelCurrentStep.AuditRefuseSql.Replace("{WorkFlowTableId}", workFlow.WorkFlow_Id.ToString()).Replace("{UserId}", userInfo.User_Id.ToString())
                            .Replace("{UserName}", userInfo.UserName).Replace("{WorkTableKey}", workFlow.WorkTableKey);
                        apiModel = modelCurrentStep.AuditRefuseApi.Replace("{WorkFlowTableId}", workFlow.WorkFlow_Id.ToString()).Replace("\"{UserId}\"", userInfo.User_Id.ToString())
                            .Replace("{UserName}", userInfo.UserName).Replace("{WorkTableKey}", workFlow.WorkTableKey);
                        break;
                    case AuditStatus.驳回:
                        sqlModel = modelCurrentStep.AuditBackSql.Replace("{WorkFlowTableId}", workFlow.WorkFlow_Id.ToString()).Replace("{UserId}", userInfo.User_Id.ToString())
                            .Replace("{UserName}", userInfo.UserName).Replace("{WorkTableKey}", workFlow.WorkTableKey);
                        apiModel = modelCurrentStep.AuditBackApi.Replace("{WorkFlowTableId}", workFlow.WorkFlow_Id.ToString()).Replace("\"{UserId}\"", userInfo.User_Id.ToString())
                            .Replace("{UserName}", userInfo.UserName).Replace("{WorkTableKey}", workFlow.WorkTableKey);
                        break;
                    default:break;
                }
            }
            

            Base_WorkFlowTableStep nextStep = null;
            //获取下一步审核id
            string nextStepId = null;

            //多人审批
            //多人并签、或签时只更新当前用户节点(不更新所有节点)
            bool isMultiAudit = workFlow.Base_WorkFlowTableStep
                .Where(x => x.StepId == workFlow.CurrentStepId
                   && (x.AuditStatus == null || x.AuditStatus == (int)AuditStatus.待审核)
                ).Count() > 1;


            //2023.07.11修复流程存在多个配置时查不到数据的问题
            var filterOptions = WorkFlowContainer.GetFlowOptions(x => x.WorkFlow_Id == workFlow.WorkFlow_Id)
                 .FirstOrDefault()
                 ?.FilterList
                 ?.Where(x => x.StepId == currentStep.StepId)
                 ?.FirstOrDefault();


            var user = UserContext.Current.UserInfo;

            //多人审批时启用并签功能,如果还有两个节点未审批(包括当前正在审批的节点)，设置下一个节点还是当前节点
            if (isMultiAudit && filterOptions?.AuditMethod == 1)
            {
                nextStepId = currentStep.StepId;
                nextStep = currentStep;
            }
            else
            {
                nextStepId = currentStep.NextStepId;
                nextStep = workFlow.Base_WorkFlowTableStep.Where(x => x.StepId == nextStepId).FirstOrDefault();
            }
            if (nextStep != null)
            {
                workFlow.StepName = nextStep.StepName;
            }
            Base_WorkFlowTableStep item = null;
            //并签与或签匹配
            if (isMultiAudit)
            {
                item = workFlow.Base_WorkFlowTableStep.Where(x => workFlow.CurrentStepId == x.StepId
                        && (x.AuditStatus == null || x.AuditStatus == (int)AuditStatus.待审核)
                        //找到当前满足条件的数据，只更新一条
                        && CheckAuditUserValue(x.StepType, x.StepValue)
                ).FirstOrDefault();
            }
            else
            {
                item = workFlow.Base_WorkFlowTableStep.Where(x => workFlow.CurrentStepId == x.StepId).FirstOrDefault();
            }
            //更新明细记录
            if (item != null)
            {
                item.AuditId = user.User_Id;
                item.Auditor = user.UserTrueName;
                item.AuditDate = DateTime.Now;
                //如果审核拒绝或驳回并退回上一步，待完
                item.AuditStatus = (int)status;
                item.Remark = remark;
            }

            //生成审核记录
            var log = new Base_WorkFlowTableAuditLog()
            {
                Id = Guid.NewGuid(),
                StepId = currentStep.StepId,
                WorkFlowTable_Id = currentStep.WorkFlowTable_Id,
                WorkFlowTableStep_Id = currentStep.Base_WorkFlowTableStep_Id,
                AuditDate = DateTime.Now,
                AuditId = user.User_Id,
                Auditor = user.UserTrueName,
                AuditResult = remark,
                Remark = remark,
                AuditStatus = (int)status,
                CreateDate = DateTime.Now,
                StepName = currentStep.StepName
            };

            if (filterOptions != null)
            {
                //审核未通过或者驳回
                if (flowWriteState != FlowWriteState.审批 || status == AuditStatus.审核未通过 || status == AuditStatus.驳回)
                {
                    switch (flowWriteState)
                    {
                        case FlowWriteState.回退上一级节点:
                            log.AuditStatus = (int)AuditStatus.审核中;
                            break;
                        case FlowWriteState.重新开始:
                            log.AuditStatus = (int)AuditStatus.待审核;
                            break;
                        case FlowWriteState.终止:
                            log.AuditStatus = (int)AuditStatus.审核通过;
                            break;
                    }
                    //记录日志
                    dbContext.Add(log);
                    autditProperty.SetValue(entity, (int)status);
                    //修改审批各节点状态
                    UpdateAuditStatus<T>(apiModel, sqlModel, tableDbContext, entity, workFlow, filterOptions, currentStep, status, remark, flowWriteState, isMultiAudit, workFlowTableName);

                    //发送邮件(appsettings.json配置文件里添加邮件信息)
                    SendMail(workFlow, filterOptions, nextStep, dbContext);

                    if (workFlowExecuted != null)
                    {
                        webResponse = workFlowExecuted.Invoke(entity, status, GetAuditUserIds(nextStep?.StepType ?? 0, nextStep?.StepValue), false);
                    }
                    return webResponse;
                }
            }


            if (autditProperty == null)
            {
                autditProperty = typeof(T).GetProperties().Where(s => s.Name.ToLower() == "auditstatus").FirstOrDefault();
            }
            bool isLast = false;


            //没有找到下一步审批，审核完成
            if ((nextStep == null || nextStep.StepAttrType == StepType.end.ToString()))
            {
                if (status == AuditStatus.审核通过)
                {
                    workFlow.CurrentStepId = "审核完成";
                    var dateProperty = typeof(T).GetProperties().Where(x => x.Name.ToLower() == "auditdate").FirstOrDefault();
                    if (dateProperty != null)
                    {
                        dateProperty.SetValue(entity, DateTime.Now);
                    }
                }
                else
                {
                    workFlow.CurrentStepId = "流程结束";
                }
                workFlow.AuditStatus = (int)status;

                if (workFlowExecuting != null)
                {
                    webResponse = workFlowExecuting.Invoke(entity, status, true);
                    if (!webResponse.Status)
                    {
                        return webResponse;
                    }
                }


                //发送邮件(appsettings.json配置文件里添加邮件信息)
                SendMail(workFlow, filterOptions, nextStep, dbContext);

                autditProperty.SetValue(entity, (int)status);
                //dbContext.Set<Base_WorkFlowTable>().Update(workFlow);
                //queryDbSet.Update(entity);
                //dbContext.Set<Base_WorkFlowTableAuditLog>().Add(log);
                //dbContext.SaveChanges();
                dbContext.Update(workFlow);//.Include(x => x.Base_WorkFlowTableStep).ExecuteCommandAsync();
                dbContext.Update(item);
                dbContext.Add(log);
                dbContext.SaveChanges();
                queryDbSet.Update<T>(entity, true);

                //添加api或者sql执行
                if (!string.IsNullOrWhiteSpace(sqlModel))
                {
                    dbContext.SqlSugarClient.ExcuteNonQuery(sqlModel, null);
                }
                if (!string.IsNullOrWhiteSpace(apiModel))
                {
                    _ = HttpManager.CallJsonApiAsync(apiModel).Result;
                }

                if (workFlowExecuted != null)
                {
                    webResponse = workFlowExecuted.Invoke(entity, status, GetAuditUserIds(nextStep?.StepType ?? 0, nextStep?.StepValue), true);
                }

                return webResponse;
            }

            //指向下一个人审批
            if (nextStep != null && status == AuditStatus.审核通过)
            {
                workFlow.CurrentStepId = nextStep.StepId;
                //原表显示审核中状态
                autditProperty.SetValue(entity, (int)AuditStatus.审核中);
                workFlow.AuditStatus = (int)AuditStatus.审核中;
            }
            else
            {
                autditProperty.SetValue(entity, (int)status);
            }


            //下一个节点=null或节下一个节点为结束节点，流程结束
            if (nextStep == null || workFlow.Base_WorkFlowTableStep.Exists(c => c.StepId == nextStep.StepId && c.StepAttrType == StepType.end.ToString()))
            {
                isLast = true;
            }

            if (workFlowExecuting != null)
            {
                webResponse = workFlowExecuting.Invoke(entity, status, isLast);
                if (!webResponse.Status)
                {
                    return webResponse;
                }
            }

            //queryDbSet.Update(entity);
            //dbContext.Set<Base_WorkFlowTable>().Update(workFlow);
            //dbContext.Set<Base_WorkFlowTableAuditLog>().Add(log);

            //dbContext.SaveChanges();
            //dbContext.Entry(workFlow).State = EntityState.Detached;

            //tableDbContext.Entry(entity).State = EntityState.Detached;

            //if (workFlowExecuted != null)
            //{
            //    webResponse = workFlowExecuted.Invoke(entity, status, GetAuditUserIds(nextStep?.StepType ?? 0, nextStep?.StepValue), isLast);
            //}
            //SendMail(workFlow, filterOptions, nextStep, dbContext);
            //return webResponse;


            //  dbContext.SqlSugarClient.InsertNav(workFlow).Include(x => x.Base_WorkFlowTableStep).ExecuteCommandAsync();
            dbContext.Update(workFlow);//.Include(x => x.Base_WorkFlowTableStep).ExecuteCommandAsync();
            dbContext.Update(item);
            dbContext.Add(log);
            dbContext.SaveChanges();
            queryDbSet.Update(entity, true);
            //添加api或者sql执行
            if (!string.IsNullOrWhiteSpace(sqlModel))
            {
                dbContext.SqlSugarClient.ExcuteNonQuery(sqlModel, null);
            }
            if (!string.IsNullOrWhiteSpace(apiModel))
            {
                _ = HttpManager.CallJsonApiAsync(apiModel).Result;
            }


            //dbContext.Entry(workFlow).State = EntityState.Detached;

            //tableDbContext.Entry(entity).State = EntityState.Detached;

            if (workFlowExecuted != null)
            {
                webResponse = workFlowExecuted.Invoke(entity, status, GetAuditUserIds(nextStep?.StepType ?? 0, nextStep?.StepValue), isLast);
            }
            SendMail(workFlow, filterOptions, nextStep, dbContext);
            return webResponse;

        }

        private static WebResponseContent UpdateAuditStatus<T>(
            string apiModel,
            string sqlModel,
            MyDbContext tableDbContext,
             T entity,
            Base_WorkFlowTable workFlow,
            FilterOptions filterOptions,
            Base_WorkFlowTableStep currentStep,
            AuditStatus status,
            string remark,
            FlowWriteState flowWriteState,
            bool isMultiAudit,
            string workFlowTableName = null
            ) where T : class, new()
        {
            WebResponseContent webResponse = new WebResponseContent(true);
            var auditProperty = typeof(T).GetProperties().Where(x => x.Name.ToLower() == "auditstatus").FirstOrDefault();
            if (auditProperty == null)
            {
                return webResponse.Error("表缺少审核状态字段：AuditStatus");
            }
            var dbContext = DbServerProvider.DbContext;
            if (flowWriteState == FlowWriteState.终止)
            {
                status = AuditStatus.终止;
                workFlow.AuditStatus = (int)status;
                auditProperty.SetValue(entity, (int)status);
            }
            else if (flowWriteState == FlowWriteState.回退上一级节点
                || (status == AuditStatus.审核未通过 && filterOptions.AuditRefuse == (int)AuditRefuse.返回上一节点)
                || (status == AuditStatus.驳回 && filterOptions.AuditBack == (int)AuditBack.返回上一节点))
            {
                //2023.12.01修复审批驳回到退到第一个节后无法审批的问题
                var preSteps = workFlow.Base_WorkFlowTableStep.Where(x => x.NextStepId == currentStep.StepId && x.StepAttrType == StepType.node.ToString()).ToList();
                if (preSteps.Count > 0)
                {
                    foreach (var preStep in preSteps)
                    {
                        preStep.AuditStatus = null;
                        preStep.AuditId = null;
                        preStep.AuditDate = null;
                        preStep.Auditor = null;
                        preStep.Remark = null;

                        //workFlow.AuditStatus = (int)AuditStatus.审核中;
                        dbContext.Update(preStep);
                    }
                    workFlow.CurrentStepId = preSteps[0].StepId;
                    workFlow.StepName = preSteps[0].StepName;
                }
                else
                {
                    //没有找到上一个节点，默认当前节点就是第一个节点
                    workFlow.CurrentStepId = currentStep.StepId;
                    workFlow.StepName = currentStep.StepName;
                }
                //清空当前节点的审批信息(2024.05.21)
                workFlow.Base_WorkFlowTableStep.ForEach(x =>
                {
                    if (x.StepId == currentStep.StepId)
                    {
                        x.AuditStatus = null;
                        x.AuditId = null;
                        x.AuditDate = null;
                        x.Auditor = null;
                        x.Remark = null;
                        dbContext.Update(x);
                    }
                });
                workFlow.AuditStatus = (int)AuditStatus.审核中;
                auditProperty.SetValue(entity, (int)AuditStatus.审核中);
            }
            else if (flowWriteState == FlowWriteState.重新开始
                || (status == AuditStatus.审核未通过 && filterOptions.AuditRefuse == (int)AuditRefuse.流程重新开始)
                || (status == AuditStatus.驳回 && filterOptions.AuditBack == (int)AuditBack.流程重新开始))
            {
                //2024.01.22设置重新审批的流程为初始化配置的状态
                var auditStatus = WorkFlowContainer.GetFlowOptions(entity, workFlowTableName)?.DefaultAuditStatus;
                //2024.04.16处理待提交流程判断
                if (auditStatus == null)
                {
                    auditStatus = WorkFlowContainer.GetFlowOptions(c => c.WorkTable == workFlowTableName).Select(s => s.DefaultAuditStatus).FirstOrDefault();
                }
                bool defaultAuditStatus = false;
                if (auditStatus == AuditStatus.草稿 || auditStatus == AuditStatus.待提交)
                {
                    defaultAuditStatus = true;
                    auditProperty.SetValue(entity, (int)auditStatus);
                }
                else
                {
                    auditProperty.SetValue(entity, (int)AuditStatus.待审核);
                }
                //重新开始
                var steps = workFlow.Base_WorkFlowTableStep.Where(x => x.StepAttrType == StepType.node.ToString() && (x.AuditStatus >= 0)).ToList();
                if (steps.Count > 0)
                {
                    foreach (var item in steps)
                    {
                        item.AuditStatus = null;
                        item.AuditId = null;
                        item.AuditDate = null;
                        item.Auditor = null;
                    }
                    //重新指向第一个节点
                    workFlow.CurrentStepId = steps.OrderBy(c => c.OrderId).Select(c => c.StepId).FirstOrDefault();
                    workFlow.AuditStatus = defaultAuditStatus ? (int)auditStatus : (int)AuditStatus.审核中;
                    DbServerProvider.DbContext.UpdateRange(steps);
                }
            }
            string msg = null;
            if (AuditStatus.审核未通过 == status)
            {
                if (filterOptions.AuditRefuse == (int)AuditRefuse.返回上一节点)
                {
                    msg = "审批未通过,返回上一节点";
                }
                else if (filterOptions.AuditRefuse == (int)AuditRefuse.流程重新开始)
                {
                    msg = "审批未通过,流程重新开始";
                }
            }
            else if (AuditStatus.驳回 == status)
            {
                if (filterOptions.AuditBack == (int)AuditBack.返回上一节点)
                {
                    msg = "审批被驳回,返回上一节点";
                }
                else if (filterOptions.AuditBack == (int)AuditBack.流程重新开始)
                {
                    msg = "审批被驳回,流程重新开始";
                }
            }
            var user = UserContext.Current.UserInfo;
            if (msg != null)
            {
                var auditLog = new Base_WorkFlowTableAuditLog()
                {
                    Id = Guid.NewGuid(),
                    StepId = currentStep.StepId,
                    WorkFlowTable_Id = currentStep.WorkFlowTable_Id,
                    WorkFlowTableStep_Id = currentStep.Base_WorkFlowTableStep_Id,
                    AuditDate = DateTime.Now,
                    AuditId = user.User_Id,
                    Auditor = user.UserTrueName,
                    AuditResult = remark,
                    Remark = msg,
                    AuditStatus = (int)status,
                    CreateDate = DateTime.Now,
                    StepName = currentStep.StepName
                };
                dbContext.Add(auditLog);
            }
            //autditProperty.SetValue(entity, (int)status);
            //query.Update(entity);
            //修改状态

            // dbContext.Set<Base_WorkFlowTable>().Update(workFlow);
            dbContext.Update(workFlow);
            dbContext.UpdateRange(workFlow.Base_WorkFlowTableStep);
            dbContext.SaveChanges();
            // dbContext.Entry(workFlow).State = EntityState.Detached;

            tableDbContext.Update<T>(entity);
            tableDbContext.SaveChanges();
            //tableDbContext.Entry(entity).State = EntityState.Detached;

            //驳回或未通过
            //添加api或者sql执行
            if (!string.IsNullOrWhiteSpace(sqlModel))
            {
                dbContext.SqlSugarClient.ExcuteNonQuery(sqlModel, null);
            }
            if (!string.IsNullOrWhiteSpace(apiModel))
            {
                _ = HttpManager.CallJsonApiAsync(apiModel).Result;
            }

            return webResponse.OK();
        }

        private static void SendMail(Base_WorkFlowTable workFlow, FilterOptions filterOptions, Base_WorkFlowTableStep nextStep, MyDbContext dbContext)
        {
            if (filterOptions == null || filterOptions.SendMail != 1)
            {
                return;
            }
            if (nextStep == null)
            {
                nextStep = new Base_WorkFlowTableStep() { };
            }
            //审核发送邮件通知待完
            var userIds = GetAuditUserIds(nextStep.StepType ?? 0, nextStep.StepValue);
            if (userIds.Count == 0)
            {
                return;
            }
            var emails = dbContext.Set<Base_User>()
                 .Where(x => userIds.Contains(x.User_Id) && x.Email != "").Select(s => s.Email)
                .ToList();
            Task.Run(() =>
            {
                string msg = "";
                try
                {
                    string title = $"有新的任务待审批：流程【{workFlow.WorkName}】,任务【{nextStep.StepName}】";
                    MailHelper.Send(title, title, string.Join(";", emails));
                    msg = $"审批流程发送邮件,流程名称：{workFlow.WorkName},流程id:{workFlow.WorkFlow_Id},步骤:{nextStep.StepName},步骤Id:{nextStep.StepId},收件人:{string.Join(";", emails)}";
                    Logger.AddAsync(msg);
                }
                catch (Exception ex)
                {
                    msg += "邮件发送异常：";
                    Logger.AddAsync(msg, ex.Message + ex.StackTrace);
                }
            });
        }

        /// <summary>
        /// 获取审批人的id
        /// </summary>
        /// <param name="stepType"></param>
        /// <returns></returns>
        private static List<int> GetAuditUserIds(int stepType, string nextId = null)
        {
            List<int> userIds = new List<int>();
            if (stepType == 0 || string.IsNullOrEmpty(nextId))
            {
                return userIds;
            }
            if (stepType == (int)AuditType.角色审批)
            {
                int roleId = nextId.GetInt();
                userIds = DbServerProvider.DbContext.Set<Base_UserRole>().Where(s => s.RoleId == roleId && s.Enable == 1)
                    .Take(200).Select(s => s.UserId).ToList();
            }
            else if (stepType == (int)AuditType.部门审批)
            {
                Guid departmentId = nextId.GetGuid() ?? Guid.Empty;
                userIds = DbServerProvider.DbContext.Set<Base_UserDepartment>().Where(s => s.DepartmentId == departmentId && s.Enable == 1).Take(200).Select(s => s.UserId).ToList();
            }
            else
            {
                return nextId.Split(",").Select(c => c.GetInt()).ToList();
            }
            return userIds;
        }
        /// <summary>
        /// 验证节节点是否满足当前用户
        /// </summary>
        /// <param name="stepType"></param>
        /// <param name="stepValue"></param>
        /// <returns></returns>
        public static bool CheckAuditUserValue(int? stepType, string stepValue = null)
        {
            switch (stepType)
            {
                case (int)AuditType.角色审批:
                    return UserContext.Current.RoleIds.Contains(stepValue.GetInt());
                case (int)AuditType.部门审批:
                    return UserContext.Current.DeptIds.Contains((Guid)stepValue.GetGuid());
                default:
                    return UserContext.Current.UserId == stepValue.GetInt();
            }
        }
        /// <summary>
        /// 增加审批记录
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="auditStatus"></param>
        /// <param name="auditReason"></param>
        public static void AddAuditLog<T>(object[] keys, int? auditStatus, string auditReason) where T : class, new()
        {
            var user = UserContext.Current.UserInfo; ;
            var logs = keys.Select(id => new Base_WorkFlowTableAuditLog()
            {
                Id = Guid.NewGuid(),
                StepId = id?.ToString(),
                StepName = typeof(T).Name,//.GetEntityTableName(false),
                Auditor = user.UserTrueName,
                AuditDate = DateTime.Now,
                AuditId = user.User_Id,
                CreateDate = DateTime.Now,
                AuditStatus = auditStatus,
                AuditResult = auditReason,
                Remark = auditReason
            }).ToList();
            DbServerProvider.DbContext.AddRange(logs, true);
        }

        /// <summary>
        /// 替换现有流程数据
        /// </summary>
        /// <param name="workFlow"></param>
        /// <param name="add"></param>
        public static void UpdateFlowData(Base_WorkFlow workFlow, List<Base_WorkFlowStep> add)
        {
            if (workFlow.AuditingEdit != 1)
            {
                return;
            }
            if (add == null || add.Count == 0)
            {
                return;
            }

            var flowStep = DbServerProvider.DbContext.Set<Base_WorkFlowStep>().Where(x => x.WorkFlow_Id == workFlow.WorkFlow_Id).ToList();
            foreach (var item in flowStep)
            {
                item.NextStepIds = flowStep.Where(c => c.ParentId == item.StepId).Select(c => c.StepId).FirstOrDefault();
            }
            foreach (var item in add)
            {
                item.NextStepIds = flowStep.Where(c => c.StepId == item.StepId).Select(c => c.NextStepIds).FirstOrDefault();
            }
            add = add.Where(x => x.StepAttrType == StepType.node.ToString()).ToList();
            //  var steps = repository.DbContext.Set<Base_WorkFlowStep>().Where(x => x.WorkFlow_Id == workFlow.WorkFlow_Id).ToList();
            var flowTable = DbServerProvider.DbContext.Set<Base_WorkFlowTable>()
                 .Where(x => x.WorkFlow_Id == workFlow.WorkFlow_Id
                 && (x.AuditStatus == (int)AuditStatus.待审核 || x.AuditStatus == (int)AuditStatus.审核中)
                 // && x.Base_WorkFlowTableStep.Any(c => c.AuditStatus == null || c.AuditStatus == (int)AuditStatus.待审核)
                 ).Includes(x => x.Base_WorkFlowTableStep).ToList();
            List<Guid> updateFlowIds = new List<Guid>();
            List<Guid> ingroFlowIds = new List<Guid>();
            //List<Guid> updateStepIds = new List<Guid>();
            foreach (var workFlowTable in flowTable)
            {
                for (int i = 0; i < workFlowTable.Base_WorkFlowTableStep.Count; i++)
                {
                    var step = workFlowTable.Base_WorkFlowTableStep[i];

                    //记录父级点节点变更
                    string parentStepId = add.Where(x => x.NextStepIds == step.StepId).Select(c => c.StepId).FirstOrDefault();
                    if (!string.IsNullOrEmpty(parentStepId))
                    {
                        step.ParentId = parentStepId;
                        // updateStepIds.Add(step.Sys_WorkFlowTableStep_Id);
                    }
                    //第三次增加节点时，添加到开始后面，节点orderid没有值，下一个节点也不对
                    //找出新加的节点前一个节点(上级)
                    var addItems = add.Where(x => x.ParentId == step.StepId).ToList();
                    if (addItems.Count > 0)
                    {
                        //设置原有节点的下一个流程
                        step.NextStepId = addItems[0].StepId;
                        //记录变更的节点,重新指向了下一个节点
                        // updateStepIds.Add(step.Sys_WorkFlowTableStep_Id);
                        //找最后一个节点
                        if (addItems[0].NextStepIds == null)
                        {
                            addItems[0].NextStepIds = workFlowTable.Base_WorkFlowTableStep.Where(x => x.StepAttrType == StepType.end.ToString()).Select(c => c.StepId).FirstOrDefault();
                        }
                        workFlowTable.Base_WorkFlowTableStep.AddRange(addItems.Select(s => new Base_WorkFlowTableStep()
                        {
                            WorkFlow_Id = s.WorkFlow_Id,
                            StepId = s.StepId,
                            StepName = s.StepName,
                            StepType = s.StepType,
                            StepValue = s.StepValue,
                            StepAttrType = s.StepAttrType,
                            WorkFlowTable_Id = workFlowTable.WorkFlowTable_Id,
                            Enable = 1,
                            NextStepId = s.NextStepIds,
                            OrderId = null,
                            ParentId = s.ParentId,
                            CreateDate = DateTime.Now
                        }));
                    }
                }

                //找不到上下级的节点可能已被删除，忽略不处理
                bool b = workFlowTable.Base_WorkFlowTableStep
                 .Exists(x => (x.StepAttrType != StepType.start.ToString() && !workFlowTable.Base_WorkFlowTableStep.Any(c => c.StepId == x.ParentId))
                      || (x.StepAttrType != StepType.end.ToString() && !workFlowTable.Base_WorkFlowTableStep.Any(c => c.StepId == x.NextStepId)));
                if (b)
                {
                    ingroFlowIds.Add(workFlowTable.WorkFlowTable_Id);
                    continue;
                }

                string parentStep = workFlowTable.Base_WorkFlowTableStep.Where(x => x.StepAttrType == StepType.start.ToString()).Select(c => c.StepId).FirstOrDefault();

                //重新排序
                int index = 1;


                foreach (var item in workFlowTable.Base_WorkFlowTableStep)
                {
                    var list = workFlowTable.Base_WorkFlowTableStep.Where(x => x.ParentId == parentStep).ToList();
                    if (list.Count > 0)
                    {
                        foreach (var item2 in list)
                        {
                            item2.OrderId = index;
                            index++;
                        }
                        parentStep = list[0].StepId;
                    }
                }

                //设置当前审批节点
                string currentId = workFlowTable.Base_WorkFlowTableStep
                    .Where(x => x.StepAttrType != StepType.start.ToString() && (x.AuditStatus == null || x.AuditStatus == (int)AuditStatus.待审核)
                          //当前审批节点=没有审批过的节点，并且节点后台不存在已经审批的数据
                          && !workFlowTable.Base_WorkFlowTableStep.Exists(c => c.OrderId > x.OrderId && c.AuditStatus > (int)AuditStatus.待审核))
                    .OrderBy(x => x.OrderId)
                    .Select(s => s.StepId)
                    .FirstOrDefault();
                if (!string.IsNullOrEmpty(currentId) && currentId != workFlowTable.CurrentStepId)
                {
                    workFlowTable.CurrentStepId = currentId;
                    updateFlowIds.Add(workFlowTable.WorkFlowTable_Id);
                }
            }

            if (updateFlowIds.Count > 0)
            {
                var updateList = flowTable.Where(x => updateFlowIds.Contains(x.WorkFlowTable_Id)).ToList();
                DbServerProvider.DbContext.UpdateRange(updateList, x => new { x.CurrentStepId });
            }

            var steps = flowTable.SelectMany(x => x.Base_WorkFlowTableStep).Where(c => c.Base_WorkFlowTableStep_Id == Guid.Empty && c.OrderId > 0).ToList();
            foreach (var item in steps)
            {
                item.Base_WorkFlowTableStep_Id = Guid.NewGuid();
            }
            DbServerProvider.DbContext.AddRange(steps);

            var updateSteps = flowTable.Where(x => !ingroFlowIds.Contains(x.WorkFlowTable_Id))
                .SelectMany(x => x.Base_WorkFlowTableStep).ToList();

            if (updateSteps.Count > 0)
            {
                DbServerProvider.DbContext.UpdateRange(updateSteps, x => new { x.NextStepId, x.ParentId, x.OrderId });
            }
            DbServerProvider.DbContext.SaveChanges();
        }

    }

    public enum FlowWriteState
    {
        审批 = 0,
        重新开始,
        回退上一级节点,
        终止
    }
}
