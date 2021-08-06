using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadData : MonoBehaviour
{

    /// <summary>
    /// 存档路径地址
    /// </summary>
    private string dbPath;
    private string[] saveData;
    //private GameObject[] btns;//客涌入创建按钮控制存档
    public BoatData BoatData;
    private SqlDbCommand _sqlite;
    private int i = 0;

    // Start is called before the first frame update  
    void Start()
    {
        dbPath = Application.dataPath + "/../Assets/StreamingAssets/";
        saveData = new[] { "SaveData.db" };
        InitUI();
        InvokeRepeating("UseSec", 1f, 1f);
        


    }
    private void Update()
    {
        if (i == BoatModel.Instance.BoatDatas.Count)//结束调用函数UseSec
        {
            CancelInvoke("UseSec");

        }
    }
    private void InitUI()//获取数据，链接数据库，加载数据
    {
        var info = GetAllSaveDataInfo();
        _sqlite = new SqlDbCommand(dbPath + saveData[0]);
        //_sqlite.CreateTable<BoatData>();//创建表
        BoatModel.Instance.InitData(_sqlite);//加载数据
        //GenerateBoatSec();
        //SqlDbConnect.CreateDbSqlite(dbPath + saveData[0]);//创建存档
        //if (info)//如果存档存在--
        //{
        //    //加载数据，金菜单界面
        //}
        //else 
        //{
        //    //创建存档
        //        CreatSaveData();
        //}


    }

    #region Util
    private Dictionary<int, bool> GetAllSaveDataInfo()
    {
        Dictionary<int, bool> ret = new Dictionary<int, bool>();
        if (File.Exists(dbPath + saveData[0]))//判断存档是否存在
        {
            ret.Add(0, true);
        }
        else
        {
            ret.Add(0, false);
        }
        return ret;
    }

    private void CreatSaveData()//创建存档
    {
        if (SqlDbConnect.CreateDbSqlite(dbPath + saveData[0]))
        {
            InitDbData();
        }
    }

    private void InitDbData()//初始化哪一个存档的数据
    {
        _sqlite = new SqlDbCommand(dbPath + saveData[0]);
        _sqlite.Dispose();//将所占用的资源清理掉,在此代码之前可以插入数据创建表等，本项目用excel插入
        _sqlite = null;

    }

    void UseSec()
    {
        var x = BoatModel.Instance.BoatDatas;

        Debug.Log($"Time{i+1} = " + x[i].Time);
        Debug.Log($"Surge{i + 1} = "+ x[i].Surge);
        Debug.Log($"Roll{i + 1} = "+ x[i].Roll);
        Debug.Log($"Pitch{i + 1} = " + x[i].Pitch);
        Debug.Log($"Heave{i + 1} = " + x[i].Heave);
        Debug.Log($"Yaw{i + 1} = " + x[i].Yaw);
        i = i + 1;
        
    }

    //private void GenerateBoatSec()
    //{
    //    foreach (var x in BoatModel.Instance.BoatDatas)
    //    {
    //        BoatData = x;
    //    }
    //}
    #endregion
}
