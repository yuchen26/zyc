using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System;
using System.IO;

/// <summary>
/// sqlite链接
/// </summary>
public class SqlDbConnect
{
    protected SqliteConnection _sqlConn;//保存Unity和sqlite连接的数据变量
    public SqlDbConnect(string dbPath)//判断路径中是否存在数据库文件，若存在则直接链接，不存在就创建一个
    {
        if (!File.Exists(dbPath))
        {
            CreateDbSqlite(dbPath);
        }
        ConnectDbSqlite(dbPath);
    }
    /// <summary>
    /// 创建数据库
    /// </summary>
    /// <param name="dbPath"></param>
    /// <returns></return s>
    public static bool CreateDbSqlite(string dbPath)
    {
        if (File.Exists(dbPath))
        {
            Debug.Log("数据库文件已存在，请勿重复创建！");
            return false;
        }
        try
        {
            var dirName = new FileInfo(dbPath).Directory.FullName;//判断父目录是否存在，若不存在就创建一个（没啥用的，增加稳定性）
            if (!Directory.Exists(dirName))
            { 
                Directory.CreateDirectory(dirName);

            }
            SqliteConnection.CreateFile(dbPath);//在此路径上创建，成功返回true，失败返回false
            return true;
        }

        catch (Exception e)
        {
            Debug.LogError($"数据库创建异常:{e.Message}");
            return false;
        }

    }
    /// <summary>
    /// 链接数据库并将其打开
    /// </summary>
    /// <param name="dbPath"></param>
    /// <returns></returns>
    private bool ConnectDbSqlite(string dbPath)
    {
        try
        {
            _sqlConn = new SqliteConnection(new SqliteConnectionStringBuilder() { DataSource = dbPath }.ToString());
            _sqlConn.Open();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"数据库连接异常:{e.Message}");
            return false;
        }

    }
    public void Dispose()//释放链接变量是所占用的资源
    {
        _sqlConn.Dispose();
    }
}
