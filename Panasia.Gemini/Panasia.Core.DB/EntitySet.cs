using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panasia.Core
{
    public partial class EntitySet
    {
        public string TableName { get; protected set; }

        public string ModuleName { get; protected set; }

        private AppModule Module { get; set; }

        private Table Table { get; set; }

        private static Dictionary<string, EntitySet> _EntitySets = new Dictionary<string, EntitySet>();

        public EntitySet(string tableName, string moduleName) :base()
        {
            Init(tableName, moduleName);
        }

        #region 初始化
        protected virtual void Init(string tableName, string moduleName)
        {
            TableName = tableName;
            ModuleName = moduleName; 
            Module = AppData.Current.Modules.FirstOrDefault(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
            if (Module == null)
            {
                throw new Exception(string.Format("没有找到模块[{0}]配置", moduleName));
            }
            Table = Module.GetTable(tableName);
            if (Table == null)
            {
                throw new Exception(string.Format("没有找到表[{0}]配置,可能是数据库连接断开,请稍候再试.", tableName));
            }
        }
         
        public static EntitySet Create( string moduleName,string tableName)
        {
            lock (_EntitySets)
            { 
                return GetExistsOrCreate(moduleName, tableName);
            }
        }

        private static EntitySet GetExistsOrCreate(string moduleName, string tableName)
        {
            EntitySet item = null;

            string name = string.Empty;
            if (moduleName.IsNullOrEmpty())
            {
                if (_EntitySets.TryGetValue(tableName, out item))
                {
                    return item;
                }
                var mt = (from m in AppData.Current.Modules
                          select new { Module = m, Table = m.GetTable(tableName) }).FirstOrDefault(m => m.Table != null);
                if (mt == null)
                {
                    throw new Exception(string.Format("没有找到表[{0}]配置,可能是数据库连接断开,请稍候再试.", tableName));
                }
                else
                {
                    moduleName = mt.Module.Name;
                    tableName = mt.Table.Name;
                    name = tableName;
                }
            }
            else
            {
                name = moduleName.ToLower() + "." + tableName.ToLower();
            }
            if (_EntitySets.TryGetValue(name, out item))
            {
                return item;
            }
            else
            {
                item = new EntitySet(tableName, moduleName);
                _EntitySets[name] = item;
                return item;
            }
        }
        #endregion

        #region 为Command做的扩展

        public virtual IEnumerable<T> ExecuteQuery<T>(string name, params object[] namevalues) where T : IEntityBase, new()
        {
            CheckNameValues(namevalues);
            var command = Table.Commands.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if(command==null)
            {
                throw new Exception(string.Format("命令【{0}.{1}.{2}】不存在",Module.Name,Table.Name, name));
            }
            return DataExtension.ExecuteCommand<T>(Module, command, namevalues); 
        }

        public virtual int ExecuteNoneQuery(string name,params object[] namevalues)
        {
            CheckNameValues(namevalues);
            var command = Table.Commands.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if(command==null)
            {
                throw new Exception(string.Format("命令【{0}.{1}.{2}】不存在",Module.Name,Table.Name, name));
            }
            int count = DataExtension.ExecuteNoneQuery(Module, command, namevalues);
            return count;
        }

        public virtual int ExecuteCount(string name,params object[] namevalues)
        {
            CheckNameValues(namevalues);
            var command = Table.Commands.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if(command==null)
            {
                throw new Exception(string.Format("命令【{0}.{1}.{2}】不存在",Module.Name,Table.Name, name));
            }
            int count = Convert.ToInt32(DataExtension.ExecuteScalar(Module, command, namevalues));
            return count;
        }

        public virtual object ExecuteScalar(string name, params object[] namevalues)
        {
            CheckNameValues(namevalues);
            var command = Table.Commands.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if(command==null)
            {
                throw new Exception(string.Format("命令【{0}.{1}.{2}】不存在",Module.Name,Table.Name, name));
            }
            var result = DataExtension.ExecuteScalar(Module, command, namevalues);
            return result;
        }

        #endregion
    }

    
    public partial class EntitySet
    {   
        #region 查询 

        /// <summary>
        /// 如果确定了实体类型
        /// </summary> 
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="columns"></param>
        /// <param name="namevalues"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Query<T>(LogicRule where, string columns = "", params object[] namevalues) where T : IEntityBase, new()
        {
            return QueryWhere<T>(where, columns, namevalues);
        }

        private IEnumerable<T> QueryWhere<T>(LogicRule where, string columns, object[] namevalues) where T : IEntityBase, new()
        {
            CheckNameValues(namevalues);
            SqlCommand sqlcmd = new SqlCommand
            {
                Where = where == null ? "" : where.ToWhere(),
                Columns = columns,
                CommandType = CommandType.Select
            };
            Module.CompileCommand(Table, sqlcmd);
            return DataExtension.ExecuteCommand<T>(Module, sqlcmd, namevalues);
        }

        public virtual int Count(LogicRule where, params object[] namevalues)
        {
            CheckNameValues(namevalues);
            SqlCommand sqlcmd = new SqlCommand
            {
                Where =where==null?"": where.ToWhere(),
                CommandType= CommandType.Count
            };
            Module.CompileCommand(Table, sqlcmd);
            return Convert.ToInt32(DataExtension.ExecuteScalar(Module, sqlcmd, namevalues));  
        }

        public virtual IEnumerable<T> QueryLimit<T>(LogicRule where, string columns, string orderBy, int limit, int start, params object[] namevalues) where T : IEntityBase, new()
        {
            CheckNameValues(namevalues);
            SqlCommand sqlcmd = new SqlCommand
            {
                Where =where==null?"": where.ToWhere(),
                Columns = columns,
                CommandType= CommandType.Select,
                OrderBy = orderBy,
                Limit = limit,
                Start= start
            };
            Module.CompileCommand(Table, sqlcmd);
            return DataExtension.ExecuteCommand<T>(Module, sqlcmd, namevalues); 
        }

        #endregion

        public virtual bool AddOrUpdate<T>(IEntityBase entity, string columns = "", bool reload = false, LogicRule where = null, params object[] namevalues) where T : IEntityBase, new()
        {
            bool result = false;            
            IEnumerable<T> list =Query<T>(where, columns,namevalues);
            //新增
            result = !list.Any() ? Add(entity, columns, reload) : Update(entity, columns, reload, @where);

            return result;
        }

        public virtual bool Add(IEntityBase entity, string columns = "", bool reload = false, LogicRule where = null)
        {
            SqlCommand sqlcmd = new SqlCommand
            {
                Where = where == null ? "" : where.ToWhere(),
                Columns = columns,
                ReloadData=reload,
                CommandType= CommandType.Insert
            };

            return ExecuteSave(entity, reload, sqlcmd);
        }

        public virtual bool Update(IEntityBase entity, string columns = "", bool reload = false,  LogicRule where = null,bool reloadByKeys=true)
        {
            SqlCommand sqlcmd = new SqlCommand
            {
                Where = where == null ? CreateKeyWhere() : where.ToWhere(),
                Columns = columns,
                ReloadData=reload,
                ReloaBykeys=reloadByKeys,
                CommandType= CommandType.Update
            };
            return ExecuteSave(entity, reload, sqlcmd);
        }

        private bool ExecuteSave(IEntityBase entity, bool reload, SqlCommand sqlcmd)
        {
            bool flag = false;
            var namevalues = entity.GetNameValues();
            Module.CompileCommand(Table, sqlcmd);
            if (reload)
            {
                var newEntity = DataExtension.ExecuteCommand<TableEntity>(Module, sqlcmd, namevalues).FirstOrDefault();
                if (newEntity != null)
                {
                    entity.UpdateWith(newEntity);
                    flag = true;
                }
            }
            else
            {
                int count = DataExtension.ExecuteNoneQuery(Module, sqlcmd, namevalues);
                flag = count > 0;
            }
            return flag;
        }


        public virtual bool Delete(IEntityBase entity, LogicRule where = null)
        {
            SqlCommand sqlcmd = new SqlCommand
            {
                Where = where == null ? CreateKeyWhere() : where.ToWhere(),
                CommandType = CommandType.Delete
            };
            var namevalues = entity.GetNameValues();
            Module.CompileCommand(Table, sqlcmd);
            int count = DataExtension.ExecuteNoneQuery(Module, sqlcmd, namevalues);
            return count>0;
        }


        public virtual bool Delete(LogicRule where, params object[] namevalues)
        {
            SqlCommand sqlcmd = new SqlCommand
            {
                Where = where == null ? CreateKeyWhere() : where.ToWhere(),
                CommandType = CommandType.Delete
            };
            Module.CompileCommand(Table, sqlcmd);
            int count = DataExtension.ExecuteNoneQuery(Module, sqlcmd,namevalues);
            return count > 0;
        }

        public virtual bool Update(string columns, LogicRule where, params object[] namevalues)
        {
            SqlCommand sqlcmd = new SqlCommand
            {
                Where = where == null ? CreateKeyWhere() : where.ToWhere(),
                Columns = columns,
                CommandType = CommandType.Update
            };
            Module.CompileCommand(Table, sqlcmd);
            int count = DataExtension.ExecuteNoneQuery(Module, sqlcmd, namevalues);
            return count > 0;
        }         

        private string CreateKeyWhere()
        {
            LogicRule rule = null;
            var column = Table.Columns.FirstOrDefault(w => w.IsAutoIncrement || w.IsKey );
            if (column != null)
            {
                rule = new LogicRule(LogicType.Equals, column.Name, "@" + column.Name);
                return rule.ToWhere();
            }
            return "";
        }

        private string GetSqlName(string sqlname)
        {
           return sqlname.StartsWith(ModuleName, StringComparison.OrdinalIgnoreCase) ? sqlname : string.Format("{0}.{1}.{2}", ModuleName, TableName, sqlname);
        }

        private void CheckNameValues(params object[] namevalues)
        {
            if (namevalues == null || namevalues.Length == 0)
            { 
            }
            else
            {
                int rem = 0;
                Math.DivRem(namevalues.Length, 2, out rem);
                if (rem != 0)
                {
                    throw new Exception("命名参数的个数应该为2的倍数");
                }
            }
        }
    }
}
