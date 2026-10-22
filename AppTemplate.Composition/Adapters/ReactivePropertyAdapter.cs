using AppTemplate.Composition.Adapters;
using GenJsonConfig;
using GenJsonConfig.Attributes;

[assembly: RegisterJsonConfigWrapper(typeof(ReactiveProperty<>), typeof(ReactivePropertyAdapter<>))]

namespace AppTemplate.Composition.Adapters;

/// <summary>
/// <see cref="ReactiveProperty{T}"/> 用の <see cref="IJsonConfigWrapper{TWrapper, TInner}"/> アダプター。
/// GenJsonConfig が ReactiveProperty を含むプロパティのシリアライズコードを自動生成する際に参照されます。
/// </summary>
public sealed class ReactivePropertyAdapter<T> : IJsonConfigWrapper<ReactiveProperty<T>, T> {
	/// <inheritdoc />
	public T Get(ReactiveProperty<T> wrapper) {
		return wrapper.Value;
	}

	/// <inheritdoc />
	public void Set(ReactiveProperty<T> wrapper, T value) {
		wrapper.Value = value;
	}
}