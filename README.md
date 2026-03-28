# Blackboard
[![Unity Version](https://img.shields.io/badge/unity-6000.0+-000.svg)](https://unity3d.com/get-unity/download/archive)

## Overview
Blackboard is a lightweight typed key-value container for Unity gameplay systems.

It supports:
- runtime storage and retrieval by variable name or GUID,
- strongly-typed values via `BlackboardVariable<T>`,
- authoring with a `BlackboardAsset` in the inspector,
- conversion from authored asset data to runtime `Blackboard` instances.

## Table of Contents
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Manual Installation](#manual-installation)
  - [UPM Installation](#upm-installation)
- [Features](#features)
- [Settings](#settings)
- [Usage](#usage)
  - [Create and Populate a Runtime Blackboard](#create-and-populate-a-runtime-blackboard)
  - [Get and Set Values with Extensions](#get-and-set-values-with-extensions)
  - [Use BlackboardAsset and Convert to Runtime](#use-blackboardasset-and-convert-to-runtime)
  - [Enable Custom Enum Types](#enable-custom-enum-types)
- [API Reference](#api-reference)
  - [Blackboard](#blackboard)
  - [BlackboardVariable and BlackboardVariableT](#blackboardvariable-and-blackboardvariablet)
  - [BlackboardAsset](#blackboardasset)
  - [BlackboardExtensions](#blackboardextensions)
  - [BlackboardEnumAttribute](#blackboardenumattribute)
- [Dependencies](#dependencies)
- [License](#license)

## Getting Started

### Prerequisites
- [GIT](https://git-scm.com/downloads)
- [Unity](https://unity.com/releases/editor/archive) 6000.0+

### Manual Installation
1. Download the `.unitypackage` from the [releases](https://github.com/DanilChizhikov/blackboard/releases/) page.
2. Import `com.dtech.blackboard.x.x.x.unitypackage` into your project.

### UPM Installation
1. Open `manifest.json` in your project's `Packages` folder.
2. Add the following line to the dependencies section:
   ```json
   "com.dtech.blackboard": "https://github.com/DanilChizhikov/blackboard.git",
   ```
3. Unity will automatically import the package.

To pin a specific release tag, use `v*.*.*`:

`https://github.com/DanilChizhikov/blackboard.git#v0.0.1`

## Features
- **Typed variables**

  Store values with `BlackboardVariable<T>` and access them via typed APIs.

- **Lookup by name or GUID**

  Variables can be retrieved/removed/replaced by `string` name or `SerializableGuid`.

- **Replace and remove workflows**

  `Blackboard` supports replace/remove with overloads that optionally return removed variables.

- **Case-insensitive name policy**

  Variable names are normalized (trimmed) and compared case-insensitively.
  For example, `"Health"`, `" health "`, and `"HEALTH"` are treated as the same key.

- **ScriptableObject authoring + runtime conversion**

  Author data in `BlackboardAsset`, then convert to runtime via `ToRuntimeBlackboard()`.

- **Editor type picker**

  The custom inspector exposes a searchable type picker for built-in Unity types, lists, enums marked with `[BlackboardEnum]`, and discovered custom component/scriptable object types.

## Settings
Blackboard does not require a global settings asset.

Configuration is asset-driven:
- create and edit a `BlackboardAsset` in the inspector,
- add variables from the type picker,
- validate names by the built-in naming policy,
- convert the asset to a runtime `Blackboard` when needed.

## Usage

### Create and Populate a Runtime Blackboard
```csharp
using DTech.Blackboard;

public static class BlackboardExample
{
    public static Blackboard CreateBlackboard()
    {
        var blackboard = new Blackboard();

        blackboard.Add(new BlackboardVariable<int>
        {
            Name = "Health",
            Value = 100
        });

        blackboard.Add(new BlackboardVariable<float>
        {
            Name = "MoveSpeed",
            Value = 5.5f
        });

        return blackboard;
    }
}
```

### Get and Set Values with Extensions
```csharp
using DTech.Blackboard;

public static class BlackboardReadWriteExample
{
    public static void Run(Blackboard blackboard)
    {
        // Typed get
        int health = blackboard.GetValue<int>("Health");

        // Typed set
        blackboard.SetValue("Health", health - 10);
        
        // Typed set without notification
        blackboard.SetValueWithoutNotif("Health", health);

        // Try-get for safe access
        if (blackboard.TryGetVariable<float>("MoveSpeed", out BlackboardVariable<float> speedVar))
        {
            speedVar.Value += 0.5f;
        }
    }
}
```

### Use BlackboardAsset and Convert to Runtime
```csharp
using DTech.Blackboard;
using UnityEngine;

public static class BlackboardAssetExample
{
    public static Blackboard CreateFromAsset()
    {
        var asset = ScriptableObject.CreateInstance<BlackboardAsset>();

#if UNITY_EDITOR
        asset.AddVariable("Health", typeof(int));
        asset.AddVariable("PlayerName", typeof(string));
#endif

        return asset.ToRuntimeBlackboard();
    }
}
```

### Enable Custom Enum Types
Mark enums with `[BlackboardEnum]` so they appear in Blackboard editor variable options.

```csharp
using DTech.Blackboard;

[BlackboardEnum]
public enum AIState
{
    Idle,
    Patrol,
    Combat
}
```

## API Reference
This section covers the main public types.

### Blackboard
Main runtime container.

```csharp
public sealed class Blackboard : IDisposable
{
    public int VariableCount { get; }

    public void Add(BlackboardVariable variable);

    public bool Replace(string sourceVariableName, BlackboardVariable to);
    public bool Replace(SerializableGuid sourceGuid, BlackboardVariable to);

    public bool Remove(string name);
    public bool Remove(SerializableGuid guid);
    public bool Remove(string name, out BlackboardVariable variable);
    public bool Remove(SerializableGuid guid, out BlackboardVariable variable);

    public bool TryGetVariable(string name, out BlackboardVariable variable);
    public bool TryGetVariable<T>(string name, out BlackboardVariable<T> variable);
    public bool TryGetVariable(SerializableGuid guid, out BlackboardVariable variable);
    public bool TryGetVariable<T>(SerializableGuid guid, out BlackboardVariable<T> variable);

    public int GetVariablesNonAlloc(BlackboardVariable[] variables);

    public void Dispose();
}
```

### BlackboardVariable and BlackboardVariable&lt;T&gt;
Represents a named variable with a stable GUID and typed/object value access.

```csharp
public abstract class BlackboardVariable
{
    public SerializableGuid Guid { get; }
    public string Name { get; set; }
    public abstract Type ValueType { get; }
    public abstract object ObjectValue { get; set; }

    public static BlackboardVariable CreateForType(Type type, string name);
    public abstract void SetObjectValueWithoutNotif(object value);
    public abstract BlackboardVariable Clone();
}

public class BlackboardVariable<T> : BlackboardVariable
{
    public event Action<SerializableGuid, T> OnValueChanged;
    public T Value { get; set; }
    public void SetValueWithoutNotif(T value);
}
```

### BlackboardAsset
ScriptableObject-based authoring container.

```csharp
public sealed partial class BlackboardAsset : ScriptableObject
{
    public Blackboard ToRuntimeBlackboard();
}
```

Editor-only partial API:

```csharp
#if UNITY_EDITOR
public sealed partial class BlackboardAsset
{
    public IReadOnlyList<BlackboardVariable> Variables { get; }
    public SerializableGuid AddVariable(string variableName, Type valueType);
    public bool RemoveVariable(string variableName);
}
#endif
```

### BlackboardExtensions
Convenience extension methods for read/write operations.

```csharp
public static class BlackboardExtensions
{
    public static BlackboardVariable GetVariable(this Blackboard blackboard, string name);
    public static BlackboardVariable GetVariable(this Blackboard blackboard, SerializableGuid guid);
    public static BlackboardVariable<T> GetVariable<T>(this Blackboard blackboard, string name);
    public static BlackboardVariable<T> GetVariable<T>(this Blackboard blackboard, SerializableGuid guid);

    public static T GetValue<T>(this Blackboard blackboard, string name);
    public static T GetValue<T>(this Blackboard blackboard, SerializableGuid guid);

    public static void SetValue<T>(this Blackboard blackboard, string name, T value);
    public static void SetValueWithoutNotif<T>(this Blackboard blackboard, string name, T value);
    public static void SetValue<T>(this Blackboard blackboard, SerializableGuid guid, T value);
    public static void SetValueWithoutNotif<T>(this Blackboard blackboard, SerializableGuid guid, T value);
}
```

Notification behavior example:

```csharp
blackboard.SetValue("Health", 90); // invokes BlackboardVariable<int>.OnValueChanged
blackboard.SetValueWithoutNotif("Health", 90); // updates value without notification
```

### BlackboardEnumAttribute
Marks enum types that should be available in Blackboard editor variable options.

```csharp
[AttributeUsage(AttributeTargets.Enum)]
public sealed class BlackboardEnumAttribute : Attribute
{
}
```

## Dependencies
No external runtime dependencies beyond Unity 6000.0+.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
