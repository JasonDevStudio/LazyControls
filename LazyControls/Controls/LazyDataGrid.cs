using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace YMS.Controls
{
    /// <summary>
    /// LazyDataGrid 类，继承 DataGrid 控件，实现懒加载和搜索功能。
    /// </summary>
    public class LazyDataGrid : DataGrid
    {
        #region 依赖属性

        /// <summary>
        /// 定义 LazyDataSource 依赖属性。
        /// </summary>
        public static readonly DependencyProperty LazyDataSourceProperty =
            DependencyProperty.Register(nameof(LazyDataSource), typeof(IEnumerable), typeof(LazyDataGrid),
                new PropertyMetadata(null, OnLazyDataSourceChanged));

        /// <summary>
        /// 定义 SearchText 依赖属性。
        /// </summary>
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(nameof(SearchText), typeof(string), typeof(LazyDataGrid),
                new PropertyMetadata(string.Empty, OnSearchTextChanged));

        /// <summary>
        /// 定义 FirstLoadCount 依赖属性，默认值 30。
        /// </summary>
        public static readonly DependencyProperty FirstLoadCountProperty =
            DependencyProperty.Register(nameof(FirstLoadCount), typeof(int), typeof(LazyDataGrid),
                new PropertyMetadata(30));

        #endregion

        #region 私有成员

        private ObservableCollection<object> _itemSource;
        private IEnumerator _lazyDataEnumerator;

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置全量数据集合。
        /// </summary>
        public IEnumerable LazyDataSource
        {
            get => (IEnumerable)GetValue(LazyDataSourceProperty);
            set => SetValue(LazyDataSourceProperty, value);
        }

        /// <summary>
        /// 获取或设置搜索文本。
        /// </summary>
        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        /// <summary>
        /// 获取或设置初始加载数量。
        /// </summary>
        public int FirstLoadCount
        {
            get => (int)GetValue(FirstLoadCountProperty);
            set => SetValue(FirstLoadCountProperty, value);
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数。
        /// </summary>
        public LazyDataGrid()
        {
            _itemSource = new ObservableCollection<object>();
            ItemsSource = _itemSource;

            AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(ScrollChanged));
            this.Loaded += (s, e) => this.LoadNextItem(this.FirstLoadCount); // 初始加载数量
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// LazyDataSource 属性更改时的回调方法。
        /// </summary>
        private static void OnLazyDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LazyDataGrid lazyDataGrid && e.NewValue is IEnumerable newValue)
            {
                lazyDataGrid.ResetLazyLoading();
            }
        }

        /// <summary>
        /// SearchText 属性更改时的回调方法。
        /// </summary>
        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LazyDataGrid lazyDataGrid && e.NewValue is string newValue)
            {
                lazyDataGrid.ResetLazyLoading();
            }
        }

        /// <summary>
        /// 滚动事件处理。
        /// </summary>
        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // 检查是否滚动到底部。
            if (e.OriginalSource is ScrollViewer scrollViewer)
            {
                if (e.VerticalOffset == scrollViewer.ScrollableHeight)
                {
                    LoadNextItem();

                    // 稍微调整滚动条的位置，使其不再处于底部。
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - 1);
                }
            }
        }

        /// <summary>
        /// 从全量数据集合中加载下N项。
        /// </summary>
        /// <param name="count">需要加载的数量</param>
        private void LoadNextItem(int count = 1)
        {
            if (_lazyDataEnumerator == null) return;

            for (int i = 0; i < count; i++)
            {
                if (_lazyDataEnumerator.MoveNext())
                    _itemSource.Add(_lazyDataEnumerator.Current);
            }
        }

        /// <summary>
        /// 重置懒加载，应用搜索过滤器并重新加载。
        /// </summary>
        private void ResetLazyLoading()
        {
            _itemSource.Clear();
            _lazyDataEnumerator = ApplySearchFilter(LazyDataSource)?.GetEnumerator();
            this.LoadNextItem(this.FirstLoadCount); // 初始加载数量
        }

        /// <summary>
        /// 应用搜索过滤器到全量数据集合。
        /// </summary>
        /// <param name="data">全量数据集合</param>
        /// <returns>经过过滤的数据集合</returns>
        private IEnumerable ApplySearchFilter(IEnumerable data)
        {
            if (data == null) return null;
            if (string.IsNullOrEmpty(SearchText)) return data;

            return data.Cast<object>().Where(item => $"{item}".Contains(SearchText));
        }

        #endregion
    }
}
