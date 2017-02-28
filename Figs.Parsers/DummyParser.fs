module Figs.Parsers.DummyParser

open System

let public parse (fileString : String) =
    Map.empty<string, string>

let public write settingsDict =
    ""