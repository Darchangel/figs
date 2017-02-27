module Figs.Common.Tests.DotNetXmlConfigParserTests.Write

open Xunit
open FsUnit.Xunit
open System.Xml.Linq

open Figs.Common

let makeConnectionStringName = DotNetXmlConfigParser.ConnectionStrings.makeConnectionStringName

[<Fact>]
let ``write with empty list returns empty but valid xml string``() =
    let input = dict([])
    let expectedXml = new XElement(XName.Get("configuration"))

    let output = DotNetXmlConfigParser.write input
    let xmlOutput = XElement.Parse(output)

    xmlOutput.ToString() |> should equal (expectedXml.ToString())

[<Fact>]
let ``write with single app setting returns xml string with correct appSettings section only``() =
    let input = dict([("TestKey", "TestValue")])
    let expectedOutput = @"<configuration>
                                <appSettings>
                                    <add key=""TestKey"" value=""TestValue"" />
                                </appSettings>
                           </configuration>"
    let expectedXml = XElement.Parse(expectedOutput)

    let output = DotNetXmlConfigParser.write input
    let xmlOutput = XElement.Parse(output)

    xmlOutput.ToString() |> should equal (expectedXml.ToString())

[<Fact>]
let ``write with single connection string returns xml string with correct connectionStrings section only``() =
    let input = dict([(makeConnectionStringName("TestName"), "TestConnectionString")])
    let expectedOutput = @"<configuration>
                                <connectionStrings>
                                    <add name=""TestName"" connectionString=""TestConnectionString"" />
                                </connectionStrings>
                           </configuration>"
    let expectedXml = XElement.Parse(expectedOutput)

    let output = DotNetXmlConfigParser.write input
    let xmlOutput = XElement.Parse(output)

    xmlOutput.ToString() |> should equal (expectedXml.ToString())

[<Fact>]
let ``write with multiple app settings returns xml string with correct appSettings only, in order``() =
    let input = dict([("TestKey1", "TestValue1");
                      ("TestKey2", "TestValue2");
                      ("TestKey3", "TestValue3")])
    let expectedOutput = @"<configuration>
                                <appSettings>
                                    <add key=""TestKey1"" value=""TestValue1"" />
                                    <add key=""TestKey2"" value=""TestValue2"" />
                                    <add key=""TestKey3"" value=""TestValue3"" />
                                </appSettings>
                           </configuration>"
    let expectedXml = XElement.Parse(expectedOutput)

    let output = DotNetXmlConfigParser.write input
    let xmlOutput = XElement.Parse(output)

    xmlOutput.ToString() |> should equal (expectedXml.ToString())

[<Fact>]
let ``write with multiple connection strings returns xml string with correct connectionStrings only, in order``() =
    let input = dict([(makeConnectionStringName("TestName1"), "TestConnectionString1");
                      (makeConnectionStringName("TestName2"), "TestConnectionString2");
                      (makeConnectionStringName("TestName3"), "TestConnectionString3")])
    let expectedOutput = @"<configuration>
                                <connectionStrings>
                                    <add name=""TestName1"" connectionString=""TestConnectionString1"" />
                                    <add name=""TestName2"" connectionString=""TestConnectionString2"" />
                                    <add name=""TestName3"" connectionString=""TestConnectionString3"" />
                                </connectionStrings>
                           </configuration>"
    let expectedXml = XElement.Parse(expectedOutput)

    let output = DotNetXmlConfigParser.write input
    let xmlOutput = XElement.Parse(output)

    xmlOutput.ToString() |> should equal (expectedXml.ToString())

[<Fact>]
let ``write with multiple app settings and connection strings returns correct xml string with all settings in order``() =
    let input = dict([(makeConnectionStringName("TestName1"), "TestConnectionString1");
                      ("TestKey1", "TestValue1");
                      ("TestKey2", "TestValue2");
                      (makeConnectionStringName("TestName2"), "TestConnectionString2");
                      (makeConnectionStringName("TestName3"), "TestConnectionString3");
                      ("TestKey3", "TestValue3")])

    let expectedOutput = @"<configuration>
                                <appSettings>
                                    <add key=""TestKey1"" value=""TestValue1"" />
                                    <add key=""TestKey2"" value=""TestValue2"" />
                                    <add key=""TestKey3"" value=""TestValue3"" />
                                </appSettings>
                                <connectionStrings>
                                    <add name=""TestName1"" connectionString=""TestConnectionString1"" />
                                    <add name=""TestName2"" connectionString=""TestConnectionString2"" />
                                    <add name=""TestName3"" connectionString=""TestConnectionString3"" />
                                </connectionStrings>
                           </configuration>"
    let expectedXml = XElement.Parse(expectedOutput)

    let output = DotNetXmlConfigParser.write input
    let xmlOutput = XElement.Parse(output)

    xmlOutput.ToString() |> should equal (expectedXml.ToString())
