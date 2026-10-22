using System.Text.Json.Serialization;
using AppTemplate.Composition.Stores.State.Model;

namespace AppTemplate.Store.SerializerContext;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AppStateModelForJson))]
[JsonSerializable(typeof(WindowStateModelForJson))]
public partial class StateJsonSerializerContext : JsonSerializerContext {
}