using Xunit.Abstractions;
using Xunit.Sdk;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using OS.OBDII.Models;
using OS.OBDII;

namespace PrimaryTests;

public class General
{

    private readonly ITestOutputHelper output;
    public General(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void FromHEX()
    {

        List<string> inputs = new List<string>()
        {
            "7EFF",
            "7eff",
            "0EFF",
            "7EFF",
            "7A4F",
            "7E2F",
            "77FF",
            "DEF8",
            "7E80"
        };
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        // For Each set of inputs, scenarios
        inputs.ForEach(strArray => {
            this.output.WriteLine("_________________");
            try
            {
                List<uint> values = new List<uint>();
                char[] j = strArray.ToCharArray();
                for(int i = 0;i<j.Length;i++)
                {
                    values.Add(HexTable.FromHex((byte)j[i]));

                }
                sb.Clear();

                sb.Append("Values:");
                values.ForEach(v => {
                    sb.Append($"{Environment.NewLine}{v}");
                });


                this.output.WriteLine(sb.ToString());

            }
            catch (Exception ex)
            {
                this.output.WriteLine($"Error: {ex.Message}");
                Assert.Fail();

            }
            finally
            {
                
            }

        });

    }


    [Fact]

    public void HashCalculation()
    {
        var appId = "bcdb9b51-4f96-4ff2-9345-179b5011154f";
        var calcHash = SecurityManager.GetCRCStringFromString(appId);
        output.WriteLine(calcHash);
        Assert.True(true);
    }







}