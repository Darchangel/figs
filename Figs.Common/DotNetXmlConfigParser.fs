module Figs.Common.DotNetXmlConfigParser

open System
open FSharp.Data
open System.Xml.Linq


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
            | "" -> []
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

                List.concat [appSettings; connectionStrings]
    with
        :? Exception as ex -> []


let private unzipSettings settingList =
    let rec unzipHelper appSettings connectionStrings (remainingSettingsList : (string * string) list) =
        if remainingSettingsList.IsEmpty then
            (List.rev appSettings, List.rev connectionStrings)
        else
            let name, value = remainingSettingsList.Head
            if ConnectionStrings.isConnectionString name then
                let cleanSetting = (ConnectionStrings.makeNormalName name, value)
                unzipHelper appSettings (cleanSetting :: connectionStrings) remainingSettingsList.Tail
            else
                unzipHelper ((name, value) :: appSettings) connectionStrings remainingSettingsList.Tail
    
    unzipHelper [] [] settingList

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
        let connectionStringElements = [for name, value in connectionStringsList do yield new XElement(XName.Get("add"), new XAttribute(XName.Get("name"), name), new XAttribute(XName.Get("connectionString"), value))]
        Some (new XElement(XName.Get("connectionStrings"), connectionStringElements))

let private buildConfigurationXml (appSettingsSection : option<XElement>) (connectionStringsSection : option<XElement>) =
    match appSettingsSection, connectionStringsSection with
        | None, None -> new XElement(XName.Get("configuration"))
        | None, Some c-> new XElement(XName.Get("configuration"), c)
        | Some a, None -> new XElement(XName.Get("configuration"), a)
        | Some a, Some c -> new XElement(XName.Get("configuration"), a, c)

let public write settingList =
    let appSettings, connectionStrings = unzipSettings settingList

    let appSettingsSection =  buildAppSettingsXml appSettings
    let connectionStringsSection = buildConnectionStringsXml connectionStrings

    let xmlStructure = buildConfigurationXml appSettingsSection connectionStringsSection

    xmlStructure.ToString()

