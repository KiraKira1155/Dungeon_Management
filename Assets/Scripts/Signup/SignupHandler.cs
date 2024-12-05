using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SignupHandler
{
    [Serializable]
    public class SignupDataEntity
    {
        public SignupData result = new SignupData();
    }

    [Serializable]
    public class SignupData
    {
        public int user_id;
        public string user_nm;
    }

    private const string apiUrl = "http://192.168.19.160/api/user/signup";

    private SignupDataEntity signupData = new SignupDataEntity();

    public void SetData(int id, string userName)
    {

        signupData.result.user_id= id;
        signupData.result.user_nm= userName;
    }

    public int GetID()
    {
        return signupData.result.user_id;
    }

    public IEnumerator PostCoroutine()
    {
        SignupData data = new SignupData()
        {
            user_id = signupData.result.user_id,
            user_nm = signupData.result.user_nm
        };            
        string jsonData = JsonUtility.ToJson(data); // データをJSONに変換

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);

            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            // リクエスト送信
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = request.downloadHandler.text;
                SignupDataEntity apiResponse = JsonUtility.FromJson<SignupDataEntity>(jsonResult); // JSONデータを変換
                signupData = apiResponse;

                Debug.Log("UserID : " + signupData.result.user_id + "UserName : " + signupData.result.user_nm);
            }
            else
            {
                Debug.LogError("Error sending score: " + request.error);
            }
        }
    }
}
