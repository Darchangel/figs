module Figs.Parsers.Tests.DotNetXmlConfigParserTests.Parse

open System
open Xunit
open FsUnit.Xunit

open Figs.Parsers

let makeConnectionStringName = DotNetXmlConfigParser.ConnectionStrings.makeConnectionStringName

[<Fact>]
let ``parse with empty string returns an empty map``() =
    let input = ""
    let output = DotNetXmlConfigParser.parse input

    output |> should be Empty

[<Fact>]
let ``parse with invalid XML returns an empty map``() =
    let input = "something"

    DotNetXmlConfigParser.parse input |> should be Empty

[<Fact>]
let ``parse with valid XML but no appSettings / connectionStrings sections returns empty map``() =
    let input = @"<configuration>
                    <someOtherSection>
                    </someOtherSection>
                  </configuration>"

    DotNetXmlConfigParser.parse input |> should be Empty

[<Fact>]
let ``parse with valid XML but empty appSettings section returns empty map``() =
    let input = @"<configuration>
                    <appSettings>
                    </appSettings>
                  </configuration>"

    DotNetXmlConfigParser.parse input |> should be Empty

[<Fact>]
let ``parse with valid XML but empty connectionStrings section returns empty map``() =
    let input = @"<configuration>
                    <connectionStrings>
                    </connectionStrings>
                  </configuration>"

    DotNetXmlConfigParser.parse input |> should be Empty

[<Fact>]
let ``parse with valid XML but empty appSettings and connectionStrings section returns empty map``() =
    let input = @"<configuration>
                    <appSettings>
                    </appSettings>
                    <connectionStrings>
                    </connectionStrings>
                  </configuration>"

    DotNetXmlConfigParser.parse input |> should be Empty

[<Fact>]
let ``parse with valid XML and only appSettings section returns map with appropriate setting``() =
    let input = @"<configuration>
                    <appSettings>
                        <add key=""TestKey"" value=""TestValue"" />
                    </appSettings>
                  </configuration>"


    let map = (DotNetXmlConfigParser.parse input)
    map.["TestKey"] |> should equal "TestValue"

[<Fact>]
let ``parse with valid XML and only connectionStrings section returns map with appropriate setting``() =
    let input = @"<configuration>
                    <connectionStrings>
                        <add name=""TestName"" connectionString=""TestConnectionString"" />
                    </connectionStrings>
                  </configuration>"

    let outputMap = DotNetXmlConfigParser.parse input

    let parsedConnectionStringName = makeConnectionStringName "TestName"
    outputMap.[parsedConnectionStringName] |> should equal  "TestConnectionString"

[<Fact>]
let ``parse with valid XML and both appSettings and connectionStrings sections with a single setting each returns map with all settings``() =
    let input = @"<configuration>
                    <connectionStrings>
                        <add name=""TestName"" connectionString=""TestConnectionString"" />
                    </connectionStrings>
                    <appSettings>
                        <add key=""TestKey"" value=""TestValue"" />
                    </appSettings>
                  </configuration>"


    let expectedConnectionStringName = makeConnectionStringName "TestName"

    let outputMap = DotNetXmlConfigParser.parse input

    outputMap.["TestKey"] |> should equal "TestValue"
    outputMap.[expectedConnectionStringName] |> should equal "TestConnectionString"


[<Fact>]
let ``parse with valid XML and both appSettings and connectionStrings sections with multiple setting each returns map with all settings``() =
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

    let outputMap = DotNetXmlConfigParser.parse input

    outputMap.[expectedConnectionStringName1] |> should equal "TestConnectionString1"
    outputMap.[expectedConnectionStringName2] |> should equal "TestConnectionString2"
    outputMap.[expectedConnectionStringName3] |> should equal "TestConnectionString3"

    outputMap.["TestKey1"] |> should equal "TestValue1"
    outputMap.["TestKey2"] |> should equal "TestValue2"

