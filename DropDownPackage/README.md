# DropDownPackage

A lightweight, customizable DropDown component built with Blazor. Easily select options from a dropdown with support for search, custom templates, and more.

✨ Features

    ✅ Select options from a dropdown

    🔍 Optional search/filter support

    🎨 Customizable item templates

    🧩 Supports binding to complex data types


🚀 Getting Started
## Installation
1. Add the following to your `_Imports.razor` file:
    ```csharp
    @using DropDownPackage.Components
    ```
## Usage
See the project `DropDownPackage` for more examples of how to use the component

### MultiSelect dropdown
```csharp
<DropDown T="DummyDataDto"
	Items="DummyData"
	DisplayProperty="FullName"
	IdentifierProperty="Id"
	ValueChanged="OnSelectedItemChanged"
	CanSearch="true"
	SearchPlaceHolder="Search..."
	CustomStyle=""
	Width="35%" />
```

#### When your Item looks like this:
```csharp
public class DummyDataDto
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Surname { get; set; } = string.Empty;
	public string FullName
	{
		get
		{
			return $"{Name} {Surname}";
		}
	}
}
```

#### And your OnSelectedItemsChanged method looks like this:

```csharp
public void OnSelectedItemChanged(DummyDataDto item)
{
	try
	{
		selectedItem = item;
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
	}
}
```
## Screenshots
