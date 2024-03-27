using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CSVManager
{
    private static string reportSeparator = ";";
    private static string timeStamplHeader = "time stamp";
    private static string[] reportHeaders = new string[7]
    {
        "Depth",
        "Piece",
        "MoveAction",
        "TypeAction",
        "Eval",
        "Player",
        "Turn"
    };

    private static string GetFilePath()
    {
        return Application.persistentDataPath + "//datas.csv";
    }

    public static void VerifyFile()
    {
        string file = GetFilePath();
        if (!File.Exists(file))
        {
            CreateReport();
        }
    }

    public static void CreateReport()
    {
        using (StreamWriter sw = File.CreateText(GetFilePath()))
        {
            string finalString = "";
            for (int i = 0; i < reportHeaders.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += reportHeaders[i];
            }
            finalString += reportSeparator + timeStamplHeader;
            sw.WriteLine(finalString);
        }      
    }

    public static void SaveData(string[] datas)
    {
        VerifyFile();

        using (StreamWriter sw = File.AppendText(GetFilePath()))
        {
            string finalString = "";
            for (int i = 0; i < datas.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += datas[i];
            }
            finalString += reportSeparator + GetTimeStamp();
            sw.WriteLine(finalString);
        }
    }

    private static string GetTimeStamp()
    {
        return System.DateTime.UtcNow.ToString();
    }

}

[Serializable]
public class Data
{
    public int maxEval;
    public string type;
    public Vector2Int bestMove;
}

[Serializable]
public class BigData
{
    public List<Data> L_datas = new List<Data>();
}
