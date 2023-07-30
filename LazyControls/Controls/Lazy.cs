using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace YMS.Controls
{
    /// <summary>
    /// Lazy 类，提供懒加载行为作为附加属性。
    /// </summary>
    public static class Lazy
    {
        #region 附加属性

        /// <summary>
        /// 定义 LazyDataSource 附加属性。
        /// </summary>
        public static readonly DependencyProperty LazyDataSourceProperty =
            DependencyProperty.RegisterAttached("LazyDataSource", typeof(IEnumerable), typeof(Lazy),
                new PropertyMetadata(null, OnLazyDataSourceChanged));

        /// <summary>
        /// 定义 SearchText 附加属性。
        /// </summary>
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.RegisterAttached("SearchText", typeof(string), typeof(Lazy),
                new PropertyMetadata(string.Empty, OnSearchTextChanged));

        /// <summary>
        /// 定义 FirstLoadCount 附加属性，默认值 20。
        /// </summary>
        public static readonly DependencyProperty FirstLoadCountProperty =
            DependencyProperty.RegisterAttached("FirstLoadCount", typeof(int), typeof(Lazy),
                new PropertyMetadata(20));

        #endregion

        #region 公共方法

        /// <summary>
        /// 获取 LazyDataSource 附加属性的值。
        /// </summary>
        public static IEnumerable GetLazyDataSource(DependencyObject obj) => (IEnumerable)obj.GetValue(LazyDataSourceProperty);

        /// <summary>
        /// 设置 LazyDataSource 附加属性的值。
        /// </summary>
        public static void SetLazyDataSource(DependencyObject obj, IEnumerable value) => obj.SetValue(LazyDataSourceProperty, value);

        /// <summary>
        /// 获取 SearchText 附加属性的值。
        /// </summary>
        public static string GetSearchText(DependencyObject obj) => (string)obj.GetValue(SearchTextProperty);

        /// <summary>
        /// 设置 SearchText 附加属性的值。
        /// </summary>
        public static void SetSearchText(DependencyObject obj, string value) => obj.SetValue(SearchTextProperty, value);

        /// <summary>
        /// 获取 FirstLoadCount 附加属性的值。
        /// </summary>
        public static int GetFirstLoadCount(DependencyObject obj) => (int)obj.GetValue(FirstLoadCountProperty);

        /// <summary>
        /// 设置 FirstLoadCount 附加属性的值。
        /// </summary>
        public static void SetFirstLoadCount(DependencyObject obj, int value) => obj.SetValue(FirstLoadCountProperty, value);

        #endregion

        #region 私有方法

        private static void OnLazyDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsControl control && e.NewValue is IEnumerable newValue)
            {
                var itemSource = new ObservableCollection<object>();
                control.ItemsSource = itemSource;

                var enumerator = newValue.GetEnumerator();
                control.SetValue(LazyDataEnumeratorProperty, enumerator);

                LoadNextItems(control, GetFirstLoadCount(d)); // 初始加载
            }
        }

        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchText = e.NewValue as string;
            if (d is ItemsControl control && control.GetValue(LazyDataSourceProperty) is IEnumerable data)
            {
                var filteredData = string.IsNullOrEmpty(searchText)
                    ? data
                    : data.Cast<object>().Where(item => $"{item}".Contains(searchText));

                control.SetValue(LazyDataSourceProperty, filteredData);
            }
        }

        private static void LoadNextItems(ItemsControl control, int count = 1)
        {
            if (control.GetValue(LazyDataEnumeratorProperty) is IEnumerator enumerator)
            {
                var itemSource = control.ItemsSource as ObservableCollection<object>;
                for (int i = 0; i < count; i++)
                {
                    if (enumerator.MoveNext())
                        itemSource.Add(enumerator.Current);
                }
            }
        }

        private static void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer scrollViewer && scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                if (sender is ItemsControl control)
                {
                    LoadNextItems(control);
                }
            }
        }

        private static readonly DependencyProperty LazyDataEnumeratorProperty =
            DependencyProperty.RegisterAttached("LazyDataEnumerator", typeof(IEnumerator), typeof(Lazy));

        #endregion
    }
}
