using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ScrollViewer = System.Windows.Controls.ScrollViewer;

namespace YMS.Controls
{
    /// <summary>
    /// LazyCheckComboBox 类，继承 HandyControl.Controls.ComboBox 控件，实现懒加载和搜索功能。
    /// </summary>
    public class LazyCheckComboBox : HandyControl.Controls.CheckComboBox
    {
        #region 依赖属性

        /// <summary>
        /// 定义 LazyDataSource 依赖属性。
        /// </summary>
        public static readonly DependencyProperty LazyDataSourceProperty =
            DependencyProperty.Register(
                nameof(LazyDataSource), 
                typeof(IEnumerable), 
                typeof(LazyCheckComboBox),
                new PropertyMetadata(null, OnLazyDataSourceChanged));

        /// <summary>
        /// 定义 SearchText 依赖属性。
        /// </summary>
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(
                nameof(SearchText), 
                typeof(string), 
                typeof(LazyCheckComboBox),
                new PropertyMetadata(string.Empty, OnSearchTextChanged));

        /// <summary>
        /// 定义 FirstLoadCount 依赖属性，默认值 30。
        /// </summary>
        public static readonly DependencyProperty FirstLoadCountProperty =
            DependencyProperty.Register(
                nameof(FirstLoadCount), 
                typeof(int), 
                typeof(LazyCheckComboBox),
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
            get => (IEnumerable)this.GetValue(LazyDataSourceProperty);
            set => this.SetValue(LazyDataSourceProperty, value);
        }

        /// <summary>
        /// 获取或设置搜索文本。
        /// </summary>
        public string SearchText
        {
            get => (string)this.GetValue(SearchTextProperty);
            set => this.SetValue(SearchTextProperty, value);
        }

        /// <summary>
        /// 获取或设置初始加载数量。
        /// </summary>
        public int FirstLoadCount
        {
            get => (int)this.GetValue(FirstLoadCountProperty);
            set => this.SetValue(FirstLoadCountProperty, value);
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数。
        /// </summary>
        public LazyCheckComboBox()
        {
            this._itemSource = new ObservableCollection<object>();
            this.ItemsSource = this._itemSource;

            this.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(this.ScrollChanged));
            this.Loaded += (s, e) => this.LoadNextItem(this.FirstLoadCount); // 初始加载数量
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// LazyDataSource 属性更改时的回调方法。
        /// </summary>
        private static void OnLazyDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LazyCheckComboBox lazyControl && e.NewValue is IEnumerable newValue)
            {
                lazyControl.ResetLazyLoading();
            }
        }

        /// <summary>
        /// SearchText 属性更改时的回调方法。
        /// </summary>
        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LazyCheckComboBox lazyControl && e.NewValue is string newValue)
            {
                lazyControl.ResetLazyLoading();
            }
        }

        /// <summary>
        /// 滚动事件处理。
        /// </summary>
        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // 检查是否滚动到底部。
            if (e.OriginalSource is System.Windows.Controls.ScrollViewer scrollViewer)
            {
                if (e.VerticalOffset == scrollViewer.ScrollableHeight)
                {
                    this.LoadNextItem();

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
            if (this._lazyDataEnumerator == null) 
                return;

            for (int i = 0; i < count; i++)
            {
                if (this._lazyDataEnumerator.MoveNext())
                    this._itemSource.Add(this._lazyDataEnumerator.Current);
            }
        }

        /// <summary>
        /// 重置懒加载，应用搜索过滤器并重新加载。
        /// </summary>
        private void ResetLazyLoading()
        {
            this._itemSource.Clear();
            this._lazyDataEnumerator = ApplySearchFilter(this.LazyDataSource)?.GetEnumerator();
            this.LoadNextItem(this.FirstLoadCount); // 初始加载数量
        }

        /// <summary>
        /// 应用搜索过滤器到全量数据集合。
        /// </summary>
        /// <param name="data">全量数据集合</param>
        /// <returns>经过过滤的数据集合</returns>
        private IEnumerable ApplySearchFilter(IEnumerable data)
        {
            if (data == null) 
                return null;

            if (string.IsNullOrEmpty(this.SearchText)) 
                return data;

            return data.Cast<object>().Where(item => $"{item}".Contains(this.SearchText));
        }

        #endregion
    }
}
