---
title: Hello World Plugin
description: How to write your first plugin for CounterStrikeSharp
---

## Creating a New Project

First, ensure you have the relevant .NET 7.0 SDK for your platform installed on your machine. You can find the links to the latest downloads on the <a href="https://dotnet.microsoft.com/en-us/download/dotnet/7.0" target="_blank"> official Microsoft download page</a>.

### Creating a Class Library

All CounterStrikeSharp plugins are installed on the server as built .dll class library binary files, so we will get started by creating a new class library using the inbuilt dotnet sdk tools.

```shell
dotnet new classlib --name HelloWorldPlugin
```

Use your IDE (Visual Studio/Rider) to add a reference to the `CounterStrikeSharp.Api.dll` file that is installed onto the server. If you are using VSCode or a text editor, you can edit the .csproj file directly and add the following:

```diff
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

+  <ItemGroup>
+    <Reference Include="CounterStrikeSharp.API">
+       <HintPath>[where you downloaded or installed]/addons/counterstrikesharp/api/CounterStrikeSharp.API.dll</HintPath>
+    </Reference>
+  </ItemGroup>
</Project>
```

### Creating a Plugin File

Rename the default class file that came with your new project (by default it should be `Class1.cs`) to something more accurate, like `HelloWorldPlugin.cs`. Inside this file, we will insert the stub hello world plugin. Be sure to change the name and namespace so it matches your project name.

```csharp
using CounterStrikeSharp.API.Core;

namespace HelloWorldPlugin;
public class HelloWorldPlugin : BasePlugin
{
    public override string ModuleName => "Hello World Plugin";

    public override string ModuleVersion => "0.0.1";

    public override void Load(bool hotReload)
    {
        Console.WriteLine("Hello World!");
    }
}
```

Now build your project using your ide or the `dotnet build` command. You should now have a built binary file in your `bin/Debug/net7.0` subdirectory in the project.

### Installing your Plugin

Locate the `plugins` folder in your CS2 dedicated server (`/game/csgo/addons/counterstrikesharp/plugins`) and create a new folder with the exact same name as your output .dll file. In this example it would be `HelloWorldPlugin.dll`, so I will make a new folder called `HelloWorldPlugin`. Inside of this folder, copy and paste all of the `.dll` files _except_ the `CounterStrikeSharp.API.dll` file. Once completed, the folder should look as follows:

```shell
.
└── SamplePlugin
    ├── McMaster.NETCore.Plugins.dll
    ├── Microsoft.DotNet.PlatformAbstractions.dll
    ├── Microsoft.Extensions.DependencyModel.dll
    ├── SamplePlugin.deps.json
    ├── SamplePlugin.dll
    └── SamplePlugin.pdb
```

:::note
Note that some of these dependencies may change depending on the version of CounterStrikeSharp being used.
:::

### Start the Server

Now start your CS2 dedicated server. Just before the `CounterStrikeSharp.API Loaded Successfully.` message you should see your `Hello World!` console write that we called from the load function, neat!

:::note[Hot Reloading!]
By default, CounterStrikeSharp will automatically hot reload your plugin if you replace the .dll file in your plugin folder. When it does so, it will call the `Unload` and `Load` functions respectively, with the `hotReload` flag set to true.

It is worth noting that the framework will automatically deregister any event handlers or listeners for you automatically, so you can safely reregister these on load without checking this flag. However you may want to do some specific actions on a hot reload, which you can do in your `Load()` call by checking the flag!
:::