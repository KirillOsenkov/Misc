using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ClassLibrary1
{
    public class EditorRotator
    {
        private static bool isEditorRotated = false;
        public static bool IsEditorRotated
        {
            get
            {
                return isEditorRotated;
            }

            set
            {
                if (isEditorRotated == value)
                {
                    return;
                }

                isEditorRotated = value;
                ScheduleWork();
            }
        }

        public static void ScheduleWork()
        {
            Application.Current.Dispatcher.InvokeAsync(DoWorkCore);
        }

        private static void DoWorkCore()
        {
            var window = Application.Current.MainWindow;
            var child = FindChildOfType(window, d => d.GetType().Name.EndsWith("WpfTextViewHost")) as FrameworkElement;
            if (child == null)
            {
                return;
            }

            if (isEditorRotated)
            {
                child.RenderTransform = new RotateTransform(20.0, 500, 400);
            }
            else
            {
                child.RenderTransform = null;
            }
        }

        private static object FindChildOfType(DependencyObject root, Func<DependencyObject, bool> predicate)
        {
            if (predicate(root))
            {
                return root;
            }

            foreach (var child in GetChildren(root))
            {
                var found = FindChildOfType(child, predicate);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        public static IEnumerable<DependencyObject> GetChildren(DependencyObject visual)
        {
            int count = VisualTreeHelper.GetChildrenCount(visual);
            if (count == 0)
            {
                return Enumerable.Empty<DependencyObject>();
            }
            
            var list = new List<DependencyObject>(count);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(visual, i);
                list.Add(child);
            }

            return list;
        }
    }
}
