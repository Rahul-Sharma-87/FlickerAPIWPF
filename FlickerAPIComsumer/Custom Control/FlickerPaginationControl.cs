using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FlickerAPIComsumer.Custom_Control {

    public interface IPaginationContract<T> {
        IList<T> GetItemsFromDataSource(uint page, uint recordsPerPage, string searchString);

        int GetTotalRecords(string searchString);
    }

    public class FlickerImageItem {
        public string ImageId { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }


    [TemplatePart(Name = "PAGINATION_First",Type = typeof(Button))]
    [TemplatePart(Name = "PAGINATION_Previous",Type = typeof(Button))]
    [TemplatePart(Name = "PAGINATION_CurrentPage",Type = typeof(Label))]
    [TemplatePart(Name = "PAGINATION_Next",Type = typeof(Button))]
    [TemplatePart(Name = "PAGINATION_Last",Type = typeof(Button))]
    [TemplatePart(Name = "PAGINATION_TotalPage",Type = typeof(Label))]
    public class FlickerPaginationControl:Control {

        protected Button btnFirstPage, btnPreviousPage, btnNextPage, btnLastPage;
        protected Label lblCurrentPage,lblTotalPage;

        #region Dependency Property
        public static readonly DependencyProperty ItemSourceProperty = 
            DependencyProperty.Register(
                "ItemSource", 
                typeof(ObservableCollection<object>), 
                typeof(FlickerPaginationControl), 
                new PropertyMetadata(default(ObservableCollection<object>))
                );

        public static readonly DependencyProperty CurrentPageProperty = 
            DependencyProperty.Register(
                "CurrentPage", 
                typeof(uint), 
                typeof(FlickerPaginationControl), 
                new PropertyMetadata(default(uint))
                );

        public static readonly DependencyProperty TotalPagesProperty = 
            DependencyProperty.Register(
                "TotalPages", 
                typeof(uint), 
                typeof(FlickerPaginationControl), 
                new PropertyMetadata(default(uint))
            );

        public static readonly DependencyProperty ImageFetcherProperty = 
            DependencyProperty.Register(
                "ImageFetcher", 
                typeof(IPaginationContract<FlickerImageItem>), 
                typeof(FlickerPaginationControl), 
                new PropertyMetadata(default(IPaginationContract<FlickerImageItem>))
            );

        public static readonly DependencyProperty PageSizeProperty = 
            DependencyProperty.Register(
                "PageSize", 
                typeof(uint), 
                typeof(FlickerPaginationControl), 
                new PropertyMetadata(default(uint))
            );

        #endregion

        #region Properties

        public uint TotalPages {
            get { return (uint) GetValue(TotalPagesProperty); }
            set { SetValue(TotalPagesProperty, value); }
        }
        public uint CurrentPage {
            get { return (uint) GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }
        public ObservableCollection<object> ItemSource {
            get { return (ObservableCollection<object>) GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }
        public IPaginationContract<FlickerImageItem> ImageFetcher {
            get { return (IPaginationContract<FlickerImageItem>) GetValue(ImageFetcherProperty); }
            set { SetValue(ImageFetcherProperty, value); }
        }
        public uint PageSize {
            get { return (uint) GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        #endregion

        public FlickerPaginationControl() {
            this.Loaded += FlickerPaginationControl_Loaded;
        }

        private void FlickerPaginationControl_Loaded(object sender, RoutedEventArgs e) {
            if (Template == null) {
                throw new Exception("Invalid Control template.");
            }

            if (ImageFetcher == null)
            {
                throw new Exception("ImageFetcher cannot be null");
            }
            RegisterEvents();
            Navigate(PaginationAction.OnInit);
        }

        public override void OnApplyTemplate() {
            btnFirstPage = this.Template.FindName("PAGINATION_First", this) as Button;
            btnPreviousPage = this.Template.FindName("PAGINATION_Previous", this) as Button;
            btnNextPage = this.Template.FindName("PAGINATION_Next", this) as Button;
            btnLastPage = this.Template.FindName("PAGINATION_Last", this) as Button;
            lblCurrentPage = this.Template.FindName("PAGINATION_CurrentPage", this) as Label;
            lblTotalPage = this.Template.FindName("PAGINATION_TotalPage", this) as Label;

            if (
                btnFirstPage == null ||
                btnPreviousPage == null ||
                btnNextPage == null ||
                btnLastPage == null ||
                lblCurrentPage == null ||
                lblTotalPage == null
            ) {
                throw new Exception("Invalid Control template.");
            }
            base.OnApplyTemplate();
        }

        private void RegisterEvents() {
            btnFirstPage.Click += BtnFirstPage_Click;
            btnPreviousPage.Click += BtnPreviousPage_Click;
            btnNextPage.Click += BtnNextPage_Click;
            btnLastPage.Click += BtnLastPage_Click;
        }

        private void BtnLastPage_Click(object sender, RoutedEventArgs e) {
            Navigate(PaginationAction.Last);
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e) {
            Navigate(PaginationAction.Next);
        }

        private void BtnPreviousPage_Click(object sender, RoutedEventArgs e) {
            Navigate(PaginationAction.Previous);
        }

        private void BtnFirstPage_Click(object sender, RoutedEventArgs e) {
            Navigate(PaginationAction.First);
        }

        enum PaginationAction {
            Previous,
            Next,
            First,
            Last,
            OnInit
        }

        private string searchString = String.Empty;

        private void Navigate(PaginationAction paginationAction) {
            switch (paginationAction) {
                case PaginationAction.First:
                    CurrentPage = 1;
                    ItemSource = new ObservableCollection<object>(ImageFetcher.GetItemsFromDataSource(CurrentPage, PageSize, searchString)); 
                    break;
                case PaginationAction.Last:
                    CurrentPage = TotalPages;
                    ItemSource = new ObservableCollection<object>(ImageFetcher.GetItemsFromDataSource(CurrentPage, PageSize, searchString)); 
                    break;
                case PaginationAction.Next:
                    if (CurrentPage < TotalPages-1 ) {
                        CurrentPage++;
                        ItemSource = new ObservableCollection<object>(ImageFetcher.GetItemsFromDataSource(CurrentPage, PageSize, searchString)); 
                    }
                    break;
                case PaginationAction.Previous:
                    if (CurrentPage > 1 ) {
                        CurrentPage--;
                        ItemSource = new ObservableCollection<object>(ImageFetcher.GetItemsFromDataSource(CurrentPage, PageSize, searchString)); 
                    }
                    break;  
                 case PaginationAction.OnInit:
                     CurrentPage = 1;
                     int totalRecords = ImageFetcher.GetTotalRecords(searchString);

                     if(totalRecords>0)
                     TotalPages = Convert.ToUInt32(totalRecords)/ PageSize;
                     ItemSource = new ObservableCollection<object>(ImageFetcher.GetItemsFromDataSource(CurrentPage, PageSize, searchString)); 
                     break;
            }
        }
    }


}
