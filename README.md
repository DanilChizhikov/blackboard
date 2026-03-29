# Blackboard
[![Unity Version](https://img.shields.io/badge/unity-6000.0+-000.svg)](https://unity3d.com/get-unity/download/archive)
![Unity Tests](https://github.com/DanilChizhikov/blackboard/actions/workflows/tests.yml/badge.svg?branch=master)

## Overview
Blackboard is a lightweight typed key-value container designed for Unity gameplay systems.

It solves the common problem of sharing and managing game state between different systems (AI, animation, UI, etc.) without tight coupling.

Think of it as a shared data space where:
- An AI system writes the current `AIState` to the blackboard
- An animation system reads that state to pick the right animation
- The UI displays the same values without direct references to either system

**Common use cases:**
- **AI behavior trees**: Share perception data, target references, and state between nodes
- **Animation systems**: Bridge gameplay values (speed, health) to animator parameters
- **Cutscenes/sequencing**: Pass actor references and scene data to timeline tracks
- **Gameplay state**: Centralize character stats, quest progress, or match state

It supports:
- runtime storage and retrieval by variable name or GUID,
- strongly-typed values via `BlackboardVariable<T>`,
- authoring with a `BlackboardAsset` in the inspector,
- conversion from authored asset data to runtime `RuntimeBlackboard` instances.

## Table of Contents
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Manual Installation](#manual-installation)
  - [UPM Installation](#upm-installation)
- [Features](#features)
- [Settings](#settings)
- [Usage](#usage)
  - [Create and Populate a Runtime Blackboard](#create-and-populate-a-runtime-blackboard)
  - [Get and Set Values](#get-and-set-values)
  - [Use BlackboardAsset and Convert to Runtime](#use-blackboardasset-and-convert-to-runtime)
  - [Enable Custom Enum Types](#enable-custom-enum-types)
  - [Enable Custom Types](#enable-custom-types)
- [API Reference](#api-reference)
  - [RuntimeBlackboard](#runtimeblackboard)
  - [BlackboardVariable and BlackboardVariableT](#blackboardvariable-and-blackboardvariablet)
  - [BlackboardAsset](#blackboardasset)
  - [BlackboardEnumAttribute](#blackboardenumattribute)
  - [BlackboardCategoryAttribute](#blackboardcategoryattribute)
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

`https://github.com/DanilChizhikov/blackboard.git#v1.0.0`

## Features
- **Typed variables**

  Store values with `BlackboardVariable<T>` and access them via typed APIs.

- **Lookup by name or GUID**

  Variables can be retrieved/removed/replaced by `string` name or `SerializableGuid`.

- **Replace and remove workflows**

  `RuntimeBlackboard` supports replace/remove with overloads that optionally return removed variables.

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
- convert the asset to a runtime `RuntimeBlackboard` when needed.

## Usage

### Create and Populate a Runtime Blackboard
```csharp
using DTech.Blackboard;

public static class BlackboardExample
{
    public static RuntimeBlackboard CreateBlackboard()
    {
        var blackboard = new RuntimeBlackboard();

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

### Get and Set Values
```csharp
using DTech.Blackboard;

public static class BlackboardReadWriteExample
{
    public static void Run(RuntimeBlackboard blackboard)
    {
        // Typed get
        int health = blackboard.GetValue<int>("Health");

        // Typed set
        blackboard.SetValue("Health", health - 10);
        
        // Typed set without notification
        blackboard.SetValueWithoutNotify("Health", health);

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
    public static RuntimeBlackboard CreateFromAsset()
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

### Enable Custom Types
Mark classes or structs with `[BlackboardCategory]` to make them available in the Blackboard type picker. Use the path parameter to organize types into custom categories.

```csharp
using DTech.Blackboard;

[BlackboardCategory("Gameplay/Stats")]
[Serializable]
public class CharacterStats
{
    public int Health;
    public int Mana;
}

[BlackboardCategory]
[Serializable]
public struct PlayerData
{
    public string PlayerName;
    public int Level;
}
```

## API Reference
This section covers the main public types.

### RuntimeBlackboard
Main runtime container.

```csharp
public sealed class RuntimeBlackboard : IDisposable
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

    public BlackboardVariable GetVariable(string name);
    public BlackboardVariable GetVariable(SerializableGuid guid);
    public BlackboardVariable<T> GetVariable<T>(string name);
    public BlackboardVariable<T> GetVariable<T>(SerializableGuid guid);

    public T GetValue<T>(string name);
    public T GetValue<T>(SerializableGuid guid);

    public void SetValue<T>(string name, T value);
    public void SetValueWithoutNotify<T>(string name, T value);
    public void SetValue<T>(SerializableGuid guid, T value);
    public void SetValueWithoutNotify<T>(SerializableGuid guid, T value);

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
    public virtual T Value { get; set; }
    public virtual void SetValueWithoutNotif(T value);
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

Notification behavior example:

```csharp
blackboard.SetValue("Health", 90); // invokes BlackboardVariable<int>.OnValueChanged
blackboard.SetValueWithoutNotify("Health", 90); // updates value without notification
```

### BlackboardEnumAttribute
Marks enum types that should be available in Blackboard editor variable options.

```csharp
[AttributeUsage(AttributeTargets.Enum)]
public sealed class BlackboardEnumAttribute : Attribute
{
}
```

### BlackboardCategoryAttribute
Marks class or struct types that should be available in Blackboard editor variable options. Types must be marked with `[Serializable]` and can optionally specify a custom category path.

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class BlackboardCategoryAttribute : Attribute
{
    public string Path { get; }

    public BlackboardCategoryAttribute();
    public BlackboardCategoryAttribute(string path);
}
```

## Dependencies
No external runtime dependencies beyond Unity 6000.0+.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
