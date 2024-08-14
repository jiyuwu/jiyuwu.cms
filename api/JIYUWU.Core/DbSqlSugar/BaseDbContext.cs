using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIYUWU.Core.DbSqlSugar
{
    public abstract class BaseDbContext
    {


        public ISqlSugarClient SqlSugarClient { get; set; }

        public bool QueryTracking
        {
            set
            {
            }
        }
        public BaseDbContext() : base() { }

        public ISugarQueryable<TEntity> Set<TEntity>(bool filterDeleted = false) where TEntity : class
        {
            return SqlSugarClient.Set<TEntity>(filterDeleted);
        }

        public int SaveChanges()
        {
            return SqlSugarClient.SaveQueues();
        }
    }
}
