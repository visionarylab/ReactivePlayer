﻿using ReactivePlayer.UI.WPF.Application.Core.ViewModels;
using ReactiveUI;
using System;
using System.Windows.Controls;

namespace ReactivePlayer.UI.WPF.Application.Views
{
    /// <summary>
    /// Interaction logic for UnhandledExceptionView.xaml
    /// </summary>
    public partial class UnhandledExceptionView : UserControl, IViewFor<UnhandledExceptionViewModel>
    {
        #region IViewFor

        private UnhandledExceptionViewModel _viewModel;
        public UnhandledExceptionViewModel ViewModel
        {
            get => this._viewModel;
            set => this._viewModel = value ?? throw new ArgumentNullException(nameof(value)); // TODO: localize
        }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as UnhandledExceptionViewModel);
        }

        #endregion

        public UnhandledExceptionView()
        {
            InitializeComponent();
        }
    }
}