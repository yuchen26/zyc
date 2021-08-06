using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelHelp:Attribute
{ 
    /// <summary>
    /// 是否创建字段
    /// </summary>
    public bool IsCreated { get; set;  }
    /// <summary>
    /// 对应到数据库的字段名称
    /// </summary>
    public string FieldName { get; set; }
    /// <summary>
    /// 对应到数据库的字段类型
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// 是否为主键
    /// </summary>
    public bool IsPrimaryKey { get; set; }
    /// <summary>
    /// 是否可以为空
    /// </summary>
    public bool IsCanBeNull { get; set; }

    public ModelHelp(bool isCreated, string fieldName, string type,bool isPrimaryKey,bool isCanBeNull=false)
    {
        IsCreated = isCreated;
        FieldName = fieldName;
        Type = type;
        IsPrimaryKey = isPrimaryKey;
        IsCanBeNull = isCanBeNull;

    }


}
