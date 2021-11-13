using System;
using System.Collections.Generic;

[Serializable]
public class DataServer
{
    public string Name { get; set; } = "Test";
    public string OtherData { get; set; } = "Other";
    public Dictionary<int, string> dict { get; set; } = new Dictionary<int, string>();
    public DataServer1 dataServer1 { get; set; } = new DataServer1();
    public List<float> pos { get; set; } = new List<float>();
}

[Serializable]
public class DataServer1
{
    public string Name { get; set; } = "Test1";
    public string OtherData { get; set; } = "Other1";
    public Dictionary<int, string> dict { get; set; } = new Dictionary<int, string>();
}