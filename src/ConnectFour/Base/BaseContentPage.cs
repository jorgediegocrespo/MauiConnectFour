﻿using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using ReactiveUI;
using ReactiveUI.Maui;
using System.Reactive.Disposables;

namespace ConnectFour.Base;

public class BaseContentPage<TViewModel> : ReactiveContentPage<TViewModel> where TViewModel : BaseViewModel
{
    public BaseContentPage()
    {
        On<Microsoft.Maui.Controls.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        this.WhenActivated(d => ManageDisposables(d));
    }

    protected CompositeDisposable ManageDisposables(CompositeDisposable disposables)
    {
        CreateBindings(disposables);
        ObserveValues(disposables);
        CreateEvents(disposables);
        return disposables;
    }

    protected virtual void CreateBindings(CompositeDisposable disposables)
    { }

    protected virtual void ObserveValues(CompositeDisposable disposables)
    { }

    protected virtual void CreateEvents(CompositeDisposable disposables)
    { }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel?.OnAppearingAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ViewModel?.OnDisappearingAsync();
    }
}
