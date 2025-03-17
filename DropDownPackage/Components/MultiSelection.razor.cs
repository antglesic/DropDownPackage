using Microsoft.AspNetCore.Components;

namespace DropDownPackage.Components
{
	public partial class MultiSelection<T> : ComponentBase
	{
		#region Parameters

		[Parameter]
		public required IEnumerable<T> Items { get; set; }

		[Parameter]
		public required string IdentifierProperty { get; set; }

		[Parameter]
		public required string DisplayProperty { get; set; }

		[Parameter]
		public EventCallback<List<T>> ValuesChanged { get; set; }

		[Parameter]
		public bool CanSearch { get; set; } = true;

		#endregion

		#region Properties

		private List<T> ItemList = new List<T>();
		private List<T> FilteredItemList = new List<T>();
		private List<object> SelectedValues = new List<object>();

		#endregion

		protected override async Task OnParametersSetAsync()
		{
			if (Items != null && Items.Any())
			{
				if (!ItemList.Any() || !ItemsAreEqual(ItemList, Items))
				{
					ItemList = Items.ToList();
					FilteredItemList = ItemList;  // Initialize the filtered list with all items
				}
			}

			await base.OnParametersSetAsync();
		}

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

		private object GetPropertyValue(T item, string propertyName)
		{
			var propertyInfo = typeof(T).GetProperty(propertyName);
			if (propertyInfo == null)
			{
				throw new InvalidOperationException($"Property '{propertyName}' not found on type '{typeof(T).Name}'");
			}
			return propertyInfo.GetValue(item);
		}

		// This method filters the list based on the current searchText immediately
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

		// Check if the item is selected based on the current selection
		private bool IsSelected(T item)
		{
			var identifierValue = GetPropertyValue(item, IdentifierProperty);
			return SelectedValues.Contains(identifierValue);
		}

		private string GetOptionClass(T item)
		{
			return IsSelected(item) ? "selected-option" : string.Empty;
		}

		// This method will handle changes in selection
		private async Task OnValueChanged(ChangeEventArgs e)
		{
			var selectedOptions = (e.Value as string[]) ?? Array.Empty<string>();

			if (SelectedValues.Count == 0)
			{
				SelectedValues = selectedOptions
								.Select(value => ItemList.FirstOrDefault(item => GetPropertyValue(item, IdentifierProperty).ToString() == value))
								.Where(item => item != null)
								.Cast<object>()
								.ToList();
			}
			else
			{
				var newSelectedValues = selectedOptions
										.Select(value => ItemList.FirstOrDefault(item => GetPropertyValue(item, IdentifierProperty).ToString() == value))
										.Where(item => item != null)
										.Cast<object>()
										.ToList();

				if (newSelectedValues.FirstOrDefault() != null && SelectedValues.Contains(newSelectedValues.FirstOrDefault()))
				{
					SelectedValues.Remove(newSelectedValues.FirstOrDefault());
				}
				else
				{
					SelectedValues.AddRange(newSelectedValues);
				}
			}

			if (ValuesChanged.HasDelegate)
			{
				await ValuesChanged.InvokeAsync(SelectedValues.Cast<T>().ToList());
			}
		}
	}
}
