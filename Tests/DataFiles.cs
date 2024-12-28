using Force.Crc32;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Newtonsoft.Json;
using OS.OBDII;
using OS.OBDII.Manufacturers;
using SkiaSharp;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using Xunit.Abstractions;
using Xunit.Sdk;
using OS.OBDII.Interfaces;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Core.Primitives;
using System.Net.Http;
using Tests;
using System.Text.Encodings.Web;
using HtmlAgilityPack;
using System.Reflection.Metadata;
using System.Xml.Linq;
using System.Net.Http.Headers;
using OxyPlot;
using System.Xml;

namespace FileTests;

public class DataFiles
{


    private readonly ITestOutputHelper output;

    public DataFiles(ITestOutputHelper output)
    {
        this.output = output;
    }

    private static T LoadFromJsonFile<T>(string filePath) where T : new()
    {
        Assert.True(File.Exists(filePath));
        string jsonData = File.ReadAllText(filePath);
        if (string.IsNullOrEmpty(jsonData)) return new T();
        T returnObject = JsonConvert.DeserializeObject<T>(jsonData); ;
        Assert.NotNull(returnObject);

        return returnObject;
    }

    [Fact]
    public async Task LoadSingleModel()
    {
        var jsonDBPath = @"..\..\..\..\SimpleOBDII\Resources\Raw\manufacturers.custom.json";

        Assert.True(File.Exists(jsonDBPath));

        var db = LoadFromJsonFile<VehicleModel>(jsonDBPath);

        foreach (var item in db.FaultCodes)
        {
            // Do something with each fault code in the file
           // item.Value.Description = "Error Code " + item.Value.PIDName;
        }

        if (false)
        {
            // Re-Write the file - saving changes that were made - make sure there's a backup
            string json = JsonConvert.SerializeObject(db, Newtonsoft.Json.Formatting.Indented);
            using (FileStream fs1 = File.Create(jsonDBPath))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(json);
                fs1.Write(info, 0, info.Length);
            }
        }

    }

    [Fact]
    public void LoadAllModelsFromFolder()
    {

        var inFolder = @"..\..\..\..\SimpleOBDII\Resources\Raw";

        DirectoryInfo di = new DirectoryInfo(inFolder);
        // get a list of files based on the search patter (*.*, *.json, honda.json etc)
        var files = Directory.EnumerateFiles(inFolder, "manufacturers.*.json");

        string fileName = string.Empty;
        IVehicleModel specificModel = null;
        // For each file found...
        foreach (string inFilePathString in files)
        {
            fileName = Path.GetFileName(inFilePathString);
            specificModel = null;
            try
            {
                specificModel = LoadFromJsonFile<VehicleModel>(inFilePathString);
                Assert.NotNull(specificModel as IVehicleModel);

                //specificModel.Name = "Some New Name";
                foreach (var item in specificModel.FaultCodes)
                {
                    // Do something to each fault code, change description etc...
                    //item.Value.Description = "Some new description for " + item.Value.PIDName;
                }


                if (false)
                {
                    // Re-Write the file - saving changes that were made - make sure there's a backup
                    string json = JsonConvert.SerializeObject(specificModel, Newtonsoft.Json.Formatting.Indented);
                    using (FileStream fs1 = File.Create(inFilePathString))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(json);
                        fs1.Write(info, 0, info.Length);
                    }
                }



            }
            catch (Exception e)
            {
            }
        }
        this.output.WriteLine($"{files.Count()} file{(files.Count() != 1 ? "s" : string.Empty)} found.");
    }




    }


public static class Extensions
{
    public static string NormalizeJson(this string input)
    {
        return JsonConvert.SerializeObject(JsonConvert.DeserializeObject<object>(input));
    }

}


static class HttpResponseMessageExtensions
{
    internal static async Task<string> ResponseToString(this HttpResponseMessage response)
    {
        if (response is null)
        {
            return string.Empty;
        }

        //var request = response.RequestMessage;
        return await response.Content.ReadAsStringAsync();
        //output.Write($"{request?.Method} ");
        //Console.Write($"{request?.RequestUri} ");
        //Console.WriteLine($"HTTP/{request?.Version}");
    }
}
