using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatModel 
{
    private static BoatModel _instance;
    public static BoatModel Instance
    {
        get 
        {
            if ((_instance == null))
            {
                _instance = new BoatModel();
            }
            return _instance;
        }
    }

    public List<BoatData> BoatDatas = new List<BoatData>();
    public void InitData(SqlDbCommand sqlDbCommand)
    {
        BoatDatas = sqlDbCommand.SelectBySql<BoatData>();
    }
    //public void InitData(SqlDbCommand sqlDbCommand)
    //{
    //    BoatDatas = sqlDbCommand.SelectBySql<BoatData>();
    //}
}
