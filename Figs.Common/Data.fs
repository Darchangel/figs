module Figs.Common.Data

type public ConfigurationType = DotNetXml = 0

type public Configuration(name, url, configType : ConfigurationType, values : Map<string, string>) =
    member this.Name = name
    member this.Url = url
    member this.Type = configType
    member this.Values = values

type public IConfigurationRepository =
    abstract member Get: string -> Configuration
    abstract member Save: Configuration -> unit
    abstract member Delete: string -> unit

type fetchUrlContents = string -> string


type public DummyRepository() =
    interface IConfigurationRepository with
        member this.Get name =
            new Configuration(name, "", ConfigurationType.DotNetXml, Map.empty<string, string>)

        member this.Save config =
            "" |> ignore

        member this.Delete config =
            "" |> ignore

