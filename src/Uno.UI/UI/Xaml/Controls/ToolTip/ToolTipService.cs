using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;

namespace Windows.UI.Xaml.Controls
{
	partial class ToolTipService
	{
		public static DependencyProperty ToolTipProperty { get; } =
			DependencyProperty.RegisterAttached(
				"ToolTip",
				typeof(object),
				typeof(ToolTipService),
				new FrameworkPropertyMetadata(default, OnToolTipChanged));

		public static object GetToolTip(DependencyObject element)
		{
			return element.GetValue(ToolTipProperty);
		}

		public static void SetToolTip( global::Windows.UI.Xaml.DependencyObject element, object value)
		{
			element.SetValue(ToolTipProperty, value);
		}

		private static void OnToolTipChanged(DependencyObject dependencyobject, DependencyPropertyChangedEventArgs args)
		{
			if (!(dependencyobject is FrameworkElement element))
			{
				return;
			}

			if (args.NewValue is ToolTip toolTip)
			{
				// First time: we're subscribing to event handlers

				long currentHoverId = 0;

				toolTip.SetAnchor(element);

				element.Loaded += (snd, evt) =>
				{
					element.PointerEntered += OnPointerEntered;
					element.PointerExited += OnPointerExited;
				};

				element.Unloaded += (snd, evt) =>
				{
					toolTip.IsOpen = false;

					element.PointerEntered -= OnPointerEntered;
					element.PointerExited -= OnPointerExited;
				};

				void OnPointerEntered(object snd, PointerRoutedEventArgs evt)
				{
					var t = HoverTask(++currentHoverId);
				}

				void OnPointerExited(object snd, PointerRoutedEventArgs evt)
				{
					currentHoverId++;
					toolTip.IsOpen = false;
				}

				async Task HoverTask(long hoverId)
				{
					await Task.Delay(1000);
					if (currentHoverId != hoverId)
					{
						return;
					}
					toolTip.IsOpen = true;
					await Task.Delay(7000);
					if (currentHoverId == hoverId)
					{
						toolTip.IsOpen = false;
					}
				}
			}
		}
	}
}
