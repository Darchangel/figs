module Figs.Common.Tests.DotNetXmlConfigParserTests.Parse

open System
open Xunit
open FsUnit.Xunit

open Figs.Common

let makeConnectionStringName = DotNetXmlConfigParser.ConnectionStrings.makeConnectionStringName

[<Fact>]
let ``parse with empty string returns an empty dictionary``() =
    let input = ""
    let output = DotNetXmlConfigParser.parse input

    output |> should be Empty

[<Fact>]
let ``parse with invalid XML returns an empty dictionary``() =
    let input = "something"

    DotNetXmlConfigParser.parse input |> should be Empty

[<Fact>]
let ``parse with valid XML but no appSettings / connectionStrings sections returns empty dictionary``() =
    let input = @"<configuration>
                    <someOtherSection>
                    </someOtherSection>
                  </configuration>"

    DotNetXmlConfigParser.parse input |> should be Empty

[<Fact>]
let ``parse with valid XML but empty appSettings section returns empty dictionary``() =
    let input = @"<configuration>
                    <appSettings>
                    </appSettings>
                  </configuration>"

    DotNetXmlConfigParser.parse input |> should be Empty

[<Fact>]
let ``parse with valid XML but empty connectionStrings section returns empty dictionary``() =
    let input = @"<configuration>
                    <connectionStrings>
                    </connectionStrings>
                  </configuration>"

    DotNetXmlConfigParser.parse input |> should be Empty

[<Fact>]
let ``parse with valid XML but empty appSettings and connectionStrings section returns empty dictionary``() =
    let input = @"<configuration>
                    <appSettings>
                    </appSettings>
                    <connectionStrings>
                    </connectionStrings>
                  </configuration>"

    DotNetXmlConfigParser.parse input |> should be Empty

[<Fact>]
let ``parse with valid XML and only appSettings section returns dictionary with appropriate setting``() =
    let input = @"<configuration>
                    <appSettings>
                        <add key=""TestKey"" value=""TestValue"" />
                    </appSettings>
                  </configuration>"


    let dictionary = (DotNetXmlConfigParser.parse input)
    dictionary.Item("TestKey") |> should equal "TestValue"

[<Fact>]
let ``parse with valid XML and only connectionStrings section returns dictionary with appropriate setting``() =
    let input = @"<configuration>
                    <connectionStrings>
                        <add name=""TestName"" connectionString=""TestConnectionString"" />
                    </connectionStrings>
                  </configuration>"

    let outputDictionary = DotNetXmlConfigParser.parse input

    let parsedConnectionStringName = makeConnectionStringName "TestName"
    outputDictionary.Item(parsedConnectionStringName) |> should equal  "TestConnectionString"

[<Fact>]
let ``parse with valid XML and both appSettings and connectionStrings sections with a single setting each returns dictionary with all settings``() =
    let input = @"<configuration>
                    <connectionStrings>
                        <add name=""TestName"" connectionString=""TestConnectionString"" />
                    </connectionStrings>
                    <appSettings>
                        <add key=""TestKey"" value=""TestValue"" />
                    </appSettings>
                  </configuration>"


    let expectedConnectionStringName = makeConnectionStringName "TestName"

    let outputDictionary = DotNetXmlConfigParser.parse input

    outputDictionary.Item("TestKey") |> should equal "TestValue"
    outputDictionary.Item(expectedConnectionStringName) |> should equal "TestConnectionString"


[<Fact>]
let ``parse with valid XML and both appSettings and connectionStrings sections with multiple setting each returns dictionary with all settings``() =
    let input = @"<configuration>
                    <connectionStrings>
                        <add name=""TestName1"" connectionString=""TestConnectionString1"" />
                        <add name=""TestName2"" connectionString=""TestConnectionString2"" />
                        <add name=""TestName3"" connectionString=""TestConnectionString3"" />
                    </connectionStrings>
                    <appSettings>
                        <add key=""TestKey1"" value=""TestValue1"" />
                        <add key=""TestKey2"" value=""TestValue2"" />
                    </appSettings>
                  </configuration>"


    let expectedConnectionStringName1 = makeConnectionStringName "TestName1"
    let expectedConnectionStringName2 = makeConnectionStringName "TestName2"
    let expectedConnectionStringName3 = makeConnectionStringName "TestName3"

    let outputDictionary = DotNetXmlConfigParser.parse input

    outputDictionary.Item(expectedConnectionStringName1) |> should equal "TestConnectionString1"
    outputDictionary.Item(expectedConnectionStringName2) |> should equal "TestConnectionString2"
    outputDictionary.Item(expectedConnectionStringName3) |> should equal "TestConnectionString3"

    outputDictionary.Item("TestKey1") |> should equal "TestValue1"
    outputDictionary.Item("TestKey2") |> should equal "TestValue2"

