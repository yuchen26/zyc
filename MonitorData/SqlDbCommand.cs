using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Text;
using System.Reflection;
using System;

public class SqlDbCommand : SqlDbConnect
{
    private SqliteCommand _sqlComm;
    public SqlDbCommand(string dbPath) : base(dbPath)
    {
        _sqlComm = new SqliteCommand(_sqlConn);//初始化，也就是将以链接的数据库绑定到SqliteCommand 
    }
    #region 表管理
    public int CreateTable()//创建表（测试用）
    {
        var sql = "create table tablename(id int,name string)";//创建表 名字 tablename 两个字段 id 和 name
        _sqlComm.CommandText = sql;//设置为执行文本
        return _sqlComm.ExecuteNonQuery();//执行

    }

    public int CreateTable<T>() where T : BaseData//创建泛型表
    {
        if (IsTableCreated<T>())
        {
            return -1;
        }
        var type = typeof(T);
        var tableName = type.Name;
        var sb = new StringBuilder();
        sb.Append($"create table {tableName} (");

        var properties = type.GetProperties();

        foreach (var p in properties)
        {
            var attribute = p.GetCustomAttribute<ModelHelp>();
            if (attribute.IsCreated)
            {
                sb.Append($"{attribute.FieldName} {attribute.Type}");

                if (attribute.IsPrimaryKey)
                {
                    sb.Append(" primary key ");
                }
                if (attribute.IsCanBeNull)
                {
                    sb.Append(" null ");
                }
                else
                {
                    sb.Append(" not null ");
                }
                sb.Append(",");

            }

        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append(")");
        _sqlComm.CommandText = sb.ToString();
        var ret = _sqlComm.ExecuteNonQuery();
        
        return ret;
    }
    //删除表
    public int DeleteTable<T>() where T : BaseData//删除表
    {
        var sql = $"drop table {typeof(T).Name}";
        _sqlComm.CommandText = sql;
        return _sqlComm.ExecuteNonQuery();

    }

    public bool IsTableCreated<T>() where T : BaseData//表是否创建
    {
        var sql = $"SELECT count(*) FROM sqlite_master WHERE type='table' AND name = '{typeof(T).Name}'";
        _sqlComm.CommandText = sql;
        var dr = _sqlComm.ExecuteReader();
        if (dr != null && dr.Read())
        {
            var ret = Convert.ToInt32(dr[dr.GetName(0)]) == 1;
            _sqlComm.Dispose();
            return ret;
        }
        return false;
    }

    #endregion

    #region 新增
    //新增表中单个值
    public int Insert<T>(T t) where T : class//插入一个T类型
    {
        if (t == default(T))
        {
            Debug.LogError("Insert()参数错误！");
            return -1;
        }
        var type = typeof(T);
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append($"INSERT INTO {type.Name} (");//新增格式

        var propertys = type.GetProperties();//获取属性
        foreach (var p in propertys)//遍历所有属性
        {
            if (p.GetCustomAttribute<ModelHelp>().IsCreated)//判断这个字段是否创建，如果创建 将其加入 
            {
                stringBuilder.Append(p.GetCustomAttribute<ModelHelp>().FieldName);//获取表名
                stringBuilder.Append(",");
            }
        }
        stringBuilder.Remove(stringBuilder.Length - 1, 1);
        stringBuilder.Append(") VALUES (");

        foreach (var p in propertys)
        {
            if (p.GetCustomAttribute<ModelHelp>().IsCreated)
            {
                if (p.GetCustomAttribute<ModelHelp>().Type == "string")
                {
                    stringBuilder.Append($"'{p.GetValue(t)}'");
                }
                else
                {
                    stringBuilder.Append(p.GetValue(t));//获取属性的值
                }
                stringBuilder.Append(",");
            }
        }
        stringBuilder.Remove(stringBuilder.Length - 1, 1);
        stringBuilder.Append(")");

        _sqlComm.CommandText = stringBuilder.ToString();
        return _sqlComm.ExecuteNonQuery();

    }

    //新增表中大量值（列表）
    public int Insert<T>(List<T> tList) where T : class//插入一个T类型
    {
        if (tList == null || tList.Count == 0)
        {
            Debug.LogError("Insert()参数错误！");
            return -1;
        }
        var type = typeof(T);
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append($"INSERT INTO {type.Name} (");//新增格式

        var propertys = type.GetProperties();//获取属性
        foreach (var p in propertys)//遍历所有属性
        {
            if (p.GetCustomAttribute<ModelHelp>().IsCreated)//判断这个字段是否创建，如果创建 将其加入 
            {
                stringBuilder.Append(p.GetCustomAttribute<ModelHelp>().FieldName);//获取表名
                stringBuilder.Append(",");
            }
        }
        stringBuilder.Remove(stringBuilder.Length - 1, 1);
        stringBuilder.Append(") VALUES ");

        foreach (var t in tList)
        {
            stringBuilder.Append(" ( ");//注意空格，可以多不能少
            foreach (var p in propertys)
            {
                if (p.GetCustomAttribute<ModelHelp>().IsCreated)
                {
                    if (p.GetCustomAttribute<ModelHelp>().Type == "string")
                    {
                        stringBuilder.Append($"'{p.GetValue(t)}'");
                    }
                    else
                    {
                        stringBuilder.Append(p.GetValue(t));//获取属性的值
                    }
                    stringBuilder.Append(",");
                }
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);//去掉最后一个逗号
            stringBuilder.Append("),");//加上括号和逗号
        }
        stringBuilder.Remove(stringBuilder.Length - 1, 1);//去掉最后一个逗号

        _sqlComm.CommandText = stringBuilder.ToString();
        return _sqlComm.ExecuteNonQuery();

    }
    #endregion

    #region 查询
    //查询
    public T SelectById<T>(int time) where T : BaseData//查询单个，通过Time来查询，因为Time是主键
    {
        //var sql = "SELECT 列名称 FROM 表名称"//查询语句
        var type = typeof(T);
        var sql = $"SELECT * FROM {type.Name} where Time = {time}";
        _sqlComm.CommandText = sql;
        var dr = _sqlComm.ExecuteReader();
        if (dr!=null && dr.Read())//如果dr类不为空则让他读取一行的数据
        {
            var ret = DataReaderToData<T>(dr);
            _sqlComm.Dispose();
            return ret;
        }
        _sqlComm.Dispose();
        return default;
    }

    //public List<T> SelectAll<T>(int id) where T : BaseData//查询所有
    //{
    //    var ret = new List<T>();
    //    //var sql = "SELECT 列名称 FROM 表名称"//查询语句
    //    var type = typeof(T);
    //    var sql = $"SELECT * FROM {type.Name} ";
    //    _sqlComm.CommandText = sql;
    //    var dr = _sqlComm.ExecuteReader();
    //    if (dr != null)//如果dr类不为空则让他读取一行的数据
    //    {
    //        while (dr.Read())//如果dr不等于null，则让其持续读取下一行
    //        {
    //            ret.Add(DataReaderToData<T>(dr));
    //        }
    //    }
    //    return ret;
    //}

    public List<T> SelectBySql<T>(string sqlWhere = "") where T : BaseData//按条件查找(什么都不传就查所有，传指定就查指定)
    {
        var ret = new List<T>();
        string sql;
        var type = typeof(T);
        if (string.IsNullOrEmpty(sqlWhere))// 如果传进来的sql语句为空或者null直接将其返回
        {
            sql = $"SELECT * FROM {type.Name}";
        }
        else 
        {
            sql = $"SELECT * FROM {type.Name} where {sqlWhere}";
        }
        //var sql = "SELECT 列名称 FROM 表名称"//查询语句
        
        _sqlComm.CommandText = sql;
        var dr = _sqlComm.ExecuteReader();
        if (dr != null)//如果dr类不为空则让他读取一行的数据
        {
            while (dr.Read())//如果dr不等于null，则让其持续读取下一行
            {
                ret.Add(DataReaderToData<T>(dr));
            }
        }
        _sqlComm.Dispose();
        return ret;
    }

    

    private T DataReaderToData<T>(SqliteDataReader dr) where T : BaseData
    {
        try
        {
            List<string> fieldNames = new List<string>();//定义一个，获取dr里面所有字段
            for (int i = 0; i < dr.FieldCount; i++)//遍历dr，这样就获取到了数据库中所有的列名
            {
                fieldNames.Add(dr.GetName(i));//获取数据库中的字段名
            }

            var type = typeof(T);
            T data = Activator.CreateInstance<T>();//简历一个T类型的实体
            var properties = type.GetProperties();//获取属性

            foreach (var p in properties)//遍历 所有字段名，并且讲所有字段名和数据库中的进行比较
            {
                if (!p.CanWrite) continue;//判断此属性是否可以惊醒赋值
                var fieldName = p.GetCustomAttribute<ModelHelp>().FieldName;//获取属性中的字段名
                if (fieldName.Contains(fieldName) && p.GetCustomAttribute<ModelHelp>().IsCreated)//判断数据库中的字段名是否包含属性中的字段名
                {
                    if (p.GetCustomAttribute<ModelHelp>().Type == "single")
                    {
                        p.SetValue(data, Convert.ToSingle(dr[fieldName]));//将我们新建的T类型实体传入，然后在获取dr中的值
                    }
                    else 
                    { 
                        p.SetValue(data, dr[fieldName]);//将我们新建的T类型实体传入，然后在获取dr中的值
                    }
                    
                }
                
            }
            return data;
        }
        catch (Exception e)
        {   
            Debug.LogError($"DataReaderToData()转换出错,类型{typeof(T).Name}出错，出错消息：{e.Message}");
            return null;
        }
    }
    #endregion

    #region 删除
    //删除表中数据
    public int DeleteById<T>(int time)
    {
        var sql = $"DELETE FROM {typeof(T).Name} where Time = {time}";//删除表语句“DELET FROM table_name”
        _sqlComm.CommandText = sql;
        return _sqlComm.ExecuteNonQuery();
    }
    //批量删除表中数据方法
    public int DeleteByIds<T>(List<int> ids)
    {
        var count = 0;

        foreach (var id in ids)
        {
            count += DeleteById<T>(id);//调用删除单个的方法
        }
        return count;
    }
    //灵活删除表中任何数据， （不依据id）
    public int DeleteBySql<T>(string sql)
    {
        _sqlComm.CommandText = $"DELETE FROM {typeof(T).Name} where {sql} ";
        return _sqlComm.ExecuteNonQuery();
    }
    #endregion

    #region 更新
    //更新
    public int Update<T>(T t) where T : BaseData//T的类型必须为BaseData类型，也就是ID，也就是规范了加入数据库的表名，返回的这个int是指修改的行数
    {
        if (t == default(T))
        {
            Debug.LogError("Update()参数错误！");
            return -1;
        }
        var type = typeof(T);
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append($"UPDATE {type.Name} set ");// update 语法 stringBuilder.Append("UPDATE table_name ")
        var propertys = type.GetProperties();//获取属性
        foreach (var p in propertys)
        {
            if (p.GetCustomAttribute<ModelHelp>().IsCreated)//判断是否创建
            {
                stringBuilder.Append($"{p.GetCustomAttribute<ModelHelp>().FieldName} = ");
                if (p.GetCustomAttribute<ModelHelp>().Type == "string")//判断是否为string类型
                {
                    stringBuilder.Append($"'{p.GetValue(t)}'");//如果为string 则为他加上一个单引号
                }
                else
                {
                    stringBuilder.Append(p.GetValue(t));//如果不为string则直接添加
                }
                stringBuilder.Append(",");

            }

        }
        stringBuilder.Remove(stringBuilder.Length - 1, 1);
        stringBuilder.Append($" where Time = {t.Time}");
        _sqlComm.CommandText = stringBuilder.ToString();
        return _sqlComm.ExecuteNonQuery();
    }

    //批量更新
    public int Update<T>(List<T> tList) where T : BaseData
    {
        
        if (tList == null || tList.Count == 0 )
        {
            Debug.LogError("Update(list)参数错误！");
            return -1;
        }
        int count = 0;
        foreach (var t in tList)
        {
            count += Update(t);
        }
        return count;
    }
    #endregion
}


