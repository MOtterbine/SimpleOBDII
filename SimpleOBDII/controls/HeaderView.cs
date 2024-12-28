using OS.OBDII.ViewModels;
using OS.OBDII.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace OS.OBDII.Controls
{
    public class HeaderView : ContentView, ICustomButtonController 
    {


        public event EventHandler Touched;

        void ICustomButtonController.SendTouched()
        {
            Touched?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler LongClicked;

        void ICustomButtonController.SendLongClicked()
        {
            LongClicked?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Released;

        void ICustomButtonController.SendReleased()
        {
            Released?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler Clicked;

        void ICustomButtonController.SendClicked()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }



        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private void SetupAnimation(Image img)
        {
            if (img == null || UseIndicator == false) return;

            if (parentAnimation == null)
            {
                parentAnimation = new Animation();
                fadeOutAnimation = new Animation(d => img.Opacity = d, 1, 0, Easing.Linear);
                fadeInAnimation = new Animation(d => img.Opacity = d, 0, 1, Easing.Linear);
                parentAnimation.Add(0, 0.5, fadeInAnimation);
                parentAnimation.Add(0.5, 1, fadeOutAnimation);
            }
            //this.AbortAnimation(Constants.STRING_COMM_BLINKING_VISUAL_ELEMENT);
            parentAnimation.Commit(img, Constants.STRING_COMM_BLINKING_VISUAL_ELEMENT, 1, 200, repeat: () => true);
        }
        // private static Image animationImage= null;
        private Animation parentAnimation = null;
        private Animation fadeOutAnimation = null;
        private Animation fadeInAnimation = null;


        //private void StartAnimation()
        //{
        //    parentAnimation.Commit(animationImage, Constants.STRING_COMM_BLINKING_VISUAL_ELEMENT, 1, 200, repeat: () => true);
        //}

        private void StopAnimation()
        {
            this.AbortAnimation(Constants.STRING_COMM_BLINKING_VISUAL_ELEMENT);
        }

        public TextAlignment TitleHorizontalOptions
        {
            get { return (TextAlignment)GetValue(TitleHorizontalOptionsProperty); }
            set { SetValue(TitleHorizontalOptionsProperty, value); }
        }
        public static readonly BindableProperty TitleHorizontalOptionsProperty =
            BindableProperty.Create("TitleHorizontalOptions", typeof(TextAlignment), typeof(HeaderView), TextAlignment.Start);

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create("Title", typeof(string), typeof(HeaderView), string.Empty);

        public bool NavButtonIsEnabled
        {
            get { return (bool)GetValue(NavButtonIsEnabledProperty); }
            set { SetValue(NavButtonIsEnabledProperty, value); }
        }
        public static readonly BindableProperty NavButtonIsEnabledProperty =
            BindableProperty.Create("NavButtonIsEnabled", typeof(bool), typeof(HeaderView), true);

        public ICommand NavCommand
        {
            get { return (ICommand)GetValue(NavCommandProperty); }
            set { SetValue(NavCommandProperty, value); }
        }
        public static readonly BindableProperty NavCommandProperty =
            BindableProperty.Create("NavCommand", typeof(ICommand), typeof(HeaderView), null);

        public bool ErrorExists
        {
            get { return (bool)GetValue(ErrorExistsProperty); }
            set { SetValue(ErrorExistsProperty, value); }
        }
        public static readonly BindableProperty ErrorExistsProperty =
            BindableProperty.Create("ErrorExists", typeof(bool), typeof(HeaderView), false);

        public bool ConnectedLEDOn
        {
            get { return (bool)GetValue(ConnectedLEDOnProperty); }
            set { SetValue(ConnectedLEDOnProperty, value); }
        }
        public static readonly BindableProperty ConnectedLEDOnProperty =
            BindableProperty.Create("ConnectedLEDOn", typeof(bool), typeof(HeaderView), true);

        public bool UseIndicator
        {
            get { return (bool)GetValue(UseIndicatorProperty); }
            set { SetValue(UseIndicatorProperty, value); }
        }
        public static readonly BindableProperty UseIndicatorProperty =
            BindableProperty.Create("UseIndicator", typeof(bool), typeof(HeaderView), true);


        public bool ActivityLEDOn
        {
            get { return (bool)GetValue(ActivityLEDOnProperty); }
            set { SetValue(ActivityLEDOnProperty, value); }
        }
        public static readonly BindableProperty ActivityLEDOnProperty =
            BindableProperty.Create("ActivityLEDOn", typeof(bool), typeof(HeaderView), false);

    }
}
