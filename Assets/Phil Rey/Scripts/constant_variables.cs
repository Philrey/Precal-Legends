using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class constant_variables
{
    public string VERSTION = "1.0";
    protected string IP_ADDRESS = "http://localhost/precal";



    protected string getCustomIpAddress(string hostName) {
        return $"http://{hostName}/precal";
    }
}
