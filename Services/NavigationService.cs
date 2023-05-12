﻿using System;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;

namespace WifftOCR.Services
{
    internal static class NavigationService
    {
        public static event NavigatedEventHandler Navigated;
        public static event NavigationFailedEventHandler NavigationFailed;

        private static Frame frame;
        private static object lastParamUsed;

        public static Frame Frame
        {
            get 
            {
                if (frame == null) {
                    frame = Window.Current.Content as Frame;
                    RegisterFrameEvents();
                }

                return frame;
            }

            set
            {
                UnregisterFrameEvents();
                frame = value;
                RegisterFrameEvents();
            }
        }

        public static bool CanGoBack => Frame.CanGoBack;

        public static bool CanGoForward => Frame.CanGoForward;

        public static bool GoBack()
        {
            if (CanGoBack) Frame.GoBack();

            return CanGoBack;
        }

        public static void GoForward() => Frame.GoForward();

        public static bool Navigate(Type pageType, object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            if (Frame.Content?.GetType() != pageType || (parameter != null && !parameter.Equals(lastParamUsed))) {
                bool navigationResult = Frame.Navigate(pageType, parameter, infoOverride);
                if (navigationResult) lastParamUsed = parameter;

                return navigationResult;
            } 
                
            return false;
        }

        public static bool Navigate<T>(object parameter = null, NavigationTransitionInfo infoOverride = null) 
            where T : Page => Navigate(typeof(T), parameter, infoOverride);

        private static void RegisterFrameEvents()
        {
            if (frame != null) {
                frame.Navigated += Frame_Navigated;
                frame.NavigationFailed += Frame_NavigationFailed;
            }
        }

        private static void UnregisterFrameEvents()
        {
            if (frame != null) {
                frame.Navigated -= Frame_Navigated;
                frame.NavigationFailed -= Frame_NavigationFailed;
            }
        }

        private static void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e) => NavigationFailed?.Invoke(sender, e);

        private static void Frame_Navigated(object sender, NavigationEventArgs e) => Navigated?.Invoke(sender, e);

        internal static void EnsurePageIsSelected(Type pageType)
        {
            if (Frame.Content == null) Frame.Navigate(pageType);
        }
    }
}
