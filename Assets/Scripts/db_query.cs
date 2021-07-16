using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class db_query : MonoBehaviour
{
    private string IP_ADDRESS = "http://localhost/precal/";
    //private string IP_ADDRESS_BACKUP = "http://localhost/precal/";

    private static string returnResult;
    private static string updateResult;

    private void Start() {
        return_values("*", "users", "");
    }

    #region CRUD Functions
    public void return_values(string select, string from, string where) {
        StartCoroutine(returnValues(select, from, where, 0));
    }
    public void update_values() {

    }
    #endregion

    
    private IEnumerator returnValues(string select, string from, string where,int functionSelected) {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("select=*&from=users&where="));

        string postData = "select=" + select + "&from=" + from + "&where=" + where;

        string uri = IP_ADDRESS + "returnValues.php?" + postData;

        Debug.Log(uri);

        UnityWebRequest httpRequest = UnityWebRequest.Get(uri);
        httpRequest.downloadHandler = new DownloadHandlerBuffer();

        yield return httpRequest.SendWebRequest();

        if (httpRequest.result == UnityWebRequest.Result.ConnectionError
            || httpRequest.result == UnityWebRequest.Result.ProtocolError){
            Debug.Log("Error Found..." + httpRequest.error);

            #region Execute when error occurs
            switch (functionSelected)
            {
                case 0:{
                    break;
                }
                case 1:{
                    break;
                }
            }
            #endregion
        }else {
            string response = httpRequest.downloadHandler.text;            
            JSONNode result = SimpleJSON.JSON.Parse(response);
            
            //Debug.Log("Server Responded: " + result["result"][0]["display_name"].ToString() + " Rows: " + result["result"].Count + " Columns: " + result["result"][0].Count);
            
            JSONArray jsonArray = result["result"].AsArray;
            

            switch (functionSelected)
            {
                case 0:{
                    break;
                }
                case 1:{
                    //StartCoroutine(loadRoomsThread(jsonArray));
                    break;
                }
            }

        }
    }
    public void log(string msg) {
        Debug.Log(msg);
    }
}
