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

namespace FK_Tests;

public class FK_Tests
{
    private readonly ITestOutputHelper output;

    public FK_Tests(ITestOutputHelper output)
    {
        this.output = output;
    }


    private List<Tuple<string, string>> appIdCodesAndHashes = new List<Tuple<string, string>>()
    {
        new Tuple<string, string>("94659CBB34C4","387D1274"),
        new Tuple<string, string>("ShitOnASickSticken","A62D0658"),
        new Tuple<string, string>("A62D0658","CA3EB2FF"),
        new Tuple<string, string>("A4100555CBFE2940F","FAE4E41C"),
    };

    [Fact]
    public void LicenseManagerHashCodes()
    {
        //  var sm = new SecurityManager();
        foreach (var t in appIdCodesAndHashes)
        {
            Assert.Equal(t.Item2, SecurityManager.GetCRCStringFromString(t.Item1));
            output.WriteLine("____________________________");
            output.WriteLine("AppId: {0}", t.Item1);
            output.WriteLine("Install Code: {0}\r", t.Item2);
        }

    }

    [Fact]
    void AesTest()
    {
        string inString = "Testing Encryption with a big, long line of text....And then some more, ta' boot.";

        byte[] Key =
        [
            0xA0, 0x2B, 0xAD, 0x1B, 0xDE, 0x4D, 0xD0, 0xFB, 0xE9, 0xF8, 0x1C, 0x02, 0xDA, 0xC0, 0x1A, 0x08,
            0x60, 0xCC, 0x1A, 0xAF, 0xD9, 0xB2, 0xC0, 0x03, 0x10, 0xEC, 0x43, 0xF0, 0x09, 0x6B, 0x0D, 0xAD,
        ];

        byte[] IV =
        [
          0x0C, 0x26, 0xB3, 0xE1, 0x4F, 0x9B, 0x46, 0x7F, 0x1E, 0x07, 0x7D, 0xBF, 0x9A, 0xE0, 0xC4, 0x55
        ];


        byte[] Key0 =
        [
            0xA0, 0x2B, 0xAD, 0x1B, 0xDE, 0x4D, 0xD0, 0xFB, 0xE9, 0xF8, 0x1C, 0x02, 0xDA, 0xC0, 0x1A, 0x08,
            0x60, 0xCC, 0x1A, 0xAF, 0xD9, 0xB2, 0xC0, 0x03, 0x10, 0xEC, 0x43, 0xF0, 0x09, 0x6B, 0x0D, 0xAD,
        ];

        byte[] IV0 =
        [
          0x0C, 0, 0xB3, 0xE1, 0, 0x9B, 0x46, 0x7F, 0x1E, 0x07, 0x7D, 0xBF, 0x9A, 0xE1, 0xC4, 0x55
        ];




        OS.OBDII.Security.AesManager f1 = new OS.OBDII.Security.AesManager(Key, IV);
        OS.OBDII.Security.AesManager f2 = new OS.OBDII.Security.AesManager(Key0, IV);
        byte[] encr = f1.Encrypt(inString);
        this.output.WriteLine($"Original: {inString}");
        this.output.WriteLine($"Encrypted: {Encoding.ASCII.GetString(encr)}");
        try
        {
            this.output.WriteLine($"Back Again: {f2.Decrypt(encr)}");
        }
        catch (Exception e)
        {
            Assert.Fail("Bad decryption");
        }
    }

    private static T LoadFromJsonFile<T>(string filePath) where T : new()
    {
        Assert.True(File.Exists(filePath));
        string jsonData = File.ReadAllText(filePath);
        if (string.IsNullOrEmpty(jsonData)) return new T();

        T returnObject = JsonConvert.DeserializeObject<T>(jsonData); ;

        Assert.NotNull(returnObject);
        Assert.NotNull(returnObject as IManufacturer);

        return returnObject;
    }

    [Fact]
    void EncryptJSONFiles()
    {

        OS.OBDII.Security.AesManager encrypter = new OS.OBDII.Security.AesManager(Key, IV);
        OS.OBDII.Security.AesManager decrypter = new OS.OBDII.Security.AesManager(Key, IV);

        try
        {

            string fileName;
            string outPath = @"C:\Visual Studio Projects\VS2022\AutoScannerMobile_Maui\ManufacturersDTCs\Test\";
            string inFolder = @"C:\Visual Studio Projects\VS2022\AutoScannerMobile_Maui\ManufacturersDTCs\Test";

            DirectoryInfo di = new DirectoryInfo(inFolder);
            var files = Directory.EnumerateFiles(inFolder, "Manufacturers.Honda*.json");

            foreach (string inFilePathString in files)
            {
                fileName = Path.GetFileName(inFilePathString);
                fileName = fileName.Substring(0, fileName.LastIndexOf('.'));

                File.WriteAllBytes(outPath + fileName + ".bin", encrypter.Encrypt(File.ReadAllText(inFilePathString)));
            }
        }
        catch (Exception e)
        {
            Assert.Fail("Bad encryption");
        }

    }


    byte[] Key =
    [
        0xA8, 0x2B, 0xAD, 0x1B, 0x2E, 0x4D, 0xD0, 0xFB, 0xE9, 0xF8, 0x1C, 0x72, 0xDA, 0xC0, 0x1A, 0x08,
        0x65, 0xCC, 0x1A, 0xAF, 0xD9, 0xB2, 0xCB, 0x03, 0x10, 0xEC, 0x43, 0xF0, 0x39, 0x6B, 0x0D, 0xAD,
    ];

    byte[] IV =
    [
        0x0A, 0x26, 0xB3, 0xE1, 0x4F, 0x9B, 0x46, 0x7F, 0x1E, 0x07, 0x7D, 0xBF, 0x9A, 0xE0, 0xC4, 0x55
    ];



    [Fact]
    void FileDecryption()
    {

        OS.OBDII.Security.AesManager decrypter = new OS.OBDII.Security.AesManager(Key, IV);

        try
        {

            string fileName;
            string outPath = @"C:\Visual Studio Projects\VS2022\AutoScannerMobile_Maui\ManufacturersDTCs\Test\Un_Encrypted_";
            string inFolder = @"C:\Visual Studio Projects\VS2022\AutoScannerMobile_Maui\ManufacturersDTCs\Test";

            DirectoryInfo di = new DirectoryInfo(inFolder);
            var files = Directory.EnumerateFiles(inFolder, "*Bajaj125.json.bin");

            foreach (string inFilePathString in files)
            {
               // fileName = Path.GetFileName(inFilePathString);
                FileInfo fi = new FileInfo(inFilePathString);
                fileName = fi.Name;
                var fileData = File.ReadAllBytes(inFilePathString);
                File.WriteAllText(outPath + fileName + ".json", decrypter.Decrypt(fileData));
            }
        }
        catch (Exception e)
        {
            Assert.Fail("Bad decryption");
        }

    }





}

