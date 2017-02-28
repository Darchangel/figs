module Figs.Parsers.DotNetXmlConfigParser

open System
open FSharp.Data
open System.Xml.Linq
open System.Collections.Generic


module ConnectionStrings =
    let CONNECTION_STRING_PREFIX = "__CONNECTION_STRING__."

    let makeConnectionStringName (name : string) =
        CONNECTION_STRING_PREFIX + name

    let makeNormalName (name : string) =
        name.Remove(0, CONNECTION_STRING_PREFIX.Length)

    let isConnectionString (name : string) =
        name.StartsWith CONNECTION_STRING_PREFIX

type private Config = XmlProvider<"file type samples/DotNetXmlConfig.config", SampleIsList = true>

let public parse (fileString : String) =
    try
        match fileString with
            | "" -> dict([])
            | str -> 
                let config = Config.Parse fileString
                let appSettings = if config.AppSettings.IsSome then
                                    [for setting in config.AppSettings.Value.Adds do yield (setting.Key, setting.Value)]
                                  else
                                    []
                let connectionStrings = if config.ConnectionStrings.IsSome then
                                          [for setting in config.ConnectionStrings.Value.Adds do
                                              yield ((ConnectionStrings.makeConnectionStringName setting.Name), setting.ConnectionString)]
                                        else
                                          []

                let allSettings = List.concat [appSettings; connectionStrings]

                dict(allSettings)
    with
        :? Exception as ex -> dict([])

let private unzipSettings (settingsDict : IDictionary<string, string>) =

    let appSettings = [for KeyValue(key, value) in settingsDict do if not (ConnectionStrings.isConnectionString key) then yield (key, value) ]
    let connectionStrings = [for KeyValue(key, value) in settingsDict  do if (ConnectionStrings.isConnectionString key) then yield (key, value) ]

    (appSettings, connectionStrings)
    

let private buildAppSettingsXml (appSettingsList : (string * string) list) =
    if appSettingsList.IsEmpty then
        None
    else
        let appSettingsElements = [for name, value in appSettingsList do yield new XElement(XName.Get("add"), new XAttribute(XName.Get("key"), name), new XAttribute(XName.Get("value"), value))]
        Some (new XElement(XName.Get("appSettings"), appSettingsElements))

let private buildConnectionStringsXml (connectionStringsList : (string * string) list) =
    if connectionStringsList.IsEmpty then
        None
    else
        let connectionStringElements = [for name, value in connectionStringsList do yield new XElement(XName.Get("add"), new XAttribute(XName.Get("name"), ConnectionStrings.makeNormalName name), new XAttribute(XName.Get("connectionString"), value))]
        Some (new XElement(XName.Get("connectionStrings"), connectionStringElements))

let private buildConfigurationXml (appSettingsSection : option<XElement>) (connectionStringsSection : option<XElement>) =
    match appSettingsSection, connectionStringsSection with
        | None, None -> new XElement(XName.Get("configuration"))
        | None, Some c-> new XElement(XName.Get("configuration"), c)
        | Some a, None -> new XElement(XName.Get("configuration"), a)
        | Some a, Some c -> new XElement(XName.Get("configuration"), a, c)

let public write settingsDict =
    let appSettings, connectionStrings = unzipSettings settingsDict

    let appSettingsSection =  buildAppSettingsXml appSettings
    let connectionStringsSection = buildConnectionStringsXml connectionStrings

    let xmlStructure = buildConfigurationXml appSettingsSection connectionStringsSection

    xmlStructure.ToString()

