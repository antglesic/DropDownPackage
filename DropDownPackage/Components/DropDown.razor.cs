using Microsoft.AspNetCore.Components;

namespace DropDownPackage.Components
{
	public partial class DropDown<T> : ComponentBase
	{
		#region Parameters

		[Parameter]
		public required IEnumerable<T> Items { get; set; }

		[Parameter]
		public required string IdentifierProperty { get; set; }

		[Parameter]
		public required string DisplayProperty { get; set; }

		[Parameter]
		public EventCallback<T> ValueChanged { get; set; }

		[Parameter]
		public bool CanSearch { get; set; } = false;

		#endregion

		#region Properties

		private List<T> ItemList = new List<T>();
		private List<T> FilteredItemList = new List<T>();

		#endregion

		#region Methods

		protected override async Task OnInitializedAsync()
		{
			if (Items != null && Items.Any() && (ItemList == null || ItemList.Count == 0))
			{
				ItemList = Items.ToList();
				FilteredItemList = ItemList;
			}

			await base.OnInitializedAsync();
		}

		protected override async Task OnParametersSetAsync()
		{
			/*
				If you got some data sent to the dropdown and the dropdown is empty, then fill it with the data.
			*/
			if (Items != null && Items.Any())
			{
				/*
					If the dropdown is empty, then fill it with the data.
				*/
				if (ItemList == null || ItemList.Count == 0)
				{
					ItemList = Items.ToList();
					FilteredItemList = ItemList;
				}
				/*
					If the dropdown is not empty, then check if the data is different from the current data.
				*/
				else if (ItemList.Count != Items.Count() || !ItemsAreEqual(ItemList, Items))
				{
					ItemList = Items.ToList();
					FilteredItemList = ItemList;
				}
			}

			await base.OnParametersSetAsync();
		}

		/*
			This method checks if the items in both lists are equal.
		*/
		private bool ItemsAreEqual(IEnumerable<T> list1, IEnumerable<T> list2)
		{
			var identifierProperty = typeof(T).GetProperty(IdentifierProperty);
			if (identifierProperty == null)
			{
				throw new InvalidOperationException($"Property '{IdentifierProperty}' not found on type '{typeof(T).Name}'");
			}

			var list1Identifiers = list1.Select(item => identifierProperty.GetValue(item)).ToList();
			var list2Identifiers = list2.Select(item => identifierProperty.GetValue(item)).ToList();

			return list1Identifiers.SequenceEqual(list2Identifiers);
		}

		/*
			Helper method to get property value dynamically via reflection
		*/
		private object? GetPropertyValue(T item, string propertyName)
		{
			var propertyInfo = typeof(T).GetProperty(propertyName);
			if (propertyInfo == null)
			{
				throw new InvalidOperationException($"Property '{propertyName}' not found on type '{typeof(T).Name}'");
			}
			return propertyInfo.GetValue(item);
		}

		// This method will be called to filter the items based on the search text
		private async Task FilterItems(ChangeEventArgs e)
		{
			var filterText = e.Value?.ToString() ?? string.Empty;
			if (string.IsNullOrWhiteSpace(filterText))
			{
				FilteredItemList = ItemList; // Show all items when search text is empty
			}
			else
			{
				// Filter items based on DisplayProperty (search text matching any part of the display property)
				FilteredItemList = ItemList.Where(item =>
				GetPropertyValue(item, DisplayProperty)
					.ToString()
					.Contains(filterText, StringComparison.CurrentCultureIgnoreCase))
					.ToList();
			}

			await InvokeAsync(StateHasChanged);
		}

		// Triggered when the selection changes
		private async Task OnValueChanged(ChangeEventArgs e)
		{
			// Ensure we are properly getting the selected item
			var selectedItemId = e.Value;

			if (selectedItemId != null && ValueChanged.HasDelegate)
			{
				var selectedItem = ItemList.FirstOrDefault(item => GetPropertyValue(item, IdentifierProperty)?.ToString() == selectedItemId.ToString());
				if (selectedItem != null)
				{
					await ValueChanged.InvokeAsync(selectedItem);
				}
			}
		}

		#endregion
	}
}
