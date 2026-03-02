using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DogAPIProvider : MonoBehaviour
{
    private const string BASE_URL = "https://api.thedogapi.com/v1/breeds";

    [SerializeField] private string apiKey = "";

    public delegate void OnBreedsLoaded(List<DogBreed> breeds, int totalCount);
    public delegate void OnRequestError(string error);

    public void GetBreeds(int page, int limit, OnBreedsLoaded onSuccess, OnRequestError onError)
    {
        StartCoroutine(GetBreedsCoroutine(page, limit, onSuccess, onError));
    }

    private IEnumerator GetBreedsCoroutine(int page, int limit, OnBreedsLoaded onSuccess, OnRequestError onError)
    {
        string url = $"{BASE_URL}?limit={limit}&page={page}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("User-Agent", "Unity-DogBrowser-App");
            
            if (!string.IsNullOrEmpty(apiKey))
            {
                request.SetRequestHeader("x-api-key", apiKey);
            }
            
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                DogBreed[] breeds = JsonHelper.FromJson<DogBreed>(json);
                
                int totalCount = -1;
                string totalHeader = request.GetResponseHeader("Pagination-Count");
                
                if (string.IsNullOrEmpty(totalHeader))
                {
                    totalHeader = request.GetResponseHeader("pagination-count");
                }

                if (!string.IsNullOrEmpty(totalHeader) && int.TryParse(totalHeader, out int count))
                {
                    totalCount = count;
                }
                
                onSuccess?.Invoke(new List<DogBreed>(breeds), totalCount);
            }
        }
    }
}
