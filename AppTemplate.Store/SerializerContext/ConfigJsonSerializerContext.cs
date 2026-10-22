using System.Text.Json.Serialization;
using AppTemplate.Composition.Stores.Config.Model;

namespace AppTemplate.Store.SerializerContext;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AppConfigModelForJson))]
[JsonSerializable(typeof(LanguageConfigModelForJson))]
[JsonSerializable(typeof(EnvironmentConfigModelForJson))]
public partial class ConfigJsonSerializerContext : JsonSerializerContext {
}