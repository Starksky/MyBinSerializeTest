using System.Collections.Generic;
using Serialization;
using UnityEngine;

public class Serialize : MonoBehaviour
{
    private DataServer _dataServer;
    private DataServer _dataServer1;

    private Dictionary<string, string> _data;
    // Start is called before the first frame update
    void Start()
    {
        _dataServer = new DataServer();
        _dataServer.Name = "TestSerialize";
        _dataServer.dict.Add(2, "fffdffdf");
        _dataServer.dataServer1.dict.Add(3, "fdfdfdw3");
        _dataServer.pos.Add(0.005f);

        var data = BinarySerialization.Serialization(_dataServer);
        _dataServer1 = BinarySerialization.Deserialization<DataServer>(data);
        
        data = BinarySerialization.Serialization(_dataServer1.dict);
        var dict = BinarySerialization.Deserialization<Dictionary<int, string>>(data);
        
        Debug.Log(_dataServer1.Name);
        Debug.Log(_dataServer1.dict[2]);
        Debug.Log(_dataServer1.dataServer1.dict[3]);
        Debug.Log(_dataServer1.pos[0]);
        
        Debug.Log(dict[2]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
