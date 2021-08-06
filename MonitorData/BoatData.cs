using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatData : BaseData
{
    private float _surge;
    [ModelHelp(true, "Surge", "single", false,true)]
    public float Surge { get { return _surge; } set {_surge = value; } }
    private float _roll;
    [ModelHelp(true, "Roll", "single", false, true)]
    public float Roll { get { return _roll; } set { _roll = value; } }
    private float _sway;
    [ModelHelp(true, "Sway", "single", false, true)]
    public float Sway { get { return _sway; } set { _sway = value; } }
    private float _pitch;
    [ModelHelp(true, "Pitch", "single", false, true)]
    public float Pitch { get { return _pitch; } set { _pitch = value; } }
    private float _heave;
    [ModelHelp(true, "Heave", "single", false, true)]
    public float Heave { get { return _heave; } set { _heave = value; } }
    private float _yaw;
    [ModelHelp(true, "Yaw", "single", false, true)]
    public float Yaw { get { return _yaw; } set { _yaw = value; } }

}

public class BaseData//将Time独立出来，并且让BoatData继承
{
    private int _time;
    [ModelHelp(true, "Time", "int", true, false)]
    public int Time { get { return _time; } set { _time = value; } }
}
