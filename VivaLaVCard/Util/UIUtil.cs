using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace VivaLaVCard.Util;

public static class UIUtil
{
    /// <summary>
    /// Focuses the first child of this ItemsControl once this items control is loaded.
    /// </summary>
    /// <remarks>If there are 0 children once this is loaded, does nothing.</remarks>
    public static void FocusFirstChildEventually(
        this ItemsControl itemsControl,
        NavigationMethod method = NavigationMethod.Unspecified,
        KeyModifiers keyModifiers = KeyModifiers.None
    )
    {
        itemsControl.DoWhenLoaded(() =>
        {
            Control? control = itemsControl.ItemsPanelRoot!.Children.FirstOrDefault();
            if (control == null)
            {
                return;
            }
            control.Focus(method, keyModifiers);
        });
    }

    /// <summary>
    /// Performs the given action once, as soon as this control is loaded (could be immediately).
    /// </summary>
    public static void DoWhenLoaded(this Control control, Action action)
    {
        if (control.IsLoaded)
        {
            action();
            return;
        }
        control.Loaded += Control_Loaded;

        void Control_Loaded(object? sender, RoutedEventArgs _)
        {
            ((Control)sender!).Loaded -= Control_Loaded;
            action();
        }
    }

    /// <summary>
    /// Performs the given action once, after the next layout update.
    /// </summary>
    public static void DoAfterNextLayoutUpdate(this Layoutable layoutable, Action action)
    {
        layoutable.LayoutUpdated += Control_LayoutUpdated;

        void Control_LayoutUpdated(object? sender, EventArgs _)
        {
            ((Layoutable)sender!).LayoutUpdated -= Control_LayoutUpdated;
            action();
        }
    }
}
