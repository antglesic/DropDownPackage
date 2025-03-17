using Microsoft.AspNetCore.Components;

namespace DropDownPackage.Components.Pages
{
	public partial class Counter : ComponentBase
	{
		private int currentCount = 0;

		private void IncrementCount()
		{
			currentCount++;
		}

		public List<DummyDataDto> DummyData = new List<DummyDataDto>();

		protected override void OnInitialized()
		{
			DummyData.Add(new DummyDataDto { Id = 1, Name = "John", Surname = "Doe" });
			DummyData.Add(new DummyDataDto { Id = 2, Name = "Jane", Surname = "Doe" });
			DummyData.Add(new DummyDataDto { Id = 3, Name = "Antonio", Surname = "Glešić" });
			DummyData.Add(new DummyDataDto { Id = 4, Name = "Diana", Surname = "Krndija" });

			base.OnInitialized();
		}

		public void OnSelectedItem(DummyDataDto item)
		{
			Console.WriteLine($"Selected item: {item.FullName}");
		}

		public void OnSelectedItemsChanged(List<DummyDataDto> items)
		{
			try
			{
				Console.WriteLine("Selected items:");
				foreach (var item in items)
				{
					Console.WriteLine(item.FullName);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
