using DropDownPackage.CustomClasses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace DropDownPackage.Components
{
	public partial class Select<TItem> : ComponentBase
	{
		private string? errorMessage;

		private TItem[]? filterItems;

		private string? show;

		private string? search = string.Empty;

		private int itemIndex = -1;

		private bool pku;

		private bool pkd;

		private bool bkd;

		[Parameter]
		public TItem? SelectedItem { get; set; }

		[Parameter]
		[EditorRequired]
		public Func<TItem, string>? Display { get; set; }

		[Parameter]
		public ICollection<TItem>? Items { get; set; }

		[Parameter]
		public string? Id { get; set; } = "cb" + Guid.NewGuid();

		[Parameter]
		public bool Disabled { get; set; }

		[Parameter]
		public bool HideIcon { get; set; }

		[Parameter]
		public string? AccessKey { get; set; }

		[Parameter]
		public string? Placeholder { get; set; }

		[Parameter]
		public string? Width { get; set; } = "100%";

		[Parameter]
		public string? ListWidth { get; set; }

		[Parameter]
		public string? Label { get; set; }

		[Parameter]
		public string? Info { get; set; }

		[Parameter]
		public EventCallback<TItem?> OnItemSelect { get; set; }

		[Parameter]
		public string? Error { get; set; }

		[CascadingParameter]
		private Dictionary<string, string>? FormErrors { get; set; }

		private bool searchable { get; set; }

		private string? text { get; set; }

		[Inject]
		private IJSRuntime jsr { get; set; }

		protected override void OnParametersSet()
		{
			this.text = Placeholder;
			errorMessage = null;
			if (FormErrors != null && Error != null && FormErrors.ContainsKey(Error))
			{
				errorMessage = FormErrors[Error];
			}
			if (Display != null && SelectedItem != null)
			{
				this.text = Display(SelectedItem);
			}
			if (ListWidth == null)
			{
				string text2 = (ListWidth = ((Width != "100%") ? Width : "fit-content"));
			}
			filterItems = Items?.ToArray();
		}

		private void HideListing()
		{
			show = null;
		}

		private async Task ToggleShow()
		{
			TItem[]? array = filterItems;
			if (array != null && array.Length == 0)
			{
				show = null;
				return;
			}
			show = ((show == null) ? "show" : null);
			if (show != null)
			{
				await Task.Delay(10);
				string text = "if( /Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent) ) {} else { document.querySelector('.search-box > input')?.focus(); }";
				await jsr.InvokeVoidAsync("eval", text);
				await UpdatePosition();
			}
		}

		private async Task SetView()
		{
			await Task.Delay(10);
			await jsr.InvokeVoidAsync("eval", "document.querySelector('.item.selected')?.scrollIntoView(false)");
		}

		private async Task UpdatePosition()
		{
			string list = ".sbc-ac-list[data-for=\"" + Id + "\"]";
			await JSRuntimeExtensions.InvokeAsync<RectBounds>(jsr, "eval", new object[1] { "document.querySelector('body').getBoundingClientRect()" });
			RectBounds parent = await JSRuntimeExtensions.InvokeAsync<RectBounds>(jsr, "eval", new object[1] { "document.querySelector('#" + Id + "').getBoundingClientRect()" });
			double width = await JSRuntimeExtensions.InvokeAsync<double>(jsr, "eval", new object[1] { "document.querySelector('" + list + "')?.clientWidth" });
			double height = await JSRuntimeExtensions.InvokeAsync<double>(jsr, "eval", new object[1] { "document.querySelector('" + list + "')?.clientHeight" });
			double windowHeight = await JSRuntimeExtensions.InvokeAsync<double>(jsr, "eval", new object[1] { "window.innerHeight" });
			double num = await JSRuntimeExtensions.InvokeAsync<double>(jsr, "eval", new object[1] { "document.body.scrollWidth" });
			double num2 = parent.Left - (width - parent.Width);
			double value = ((windowHeight - height > parent.Bottom) ? (parent.Bottom + 4.0) : (parent.Top - height - 6.0));
			double value2 = ((parent.Left + width > num) ? num2 : parent.Left);
			string text = ((!(height < windowHeight - parent.Top)) ? $"document.querySelector('{list}').style.position = 'fixed'; document.querySelector('{list}').style.bottom = '{windowHeight - parent.Top + 4.0}px'; document.querySelector('{list}').style.left = '{value2}px';" : $"document.querySelector('{list}').style.position = 'fixed'; document.querySelector('{list}').style.top = '{value}px'; document.querySelector('{list}').style.left = '{value2}px';");
			await jsr.InvokeVoidAsync("eval", text);
		}

		private async Task SetItem(TItem? item)
		{
			pkd = true;
			pku = true;
			SelectedItem = item;
			text = Display?.Invoke(item);
			show = null;
			itemIndex = -1;
			await OnItemSelect.InvokeAsync(SelectedItem);
		}

		private void HandleKeyUp(KeyboardEventArgs args)
		{
			pku = false;
			"ArrowUp,ArrowDown,PageUp,PageDown,Home,End".Contains(args.Key);
		}

		private int GetMaxItems()
		{
			if (filterItems == null || filterItems.Length == 0)
			{
				return 0;
			}
			return filterItems.Count((TItem a) => Display(a) != "-" && !Display(a).StartsWith("**") && !Display(a).EndsWith("**"));
		}

		private TItem GetFilteredItem()
		{
			try
			{
				TItem[] array = filterItems?.Where((TItem a) => Display(a) != "-" && !Display(a).StartsWith("**") && !Display(a).EndsWith("**")).ToArray();
				if (array == null)
				{
					return default(TItem);
				}
				if (array.Length >= itemIndex)
				{
					return array[itemIndex];
				}
				if (array.Length != 0)
				{
					return array[0];
				}
				return default(TItem);
			}
			catch
			{
				return default(TItem);
			}
		}

		private async Task HandleKeyDown(KeyboardEventArgs args)
		{
			if (filterItems == null || filterItems.Length == 0)
			{
				return;
			}
			int maxItems = GetMaxItems();
			pkd = false;
			if (args.Key == "Enter" || args.Key == "Tab")
			{
				pkd = true;
				await SetItem(GetFilteredItem());
			}
			else if (args.Key == "Escape")
			{
				pkd = true;
				filterItems = Items?.ToArray();
				search = null;
				show = null;
				itemIndex = -1;
			}
			else if (args.Key == "End")
			{
				pkd = true;
				itemIndex = maxItems - 1;
				await SetView();
			}
			else if (args.Key == "Home")
			{
				pkd = true;
				itemIndex = 0;
				await SetView();
			}
			else if (args.Key == "ArrowDown")
			{
				pkd = true;
				if (itemIndex < maxItems - 1)
				{
					itemIndex++;
				}
				await SetView();
			}
			else if (args.Key == "ArrowUp")
			{
				pkd = true;
				if (itemIndex > 0)
				{
					itemIndex--;
				}
				await SetView();
			}
			else if (args.Key == "PageDown")
			{
				pkd = true;
				if (itemIndex + 10 < maxItems - 1)
				{
					itemIndex += 10;
				}
				else
				{
					itemIndex = maxItems - 1;
				}
				await SetView();
			}
			else if (args.Key == "PageUp")
			{
				pkd = true;
				if (itemIndex > 10)
				{
					itemIndex -= 10;
				}
				else
				{
					itemIndex = 0;
				}
				await SetView();
			}
			else if (args.Key == "ArrowLeft" || args.Key == "ArrowRight")
			{
				pkd = true;
			}
		}

		private async Task HandleButtonKeyDown(KeyboardEventArgs args)
		{
			bkd = false;
			int maxItems = GetMaxItems();
			if (args.Key == "ArrowDown" && show == null)
			{
				bkd = true;
				await ToggleShow();
				if (filterItems != null && maxItems > 0 && itemIndex == -1)
				{
					itemIndex = 0;
				}
				await HandleOnFocus();
			}
		}

		private void HandleSearch(ChangeEventArgs args)
		{
			if (show != null)
			{
				search = args.Value?.ToString();
				filterItems = ((!string.IsNullOrEmpty(search)) ? Items?.Where((TItem a) => Display(a).Contains(search, StringComparison.OrdinalIgnoreCase)).ToArray() : Items?.ToArray());
				if (itemIndex == -1)
				{
					itemIndex = 0;
				}
			}
		}

		private async Task HandleOnFocus()
		{
			search = null;
			HandleSearch(new ChangeEventArgs());
			await SetView();
		}
	}
}
