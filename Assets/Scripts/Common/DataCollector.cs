using System;
using System.IO;
using UnityEngine;
using UnityEngine.WSA;

public class DataCollector : MonoBehaviour
{

    [SerializeField] public string folderLocation = Environment.CurrentDirectory + "\\data";
    [SerializeField] private string fileLocation;
    [SerializeField] private string date = DateTime.Now.ToString().Replace(" ", "-");
    [SerializeField] public string header;
    void Start()
    {
        if(!Directory.Exists(folderLocation))
        {
            Debug.Log("Creating Folder");
            Directory.CreateDirectory(folderLocation);
        }

        fileLocation = Path.Combine(folderLocation, $"{date}.log");
        File.Create(fileLocation);
        if (header != null)
        {
            WriteLine(header);
        }

    }

    public void WriteLine(string line)
    {
        using (StreamWriter outputWriter = new StreamWriter(fileLocation))
        {
            outputWriter.WriteLine(line);
        }
    }

    public void WriteEntries<T>(T[] entries)
    {
        string line = string.Empty;
        foreach (T entry in entries)
        {
            line += entry.ToString() + " ";
        }
        line = line.Trim();
        WriteLine(line);
    }
}
