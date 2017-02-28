module Figs.Common.Configuration

open System.Collections.Generic
open FSharp.Data

open Figs.Common.Data
open Figs.Parsers


type Parser(parse, write) =
    member this.parse : string -> Map<string, string> = parse
    member this.write : Map<string, string> -> string = write

let private getParserForType configType = 
    match configType with
        | ConfigurationType.DotNetXml -> new Parser(DotNetXmlConfigParser.parse, DotNetXmlConfigParser.write)
        | _ -> new Parser(DummyParser.parse, DummyParser.write)

let private loadAndParse (load : fetchUrlContents) url configType = 
    let parser = getParserForType configType
    let urlString = load url

    parser.parse urlString


let public create (repository : IConfigurationRepository) (load : fetchUrlContents) name url configType =
    let values = loadAndParse load url configType
    let config = new Configuration(name, url, configType, values)

    repository.Save config

let public delete (repository: IConfigurationRepository) name =
    repository.Delete name

let public get (repository: IConfigurationRepository) name =
    repository.Get name

let public updateValues (repository: IConfigurationRepository) (config : Configuration) newValues =
    let updatedValues = Map.fold (fun map key value -> Map.add key value map) config.Values newValues
    let updatedConfig = new Configuration(config.Name, config.Url, config.Type, updatedValues)

    repository.Save updatedConfig
    updatedConfig
