module Figs.Common.Tests.DotNetXmlConfigParserTests.Parse

open Xunit
open FsUnit.Xunit

open Figs.Common

let CONNECTION_STRING_PREFIX = DotNetXmlConfigParser.ConnectionStrings.CONNECTION_STRING_PREFIX

[<Fact>]
let ``parse with empty string returns an empty list``() =
    let input = ""
    let output = DotNetXmlConfigParser.parse input

    output |> should be Empty

[<Fact>]
let ``parse with invalid XML returns an empty list``() =
    let input = "something"

    DotNetXmlConfigParser.parse input |> should be Empty

[<Fact>]
let ``parse with valid XML but no appSettings / connectionStrings sections returns empty list``() =
    let input = @"<configuration>
                    <someOtherSection>
                    </someOtherSection>
                  </configuration>"

    DotNetXmlConfigParser.parse input |> should be Empty

[<Fact>]
let ``parse with valid XML but empty appSettings section returns empty list``() =
    let input = @"<configuration>
                    <appSettings>
                    </appSettings>
                  </configuration>"

    DotNetXmlConfigParser.parse input |> should be Empty

[<Fact>]
let ``parse with valid XML but empty connectionStrings section returns empty list``() =
    let input = @"<configuration>
                    <connectionStrings>
                    </connectionStrings>
                  </configuration>"

    DotNetXmlConfigParser.parse input |> should be Empty

[<Fact>]
let ``parse with valid XML but empty appSettings and connectionStrings section returns empty list``() =
    let input = @"<configuration>
                    <appSettings>
                    </appSettings>
                    <connectionStrings>
                    </connectionStrings>
                  </configuration>"

    DotNetXmlConfigParser.parse input |> should be Empty

[<Fact>]
let ``parse with valid XML and only appSettings section returns list with appropriate setting``() =
    let input = @"<configuration>
                    <appSettings>
                        <add key=""TestKey"" value=""TestValue"" />
                    </appSettings>
                  </configuration>"


    let expectedResult = [("TestKey", "TestValue")]
    DotNetXmlConfigParser.parse input |> should equal expectedResult

[<Fact>]
let ``parse with valid XML and only connectionStrings section returns list with appropriate setting``() =
    let input = @"<configuration>
                    <connectionStrings>
                        <add name=""TestName"" connectionString=""TestConnectionString"" />
                    </connectionStrings>
                  </configuration>"


    let expectedName = CONNECTION_STRING_PREFIX + "TestName"
    let expectedResult = [(expectedName, "TestConnectionString")]
    DotNetXmlConfigParser.parse input |> should equal expectedResult

[<Fact>]
let ``parse with valid XML and both appSettings and connectionStrings sections with a single setting each returns list with all settings``() =
    let input = @"<configuration>
                    <connectionStrings>
                        <add name=""TestName"" connectionString=""TestConnectionString"" />
                    </connectionStrings>
                    <appSettings>
                        <add key=""TestKey"" value=""TestValue"" />
                    </appSettings>
                  </configuration>"


    let expectedConnectionStringName = CONNECTION_STRING_PREFIX + "TestName"
    let expectedConnectionStringResult = (expectedConnectionStringName, "TestConnectionString")

    let expectedAppSettingsResult = ("TestKey", "TestValue")

    let expectedResult = [expectedAppSettingsResult; expectedConnectionStringResult]

    DotNetXmlConfigParser.parse input |> should equal expectedResult



let listsAreEqual list1 list2 =  set list1 = set list2

[<Fact>]
let ``parse with valid XML and both appSettings and connectionStrings sections with multiple setting each returns list with all settings``() =
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


    let expectedConnectionStringName1 = CONNECTION_STRING_PREFIX + "TestName1"
    let expectedConnectionStringName2 = CONNECTION_STRING_PREFIX + "TestName2"
    let expectedConnectionStringName3 = CONNECTION_STRING_PREFIX + "TestName3"
    let expectedConnectionStringResult1 = (expectedConnectionStringName1, "TestConnectionString1")
    let expectedConnectionStringResult2 = (expectedConnectionStringName2, "TestConnectionString2")
    let expectedConnectionStringResult3 = (expectedConnectionStringName3, "TestConnectionString3")

    let expectedAppSettingsResult1 = ("TestKey1", "TestValue1")
    let expectedAppSettingsResult2 = ("TestKey2", "TestValue2")

    let expectedResult = [expectedAppSettingsResult1; expectedAppSettingsResult2; expectedConnectionStringResult1; expectedConnectionStringResult2; expectedConnectionStringResult3]
    let result = DotNetXmlConfigParser.parse input

    listsAreEqual result expectedResult |> should equal true

