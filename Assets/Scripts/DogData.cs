using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DogBreed
{
    public int id;
    public string name;
    public string bred_for;
    public string breed_group;
    public string life_span;
    public string temperament;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{ \"Items\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.Items;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
